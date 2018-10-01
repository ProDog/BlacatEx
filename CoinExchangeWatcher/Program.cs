using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private static HttpListener httpPostRequest = new HttpListener();
        private static List<string> btcAddrList = new List<string>();
        private static List<string> ethAddrList = new List<string>();
        private static Dictionary<string, int> confirmCountDic = new Dictionary<string, int>();
        private static string getPostUrl = "http://127.0.0.1:30000/newaddr/";
        private static string btcRpcUrl = "http:";
        private static string ethRpcUrl = "http:";
        private static int btcHeight = 1;
        private static int ethHeight = 1;
        private static string dbName = "MonitorData.db";
        private static List<TransResponse> btcTransRspList = new List<TransResponse>();
        private static List<TransResponse> ethTransRspList = new List<TransResponse>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DbHelper.CreateDb(dbName);
            var confirmOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            confirmCountDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(confirmOj["confirm_count"].ToString());

            var btcAddr = File.ReadAllLines("BTCAddress.txt").ToArray();
            foreach (var s in btcAddr)
            {
                btcAddrList.Add(s);
            }
            var ethAddr = File.ReadAllLines("ETHAddress.txt").ToArray();
            foreach (var s in ethAddr)
            {
                ethAddrList.Add(s);
            }

            Thread BtcThread = new Thread(BtcWatcherStartAsync);
            Thread EthThread = new Thread(EthWatcherStartAsync);
            Thread HttpThread = new Thread(HttpServerStart);
            BtcThread.Start();
            //EthThread.Start();
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

            while (true)
            {
                var count = await rpcC.GetBlockCountAsync();
                if (count > btcHeight)
                {
                    for (int i = btcHeight; i < count; i++)
                    {
                        //if (btcHeight % 10 == 0)
                            Console.WriteLine("Parse BTC Height:" + btcHeight);
                        await ParseBtcBlock(rpcC, i);
                    }
                }
            }
        }

        /// <summary>
        /// 解析一个比特币区块
        /// </summary>
        /// <param name="rpcC"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static async Task ParseBtcBlock(NBitcoin.RPC.RPCClient rpcC, int index)
        {
            var block = await rpcC.GetBlockAsync(index);
            //Console.WriteLine("TransCount:" + block.Transactions.Count);
            if (block.Transactions.Count > 0 && btcAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Count; i++)
                {
                    var tran = block.Transactions[i];
                    for (var vo = 0; vo < tran.Outputs.Count; vo++)
                    {
                        var vout = tran.Outputs[vo];
                        var address = vout.ScriptPubKey.GetDestinationAddress(rpcC.Network); //注意比特币地址和网络有关，testnet 和 mainnet 地址不通用
                        
                        for (int j = 0; j < btcAddrList.Count; j++)
                        {
                            if (address?.ToString() == btcAddrList[j])
                            {
                                Console.WriteLine("Have a btc transfer for:" + address + "; amount:" + vout.Value);
                                var btcTrans = new TransResponse();
                                btcTrans.coinType = "btc";
                                btcTrans.address = address.ToString();
                                btcTrans.value = vout.Value.ToDecimal(MoneyUnit.BTC);
                                btcTrans.confirmcount = 1;
                                btcTrans.height = index;
                                btcTrans.txid = tran.ToHex();
                                btcTransRspList.Add(btcTrans);
                            }
                        }
                    }
                }
            }
            if (btcTransRspList.Count > 0)
            {
                CheckBtcConfirm(confirmCountDic["btc"], btcTransRspList, index);
                SendTransInfo(btcTransRspList);
            }
            btcHeight = index;
        }

        private static void CheckBtcConfirm(int num, List<TransResponse> btcTransRspList, int index)
        {
            foreach (var btcTran in btcTransRspList)
            {
                if (btcTran.height == index)
                    return;
                for (int i = 1; i <= num; i++)
                {
                    if (index - btcTran.height == i)
                    {

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
                if (sync.CurrentBlock.Value >= ethHeight)
                {
                    for (int i = ethHeight; i <= sync.CurrentBlock.Value; i++)
                    {
                        if (ethHeight % 1000 == 0)
                            Console.WriteLine("Parse ETH Height:" + ethHeight);
                        await ParseEthBlock(web3, i);
                    }
                }
            }
        }

        /// <summary>
        /// 解析一个ETH区块
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
                        }
                    }
                }
            }

            ethHeight = index;
        }

        private static void SendTransInfo(List<TransResponse> transRspList)
        {
            if (transRspList.Count > 0)
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新地址接收
        /// </summary>
        private static void HttpServerStart()
        {
            httpPostRequest.Prefixes.Add(getPostUrl);
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

                    Console.WriteLine("Add a new " + json["type"].ToString() + " address: " + json["address"].ToString());
                }
                httpPostRequest.Stop();
            }
        }
    }
}
