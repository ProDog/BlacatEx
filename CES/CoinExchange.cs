using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CoinExchangeService
{
    public class CoinExchange
    {
        private static string api = "https://api.nel.group/api/testnet"; //NEO api
        private static string id_GAS = "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"; //gas
        private static string nep5Btc = "07bc2c1398e1a472f3841a00e7e7e02029b8b38b";//BTC
        private static string btcwif = "*";//Nep5 BitCoin管理员
        private static string nepwif = "*";//BCP 等资产管理员

        public static string DeployNep5Token(string type, JObject json)
        {
            byte[] script;
            var prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(btcwif);
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + json["address"]);
                array.AddArrayValue("(int)" + json["value"]); //value
                sb.EmitParamJson(array); //参数倒序入
                sb.EmitPushString("deploy"); //参数倒序入
                if (type == "btc")
                    sb.EmitAppCall(new Hash160(nep5Btc)); //nep5脚本
                if (type == "eth")
                    sb.EmitAppCall(new Hash160(""));
                script = sb.ToArray();
            }

            decimal gasfee = 0;
            return SendTransWithoutUtxo(prikey, script);
            //return SendTransaction(prikey, script, gasfee);
        }

        public static object Exchange(JObject json)
        {
            byte[] prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(nepwif);
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
                sb.EmitAppCall(new Hash160(json["token"].ToString()));
                script = sb.ToArray();
            }

            decimal gasfee = 0;
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
            if (dir.ContainsKey(id_GAS) == false)
            {
                Console.WriteLine("no gas");
                return null;
            }
            //MakeTran
            ThinNeo.Transaction tran = null;
            {

                byte[] data = script;
                tran = Helper.makeTran(dir[id_GAS], null, new ThinNeo.Hash256(id_GAS), gasfee);
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

    public class Utxo
    {
        //txid[n] 是utxo的属性
        public ThinNeo.Hash256 txid;
        public int n;

        //asset资产、addr 属于谁，value数额，这都是查出来的
        public string addr;
        public string asset;
        public decimal value;

        public Utxo(string _addr, ThinNeo.Hash256 _txid, string _asset, decimal _value, int _n)
        {
            this.addr = _addr;
            this.txid = _txid;
            this.asset = _asset;
            this.value = _value;
            this.n = _n;
        }
    }
}
