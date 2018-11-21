using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;

namespace CES
{
    class Program
    {
        private static List<TransactionInfo> btcTransRspList = new List<TransactionInfo>(); //BTC 交易列表
        private static List<TransactionInfo> ethTransRspList = new List<TransactionInfo>(); //ETH 交易列表
        public static Logger logger;
        public static bool runnig = true;
        
        static void Main(string[] args)
        {
            DbHelper.CreateDb("MonitorData.db");
            Config.Init("config.json");
            string filename = $"{DateTime.Now:yyyy-MM-dd}.log";
            logger = new Logger(filename);
            DbHelper.GetRspList(ref btcTransRspList, Config.confirmCountDic["btc"], "btc");
            DbHelper.GetRspList(ref ethTransRspList, Config.confirmCountDic["eth"], "eth");
            AppStart();
            
            if (Console.ReadLine() == "exit")
            {
                runnig = false;
                Thread.Sleep(10000);
                logger.Dispose();
            }
        }
        
        private static void AppStart()
        {
            Thread BtcThread = new Thread(BtcWatcherStartAsync);
            Thread EthThread = new Thread(EthWatcherStartAsync);
            Thread NeoThread = new Thread(NeoWatcherStart);
            Thread HttpThread = new Thread(HttpHelper.HttpServerStart);
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
            logger.Log("Btc Watcher Start! Index: " + Config.btcIndex);
            var key = new System.Net.NetworkCredential("1","1");
            var uri = new Uri(Config.apiDic["btc"]);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (runnig)
            {
                try
                {
                    var count = await rpcC.GetBlockCountAsync();
                    if (count >= Config.btcIndex)
                    {
                        for (int i = Config.btcIndex; i <= count; i++)
                        {
                            if (i % 1 == 0)
                            {
                                logger.Log("Parse BTC Height:" + i);
                            }

                            await ParseBtcBlock(rpcC, i);
                            DbHelper.SaveIndex(i, "btc");
                            Config.btcIndex = i + 1;
                        }
                    }

                    if (count == Config.btcIndex)
                        Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    if (e.Source == "System.Net.Requests")
                    {
                        logger.Log("Waiting for next btc block.");
                    }
                    else
                    {
                        logger.Log(e.ToString());
                    }

                    Thread.Sleep(10000);
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

            if (block.Transactions.Count > 0 && Config.btcAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Count; i++)
                {
                    var tran = block.Transactions[i];
                    for (var vo = 0; vo < tran.Outputs.Count; vo++)
                    {
                        var vout = tran.Outputs[vo];
                        var address = vout.ScriptPubKey.GetDestinationAddress(Config.nettype); //比特币地址和网络有关，testnet 和 mainnet 地址不通用

                        for (int j = 0; j < Config.btcAddrList.Count; j++)
                        {
                            if (address?.ToString() == Config.btcAddrList[j])
                            {
                                var btcTrans = new TransactionInfo();
                                btcTrans.coinType = "btc";
                                btcTrans.toAddress = address.ToString();
                                btcTrans.value = vout.Value.ToDecimal(MoneyUnit.BTC);
                                btcTrans.confirmcount = 1;
                                btcTrans.height = index;
                                btcTrans.txid = tran.GetHash().ToString();
                                if (btcTransRspList.Exists(x => x.txid == btcTrans.txid))
                                    continue;
                                btcTransRspList.Add(btcTrans);
                                logger.Log(index + " Have A BTC Transaction To:" + address + "; Value:" + vout.Value.ToDecimal(MoneyUnit.BTC) + "; Txid:" + btcTrans.txid);
                            }
                        }
                    }
                }
            }

            if (btcTransRspList.Count > 0)
            {
                //更新确认次数
                CheckBtcConfirm(Config.confirmCountDic["btc"], btcTransRspList, index, rpcC);
                //发送和保存交易信息
                SendTransInfo(btcTransRspList);
                //移除确认次数为 设定数量 和 0 的交易
                btcTransRspList.RemoveAll(x => x.confirmcount >= Config.confirmCountDic["btc"] || x.confirmcount == 0);
            }
        }

        /// <summary>
        /// 检查 BTC 确认次数
        /// </summary>
        /// <param name="num">需确认次数</param>
        /// <param name="btcTransRspList">交易列表</param>
        /// <param name="index">当前解析区块</param>
        /// <param name="rpcC"></param>
        private static void CheckBtcConfirm(int num, List<TransactionInfo> btcTransRspList, int index, NBitcoin.RPC.RPCClient rpcC)
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
            logger.Log("Eth Watcher Start! Index: " + Config.ethIndex);
            Web3Geth web3 = new Web3Geth(Config.apiDic["eth"]);
            while (runnig)
            {
                try
                {
                    var aa = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                    var height = aa.Value;
                    
                    if (height >= Config.ethIndex)
                    {
                        for (int i = Config.ethIndex; i <= height; i++)
                        {
                            if (Config.ethIndex % 1 == 0)
                            {
                                logger.Log("Parse ETH Height:" + Config.ethIndex);
                            }

                            await ParseEthBlock(web3, i);
                            DbHelper.SaveIndex(i, "eth");
                            Config.ethIndex = i + 1;
                        }
                    }
                    if (height == Config.ethIndex)
                        Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    logger.Log(e.ToString());
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
            if (block.Transactions.Length > 0 && Config.ethAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Length; i++)
                {
                    var tran = block.Transactions[i];
                    for (int j = 0; j < Config.ethAddrList.Count; j++)
                    {
                        if (tran.To == Config.ethAddrList[j].ToLower())
                        {
                            decimal v = (decimal)tran.Value.Value;
                            decimal v2 = 1000000000000000000;
                            var value = v / v2;
                            var ethTrans = new TransactionInfo();
                            ethTrans.coinType = "eth";
                            ethTrans.toAddress = tran.To.ToString();
                            ethTrans.value = value;
                            ethTrans.confirmcount = 1;
                            ethTrans.height = index;
                            ethTrans.txid = tran.TransactionHash;
                            if (ethTransRspList.Exists(x => x.txid == ethTrans.txid))
                                continue;
                            ethTransRspList.Add(ethTrans);
                            logger.Log(index + " Have An ETH Transaction To:" + tran.To.ToString() + "; Value:" + value + "; Txid:"+ ethTrans.txid);
                        }
                    }
                }
            }

            if (ethTransRspList.Count > 0)
            {
                //更新确认次数
                await CheckEthConfirmAsync(Config.confirmCountDic["eth"], ethTransRspList, index, web3);
                //发送和保存交易信息
                SendTransInfo(ethTransRspList);
                //移除确认次数为 设定数量 和 0 的交易
                ethTransRspList.RemoveAll(x => x.confirmcount >= Config.confirmCountDic["eth"] || x.confirmcount == 0);
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
        private static async Task CheckEthConfirmAsync(int num, List<TransactionInfo> ethTransRspList, int index, Web3Geth web3)
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
            logger.Log("Neo Watcher Start! Index: " + Config.neoIndex);
            while (runnig)
            {
                try
                {
                    var count = Config.GetNeoHeightAsync().Result;
                    if (count >= Config.neoIndex)
                    {
                        for (int i = Config.neoIndex; i < count; i++)
                        {
                            if (i % 1 == 0)
                            {
                                logger.Log("Parse NEO Height:" + i);
                            }

                            var transRspList = NeoHandler.ParseNeoBlock(i, Config.myAccountDic["cneo"]);
                            SendTransInfo(transRspList);
                            DbHelper.SaveIndex(i, "neo");
                            Config.neoIndex = i + 1;
                        }
                    }

                    if (count == Config.neoIndex)
                        Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    logger.Log(e.ToString());
                    Thread.Sleep(5000);
                    continue;
                }
            }
        }

        /// <summary>
        /// 发送交易数据
        /// </summary>
        /// <param name="transRspList">交易数据列表</param>
        private static void SendTransInfo(List<TransactionInfo> transRspList)
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
                    HttpWebRequest req = (HttpWebRequest) WebRequest.Create(Config.apiDic["blacat"]);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = dataBytes;
                    req.ContentLength = data.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }

                    logger.Log("SendTransInfo : " + Encoding.UTF8.GetString(data));
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = reader.ReadToEnd();
                        var rjson = JObject.Parse(result);
                        logger.Log("rsp: " + result);
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
                    logger.Log("send error:" + ex.ToString());
                    return;
                }
                
            }

        }

    }
}
