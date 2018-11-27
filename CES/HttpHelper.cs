using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace CES
{
    public class HttpHelper
    {
        private static HttpListener httpPostRequest = new HttpListener();

        public static void HttpServerStart()
        {
            httpPostRequest.Prefixes.Add(Config.apiDic["http"]);
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
            Program.logger.Log("Http Server Start!");
        }

        /// <summary>
        /// Http 服务接口
        /// </summary>
        private static void httpPostRequestHandle()
        {
            while (Program.runnig)
            {
                httpPostRequest.Start();
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "false", msg = "request error,please check your url or post data!" }));
                StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                var rawUrl = requestContext.Request.RawUrl;
                var urlPara = rawUrl.Split('/');
                var json = new JObject();
                try
                {
                    if (requestContext.Request.HttpMethod == "POST")
                    {
                        var info = sr.ReadToEnd();
                        json = JObject.Parse(info);
                    }
                    if (urlPara.Length > 1)
                    {
                        Program.logger.Log("Url: " + rawUrl + "; json: " + json);
                        var method = urlPara[1];
                        if (method == "addr")
                        {
                            string coinType = json["type"].ToString();
                            string address = json["address"].ToString();
                            if (coinType == "btc" && !Config.btcAddrList.Contains(address))
                            {
                                Config.btcAddrList.Add(address);
                                DbHelper.SaveAddress(coinType, address);
                            }

                            if (coinType == "eth" && !Config.ethAddrList.Contains(address))
                            {
                                Config.ethAddrList.Add(address);
                                DbHelper.SaveAddress(coinType, address);
                            }

                            Program.logger.Log("Add a new " + coinType + " address: " + address);
                            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true" }));

                        }

                        if (method == "trans")
                        {
                            var msg = "";
                            switch (json["type"].ToString())
                            {
                                case "btc":
                                    msg = SendBtcTrans(json);
                                    break;
                                case "eth":
                                    msg = SendEthTrans(json).Result;
                                    break;
                                default:
                                    msg = "Error: error coin type";
                                    break;
                            }

                            if (msg.Contains("Error"))
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "false", msg = msg }));
                            else
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", txid = msg }));
                            Program.logger.Log(json["type"].ToString() + " transaction : " + msg);
                        }

                        if (method == "exchange")
                        {
                            string recTxid = json["txid"].ToString();
                            string sendTxid = DbHelper.GetSendTxid(recTxid);
                            if (string.IsNullOrEmpty(sendTxid))
                            {
                                var coinType = json["type"].ToString();
                                var result = NeoHandler.ExchangeAsync(coinType, json, Config.minerFeeDic["gas_fee"]).Result;
                                if (result != null && result.Contains("result"))
                                {
                                    var res = JObject.Parse(result)["result"] as JArray;
                                    sendTxid = (string)res[0]["txid"];
                                }

                                if (sendTxid != null)
                                {
                                    DbHelper.SaveExchangeInfo(recTxid, sendTxid);
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", txid = sendTxid }));
                                    Program.logger.Log("Exchange " + coinType + ",txid: " + sendTxid);
                                }
                                else
                                {
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "false", msg = result }));
                                    Program.logger.Log("Exchange " + coinType + ",result: " + result);
                                }
                            }
                            else
                            {
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", sendTxid }));
                            }

                        }

                        if (urlPara.Length > 2)
                        {
                            var coinType = urlPara[2];
                            if (method == "getbalance")
                            {
                                var balance = NeoHandler.GetBalanceAsync(coinType).Result;
                                buffer = Encoding.UTF8.GetBytes(
                                    JsonConvert.SerializeObject(new { state = "true", balance }));
                                Program.logger.Log("Get " + coinType + " Balance: " + balance);
                            }

                            if (method == "getaccount")
                            {
                                string address = string.Empty;
                                string priKey = string.Empty;
                                switch (coinType)
                                {
                                    case "btc":
                                        var btcPrikey = new Key();
                                        priKey = btcPrikey.GetWif(Config.nettype).ToString();
                                        address = btcPrikey.PubKey.GetAddress(Config.nettype).ToString();
                                        break;
                                    case "eth":
                                        var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
                                        var ethPrikey = ecKey.GetPrivateKeyAsBytes().ToHex();
                                        priKey = ethPrikey.ToString();
                                        address = new Account(ethPrikey).Address;
                                        break;
                                    default:
                                        break;
                                }

                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", type = coinType, address, priKey }));
                            }

                            if (method == "deploy")
                            {
                                if (coinType != "btc" && coinType != "eth" && coinType != "cneo" && coinType != "bct")
                                    continue;
                                string deployTxid = DbHelper.GetDeployStateByTxid(coinType, json["txid"].ToString());
                                if (string.IsNullOrEmpty(deployTxid)) //没有发行NEP5 BTC/ETH
                                {
                                    var deployResult = NeoHandler
                                        .DeployNep5TokenAsync(coinType, json, Config.minerFeeDic["gas_fee"]).Result;
                                    if (deployResult != null && deployResult.Contains("result"))
                                    {
                                        var res = JObject.Parse(deployResult)["result"] as JArray;

                                        if (!string.IsNullOrEmpty((string)res[0]["txid"]))
                                        {
                                            if (coinType == "cneo" || coinType == "bct")
                                            {
                                                var transInfo = new TransactionInfo();
                                                transInfo.coinType = coinType;
                                                transInfo.txid = json["txid"].ToString();
                                                transInfo.deployTxid = (string)res[0]["txid"];
                                                transInfo.confirmcount = 1;
                                                transInfo.value = (decimal)json["value"];
                                                transInfo.toAddress = json["address"].ToString();
                                                transInfo.height = Config.GetNeoHeightAsync().Result;
                                                transInfo.deployTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                DbHelper.SaveDeployInfo(transInfo);
                                            }
                                            else
                                            {
                                                DbHelper.SaveDeployInfo(res[0]["txid"].ToString(), json["txid"].ToString(), coinType);
                                            }

                                            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                                            { state = "true", txid = res[0]["txid"] }));
                                            Program.logger.Log("Nep5 " + coinType + " Deployed,txid: " + res[0]["txid"]);
                                        }

                                        else //转账出错
                                        {
                                            buffer = Encoding.UTF8.GetBytes(
                                                JsonConvert.SerializeObject(new { state = "false", msg = deployResult }));
                                            Program.logger.Log("Nep5 " + coinType + " Deployed, result: " +
                                                              deployResult);
                                        }
                                    }

                                }
                                else //已发行
                                {
                                    Program.logger.Log("Already deployed! txid: " + deployTxid);
                                    buffer = Encoding.UTF8.GetBytes(
                                        JsonConvert.SerializeObject(new { state = "true", txid = deployTxid }));
                                }
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Program.logger.Log("Url: " + rawUrl + "; json: " + json);
                    Program.logger.Log(e.ToString());
                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "false", msg = e.ToString() }));
                    continue;
                }
                finally
                {
                    requestContext.Response.StatusCode = 200;
                    requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    requestContext.Response.ContentType = "application/json";
                    requestContext.Response.ContentEncoding = Encoding.UTF8;
                    requestContext.Response.ContentLength64 = buffer.Length;
                    var output = requestContext.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }


        /// <summary>
        /// 发送比特币交易
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static string SendBtcTrans(JObject json)
        {
            var result = string.Empty;
            var uri = new Uri(Config.apiDic["btc"]);

            var btcPriKey = new BitcoinSecret(json["priKey"].ToString());
            var client = new QBitNinjaClient(Config.nettype);

            var txidArr = json["txid"].ToString().Split(',');

            var transaction = Transaction.Create(Config.nettype);
            var minerFee = Config.minerFeeDic["btc"];
            //BitcoinPubKeyAddress pubKeyAddress = new BitcoinPubKeyAddress(json["to"].ToString());
            var receiveAddress = BitcoinAddress.Create(Config.myAccountDic["btc"], Config.nettype);
            var amount = Money.Zero;

            foreach (var txid in txidArr)
            {
                var transactionId = uint256.Parse(txid);
                var transactionResponse = client.GetTransaction(transactionId).Result;
                foreach (var rec in transactionResponse.ReceivedCoins)
                {
                    if (rec.TxOut.ScriptPubKey == btcPriKey.ScriptPubKey)
                    {
                        var txInAmount = (Money)transactionResponse.ReceivedCoins[(int)rec.Outpoint.N].Amount;
                        transaction.Inputs.Add(new TxIn()
                        {
                            PrevOut = rec.Outpoint,
                            ScriptSig = btcPriKey.ScriptPubKey
                        });
                        amount += txInAmount;
                    }
                }
            }

            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Coins(amount.ToDecimal(MoneyUnit.BTC) - minerFee),
                ScriptPubKey = receiveAddress.ScriptPubKey
            });

            transaction.Sign(btcPriKey, false);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
            {
                result = "Error message: " + broadcastResponse.Error.Reason;
            }
            else
            {
                result = transaction.GetHash().ToString();
            }

            return result;
        }

        /// <summary>
        /// 发送以太坊交易
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static async Task<string> SendEthTrans(JObject json)
        {
            var account = new Account(json["priKey"].ToString()); // or load it from your keystore file as you are doing.
            var web3 = new Web3(account, Config.apiDic["eth"]);

            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(json["account"].ToString());
            var balanceEther = Web3.Convert.FromWei(balanceWei);
            var gasfee = (decimal)Config.minerFeeDic["eth"];
            var value = balanceEther - gasfee;
            if (value <= 0)
                return "Error,not enougt money!";
            var sendValue = new HexBigInteger(Web3.Convert.ToWei(value));
            var sendTxHash = await web3.Eth.TransactionManager.SendTransactionAsync(account.Address, Config.myAccountDic["eth"], sendValue);
            return sendTxHash;
        }
    }
}
