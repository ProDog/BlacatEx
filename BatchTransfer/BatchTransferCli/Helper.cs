using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;

namespace BatchTransferCli
{
    public class Helper
    {
        public static Dictionary<string, List<Utxo>> GetBalanceByAddress(string api, string _addr, ref Dictionary<string, string> usedUtxoDic)
        {
            JObject response = JObject.Parse(HttpGet(api + "?method=getutxo&id=1&params=['" + _addr + "']"));
            JArray resJA = (JArray)response["result"];
            Dictionary<string, List<Utxo>> _dir = new Dictionary<string, List<Utxo>>();
            List<string> usedList = new List<string>(usedUtxoDic.Keys);
            foreach (JObject j in resJA)
            {
                Utxo utxo = new Utxo(j["addr"].ToString(), new ThinNeo.Hash256(j["txid"].ToString()), j["asset"].ToString(), decimal.Parse(j["value"].ToString()), int.Parse(j["n"].ToString()));
                if (_dir.ContainsKey(j["asset"].ToString()))
                {
                    _dir[j["asset"].ToString()].Add(utxo);
                }
                else
                {
                    List<Utxo> l = new List<Utxo>();
                    l.Add(utxo);
                    _dir[j["asset"].ToString()] = l;
                }

                for (int i = usedList.Count - 1; i >= 0; i--)
                {
                    if (usedUtxoDic[usedList[i]] == utxo.txid.ToString())
                    {
                        usedUtxoDic.Remove(usedList[i]);
                        usedList.Remove(usedList[i]);
                    }
                }

            }
            return _dir;
        }

        public static Transaction makeTran(ref List<Utxo> list_Gas, Dictionary<string, string> usedUtxoDic, Hash256 assetid, decimal gasfee)
        {
            var tran = new ThinNeo.Transaction();
            tran.type = ThinNeo.TransactionType.ContractTransaction;
            tran.version = 0;//0 or 1

            tran.attributes = new ThinNeo.Attribute[0];
            var scraddr = "";

            decimal count = decimal.Zero;
            List<ThinNeo.TransactionInput> list_inputs = new List<ThinNeo.TransactionInput>();
            for (var i = list_Gas.Count - 1; i >= 0; i--)
            {
                if (usedUtxoDic.ContainsKey(list_Gas[i].txid.ToString() + list_Gas[i].n))
                    continue;

                ThinNeo.TransactionInput input = new ThinNeo.TransactionInput();
                input.hash = list_Gas[i].txid;
                input.index = (ushort)list_Gas[i].n;
                list_inputs.Add(input);
                count += list_Gas[i].value;
                scraddr = list_Gas[i].addr;
                list_Gas.Remove(list_Gas[i]);
                if (count >= gasfee)
                    break;
            }

            tran.inputs = list_inputs.ToArray();
            if (count >= gasfee)//输入大于等于输出
            {
                List<ThinNeo.TransactionOutput> list_outputs = new List<ThinNeo.TransactionOutput>();
                //输出
                //if (gasfee > decimal.Zero && targetaddr != null)
                //{
                //    ThinNeo.TransactionOutput output = new ThinNeo.TransactionOutput();
                //    output.assetId = assetid;
                //    output.value = gasfee;
                //    output.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(targetaddr);
                //    list_outputs.Add(output);
                //}

                //找零
                var change = count - gasfee;
                if (change > decimal.Zero)
                {
                    decimal splitvalue = (decimal)0.01;
                    int i = 0;
                    while (change > splitvalue && list_Gas.Count < 20)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = Helper_NEO.GetScriptHash_FromAddress(scraddr);
                        outputchange.value = splitvalue;
                        outputchange.assetId = assetid;
                        list_outputs.Add(outputchange);
                        change -= splitvalue;
                        i += 1;
                        if (i > 20)
                        {
                            break;
                        }
                    }

                    if (change > 0)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = Helper_NEO.GetScriptHash_FromAddress(scraddr);
                        outputchange.value = change;
                        outputchange.assetId = assetid;
                        list_outputs.Add(outputchange);
                    }

                }
                tran.outputs = list_outputs.ToArray();
            }
            else
            {
                throw new Exception("no enough money.");
            }
            return tran;
        }

        public static string SendTransWithoutUtxo(byte[] prikey, string url, string contractHash, string method, JArray array)
        {            
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            var address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

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
            sb.EmitAppCall(new Hash160(contractHash));//合约脚本hash
            data = sb.ToArray();

            ThinNeo.Transaction tran = new Transaction();
            tran.inputs = new ThinNeo.TransactionInput[0];
            tran.outputs = new TransactionOutput[0];
            tran.attributes = new ThinNeo.Attribute[1];
            tran.attributes[0] = new ThinNeo.Attribute();
            tran.attributes[0].usage = TransactionAttributeUsage.Script;
            tran.attributes[0].data = ThinNeo.Helper_NEO.GetScriptHash_FromAddress(address);
            tran.version = 1;
            tran.type = ThinNeo.TransactionType.InvocationTransaction;

            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = data;
            idata.gas = 0;

            byte[] msg = tran.GetMessage();
            byte[] signdata = ThinNeo.Helper_NEO.Sign(msg, prikey);
            tran.AddWitness(signdata, pubkey, address);
            byte[] trandata = tran.GetRawData();

            string input = @"{
	            'jsonrpc': '2.0',
                'method': 'sendrawtransaction',
	            'params': ['#'],
	            'id': '1'
            }";

            input = input.Replace("#", ThinNeo.Helper.Bytes2HexString(trandata));
            string result = Helper.Post(url, input, System.Text.Encoding.UTF8, 1);

            return result;
        }


        public static string Post(string url, string data, Encoding encoding, int type = 3)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            Stream reqStream = null;
            //Stream resStream = null;

            try
            {
                req = WebRequest.CreateHttp(new Uri(url));
                if (type == 1)
                {
                    req.ContentType = "application/json;charset=utf-8";
                }
                else if (type == 2)
                {
                    req.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                }

                req.Method = "POST";
                //req.Accept = "text/xml,text/javascript";
                req.ContinueTimeout = 60000;

                byte[] postData = encoding.GetBytes(data);
                reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                //reqStream.Dispose();

                rsp = (HttpWebResponse)req.GetResponse();
                string result = GetResponseAsString(rsp, encoding);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // 释放资源
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream = null;
                }
                if (rsp != null)
                {
                    rsp.Close();
                    rsp = null;
                }
                if (req != null)
                {
                    req.Abort();

                    req = null;
                }
            }
        }

        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null)
                    reader.Close();
                if (stream != null)
                    stream.Close();

                reader = null;
                stream = null;

            }
        }

        public static string HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }

        public static async Task<string> HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = await wc.UploadDataTaskAsync(url, "POST", data);
            return System.Text.Encoding.UTF8.GetString(retdata);
        }

        public static string Bytes2HexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }
        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
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
