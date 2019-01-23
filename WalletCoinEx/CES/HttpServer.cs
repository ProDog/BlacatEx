using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NBitcoin;
using Neo.VM;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using Zoro;
using Zoro.IO;
using Zoro.Network.P2P.Payloads;
using Zoro.Wallets;
using Transaction = NBitcoin.Transaction;

namespace CES
{
    public class HttpServer
    {
        private static HttpListener httpListener = new HttpListener();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Http 服务接口
        /// </summary>
        public static void Start()
        {
            Logger.Info("Http Server Start!");
            httpListener.Prefixes.Add(Config.apiDic["http"]);
            while (true)
            {
                httpListener.Start();
                HttpListenerContext requestContext = httpListener.GetContext();

                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RspInfo()
                { state = false, msg = null }));

                try
                {
                    //获取客户端传递的参数
                    StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                    var reqMethod = requestContext.Request.RawUrl.Replace("/", "");
                    var data = sr.ReadToEnd();

                    var json = new JObject();
                    if (!string.IsNullOrEmpty(data))
                        json = JObject.Parse(data);

                    Logger.Info($"Have a request:{reqMethod}; post data:{data}");

                    buffer = GetResponse(reqMethod, json);
                }

                catch (Exception e)
                {
                    var rsp = JsonConvert.SerializeObject(new RspInfo() { state = false, msg = e.Message });
                    buffer = Encoding.UTF8.GetBytes(rsp);
                    Logger.Error(rsp);
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

        private static byte[] GetResponse(string reqMethod, JObject json)
        {
            RspInfo rspInfo = new RspInfo();
            switch (reqMethod)
            {
                case "getBalance":
                    rspInfo = GetBalanceRsp(json["coinType"].ToString());
                    break;
                case "getAccount":
                    rspInfo = GetAccountRsp(json["coinType"].ToString());
                    break;
                case "deployNep5":
                    rspInfo = DeployNep5Rsp(json);
                    break;
                case "addAddress":
                    rspInfo = AddAddressRsp(json);
                    break;
                case "gatherCoin":
                    rspInfo = GatherCoinRsp(json);
                    break;
                case "exchange":
                    rspInfo = ExchangeCoinRsp(json);
                    break;
                default:
                    break;
            }          

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rspInfo));
        }

        private static RspInfo ExchangeCoinRsp(JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            string recTxid = json["txid"].ToString();
            var coinType = json["coinType"].ToString();
            UInt160 nep5Hash;
            decimal value = 0;
            string sendTxid = DbHelper.GetSendTxid(recTxid);
            if (string.IsNullOrEmpty(sendTxid))
            {
                nep5Hash = UInt160.Parse(Config.tokenHashDic[coinType]);

                value = Math.Round((decimal)json["value"] * (decimal)100000000.00000000, 0);
                UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(json["address"].ToString());
                ScriptBuilder sb = new ScriptBuilder();

                KeyPair keypair = ZoroHelper.GetKeyPairFromWIF(Config.adminWifDic[coinType]);
                var adminHash = ZoroHelper.GetPublicKeyHashFromWIF(Config.adminWifDic[coinType]);

                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", nep5Hash, adminHash, targetscripthash, new BigInteger(value));
                decimal gas = ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");

                InvocationTransaction tx = ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(gas), Fixed8.One);
                var result = ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
                sendTxid = tx.Hash.ToString();

                var state = (bool)(JObject.Parse(result)["result"]);
                if (state)
                {
                    rspInfo = new RspInfo()
                    {
                        state = true,
                        msg = new TransResult() { transTxid = sendTxid, key = json["key"].ToString() }
                    };
                    DbHelper.SaveExchangeInfo(recTxid, sendTxid);
                }
                else
                {
                    rspInfo = new RspInfo()
                    {
                        state = false,
                        msg = result
                    };
                }
                return rspInfo;
            }
            else
            {
                rspInfo = new RspInfo()
                {
                    state = true,
                    msg = new TransResult() { transTxid = sendTxid, key = json["key"].ToString() }
                };
            }
            return rspInfo;
        }


        private static RspInfo GatherCoinRsp(JObject json)
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
                return new RspInfo { state = false, msg = msg };
            else
                return new RspInfo { state = true, msg = msg };
        }

        private static RspInfo AddAddressRsp(JObject json)
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

            Logger.Info("Add a new " + coinType + " address: " + address);
            return new RspInfo { state = true, msg = "Add a new " + coinType + " address: " + address };
        }

        private static RspInfo DeployNep5Rsp(JObject json)
        {
            string coinType = json["coinType"].ToString();

            if (coinType == "bct")
                return ExchangeCoinRsp(json);

            string oldTxid = json["txid"].ToString();
            string deployTxid = DbHelper.GetDeployStateByTxid(coinType, oldTxid);
            if (string.IsNullOrEmpty(deployTxid)) //没有发放NEP5 BTC/ETH
            {
                var deployResult = ZoroTrans.DeployNep5Coin(coinType, json);

                if (deployResult != null && deployResult.Contains("result"))
                {
                    var res = JObject.Parse(deployResult)["result"] as JArray;

                    if (!string.IsNullOrEmpty((string)res[0]["txid"]))
                    {
                        DbHelper.SaveDeployInfo(res[0]["txid"].ToString(), oldTxid, coinType);
                        return new RspInfo { state = true, msg = new TransResult() { coinType = coinType, key = oldTxid, transTxid = (string)res[0]["txid"] } };
                    }

                    else //转账出错
                        return new RspInfo { state = false, msg = deployResult };

                }
            }
            return new RspInfo { state = true, msg = new TransResult() { coinType = coinType, key = oldTxid, transTxid = deployTxid } };

        }

        private static RspInfo GetAccountRsp(string coinType)
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
            return new RspInfo { state = true, msg = new AccountInfo() { coinType = coinType, prikey = priKey, address = address } };
        }

        private static RspInfo GetBalanceRsp(string coinType)
        {
            var balance = ZoroTrans.GetBalance(coinType);
            return new RspInfo { state = true, msg = new CoinInfon() { coinType = coinType, balance = balance } };
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
