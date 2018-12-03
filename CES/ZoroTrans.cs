using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class ZoroTrans
    {
        private static Dictionary<string, string> usedUtxoDic = new Dictionary<string, string>(); //本区块内同一账户已使用的 UTXO 记录
        private static Dictionary<string, List<Utxo>> dic_UTXO = new Dictionary<string, List<Utxo>>();
        private static List<Utxo> list_Gas = new List<Utxo>();
        /// <summary>
        /// 发行 Nep5 BTC ETH 资产
        /// </summary>
        /// <param name="type">Nep5 币种</param>
        /// <param name="json">参数</param>
        /// <param name="gasfee">交易费</param>
        /// <returns></returns>
        public static async Task<string> DeployNep5TokenAsync(string type, JObject json)
        {
            if (type == "cneo" || type == "bct")
            {
                return await ExchangeAsync(type, json);
            }

            byte[] script;
            var prikey = Helper_NEO.GetPrivateKeyFromWIF(Config.adminWifDic[type]);
            using (var sb = new ScriptBuilder())
            {
                var amount = Math.Round((decimal) json["value"] * Config.factorDic[type], 0);
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
            return await SendTransactionAsync(prikey, script);
        }

        /// <summary>
        /// 购买交易
        /// </summary>
        /// <param name="coinType">发放币种</param>
        /// <param name="json">参数</param>
        /// <param name="gasfee">交易费</param>
        /// <returns></returns>
        public static async Task<string> ExchangeAsync(string coinType, JObject json)
        {
            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(Config.adminWifDic[coinType]);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);
            byte[] script;
            //if (coinType == "gas" || coinType == "neo")
            //{
            //    return await SendUtxoTransAsync(coinType, prikey, json["address"].ToString(),
            //        Convert.ToDecimal(json["value"]), gasfee);
            //}
            //else
            //{
            using (var sb = new ScriptBuilder())
            {
                var amount = Math.Round((decimal) json["value"] * Config.factorDic[coinType], 0);
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

            return await SendTransactionAsync(prikey, script);
            //}
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="coinType">币种</param>
        /// <returns></returns>
        public static async Task<decimal> GetBalanceAsync(string coinType)
        {
            string address = getAddressFromWif(Config.adminWifDic[coinType]);
            if (coinType == "gas" || coinType == "neo")
            {
                decimal balance = 0;
                var url = Config.apiDic["neo"] + "?method=getbalance&id=1&params=['" + address + "']";
                var result = await MyHelper.HttpGet(url);
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

            return await GetNep5BalancAsync(coinType, data);
        }

        /// <summary>
        /// 获取 Nep5 资产余额
        /// </summary>
        /// <param name="coinType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static async Task<decimal> GetNep5BalancAsync(string coinType, byte[] data)
        {
            decimal balance = 0;
            string script = Helper.Bytes2HexString(data);
            var result =
                await MyHelper.HttpGet($"{Config.apiDic["neo"]}?method=invokescript&id=1&params=[\"{script}\"]");
            if (Newtonsoft.Json.Linq.JObject.Parse(result)["result"] is JArray res && res.Count > 0)
            {
                var stack = (res[0]["stack"] as JArray)[0] as JObject;
                var vBanlance = new BigInteger(Helper.HexString2Bytes((string)stack["value"]));
                balance = (decimal)vBanlance / Config.factorDic[coinType];
            }

            return balance;
        }

        /// <summary>
        /// 不使用 UTXO 发送 Nep5 交易
        /// </summary>
        /// <param name="prikey"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private static async Task<string> SendTransWithoutUtxoAsync(byte[] prikey, byte[] script)
        {
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

            Transaction tran = new Transaction();
            tran.inputs = new TransactionInput[0];
            tran.outputs = new TransactionOutput[0];
            tran.attributes = new ThinNeo.Attribute[1];
            tran.attributes[0] = new ThinNeo.Attribute();
            tran.attributes[0].usage = TransactionAttributeUsage.Script;
            tran.attributes[0].data = pubkey;
            tran.version = 1;
            tran.type = TransactionType.InvocationTransaction;

            var idata = new InvokeTransData();
            tran.extdata = idata;
            idata.script = script;
            idata.gas = 0;

            byte[] msg = tran.GetMessage();
            string msgstr = Helper.Bytes2HexString(msg);
            byte[] signdata = Helper_NEO.Sign(msg, prikey);
            tran.AddWitness(signdata, pubkey, address);
            string txid = tran.GetHash().ToString();
            byte[] data = tran.GetRawData();
            string rawdata = Helper.Bytes2HexString(data);
            var result =
                await MyHelper.HttpGet($"{Config.apiDic["neo"]}?method=sendrawtransaction&id=1&params=[\"{rawdata}\"]");
            var json = JObject.Parse(result);
            //Program.logger.Log(result);
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
        private static async Task<string> SendTransactionAsync(byte[] prikey, byte[] script)
        {
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

            if (dic_UTXO.ContainsKey(Config.tokenHashDic["gas"]) == false || list_Gas.Count - 10 < usedUtxoDic.Count)
            {
                dic_UTXO = MyHelper.GetBalanceByAddress(Config.apiDic["neo"], address, ref usedUtxoDic);
            }

            if (dic_UTXO.ContainsKey(Config.tokenHashDic["gas"]) == false)
            {
                throw new Exception("no gas.");
            }
            list_Gas = dic_UTXO[Config.tokenHashDic["gas"]];
            //MakeTran
            Transaction tran = MyHelper.makeTran(ref list_Gas, usedUtxoDic, new Hash256(Config.tokenHashDic["gas"]), Config.minerFeeDic["gas_fee"]);

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
            var strtrandata = Helper.Bytes2HexString(trandata);
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

            var result = await MyHelper.PostAsync(Config.apiDic["neo"], input, Encoding.UTF8, 1);
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
        //private static async System.Threading.Tasks.Task<string> SendUtxoTransAsync(string type, byte[] prikey,
        //    string targetAddr, decimal sendCount, decimal gasfee)
        //{
        //    byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
        //    string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);
        //    Dictionary<string, List<Utxo>> dic_UTXO =
        //        await MyHelper.GetBalanceByAddressAsync(Config.apiDic["neo"], address);
        //    if (dic_UTXO.ContainsKey(Config.tokenHashDic[type]) == false)
        //    {
        //        return "No " + type;
        //    }

        //    Transaction tran = MyHelper.makeTran(dic_UTXO[Config.tokenHashDic[type]], ref usedUtxoList, targetAddr,
        //        new ThinNeo.Hash256(Config.tokenHashDic[type]), sendCount, gasfee);

        //    tran.version = 0;
        //    tran.attributes = new ThinNeo.Attribute[0];
        //    tran.type = ThinNeo.TransactionType.ContractTransaction;

        //    byte[] msg = tran.GetMessage();
        //    string msgstr = ThinNeo.Helper.Bytes2HexString(msg);
        //    byte[] signdata = Helper_NEO.Sign(msg, prikey);
        //    tran.AddWitness(signdata, pubkey, address);
        //    string txid = tran.GetHash().ToString();
        //    byte[] data = tran.GetRawData();
        //    string strtrandata = ThinNeo.Helper.Bytes2HexString(data);

        //    string input = @"{
	       //     'jsonrpc': '2.0',
        //        'method': 'sendrawtransaction',
	       //     'params': ['#'],
	       //     'id': '1'
        //    }";
        //    input = input.Replace("#", strtrandata);

        //    var result = await MyHelper.PostAsync(Config.apiDic["neo"], input, System.Text.Encoding.UTF8, 1);

        //    JObject resJO = JObject.Parse(result);
        //    return result;
        //}

        private static string getAddressFromWif(string strWif)
        {
            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(strWif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            return Helper_NEO.GetAddress_FromPublicKey(pubkey);
        }
    }
}
