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

namespace CoinExchangeService
{
    class Program
    {
        private static List<string> btcAddrList = new List<string>(); //BTC监听地址列表
        private static List<string> ethAddrList = new List<string>();  //ETH监听地址列表
        private static Dictionary<string, int> confirmCountDic = new Dictionary<string, int>();  //各币种确认次数
        private static Dictionary<string, decimal> minerFeeDic = new Dictionary<string, decimal>();//矿工费
        private static Dictionary<string, string> myAccountDic = new Dictionary<string, string>();//我的收款地址
        private static Dictionary<string, string> apiDic = new Dictionary<string, string>();
        private static int btcIndex = 1440069; //BTC 监控高度
        private static int ethIndex = 3186400; //ETH监控高度
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
            CoinExchange.GetConfig();
            //程序启动时读取监控的地址、上一次解析的区块高度、上次确认数未达到设定数目的交易
            btcAddrList = DbHelper.GetBtcAddr();
            ethAddrList = DbHelper.GetEthAddr();
            btcIndex = DbHelper.GetBtcIndex() + 1;
            ethIndex = DbHelper.GetEthIndex() + 1;
            DbHelper.GetRspList(ref btcTransRspList, confirmCountDic["btc"], "btc");
            DbHelper.GetRspList(ref ethTransRspList, confirmCountDic["eth"], "eth");

            Thread BtcThread = new Thread(BtcWatcherStartAsync);
            Thread EthThread = new Thread(EthWatcherStartAsync);
            Thread HttpThread = new Thread(HttpServerStart);
            BtcThread.Start();
            EthThread.Start();
            HttpThread.Start();
        }

        /// <summary>
        /// 比特币转账监听服务
        /// </summary>
        private static async void BtcWatcherStartAsync()
        {
            Console.WriteLine(Time() + "Btc watcher start!");
            var key = new System.Net.NetworkCredential("1","1");
            var uri = new Uri(apiDic["btc"]);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (true)
            {
                var count = await rpcC.GetBlockCountAsync();
                if (count >= btcIndex)
                {
                    for (int i = btcIndex; i <= count; i++)
                    {
                        if (i % 1 == 0)
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
                                btcTransRspList.Add(btcTrans);
                                Console.WriteLine(Time() + index + " Have a btc transfer for:" + address + "; amount:" + vout.Value.ToDecimal(MoneyUnit.BTC));
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
                btcTransRspList.RemoveAll(x => x.confirmcount == confirmCountDic["btc"] || x.confirmcount == 0);
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
                if (index > btcTran.height && index - btcTran.height < num)
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
            Console.WriteLine(Time() + "Eth watcher start!");
            Web3Geth web3 = new Web3Geth(apiDic["eth"]);
            while (true)
            {
                var sync = await web3.Eth.Syncing.SendRequestAsync();
                if (sync.CurrentBlock == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (sync.CurrentBlock.Value >= ethIndex)
                {
                    for (int i = ethIndex; i <= sync.CurrentBlock.Value; i++)
                    {
                        if (ethIndex % 200 == 0)
                        {
                            Console.WriteLine(Time() + "Parse ETH Height:" + ethIndex);
                        }

                        await ParseEthBlock(web3, i);
                        DbHelper.SaveIndex(i, "eth");
                        ethIndex = i + 1;
                    }
                }
                if (sync.CurrentBlock.Value == ethIndex)
                    Thread.Sleep(3000);
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
                            ethTransRspList.Add(ethTrans);
                            Console.WriteLine(Time() + index + " Have a eth transfer for:" + tran.To.ToString() + "; amount:" + value);
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
                ethTransRspList.RemoveAll(x => x.confirmcount == confirmCountDic["eth"] || x.confirmcount == 0);
            }
        }

        private static async Task CheckEthConfirmAsync(int num, List<TransResponse> ethTransRspList, int index, Web3Geth web3)
        {
            foreach (var ethTran in ethTransRspList)
            {
                if (index > ethTran.height && index - ethTran.height < num)
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
                    Console.WriteLine(Time() + Encoding.UTF8.GetString(data));
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
                    File.WriteAllText("sendErrLog.txt", ex.ToString());
                    return;
                }
                
            }

        }

        private static HttpListener httpPostRequest = new HttpListener();
        
        private static void HttpServerStart()
        {
            Console.WriteLine(Time() + "HttpServer start!");
            httpPostRequest.Prefixes.Add(apiDic["http"]);
            httpPostRequest.Start();
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
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", msg = "request error,please check your url or post data!"}));
                try
                {
                    StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                    var urlPara = requestContext.Request.RawUrl.Split('/');
                    var json = new JObject();
                    if (requestContext.Request.HttpMethod == "POST")
                    {
                        var info = sr.ReadToEnd();
                        json = JObject.Parse(info);
                    }

                    if (urlPara.Length > 1)
                    {
                        var method = urlPara[1];
                        
                        if (method == "addr")
                        {
                            if (json.ContainsKey("type") && json.ContainsKey("address"))
                            {
                                if (json["type"].ToString() == "btc" && !btcAddrList.Contains(json["address"].ToString()))
                                {
                                    btcAddrList.Add(json["address"].ToString());
                                    DbHelper.SaveAddress(json);
                                }
                                if (json["type"].ToString() == "eth" && !ethAddrList.Contains(json["address"].ToString()))
                                {
                                    ethAddrList.Add(json["address"].ToString());
                                    DbHelper.SaveAddress(json);
                                }

                                Console.WriteLine(Time() + "Add a new " + json["type"].ToString() + " address: " +
                                    json["address"].ToString());
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true"}));
                            }
                        }

                        if (method == "trans")
                        {
                            var msg = "";
                            if (!json.ContainsKey("account") || !json.ContainsKey("priKey"))
                                return;
                            switch (json["type"].ToString())
                            {
                                case "btc":
                                    msg = SendBtcTrans(json);
                                    break;
                                case "eth":
                                    var ss = SendEthTrans(json);
                                    msg = ss.Result;
                                    break;
                                default:
                                    msg = "Error: error coin type";
                                    break;
                            }

                            if (msg.Contains("Error"))
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "false", txid = msg}));
                            else
                            {
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid = msg}));
                            }
                            Console.WriteLine(Time() + json["type"].ToString()+ " transaction,txid: " + msg);
                        }

                        if (method == "exchange")
                        {
                            string txid = DbHelper.AssetIsSend(json["txid"].ToString());
                            if (string.IsNullOrEmpty(txid))
                            {
                                txid = CoinExchange.Exchange(json, minerFeeDic["gas_fee"]);
                                DbHelper.SaveExchangeInfo(json, txid);
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid}));
                                Console.WriteLine(Time() + "Exchange Nep5,txid: " + txid);
                            }
                            else
                            {
                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", txid }));
                            }

                            
                        }

                        if (urlPara.Length > 2)
                        {
                            var coinType = urlPara[2];
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
                                        priKey = null;
                                        address = null;
                                        break;
                                }

                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { state = "true", type = coinType, address, priKey }));
                            }

                            if (method == "deploy")
                            {
                                DeployInfo deployInfo = DbHelper.GetDeployStateByTxid(coinType, json["txid"].ToString());
                                if (string.IsNullOrEmpty(deployInfo.deployTime) && string.IsNullOrEmpty(deployInfo.deployTxid)) //没有发行NEP5 BTC/ETH
                                {
                                    var deployTxid = CoinExchange.DeployNep5Token(coinType, json, minerFeeDic["gas_fee"]);
                                    DbHelper.SaveDeployInfo(deployInfo, deployTxid);
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid = deployTxid}));
                                    Console.WriteLine(Time() + "Nep5 BTC Deployed,txid: " + deployTxid);
                                }
                                else //已发行
                                {
                                    buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {state = "true", txid = deployInfo.deployTxid}));
                                }

                            }
                        }
                    }

                }
                catch (Exception e)
                {
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
            
            var transactionId = uint256.Parse(json["txid"].ToString());
            var transactionResponse = client.GetTransaction(transactionId).Result;

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == btcPriKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;

            //BitcoinPubKeyAddress pubKeyAddress = new BitcoinPubKeyAddress(json["to"].ToString());
            var receiveAddress = BitcoinAddress.Create(myAccountDic["btc"], nettype);
            var transaction = Transaction.Create(nettype);
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            var minerFee = minerFeeDic["btc"];

            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Coins(txInAmount.ToDecimal(MoneyUnit.BTC) - minerFee),
                ScriptPubKey = receiveAddress.ScriptPubKey
            });

            transaction.Inputs[0].ScriptSig = btcPriKey.ScriptPubKey;
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

        private static string Time()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ";
        }
    }
}
