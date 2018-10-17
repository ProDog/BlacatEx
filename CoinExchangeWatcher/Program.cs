using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchangeWatcher;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using Nethereum.Geth;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace CoinExchange
{
    class Program
    {
        private static List<string> btcAddrList = new List<string>(); //BTC监听地址列表
        private static List<string> ethAddrList = new List<string>();  //ETH监听地址列表
        private static Dictionary<string, int> confirmCountDic = new Dictionary<string, int>();  //各币种确认次数
        private static Dictionary<string, decimal> minerFeeDic = new Dictionary<string, decimal>();//矿工费
        private static string httpUrl = "http://+:7080/"; //接收新地址 url
        private static string sendTranUrl = "http://0.0.0.0:0000/send/"; //发送交易信息 url
        private static string btcRpcUrl = "http://xx.xx.xx.xx:xxxx";  //BTC RPC url
        private static string ethRpcUrl = "http://xx.xx.xx.xx:xxxx/";  //ETH RPC url
        private static int btcIndex = 1438630; //BTC 监控高度
        private static int ethIndex = 6525300; //ETH监控高度
        private static string dbName = "MonitorData.db";  //Sqlite 数据库名
        private static List<TransResponse> btcTransRspList = new List<TransResponse>(); //BTC 交易列表
        private static List<TransResponse> ethTransRspList = new List<TransResponse>(); //ETH 交易列表
        private static Network nettype = Network.TestNet;
        private static string time = DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss") + " ";

        static void Main(string[] args)
        {
            Console.WriteLine(time + "Hello Boy!");
            DbHelper.CreateDb(dbName);
            var confirmOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            confirmCountDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(confirmOj["confirm_count"].ToString());
            minerFeeDic = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(confirmOj["miner_fee"].ToString());
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
            var key = new System.Net.NetworkCredential("1","1");
            var uri = new Uri(btcRpcUrl);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (true)
            {
                var count = await rpcC.GetBlockCountAsync();
                if (count > btcIndex)
                {
                    for (int i = btcIndex; i <= count; i++)
                    {
                        if (i % 10 == 0)
                            Console.WriteLine(time + "Parse BTC Height:" + i);
                        await ParseBtcBlock(rpcC, i);
                        DbHelper.SaveIndex(i, "btc");
                        btcIndex = i + 1;
                    }
                }

                if (count == btcIndex)
                    Thread.Sleep(10000);
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
                                Console.WriteLine(time + index + " Have a btc transfer for:" + address + "; amount:" + vout.Value.ToDecimal(MoneyUnit.BTC));
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
            Web3Geth web3 = new Web3Geth(ethRpcUrl);

            while (true)
            {
                var sync = await web3.Eth.Syncing.SendRequestAsync();
                if (sync.CurrentBlock == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (sync.CurrentBlock.Value > ethIndex)
                {
                    for (int i = ethIndex; i <= sync.CurrentBlock.Value; i++)
                    {
                        if (ethIndex % 100 == 0)
                            Console.WriteLine(time + "Parse ETH Height:" + ethIndex);
                        await ParseEthBlock(web3, i);
                        DbHelper.SaveIndex(i, "eth");
                        ethIndex = i + 1;
                    }
                }
                if (sync.CurrentBlock.Value == ethIndex)
                    Thread.Sleep(10000);
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
                            Console.WriteLine(time + index + " Have a eth transfer for:" + tran.To.ToString() + "; amount:" + value);
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
                    meStream.Read(dataBytes, 0, (int)meStream.Length);
                    Encoding.UTF8.GetString(dataBytes);

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sendTranUrl);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = dataBytes;
                    req.ContentLength = data.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }

                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    Stream stream = resp.GetResponseStream();

                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = reader.ReadToEnd();
                        if (result.Contains("errCode"))
                            Console.WriteLine(time + result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(time + "senderror:" + ex.ToString());
                    File.WriteAllText("sendErrLog.txt", ex.ToString());
                    return;
                }

                //保存交易信息
                DbHelper.SaveTransInfo(transRspList);
            }

        }


        private static HttpListener httpPostRequest = new HttpListener();
        /// <summary>
        /// 新地址接收
        /// </summary>
        private static void HttpServerStart()
        {
            httpPostRequest.Prefixes.Add(httpUrl);
            httpPostRequest.Start();
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
        }

        private static void httpPostRequestHandle()
        {
            while (true)
            {
                try
                {
                    httpPostRequest.Start();
                    HttpListenerContext requestContext = httpPostRequest.GetContext();
                    StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                    var urlPara = requestContext.Request.RawUrl.Split('/');

                    var json = new JObject();
                    if (requestContext.Request.HttpMethod == "POST")
                    {
                        var info = sr.ReadToEnd();
                        json = Newtonsoft.Json.Linq.JObject.Parse(info);
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {state = "false", msg = "request error,please check your url or post data!"}));

                    if (urlPara.Length > 1)
                    {
                        var method = urlPara[1];
                        if (method == "getaccount")
                        {
                            var coinType = urlPara[2];
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
                                    address = new Nethereum.Web3.Accounts.Account(ethPrikey).Address;
                                    break;
                                default:
                                    priKey = null;
                                    address = null;
                                    break;
                            }

                            buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new
                                {state = "true", type = coinType, address = address, priKey}));
                        }

                        if (method == "addr")
                        {
                            DbHelper.SaveAddress(json);
                            if (json["type"].ToString() == "btc")
                                btcAddrList.Add(json["address"].ToString());
                            if (json["type"].ToString() == "eth")
                                ethAddrList.Add(json["address"].ToString());
                            Console.WriteLine("Add a new " + json["type"].ToString() + " address: " +
                                              json["address"].ToString());
                            buffer = System.Text.Encoding.UTF8.GetBytes(
                                Newtonsoft.Json.JsonConvert.SerializeObject(new {state = "true"}));
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
                                    SendEthTrans(json);
                                    break;
                                default:
                                    return;
                            }

                            buffer = System.Text.Encoding.UTF8.GetBytes(
                                Newtonsoft.Json.JsonConvert.SerializeObject(new {state = "true", msg}));
                        }
                    }

                    requestContext.Response.StatusCode = 200;
                    requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    requestContext.Response.ContentType = "application/json";
                    requestContext.Response.ContentEncoding = Encoding.UTF8;
                    requestContext.Response.ContentLength64 = buffer.Length;
                    var output = requestContext.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(time + e.ToString());
                    return;
                }
            }
        }

        private static string SendBtcTrans(JObject json)
        {
            var result = string.Empty;
            //var key = new System.Net.NetworkCredential("1", "1");
            //var uri = new Uri(btcRpcUrl);
            //NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);
            var uri = new Uri(btcRpcUrl);

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
            var receiveAddress = BitcoinAddress.Create(json["to"].ToString(), nettype);
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

            //using (var node = Node.Connect(nettype)) //Connect to the node
            //{
            //    node.VersionHandshake(); //Say hello
            //    //Advertize your transaction (send just the hash)
            //    node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
            //    //Send it
            //    node.SendMessage(new TxPayload(transaction));
            //    Thread.Sleep(500); //Wait a bit

            //}

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
            {
                result = "send error, Error message: " + broadcastResponse.Error.Reason;
            }
            else
            {
                result = "send success, txid:" + transaction.GetHash();
            }
           
            Console.WriteLine(result);
            return result;
        }

        private static async System.Threading.Tasks.Task SendEthTrans(JObject json)
        {
            //const int UNLOCK_TIMEOUT = 2 * 60; // 2 minutes (arbitrary)
            //var account = new ManagedAccount(json["address"].ToString(), json["prikey"].ToString());
            //var web3 = new Web3(account,ethRpcUrl);
            //await web3.TransactionManager.SendTransactionAsync(account.Address, "", new HexBigInteger(20));
            var web3 = new Web3(ethRpcUrl);
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(json["account"].ToString());
            var balanceEther = Web3.Convert.FromWei(balanceWei);

            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(json["account"].ToString(), json["priKey"].ToString(), 2 * 60);
            var sendTxHash = await web3.Eth.TransactionManager.SendTransactionAsync(json["account"].ToString(), "to", new HexBigInteger(balanceWei));
           
        }
    }
}
