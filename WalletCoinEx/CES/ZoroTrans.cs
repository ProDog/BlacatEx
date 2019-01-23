using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class ZoroTrans
    {
        private static Dictionary<string, string> usedUtxoDic = new Dictionary<string, string>(); //本区块内同一账户已使用的 UTXO 记录
        private static Dictionary<string, List<Utxo>> dic_UTXO = new Dictionary<string, List<Utxo>>();
        private static List<Utxo> list_Gas = new List<Utxo>();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 购买交易
        /// </summary>
        /// <param name="coinType">发放币种</param>
        /// <param name="json">参数</param>
        /// <param name="gasfee">交易费</param>
        /// <returns></returns>
        public static string Exchange(string coinType, JObject json)
        {
            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(Config.adminWifDic[coinType]);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);
            byte[] script;
            using (var sb = new ScriptBuilder())
            {
                var amount = Math.Round((decimal)json["value"] * Config.factorDic[coinType], 0);
                var array = new JArray();
                array.Add("(addr)" + address); //from
                array.Add("(addr)" + json["address"]); //to
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
                sb.EmitPushString("transfer"); //参数倒序入
                sb.EmitAppCall(new Hash160(Config.tokenHashDic[coinType]));
                script = sb.ToArray();
            }

            return SendTransaction(prikey, script);
          
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="coinType">币种</param>
        /// <returns></returns>
        public static decimal GetBalance(string coinType)
        {
            string address = getAddressFromWif(Config.adminWifDic[coinType]);
            if (coinType == "gas" || coinType == "neo")
            {
                decimal balance = 0;
                var url = Config.apiDic["neo"] + "?method=getbalance&id=1&params=['" + address + "']";
                var result = Helper.HttpGet(url);
                if (JObject.Parse(result)["result"] is JArray res && res.Count > 0)
                {
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (res[i]["asset"].ToString() == Config.tokenHashDic[coinType])
                            balance = (decimal)res[i]["balance"];
                    }
                }

                return balance;
            }

            byte[] data = null;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                JArray array = new JArray();
                array.Add("(addr)" + address);
                sb.EmitParamJson(array);
                sb.EmitPushString("balanceOf");
                sb.EmitAppCall(new Hash160(Config.tokenHashDic[coinType])); //合约脚本hash
                data = sb.ToArray();
            }

            return GetNep5Balanc(coinType, data);
        }

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
            var result = Helper.HttpGet($"{Config.apiDic["neo"]}?method=invokescript&id=1&params=[\"{script}\"]");
            if (Newtonsoft.Json.Linq.JObject.Parse(result)["result"] is JArray res && res.Count > 0)
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
                dic_UTXO = Helper.GetBalanceByAddress(Config.apiDic["neo"], address, ref usedUtxoDic);
            }

            if (dic_UTXO.ContainsKey(Config.tokenHashDic["gas"]) == false)
            {
                throw new Exception("no gas.");
            }
            list_Gas = dic_UTXO[Config.tokenHashDic["gas"]];
            //MakeTran
            Transaction tran = Helper.makeTran(ref list_Gas, usedUtxoDic, new Hash256(Config.tokenHashDic["gas"]), Config.minerFeeDic["gas_fee"]);

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

            var result = Helper.PostAsync(Config.apiDic["neo"], input, Encoding.UTF8, 1).Result;
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
