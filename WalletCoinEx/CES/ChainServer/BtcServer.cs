using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using NBitcoin;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace CES
{
    public class BtcServer
    {
        private static List<TransactionInfo> btcTransRspList = new List<TransactionInfo>(); //BTC 交易列表
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 比特币转账监听服务
        /// </summary>
        public static void Start()
        {
            Helper.DbHelper.GetRspList(ref btcTransRspList, Config.confirmCountDic["btc"], "btc");
            Logger.Info("Btc Watcher Start! Index: " + Config.btcIndex);
            
            var key = new System.Net.NetworkCredential("1", "1");
            var uri = new Uri(Config.apiDic["btc"]);
            NBitcoin.RPC.RPCClient rpcC = new NBitcoin.RPC.RPCClient(key, uri);

            while (true)
            {
                try
                {
                    var count = rpcC.GetBlockCountAsync().Result;

                    while (Config.btcIndex <= count)
                    {
                        if (Config.btcIndex % 10 == 0)
                        {
                            Logger.Info("Parse BTC Height:" + Config.btcIndex);
                        }
                        ParseBtcBlock(rpcC, Config.btcIndex);
                        Helper.DbHelper.SaveIndex(Config.btcIndex, "btc");
                        Config.btcIndex++;
                    }

                    if (count + 1 == Config.btcIndex)
                        Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Error("btc " + e.Message);
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
        private static void ParseBtcBlock(NBitcoin.RPC.RPCClient rpcC, int index)
        {
            var block = rpcC.GetBlockAsync(index).Result;

            if (block.Transactions.Count > 0 && Config.btcAddrList.Count > 0)
            {
                for (var i = 0; i < block.Transactions.Count; i++)
                {
                    var tran = block.Transactions[i];
                    for (var vo = 0; vo < tran.Outputs.Count; vo++)
                    {
                        var vout = tran.Outputs[vo];
                        var address = vout.ScriptPubKey.GetDestinationAddress(Config.nettype); //比特币地址和网络有关

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
                                Logger.Info(index + " Have A BTC Transaction To:" + address + "; Value:" + vout.Value.ToDecimal(MoneyUnit.BTC) + "; Txid:" + btcTrans.txid);
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
                Helper.Helper.SendTransInfo(btcTransRspList);
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
        /// 发送比特币交易
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string SendBtcTrans(JObject json)
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

        public static AccountInfo GetBtcAccount()
        {
            var btcPrikey = new Key();
            var  priKey = btcPrikey.GetWif(Config.nettype).ToString();
            var address = btcPrikey.PubKey.GetAddress(Config.nettype).ToString();
            return new AccountInfo() { coinType = "btc", prikey = priKey, address = address };
        }
    }
}
