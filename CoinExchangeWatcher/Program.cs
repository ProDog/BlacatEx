using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin.RPC;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;

namespace CoinExchange
{
    class Program
    {
        private static HttpListener httpPostRequest = new HttpListener();
        private static List<string> btcAddrList = new List<string>();
        private static List<string> ethAddrList = new List<string>();
        private static string getPostUrl = "http://127.0.0.1:30000/newaddr/";
        private static string btcRpcUrl = "http://127.0.0.1:8332";
        private static string ethRpcUrl = "http://127.0.0.1:8545/";
        private static int bitCoinHeight = 296261;
        private static int ethHeight = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpServerStart();
            BitCoinWatcherStartAsync();
            EthWatcherStart();

        }

        /// <summary>
        /// 比特币转账监听服务
        /// </summary>
        private static async void BitCoinWatcherStartAsync()
        {
            var key = new System.Net.NetworkCredential("1", "1");
            var uri = new Uri(btcRpcUrl);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (true)
            {
                var count = await rpcC.GetBlockCountAsync();
                if (count > bitCoinHeight)
                {
                    for (int i = bitCoinHeight; i < count; i++)
                    {
                        await ParseBtcBlock(rpcC, i);
                        Console.WriteLine("BTC:" + bitCoinHeight);
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
            if (block.Transactions.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Count; i++)
                {
                    var tran = block.Transactions[i];
                    
                    for (var vi = 0; vi < tran.Inputs.Count; vi++)
                    {
                        //Console.WriteLine("Input" + vi + ":  ref=" + tran.Inputs[vi].PrevOut.Hash.ToString() + " n=" + tran.Inputs[vi].PrevOut.N.ToString("X08"));
                    }
                    
                    for (var vo = 0; vo < tran.Outputs.Count; vo++)
                    {
                        var vout = tran.Outputs[vo];
                        var address = vout.ScriptPubKey.GetDestinationAddress(rpcC.Network); //注意比特币地址和网络有关，testnet 和mainnet地址不通用
                        btcAddrList.ForEach(x =>
                        {
                            if (address.ToString() == x)
                                Console.WriteLine("Have a btc transfer for:" + address + "; amount:" + vout.Value);
                        });
                    }
                }
            }

            bitCoinHeight = index;
        }

        /// <summary>
        /// ETH转账监听服务
        /// </summary>
        private static async void EthWatcherStart()
        {
            Web3Geth web3 = new Web3Geth(ethRpcUrl);
            while (true)
            {
                var sync = await web3.Eth.Syncing.SendRequestAsync();
                if (sync.CurrentBlock.Value > ethHeight)
                {
                    for (int i = ethHeight; i < sync.CurrentBlock.Value; i++)
                    {
                        await ParseEthBlock(web3, i);
                        Console.WriteLine("ETH:" + ethHeight);
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
            if (block.Transactions.Length > 0)
            {
                for (var i = 0; i < block.Transactions.Length; i++)
                {
                    var tran = block.Transactions[i];
                    ethAddrList.ForEach(x =>
                    {
                        if (tran.To == x)
                            Console.WriteLine("Have a eth transfer for:" + tran.To + "; amount:" + tran.Value);
                    });
                    
                    //decimal v = (decimal) tran.Value.Value;
                    //decimal v2 = 1000000000000000000;
                    //Console.WriteLine("tran.value(ETH)=" + (v / v2));
                }
            }

            ethHeight = index;
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

                }
                httpPostRequest.Stop();
            }
        }
    }
}
