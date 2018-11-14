using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Geth;
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
    class Program
    {
        private static List<string> btcAddrList = new List<string>(); //BTC 监听地址列表
        private static List<string> ethAddrList = new List<string>();  //ETH 监听地址列表
        private static Dictionary<string, int> confirmCountDic = new Dictionary<string, int>();  //各币种确认次数
        private static Dictionary<string, decimal> minerFeeDic = new Dictionary<string, decimal>();//矿工费
        private static Dictionary<string, string> myAccountDic = new Dictionary<string, string>();//我的收款地址
        private static Dictionary<string, string> apiDic = new Dictionary<string, string>();
        private static int btcIndex = 1440069; //BTC 监控高度
        private static int ethIndex = 3309077; //ETH 监控高度
        private static int neoIndex = 1979247; //NEO 高度
        private static string dbName = "MonitorData.db";  //Sqlite 数据库名
        private static List<TransResponse> btcTransRspList = new List<TransResponse>(); //BTC 交易列表
        private static List<TransResponse> ethTransRspList = new List<TransResponse>(); //ETH 交易列表
        private static Network nettype = Network.TestNet;

        static void Main(string[] args)
        {
            DbHelper.CreateDb(dbName);
            var configOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            confirmCountDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(configOj["confirm_count"].ToString());
            minerFeeDic = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(configOj["miner_fee"].ToString());
            myAccountDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["my_account"].ToString());
            apiDic= JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["api"].ToString());
            NeoHandler.GetConfig();
            //程序启动时读取监控的地址、上一次解析的区块高度、上次确认数未达到设定数目的交易
            btcAddrList = DbHelper.GetBtcAddr();
            ethAddrList = DbHelper.GetEthAddr();
            btcIndex = DbHelper.GetBtcIndex() + 1;
            ethIndex = DbHelper.GetEthIndex() + 1;
            neoIndex = DbHelper.GetNeoIndex() + 1;
            DbHelper.GetRspList(ref btcTransRspList, confirmCountDic["btc"], "btc");
            DbHelper.GetRspList(ref ethTransRspList, confirmCountDic["eth"], "eth");

            Thread BtcThread = new Thread(BtcWatcherStartAsync);
            Thread EthThread = new Thread(EthWatcherStartAsync);
            Thread HttpThread = new Thread(HttpServerStart);
            Thread NeoThread=new Thread(NeoWatcherStart);
            BtcThread.Start();
            EthThread.Start();
            NeoThread.Start();
            HttpThread.Start();
        }

        /// <summary>
        /// 比特币转账监听服务
        /// </summary>
        private static async void BtcWatcherStartAsync()
        {
            Console.WriteLine(Time() + "Btc Watcher Start! Index: " + btcIndex);
            var key = new System.Net.NetworkCredential("1","1");
            var uri = new Uri(apiDic["btc"]);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (true)
            {
                try
                {
                    var count = await rpcC.GetBlockCountAsync();
                    if (count >= btcIndex)
                    {
                        for (int i = btcIndex; i <= count; i++)
                        {
                            if (i % 10 == 0)
                            {
                                Console.WriteLine(Time() + "Parse BTC Height:" + i);
                            }

                            await ParseBtcBlock(rpcC, i);
                            DbHelper.SaveIndex(i, "btc");
                            btcIndex = i + 1;
                        }
                    }

                    if (count == btcIndex)
                        Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Time() + e);
                    continue;
                }
                
            }
        }

        /// <summary>
        /// 解析比特币区块
        /// </summary>
        /// <param name="rpcC"></param>
        /// <param name="index">被解析区块</param>
        /// <param name="height">区块高度</param>
        /// <returns></returns>
        private static async Task ParseBtcBlock(NBitcoin.RPC.RPCClient rpcC, int index)
        {
            var block = await rpcC.GetBlockAsync(index);

            if (block.Transactions.Count > 0 && btcAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Count; i++)
                {
                    var tran = block.Transactions[i];
                    for (var vo = 0; vo < tran.Outputs.Count; vo++)
                    {
                        var vout = tran.Outputs[vo];
                        var address = vout.ScriptPubKey.GetDestinationAddress(nettype); //比特币地址和网络有关，testnet 和 mainnet 地址不通用

                        for (int j = 0; j < btcAddrList.Count; j++)
                        {
                            if (address?.ToString() == btcAddrList[j])
                            {
                                var btcTrans = new TransResponse();
                                btcTrans.coinType = "btc";
                                btcTrans.address = address.ToString();
                                btcTrans.value = vout.Value.ToDecimal(MoneyUnit.BTC);
                                btcTrans.confirmcount = 1;
                                btcTrans.height = index;
                                btcTrans.txid = tran.GetHash().ToString();
                                if (btcTransRspList.Exists(x => x.txid == btcTrans.txid))
                                    continue;
                                btcTransRspList.Add(btcTrans);
                                Console.WriteLine(Time() + index + " Have A BTC Transaction To:" + address +
                                                  "; Value:" + vout.Value.ToDecimal(MoneyUnit.BTC) + "; Txid:" +
                                                  btcTrans.txid);
                            }
                        }
                    }
                }
            }

            if (btcTransRspList.Count > 0)
            {
                //更新确认次数
                CheckBtcConfirm(confirmCountDic["btc"], btcTransRspList, index, rpcC);
                //发送和保存交易信息
                SendTransInfo(btcTransRspList);
                //移除确认次数为 设定数量 和 0 的交易
                btcTransRspList.RemoveAll(x => x.confirmcount >= confirmCountDic["btc"] || x.confirmcount == 0);
            }
        }

        /// <summary>
        /// 检查 BTC 确认次数
        /// </summary>
        /// <param name="num">需确认次数</param>
        /// <param name="btcTransRspList">交易列表</param>
        /// <param name="index">当前解析区块</param>
        /// <param name="rpcC"></param>
        private static void CheckBtcConfirm(int num, List<TransResponse> btcTransRspList, int index, NBitcoin.RPC.RPCClient rpcC)
        {
            foreach (var btcTran in btcTransRspList)
            {
                if (index > btcTran.height)
                {
                    var block = rpcC.GetBlock(btcTran.height);
                    //如果原区块中还包含该交易，则确认数 = 当前区块高度 - 交易所在区块高度 + 1，不包含该交易，确认数统一记为 0
                    if (block.Transactions.Count > 0 && block.Transactions.Exists(x => x.GetHash().ToString() == btcTran.txid))
                        btcTran.confirmcount = index - btcTran.height + 1;
                    else
                    {
                        btcTran.confirmcount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// ETH转账监听服务
        /// </summary>
        private static async void EthWatcherStartAsync()
        {
            Console.WriteLine(Time() + "Eth Watcher Start! Index: " + ethIndex);
            Web3Geth web3 = new Web3Geth(apiDic["eth"]);
            while (true)
            {
                try
                {
                    var aa = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                    var height = aa.Value;
                    
                    if (height >= ethIndex)
                    {
                        for (int i = ethIndex; i <= height; i++)
                        {
                            if (ethIndex % 100 == 0)
                            {
                                Console.WriteLine(Time() + "Parse ETH Height:" + ethIndex);
                            }

                            await ParseEthBlock(web3, i);
                            DbHelper.SaveIndex(i, "eth");
                            ethIndex = i + 1;
                        }
                    }
                    if (height == ethIndex)
                        Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Time() + e);
                    continue;
                }
               
            }
        }

        /// <summary>
        /// 解析ETH区块
        /// </summary>
        /// <param name="web3"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static async Task ParseEthBlock(Web3Geth web3, int index)
        {
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(index));
            if (block.Transactions.Length > 0 && ethAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Length; i++)
                {
                    var tran = block.Transactions[i];
                    for (int j = 0; j < ethAddrList.Count; j++)
                    {
                        if (tran.To == ethAddrList[j].ToLower())
                        {
                            decimal v = (decimal)tran.Value.Value;
                            decimal v2 = 1000000000000000000;
                            var value = v / v2;
                            var ethTrans = new TransResponse();
                            ethTrans.coinType = "eth";
                            ethTrans.address = tran.To.ToString();
                            ethTrans.value = value;
                            ethTrans.confirmcount = 1;
                            ethTrans.height = index;
                            ethTrans.txid = tran.TransactionHash;
                            if (ethTransRspList.Exists(x => x.txid == ethTrans.txid))
                                continue;
                            ethTransRspList.Add(ethTrans);
                            Console.WriteLine(Time() + index + " Have An ETH Transaction To:" + tran.To.ToString() + "; Value:" + value + "; Txid:"+ ethTrans.txid);
                        }
                    }
                }
            }

            if (ethTransRspList.Count > 0)
            {
                //更新确认次数
                await CheckEthConfirmAsync(confirmCountDic["eth"], ethTransRspList, index, web3);
                //发送和保存交易信息
                SendTransInfo(ethTransRspList);
                //移除确认次数为 设定数量 和 0 的交易
                ethTransRspList.RemoveAll(x => x.confirmcount >= confirmCountDic["eth"] || x.confirmcount == 0);
            }
        }

        /// <summary>
        /// 检查 ETH 确认次数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="ethTransRspList"></param>
        /// <param name="index"></param>
        /// <param name="web3"></param>
        /// <returns></returns>
        private static async Task CheckEthConfirmAsync(int num, List<TransResponse> ethTransRspList, int index, Web3Geth web3)
        {
            foreach (var ethTran in ethTransRspList)
            {
                if (index > ethTran.height)
                {
                    var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(ethTran.height));

                    //如果原区块中还包含该交易，则确认数 = 当前区块高度 - 交易所在区块高度 + 1，不包含该交易，确认数统一记为 0
                    if (block.Transactions.Length > 0 && block.Transactions.ToList().Exists(x => x.TransactionHash.ToString() == ethTran.txid))
                        ethTran.confirmcount = index - ethTran.height + 1;
                    else
                    {
                        ethTran.confirmcount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// NEO 监听服务
        /// </summary>
        private static void NeoWatcherStart()
        {
            Console.WriteLine(Time() + "Neo Watcher Start! Index: " + neoIndex);
            while (true)
            {
                try
                {
                    var count = GetNeoHeight(apiDic["neo"]);
                    if (count >= neoIndex)
                    {
                        for (int i = neoIndex; i < count; i++)
                        {
                            if (i % 100 == 0)
                            {
                                Console.WriteLine(Time() + "Parse NEO Height:" + i);
                            }

                            var transRspList = NeoHandler.ParseNeoBlock(i, myAccountDic["cneo"]);
                            SendTransInfo(transRspList);
                            DbHelper.SaveIndex(i, "neo");
                            neoIndex = i + 1;
                        }
                    }

                    if (count == neoIndex)
                        Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Time() + e);
                    Thread.Sleep(5000);
                    continue;
                }
            }
        }

        /// <summary>
        /// 发送交易数据
        /// </summary>
        /// <param name="transRspList">交易数据列表</param>
        private static void SendTransInfo(List<TransResponse> transRspList)
        {
            if (transRspList.Count > 0)
            {
                try
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(transRspList.GetType());
                    MemoryStream meStream = new MemoryStream();
                    serializer.WriteObject(meStream, transRspList);
                    byte[] dataBytes = new byte[meStream.Length];
                    meStream.Position = 0;
                    meStream.Read(dataBytes, 0, (int) meStream.Length);
                    HttpWebRequest req = (HttpWebRequest) WebRequest.Create(apiDic["blacat"]);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = dataBytes;
                    req.ContentLength = data.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }

                    Console.WriteLine(Time() + "SendTransInfo : " + Encoding.UTF8.GetString(data));
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = reader.ReadToEnd();
                        var rjson = JObject.Parse(result);
                        Console.WriteLine(Time() + "rsp: " + result);
                        if (Convert.ToInt32(rjson["r"]) == 0)
                        {
                            Thread.Sleep(5000);
                            SendTransInfo(transRspList);
                        }

                        if (Convert.ToInt32(rjson["r"]) == 1)
                        {
                            //保存交易信息
                            DbHelper.SaveTransInfo(transRspList);
                        }
                       
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(Time() + "send error:" + ex.ToString());
                    File.WriteAllText("sendErrLog.txt", Time() + ex.ToString());
                    return;
                }
                
            }

        }

        private static HttpListener httpPostRequest = new HttpListener();
        
        private static void HttpServerStart()
        {
            Console.WriteLine(Time() + "Http Server Start!");
            httpPostRequest.Prefixes.Add(apiDic["http"]);
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
        }

        /// <summary>
        /// Http 服务接口
        /// </summary>
        private static void httpPostRequestHandle()
        {
            while (true)
            {
                httpPostRequest.Start();
                bool clear = false;

                HttpListenerContext requestContext = httpPostRequest.GetContext();
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = "request error,please check your url or post data!"}));
                StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                var rawUrl = requestContext.Request.RawUrl;
                var urlPara = rawUrl.Split('/');
                var json = new JObject();
                if (requestContext.Request.HttpMethod == "POST")
                {
                    var info = sr.ReadToEnd();
                    json = JObject.Parse(info);
                }

                try
                {
                    if (urlPara.Length > 1)
                    {
                        Console.WriteLine(Time() + "Url: " + rawUrl + "; json: " + json);
                        var method = urlPara[1];
                        if (method == "addr")
                        {
                            string coinType = json["type"].ToString();
                            string address = json["address"].ToString();
                            if (coinType == "btc" && !btcAddrList.Contains(address))
                            {
                                btcAddrList.Add(address);
                                DbHelper.SaveAddress(coinType, address);
                            }

                            if (coinType == "eth" && !ethAddrList.Contains(address))
                            {
                                ethAddrList.Add(address);
                                DbHelper.SaveAddress(coinType, address);
                            }

                            Console.WriteLine(Time() + "Add a new " + coinType + " address: " + address);
                            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true"}));

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
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = msg}));
                            else
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid = msg}));
                            Console.WriteLine(Time() + json["type"].ToString() + " transaction : " + msg);
                        }

                        if (method == "exchange")
                        {
                            clear = IsClear();
                            string recTxid = json["txid1"].ToString();
                            string sendTxid = DbHelper.GetSendTxid(recTxid);
                            if (string.IsNullOrEmpty(sendTxid))
                            {
                                var coinType = json["type"].ToString();
                                var result = NeoHandler.ExchangeAsync(coinType, json, minerFeeDic["gas_fee"], clear).Result;
                                if (result != null && result.Contains("result"))
                                {
                                    var res = JObject.Parse(result)["result"] as JArray;
                                    sendTxid = (string)res[0]["txid"];
                                }

                                if (sendTxid != null)
                                {
                                    DbHelper.SaveExchangeInfo(recTxid, sendTxid);
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", txid = sendTxid }));
                                    Console.WriteLine(Time() + "Exchange " + coinType + ",txid: " + sendTxid);
                                }
                                else
                                {
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "false", msg = result }));
                                    Console.WriteLine(Time() + "Exchange " + coinType + ",result: " + result);
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
                                    JsonConvert.SerializeObject(new {state = "true", balance}));
                                Console.WriteLine(Time() + "Get " + coinType + " Balance: " + balance);
                            }

                            if (method == "getaccount")
                            {
                                string address = string.Empty;
                                string priKey = string.Empty;
                                switch (coinType)
                                {
                                    case "btc":
                                        var btcPrikey = new Key();
                                        priKey = btcPrikey.GetWif(nettype).ToString();
                                        address = btcPrikey.PubKey.GetAddress(nettype).ToString();
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

                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", type = coinType, address, priKey}));
                            }

                            if (method == "deploy")
                            {
                                if (coinType == "btc" || coinType == "eth" || coinType == "cneo" || coinType == "bct")
                                {
                                    clear = IsClear();
                                    DeployInfo deployInfo =
                                        DbHelper.GetDeployStateByTxid(coinType, json["txid"].ToString());
                                    if (string.IsNullOrEmpty(deployInfo.deployTime) &&
                                        string.IsNullOrEmpty(deployInfo.deployTxid)) //没有发行NEP5 BTC/ETH
                                    {
                                        var deployResult = NeoHandler
                                            .DeployNep5TokenAsync(coinType, json, minerFeeDic["gas_fee"], clear).Result;
                                        if (deployResult != null && deployResult.Contains("result"))
                                        {
                                            var res = JObject.Parse(deployResult)["result"] as JArray;
                                            deployInfo.deployTxid = (string) res[0]["txid"];
                                            if (!string.IsNullOrEmpty(deployInfo.deployTxid))
                                            {
                                                DbHelper.SaveDeployInfo(deployInfo);
                                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                                                    {state = "true", txid = deployInfo.deployTxid}));
                                                Console.WriteLine(Time() + "Nep5 " + coinType + " Deployed,txid: " +
                                                                  deployInfo.deployTxid);
                                            }
                                        }

                                        else
                                        {
                                            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = deployResult}));
                                            Console.WriteLine(Time() + "Nep5 " + coinType + " Deployed, result: " + deployInfo.deployTxid);
                                        }

                                    }
                                    else //已发行
                                    {
                                        Console.WriteLine(Time() + "Already deployed!");
                                        buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid = deployInfo.deployTxid}));
                                    }
                                }
                                else
                                {
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = "coinType error"}));
                                    Console.WriteLine(Time() + "CoinType error! " + coinType);
                                }

                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(Time() + "Url: " + rawUrl + "; json: " + json);
                    Console.WriteLine(Time() + e.ToString());
                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = e.ToString()}));
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
            var uri = new Uri(apiDic["btc"]);

            var btcPriKey = new BitcoinSecret(json["priKey"].ToString());
            var client = new QBitNinjaClient(nettype);

            var txidArr = json["txid"].ToString().Split(',');
           
            var transaction = Transaction.Create(nettype);
            var minerFee = minerFeeDic["btc"];
            //BitcoinPubKeyAddress pubKeyAddress = new BitcoinPubKeyAddress(json["to"].ToString());
            var receiveAddress = BitcoinAddress.Create(myAccountDic["btc"], nettype);
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
            var web3 = new Web3(account, apiDic["eth"]);
            
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(json["account"].ToString());
            var balanceEther = Web3.Convert.FromWei(balanceWei);
            var gasfee = (decimal) minerFeeDic["eth"];
            var value = balanceEther - gasfee;
            var sendValue = new HexBigInteger(Web3.Convert.ToWei(value));
            var sendTxHash = await web3.Eth.TransactionManager.SendTransactionAsync(account.Address, myAccountDic["eth"], sendValue);
            return sendTxHash;
        }

        public static bool IsClear()
        {
            var v = GetNeoHeight(apiDic["neo"]);
            if (v > neoIndex + 1)
            {
                neoIndex = v;
                return true;
            }

            return false;
        }

        public static int GetNeoHeight(string api)
        {
            var url = api + "?method=getblockcount&id=1&params=[]";
            var result = Helper.HttpGet(url).Result;
            var res = Newtonsoft.Json.Linq.JObject.Parse(result)["result"] as Newtonsoft.Json.Linq.JArray;
            int height = (int)res[0]["blockcount"];
            return height;
        }

        private static string Time()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ";
        }
    }
}
