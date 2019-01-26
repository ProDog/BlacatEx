using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using log4net;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class NeoServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void Start()
        {
            Logger.Info("Neo Watcher Start! Index: " + Config.neoIndex);
            while (true)
            {
                try
                {
                    var count = Config.GetNeoHeight();
                    while (Config.neoIndex < count)
                    {
                        if (Config.neoIndex % 100 == 0)
                        {
                            Logger.Info("Parse NEO Height:" + Config.neoIndex);
                        }
                        var transRspList = ParseNeoBlock(Config.neoIndex, Config.myAccountDic["cneo"]);
                        Helper.Helper.SendTransInfo(transRspList);
                        Helper.DbHelper.SaveIndex(Config.neoIndex, "neo");
                        Config.neoIndex++;
                    }

                    if (count + 1 == Config.neoIndex)
                        Thread.Sleep(1000);

                }
                catch (Exception e)
                {
                    Logger.Error("neo " + e.Message);
                    Thread.Sleep(5000);
                    continue;
                }
            }
        }

        private static List<TransactionInfo> ParseNeoBlock(int i, string address)
        {
            var transRspList = new List<TransactionInfo>();
            var block = _getBlock(i);
            var txs = (JArray)block["tx"];
            foreach (JObject tx in txs)
            {
                var txid = (string)tx["txid"];
                var type = (string)tx["type"];
                if (type == "InvocationTransaction")
                {
                    var notify = _getNotify(txid);
                    if (notify != null && notify.Count > 0)
                    {
                        foreach (JObject n in notify)
                        {
                            //过滤 事件太多，只监视关注的合约
                            var contract = (string) n["contract"];
                            if (contract != "0x" + Config.tokenHashDic["cneo"])
                                continue;

                            var value = n["state"] as JObject;
                            var method = (value["value"] as JArray)[0] as JObject;
                            var name = Encoding.UTF8.GetString(
                                ThinNeo.Helper.HexString2Bytes((string) method["value"]));

                            if (name == "transfer")
                            {
                                var to = (value["value"] as JArray)[2] as JObject;
                                if (string.IsNullOrEmpty((string) to["value"]))
                                    continue;
                                var to_address =
                                    Helper_NEO.GetAddress_FromScriptHash(ThinNeo.Helper.HexString2Bytes((string) to["value"]));
                                if (to_address == address)
                                {
                                    var neoTrans = new TransactionInfo();
                                    var from = (value["value"] as JArray)[1] as JObject;
                                    var from_address =
                                        Helper_NEO.GetAddress_FromScriptHash(
                                            ThinNeo.Helper.HexString2Bytes((string) from["value"]));
                                    var amount = (value["value"] as JArray)[3] as JObject;
                                    var transAmount =
                                        (decimal) new BigInteger(
                                            ThinNeo.Helper.HexString2Bytes((string) amount["value"])) /
                                        Config.factorDic["cneo"];
                                    neoTrans.toAddress = address;
                                    neoTrans.coinType = "cneo";
                                    neoTrans.confirmcount = 1;
                                    neoTrans.fromAddress = from_address;
                                    neoTrans.height = i;
                                    neoTrans.txid = txid;
                                    neoTrans.value = transAmount;
                                    transRspList.Add(neoTrans);
                                    Logger.Info(i + " Aave A Cneo Transaction From :" + from_address +
                                                  "; Value:" + transAmount + "; Txid:" + txid);

                                }
                            }
                        }
                    }
                }
            }

            return transRspList;

        }

        static JObject _getBlock(int block)
        {
            WebClient wc = new WebClient();
            var getcounturl = Config.apiDic["neo"] + "?jsonrpc=2.0&id=1&method=getblock&params=[" + block + ",1]";
            var info = wc.DownloadString(getcounturl);
            var json = JObject.Parse(info);
            if (info.Contains("result") == false)
                return null;
            return (JObject)(((JArray)json["result"])[0]);
        }

        static JArray _getNotify(string txid)
        {
            WebClient wc = new WebClient();

            var getcounturl = Config.apiDic["neo"] + "?jsonrpc=2.0&id=1&method=getnotify&params=[\"" + txid + "\"]";
            var info = wc.DownloadString(getcounturl);
            var json = JObject.Parse(info);
            if (json.ContainsKey("result") == false)
                return null;
            var result = (JObject)(((JArray)json["result"])[0]);
            var executions = ((JArray)result["executions"])[0] as JObject;

            return executions["notifications"] as JArray;

        }

        private static Dictionary<string, string> usedUtxoDic = new Dictionary<string, string>(); //本区块内同一账户已使用的 UTXO 记录
        private static Dictionary<string, List<Utxo>> dic_UTXO = new Dictionary<string, List<Utxo>>();
        private static List<Utxo> list_Gas = new List<Utxo>();

        /// <summary>
        /// 获取 Nep5 资产余额
        /// </summary>
        /// <param name="coinType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static decimal GetNep5Balanc(string coinType, byte[] data)
        {
            decimal balance = 0;
            string script = ThinNeo.Helper.Bytes2HexString(data);
            var result = Helper.Helper.HttpGet($"{Config.apiDic["neo"]}?method=invokescript&id=1&params=[\"{script}\"]");
            var res = JObject.Parse(result)["result"] as JArray;
            if (res.Count > 0)
            {
                var stack = (res[0]["stack"] as JArray)[0] as JObject;
                var vBanlance = new BigInteger(ThinNeo.Helper.HexString2Bytes((string)stack["value"]));
                balance = (decimal)vBanlance / Config.factorDic[coinType];
            }

            return balance;
        }

        public static string DeployNep5Coin(string coinType, JObject json)
        {
            var type = json["coninType"].ToString();
            byte[] script;
            var prikey = Helper_NEO.GetPrivateKeyFromWIF(Config.adminWifDic[type]);
            using (var sb = new ScriptBuilder())
            {
                var amount = Math.Round((decimal)json["value"] * Config.factorDic[type], 0);
                var array = new JArray();
                array.Add("(addr)" + json["address"]);
                array.Add("(int)" + amount); //value
                byte[] randomBytes = new byte[32];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                BigInteger randomNum = new BigInteger(randomBytes);
                sb.EmitPushNumber(randomNum);
                sb.Emit(ThinNeo.VM.OpCode.DROP);
                sb.EmitParamJson(array); //参数倒序入
                sb.EmitPushString("deploy"); //参数倒序入
                sb.EmitAppCall(new Hash160(Config.tokenHashDic[type])); //nep5脚本
                script = sb.ToArray();
            }

            //return SendTransWithoutUtxo(prikey, script);
            return SendTransaction(prikey, script);
        }

        /// <summary>
        /// 带交易费的 Nep5 资产转账
        /// </summary>
        /// <param name="prikey"></param>
        /// <param name="script"></param>
        /// <param name="to"></param>
        /// <param name="gasfee"></param>
        /// <returns></returns>
        private static string SendTransaction(byte[] prikey, byte[] script)
        {
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

            if (dic_UTXO.ContainsKey(Config.tokenHashDic["gas"]) == false || list_Gas.Count - 10 < usedUtxoDic.Count)
            {
                dic_UTXO = Helper.NeoHelper.GetBalanceByAddress(Config.apiDic["neo"], address, ref usedUtxoDic);
            }

            if (dic_UTXO.ContainsKey(Config.tokenHashDic["gas"]) == false)
            {
                throw new Exception("no gas.");
            }
            list_Gas = dic_UTXO[Config.tokenHashDic["gas"]];
            //MakeTran
            Transaction tran = Helper.NeoHelper.makeTran(ref list_Gas, usedUtxoDic, new Hash256(Config.tokenHashDic["gas"]), Config.minerFeeDic["gas_fee"]);

            //Console.WriteLine($"Utxo:{list_Gas.Count}; usedUtxo:{usedUtxoDic.Count}; inPut:{tran.inputs.Length}; outPut:{tran.outputs.Length}.");
            tran.type = TransactionType.InvocationTransaction;
            var idata = new InvokeTransData();
            tran.extdata = idata;
            idata.script = script;
            idata.gas = 0;


            //sign and broadcast
            var signdata = Helper_NEO.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            string txid = tran.GetHash().ToString();
            foreach (var item in tran.inputs)
            {
                usedUtxoDic[((Hash256)item.hash).ToString() + item.index] = txid;
            }
            string input = @"{
	            'jsonrpc': '2.0',
                'method': 'sendrawtransaction',
	            'params': ['#'],
	            'id': '1'
            }";
            input = input.Replace("#", strtrandata);

            var result = Helper.Helper.PostAsync(Config.apiDic["neo"], input, Encoding.UTF8, 1).Result;
            return result;
        }

        private static string getAddressFromWif(string strWif)
        {
            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(strWif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            return Helper_NEO.GetAddress_FromPublicKey(pubkey);
        }
    }
}
