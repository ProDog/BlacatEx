using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;

namespace CES
{
    public class EthWatcher
    {
        private static List<TransactionInfo> ethTransRspList = new List<TransactionInfo>(); //ETH 交易列表
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// ETH转账监听服务
        /// </summary>
        public static void Start()
        {
            DbHelper.GetRspList(ref ethTransRspList, Config.confirmCountDic["eth"], "eth");
            Logger.Info("Eth Watcher Start! Index: " + Config.ethIndex);

            Web3Geth web3 = new Web3Geth(Config.apiDic["eth"]);
            while (true)
            {
                try
                {
                    var aa = web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result;
                    var height = aa.Value;

                    while (Config.ethIndex <= height)
                    {
                        if (Config.ethIndex % 100 == 0)
                        {
                            Logger.Info("Parse ETH Height:" + Config.ethIndex);
                        }
                        ParseEthBlock(web3, Config.ethIndex);
                        DbHelper.SaveIndex(Config.ethIndex, "eth");
                        Config.ethIndex++;
                    }
                    if (height + 1 == Config.ethIndex)
                        Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    Logger.Error("eth " + e.Message);
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
        private static void ParseEthBlock(Web3Geth web3, int index)
        {
            var block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(index)).Result;
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
                            Logger.Info(index + " Have An ETH Transaction To:" + tran.To.ToString() + "; Value:" + value + "; Txid:" + ethTrans.txid);
                            
                        }

                    }

                   
                }
            }

            if (ethTransRspList.Count > 0)
            {
                //更新确认次数
                CheckEthConfirm(Config.confirmCountDic["eth"], ethTransRspList, index, web3);
                //发送和保存交易信息
                MyHelper.SendTransInfo(ethTransRspList);
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
        private static void CheckEthConfirm(int num, List<TransactionInfo> ethTransRspList, int index, Web3Geth web3)
        {
            foreach (var ethTran in ethTransRspList)
            {
                if (index > ethTran.height)
                {
                    var block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(ethTran.height)).Result;

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
