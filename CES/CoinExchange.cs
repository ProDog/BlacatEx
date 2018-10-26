using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CoinExchangeService
{
    public class CoinExchange
    {
        private static string api = "https://api.nel.group/api/testnet"; //NEO api
        private static Dictionary<string, string> adminWifDic = new Dictionary<string, string>();//管理员
        private static Dictionary<string, string> tokenHashDic = new Dictionary<string, string>();//token类型

        public static void GetConfig()
        {
            var configOj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.json").ToString());
            adminWifDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["admin"].ToString());
            tokenHashDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(configOj["token"].ToString());
        }

        public static string DeployNep5Token(string type, JObject json, decimal gasfee)
        {
            byte[] script;
            var prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(adminWifDic["btc"]);
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + json["address"]);
                array.AddArrayValue("(int)" + json["value"]); //value
                sb.EmitParamJson(array); //参数倒序入
                sb.EmitPushString("deploy"); //参数倒序入
                if (type == "btc")
                    sb.EmitAppCall(new Hash160(tokenHashDic["btc"])); //nep5脚本
                if (type == "eth")
                    sb.EmitAppCall(new Hash160(tokenHashDic["btc"]));
                script = sb.ToArray();
            }

            return SendTransWithoutUtxo(prikey, script);
            //return SendTransaction(prikey, script, gasfee);
        }

        public static string Exchange(JObject json, decimal gasfee)
        {
            byte[] prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(adminWifDic["bcp"]);
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            byte[] script;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + address);//from
                array.AddArrayValue("(addr)" + json["address"]);//to
                array.AddArrayValue("(int)" + json["value"]);//value
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitPushString("transfer");//参数倒序入
                sb.EmitAppCall(new Hash160(tokenHashDic[json["type"].ToString()]));
                script = sb.ToArray();
            }

            return SendTransWithoutUtxo(prikey, script);
        }

        private static string SendTransWithoutUtxo(byte[] prikey, byte[] script)
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
            var result = Helper.HttpPost(url, postdata);
            var json = Newtonsoft.Json.Linq.JObject.Parse(result);
            //Console.WriteLine("{0:u} rsp: " + result, DateTime.Now);
            return txid;
        }

        private static string SendTransaction(byte[] prikey, byte[] script, decimal gasfee)
        {
            byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = Helper.GetBalanceByAddress(api, address);
            if (dir.ContainsKey(tokenHashDic["gas"]) == false)
            {
                Console.WriteLine("no gas");
                return null;
            }
            //MakeTran
            ThinNeo.Transaction tran = null;
            {
                byte[] data = script;
                tran = Helper.makeTran(dir[tokenHashDic["gas"]], null, new ThinNeo.Hash256(tokenHashDic["gas"]), gasfee);
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
            Console.WriteLine("{0:u} txid: " + txid, DateTime.Now);
            var result = Helper.HttpPost(url, postdata);
            return txid;
        }
    }
}
