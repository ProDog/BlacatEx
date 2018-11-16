using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class NeoHandler
    {
        private static string api = "https://api.nel.group/api/testnet"; //NEO api
        private static Dictionary<string, string> adminWifDic = new Dictionary<string, string>();//管理员
        private static Dictionary<string, string> tokenHashDic = new Dictionary<string, string>();//token类型
        private static Dictionary<string, decimal> factorDic = new Dictionary<string, decimal>();//精度
        private static List<string> usedUtxoList; //本区块内同一账户已使用的 UTXO 记录

        public static void GetConfig()
        {
            var configOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            adminWifDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["admin"].ToString());
            tokenHashDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["token"].ToString());
            factorDic = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(configOj["factor"].ToString());
            usedUtxoList = new List<string>();
        }

        /// <summary>
        /// 发行 Nep5 BTC ETH 资产
        /// </summary>
        /// <param name="type">Nep5 币种</param>
        /// <param name="json">参数</param>
        /// <param name="gasfee">交易费</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<string> DeployNep5TokenAsync(string type, JObject json, decimal gasfee, bool clear)
        {
            if (type == "cneo" || type == "bct")
            {
                return await ExchangeAsync(type, json, gasfee, clear);
            }

            if (clear)
            {
                usedUtxoList.Clear();
            }
            byte[] script;
            var prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(adminWifDic[type]);
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var amount = Math.Round((decimal) json["value"] * factorDic[type], 0);
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + json["address"]);
                array.AddArrayValue("(int)" + amount); //value
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
                sb.EmitAppCall(new Hash160(tokenHashDic[type])); //nep5脚本
                script = sb.ToArray();
            }
            //return SendTransWithoutUtxo(prikey, script);
            return await SendTransactionAsync(prikey, script, null, gasfee);
        }

        /// <summary>
        /// 购买交易
        /// </summary>
        /// <param name="coinType">发放币种</param>
        /// <param name="json">参数</param>
        /// <param name="gasfee">交易费</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<string> ExchangeAsync(string coinType, JObject json, decimal gasfee, bool clear)
        {
            if (clear)
            {
                usedUtxoList.Clear();
            }
            byte[] prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(adminWifDic[coinType]);
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            byte[] script;
            if (coinType == "gas"|| coinType == "neo")
            {
                return await SendUtxoTransAsync(coinType, prikey, json["address"].ToString(), Convert.ToDecimal(json["value"]), gasfee);
            }
            else
            {
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    var amount = Math.Round((decimal)json["value"] * factorDic[coinType],0);
                    var array = new MyJson.JsonNode_Array();
                    array.AddArrayValue("(addr)" + address); //from
                    array.AddArrayValue("(addr)" + json["address"]); //to
                    array.AddArrayValue("(int)" + amount); //value
                    byte[] randomBytes = new byte[32];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(randomBytes);
                    }
                    BigInteger randomNum = new BigInteger(randomBytes);
                    sb.EmitPushNumber(randomNum);
                    sb.Emit(ThinNeo.VM.OpCode.DROP);
                    sb.EmitParamJson(array); //参数倒序入
                    sb.EmitPushString("transfer"); //参数倒序入
                    sb.EmitAppCall(new Hash160(tokenHashDic[coinType]));
                    script = sb.ToArray();
                }
                //return await SendTransWithoutUtxoAsync(prikey, script);
                return await SendTransactionAsync(prikey, script, null, gasfee);
            }
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="coinType">币种</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<decimal> GetBalanceAsync(string coinType)
        {
            byte[] prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(adminWifDic[coinType]);
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            if (coinType == "gas" || coinType == "neo")
            {
                decimal balance = 0;
                var url = api + "?method=getbalance&id=1&params=['" + address + "']";
                var result = await Helper.HttpGet(url);
                var res = Newtonsoft.Json.Linq.JObject.Parse(result)["result"] as Newtonsoft.Json.Linq.JArray;
                if (res != null)
                {
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (res[i]["asset"].ToString() == tokenHashDic[coinType])
                            balance = (decimal) res[i]["balance"];
                    }
                }

                return balance;
            }

            byte[] data = null;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                MyJson.JsonNode_Array array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + address);
                sb.EmitParamJson(array);
                sb.EmitPushString("balanceOf");
                sb.EmitAppCall(new Hash160(tokenHashDic[coinType]));//合约脚本hash
                data = sb.ToArray();
            }

            return await GetNep5BalancAsync(coinType, data);
        }

        /// <summary>
        /// 获取 Nep5 资产余额
        /// </summary>
        /// <param name="coinType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task<decimal> GetNep5BalancAsync(string coinType, byte[] data)
        {
            decimal balance = 0;
            string script = ThinNeo.Helper.Bytes2HexString(data);
            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(api, "invokescript", out postdata, new MyJson.JsonNode_ValueString(script));
            var result = await Helper.HttpPost(url, postdata);
            var res = Newtonsoft.Json.Linq.JObject.Parse(result)["result"] as Newtonsoft.Json.Linq.JArray;
            if (res != null)
            {
                var stack = (res[0]["stack"] as Newtonsoft.Json.Linq.JArray)[0] as Newtonsoft.Json.Linq.JObject;
                var vBanlance = new BigInteger(ThinNeo.Helper.HexString2Bytes((string) stack["value"]));
                balance = (decimal) vBanlance / factorDic[coinType];
            }

            return balance;
        }

        /// <summary>
        /// 不使用 UTXO 发送 Nep5 交易
        /// </summary>
        /// <param name="prikey"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task<string> SendTransWithoutUtxoAsync(byte[] prikey, byte[] script)
        {
            var pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            var address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);

            ThinNeo.Transaction tran = new Transaction();
            tran.inputs = new ThinNeo.TransactionInput[0];
            tran.outputs = new TransactionOutput[0];
            tran.attributes = new ThinNeo.Attribute[1];
            tran.attributes[0] = new ThinNeo.Attribute();
            tran.attributes[0].usage = TransactionAttributeUsage.Script;
            tran.attributes[0].data = ThinNeo.Helper.GetPublicKeyHashFromAddress(address);
            tran.version = 1;
            tran.type = ThinNeo.TransactionType.InvocationTransaction;

            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = script;
            idata.gas = 0;

            byte[] msg = tran.GetMessage();
            string msgstr = ThinNeo.Helper.Bytes2HexString(msg);
            byte[] signdata = ThinNeo.Helper.Sign(msg, prikey);
            tran.AddWitness(signdata, pubkey, address);
            string txid = tran.GetHash().ToString();
            byte[] data = tran.GetRawData();
            string rawdata = ThinNeo.Helper.Bytes2HexString(data);

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(rawdata));
            var result = await Helper.HttpPost(url, postdata);
            var json = Newtonsoft.Json.Linq.JObject.Parse(result);
            Console.WriteLine(result);
            return result;
        }

        /// <summary>
        /// 带交易费的 Nep5 资产转账
        /// </summary>
        /// <param name="prikey"></param>
        /// <param name="script"></param>
        /// <param name="to"></param>
        /// <param name="gasfee"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task<string> SendTransactionAsync(byte[] prikey, byte[] script, string to, decimal gasfee)
        {
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddressAsync(api, address);
            if (dir.ContainsKey(tokenHashDic["gas"]) == false)
            {
                return "No gas";
            }

            //MakeTran
            ThinNeo.Transaction tran = null;
            {
                byte[] data = script;
                tran = Helper.makeTran(dir[tokenHashDic["gas"]], usedUtxoList, to, new ThinNeo.Hash256(tokenHashDic["gas"]), 0, gasfee);
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                var idata = new ThinNeo.InvokeTransData();
                tran.extdata = idata;
                idata.script = data;
                idata.gas = 0;
            }

            //sign and broadcast
            var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));
            string txid = tran.GetHash().ToString();
            var result = await Helper.HttpPost(url, postdata);
            foreach (var input in tran.inputs)
            {
                usedUtxoList.Add(((Hash256)input.hash).ToString() + input.index);
            }
            return result;
        }

        /// <summary>
        /// UTXO 资产转账
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prikey"></param>
        /// <param name="targetAddr"></param>
        /// <param name="sendCount"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task<string> SendUtxoTransAsync(string type, byte[] prikey, string targetAddr, decimal sendCount, decimal gasfee)
        {
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            Dictionary<string, List<Utxo>> dic_UTXO = await Helper.GetBalanceByAddressAsync(api, address);
            if (dic_UTXO.ContainsKey(tokenHashDic[type]) == false)
            {
                return "No " + type;
            }

            Transaction tran = Helper.makeTran(dic_UTXO[tokenHashDic[type]], usedUtxoList, targetAddr, new ThinNeo.Hash256(tokenHashDic[type]), sendCount, gasfee);
            
            tran.version = 0; 
            tran.attributes = new ThinNeo.Attribute[0];
            tran.type = ThinNeo.TransactionType.ContractTransaction;
            
            byte[] msg = tran.GetMessage();
            string msgstr = ThinNeo.Helper.Bytes2HexString(msg);
            byte[] signdata = ThinNeo.Helper.Sign(msg, prikey);
            tran.AddWitness(signdata, pubkey, address);
            string txid = tran.GetHash().ToString();
            byte[] data = tran.GetRawData();
            string rawdata = ThinNeo.Helper.Bytes2HexString(data);
            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(rawdata));
            var result = await Helper.HttpPost(url, postdata);

            foreach (var input in tran.inputs)
            {
                usedUtxoList.Add(((Hash256)input.hash).ToString() + input.index);
            }

            MyJson.JsonNode_Object resJO = (MyJson.JsonNode_Object)MyJson.Parse(result);
            return result;
        }

        public static List<TransactionInfo> ParseNeoBlock(int i, string address)
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
                    if (notify != null)
                    {
                        foreach (JObject n in notify)
                        {
                            //过滤 事件太多，只监视关注的合约
                            var contract = (string)n["contract"];
                            if (contract != "0x" + tokenHashDic["cneo"])
                                continue;

                            var value = n["state"] as JObject;
                            var method = (value["value"] as JArray)[0] as JObject;
                            var name = Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes((string)method["value"]));

                            if (name == "transfer")
                            {
                                var to = (value["value"] as JArray)[2] as JObject;
                                if (string.IsNullOrEmpty((string) to["value"]))
                                    continue;
                                var to_address = ThinNeo.Helper.GetAddressFromScriptHash(ThinNeo.Helper.HexString2Bytes((string)to["value"]));
                                if (to_address == address)
                                {
                                    var neoTrans = new TransactionInfo();
                                    var from = (value["value"] as JArray)[1] as JObject;
                                    var from_address = ThinNeo.Helper.GetAddressFromScriptHash(ThinNeo.Helper.HexString2Bytes((string)from["value"]));
                                    var amount = (value["value"] as JArray)[3] as JObject;
                                    var transAmount =
                                        (decimal) new BigInteger(ThinNeo.Helper.HexString2Bytes((string) amount["value"])) / factorDic["cneo"];
                                    neoTrans.toAddress = address;
                                    neoTrans.coinType = "cneo";
                                    neoTrans.confirmcount = 1;
                                    neoTrans.fromAddress = from_address;
                                    neoTrans.height = i;
                                    neoTrans.txid = txid;
                                    neoTrans.value = transAmount;
                                    transRspList.Add(neoTrans);
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + i +
                                                      " Aave A Cneo Transaction From :" + from_address + "; Value:" +
                                                      transAmount + "; Txid:" + txid);

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
            var getcounturl = api + "?jsonrpc=2.0&id=1&method=getblock&params=[" + block + ",1]";
            var info = wc.DownloadString(getcounturl);
            var json = Newtonsoft.Json.Linq.JObject.Parse(info);
            if (info.Contains("result") == false)
                return null;
            return (JObject)(((JArray)json["result"])[0]);
        }   

        static JArray _getNotify(string txid)
        {
            WebClient wc = new WebClient();
            
            var getcounturl = api + "?jsonrpc=2.0&id=1&method=getnotify&params=[\"" + txid + "\"]";
            var info = wc.DownloadString(getcounturl);
            var json = Newtonsoft.Json.Linq.JObject.Parse(info);
            if (json.ContainsKey("result") == false)
                return null;
            var result = (JObject)(((JArray)json["result"])[0]);
            var executions = ((JArray)result["executions"])[0] as JObject;

            return executions["notifications"] as JArray;

        }
    }
}
