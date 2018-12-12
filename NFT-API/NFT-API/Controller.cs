using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace NFT_API
{
    public class Controller
    {
        private static Dictionary<string, string> usedUtxoDic = new Dictionary<string, string>();
        private static Dictionary<string, List<Utxo>> dic_UTXO = new Dictionary<string, List<Utxo>>();
        private static List<Utxo> list_Gas = new List<Utxo>();
        
        public static async Task<string> SendrawTransactionAsync(JArray array, string method)
        {
            byte[] data = null;
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitParamJson(array);
            byte[] randomBytes = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            BigInteger randomNum = new BigInteger(randomBytes);
            sb.EmitPushNumber(randomNum);
            sb.Emit(ThinNeo.VM.OpCode.DROP);
            sb.EmitPushString(method);
            sb.EmitAppCall(new Hash160(Config.nftHash));//合约脚本hash
            data = sb.ToArray();

            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(Config.adminAif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            var address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

            if (dic_UTXO.ContainsKey(Config.gasId) == false || list_Gas.Count - 10 < usedUtxoDic.Count)
            {
                dic_UTXO = Helper.GetBalanceByAddress(Config.nelApi, address, ref usedUtxoDic);
            }
            
            if (dic_UTXO.ContainsKey(Config.gasId) == false)
            {
                throw new Exception("no gas.");
            }

            list_Gas = dic_UTXO[Config.gasId];
            Transaction tran = Helper.makeTran(ref list_Gas, usedUtxoDic, new Hash256(Config.gasId), Config.gasFee);

            tran.type = ThinNeo.TransactionType.InvocationTransaction;
            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = data;
            idata.gas = 0;

            var signdata = Helper_NEO.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            var txid = tran.GetHash().ToString();
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
            string result = await Helper.PostAsync(Config.nelApi, input, System.Text.Encoding.UTF8, 1);
            
            return result;
        }

        internal static void SetConfig()
        {
            JArray array = new JArray();
            array.Add("(int)16990000");
            array.Add("(int)166990000");
            array.Add("(int)1669990000");
            array.Add("(int)16669990000");

            array.Add("(int)1");
            array.Add("(int)10");
            array.Add("(int)100");
            array.Add("(int)1000");
            array.Add("(int)0");

            array.Add("(int)100");
            array.Add("(int)1000");
            array.Add("(int)10000");
           
            var aa = SendrawTransactionAsync(array, "setconfig").Result;
            Console.WriteLine(aa);
        }

        public static async Task<string> CallInvokescriptAsync(JArray array, string method)
        {
            byte[] data = null;
            byte[] script;
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitParamJson(array);
            byte[] randomBytes = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            BigInteger randomNum = new BigInteger(randomBytes);
            sb.EmitPushNumber(randomNum);
            sb.Emit(ThinNeo.VM.OpCode.DROP);
            sb.EmitPushString(method);
            sb.EmitAppCall(new Hash160(Config.nftHash));//合约脚本hash
            data = sb.ToArray();
            script = sb.ToArray();
            var strscript = ThinNeo.Helper.Bytes2HexString(script);
            var result = await Helper.HttpGet($"{Config.nelApi}?method=invokescript&id=1&params=[\"{strscript}\"]");
            return result;
        }
       
    }
}
