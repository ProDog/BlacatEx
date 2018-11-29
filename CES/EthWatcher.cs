using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;

namespace CES
{
    public class EthWatcher
    {
        private static List<TransactionInfo> ethTransRspList = new List<TransactionInfo>(); //ETH 交易列表
        private static Logger ethLogger;
        /// <summary>
        /// ETH转账监听服务
        /// </summary>
        public static async void EthWatcherStartAsync()
        {
            ethLogger = new Logger($"{DateTime.Now:yyyy-MM-dd}_eth.log");
            DbHelper.GetRspList(ref ethTransRspList, Config.confirmCountDic["eth"], "eth");
            ethLogger.Log("Eth Watcher Start! Index: " + Config.ethIndex);

            Web3Geth web3 = new Web3Geth(Config.apiDic["eth"]);
            while (true)
            {
                try
                {
                    var aa = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                    var height = aa.Value;

                    if (height >= Config.ethIndex)
                    {
                        for (int i = Config.ethIndex; i <= height; i++)
                        {
                            if (Config.ethIndex % 100 == 0)
                            {
                                ethLogger.Log("Parse ETH Height:" + Config.ethIndex);
                            }

                            await ParseEthBlock(web3, i);
                            await DbHelper.SaveIndexAsync(i, "eth");
                            Config.ethIndex = i + 1;
                        }
                    }
                    if (height == Config.ethIndex)
                        Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    ethLogger.Log(e.Message);
                    Thread.Sleep(3000);
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
                            ethLogger.Log(index + " Have An ETH Transaction To:" + tran.To.ToString() + "; Value:" + value + "; Txid:" + ethTrans.txid);
                        }
                    }
                }
            }

            if (ethTransRspList.Count > 0)
            {
                //更新确认次数
                await CheckEthConfirmAsync(Config.confirmCountDic["eth"], ethTransRspList, index, web3);
                //发送和保存交易信息
                await MyHelper.SendTransInfoAsync(ethTransRspList, ethLogger);
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
    }
}
