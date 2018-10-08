using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchangeWatcher;
using NBitcoin;
using NBitcoin.RPC;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinExchange
{
    class Program
    {
        private static List<string> btcAddrList = new List<string>(); //BTC监听地址列表
        private static List<string> ethAddrList = new List<string>();  //ETH监听地址列表
        private static Dictionary<string, int> confirmCountDic = new Dictionary<string, int>();  //各币种确认次数
        private static string getAddrUrl = "http://127.0.0.1:30000/addr/"; //接收新地址 url
        private static string sendTranUrl = "http://0.0.0.0:0000/send/"; //发送交易信息 url
        private static List<TransResponse> btcTransRspList = new List<TransResponse>(); //BTC 交易列表
        private static List<TransResponse> ethTransRspList = new List<TransResponse>(); //ETH 交易列表
        private static string btcRpcUrl = "http://47.52.192.77:8332";  //BTC RPC url
        private static string ethRpcUrl = "http://47.52.192.77:8545/";  //ETH RPC url
        private static int btcIndex = 1; //BTC 监控高度
        private static int ethIndex = 1; //ETH监控高度
        private static string dbName = "MonitorData.db";  //Sqlite 数据库名

        static void Main(string[] args)
        {
            DbHelper.CreateDb(dbName);
            var confirmOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            confirmCountDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(confirmOj["confirm_count"].ToString());

            btcAddrList = DbHelper.GetBtcAddr();
            ethAddrList = DbHelper.GetEthAddr();

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
            var key = new System.Net.NetworkCredential("1", "1");
            var uri = new Uri(btcRpcUrl);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);
            btcIndex = DbHelper.GetBtcIndex();

            while (true)
            {
                var count = await rpcC.GetBlockCountAsync();
                if (count > btcIndex)
                {
                    for (int i = btcIndex; i <= count; i++)
                    {
                        Console.WriteLine("Parse BTC Height:" + i);
                        await ParseBtcBlock(rpcC, i);
                        btcIndex = i;
                    }
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
                        var address = vout.ScriptPubKey.GetDestinationAddress(rpcC.Network); //比特币地址和网络有关，testnet 和 mainnet 地址不通用

                        for (int j = 0; j < btcAddrList.Count; j++)
                        {
                            if (address?.ToString() == btcAddrList[j])
                            {
                                Console.WriteLine("Have a btc transfer for:" + address + "; amount:" + vout.Value.ToDecimal(MoneyUnit.BTC));
                                var btcTrans = new TransResponse();
                                btcTrans.coinType = "btc";
                                btcTrans.address = address.ToString();
                                btcTrans.value = vout.Value.ToDecimal(MoneyUnit.BTC);
                                btcTrans.confirmcount = 1;
                                btcTrans.height = index;
                                btcTrans.txid = tran.GetHash().ToString();
                                btcTransRspList.Add(btcTrans);
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
            ethIndex = DbHelper.GetEthIndex();

            while (true)
            {
                var sync = await web3.Eth.Syncing.SendRequestAsync();
                if (sync.CurrentBlock.Value >= ethIndex)
                {
                    for (int i = ethIndex; i <= sync.CurrentBlock.Value; i++)
                    {
                        if (ethIndex % 1000 == 0)
                            Console.WriteLine("Parse ETH Height:" + ethIndex);
                        await ParseEthBlock(web3, i);
                        ethIndex = i;
                    }
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
                            Console.WriteLine("Have a eth transfer for:" + tran.To + "; amount:" + value);

                            var ethTrans = new TransResponse();
                            ethTrans.coinType = "eth";
                            ethTrans.address = tran.To.ToString();
                            ethTrans.value = value;
                            ethTrans.confirmcount = 1;
                            ethTrans.height = index;
                            ethTrans.txid = tran.TransactionHash;
                            ethTransRspList.Add(ethTrans);
                        }
                    }
                }
            }

            if (ethTransRspList.Count > 0)
            {
                //更新确认次数
                await CheckEthConfirmAsync(confirmCountDic["eth"], ethTransRspList, index, web3);
                //发送和保存交易信息
                SendTransInfo(btcTransRspList);
                //移除确认次数为 设定数量 和 0 的交易
                ethTransRspList.RemoveAll(x => x.confirmcount == confirmCountDic["btc"] || x.confirmcount == 0);
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
                    //Encoding.UTF8.GetString(dataBytes);

                    //HttpWebRequest req = (HttpWebRequest) WebRequest.Create(sendTranUrl);
                    //req.Method = "POST";
                    //req.ContentType = "application/x-www-form-urlencoded";

                    //byte[] data = dataBytes;
                    //req.ContentLength = data.Length;
                    //using (Stream reqStream = req.GetRequestStream())
                    //{
                    //    reqStream.Write(data, 0, data.Length);
                    //    reqStream.Close();
                    //}

                    //HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    //Stream stream = resp.GetResponseStream();

                    //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    //{
                    //    var result = reader.ReadToEnd();
                    //}
                }
                catch (Exception ex)
                {
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
            httpPostRequest.Prefixes.Add(getAddrUrl);
            httpPostRequest.Start();
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
        }

        private static void httpPostRequestHandle()
        {
            while (true)
            {
                httpPostRequest.Start();
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                var info = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(info))
                {
                    var json = Newtonsoft.Json.Linq.JObject.Parse(info);
                    if (!json.ContainsKey("address"))
                        return;
                    switch (json["type"].ToString())
                    {
                        case "btc":
                            btcAddrList.Add(json["address"].ToString());
                            break;
                        case "eth":
                            ethAddrList.Add(json["address"].ToString());
                            break;
                        default:
                            return;
                    }

                    DbHelper.SaveAddress(json);
                    Console.WriteLine("Add a new " + json["type"].ToString() + " address: " + json["address"].ToString());
                }

                requestContext.Response.StatusCode = 200;
                requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                requestContext.Response.ContentType = "application/json";
                requestContext.Response.ContentEncoding = Encoding.UTF8;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = "true", msg = "send success" }));
                requestContext.Response.ContentLength64 = buffer.Length;
                var output = requestContext.Response.OutputStream; output.Write(buffer, 0, buffer.Length);
                output.Close();
                //httpPostRequest.Stop();
            }
        }
    }
}
