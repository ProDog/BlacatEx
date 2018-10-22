using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CoinExchange
{
    public class Program
    {
        private static string httpUrl = "http://127.0.0.1:7070/"; //http 服务 url
        private static string api = ""; //NEO api
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }

        private static HttpListener httpPostRequest = new HttpListener();

        private static void HttpServerStart()
        {
            httpPostRequest.Prefixes.Add(httpUrl);
            httpPostRequest.Start();
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
        }

        private static void httpPostRequestHandle()
        {
            while (true)
            {
                httpPostRequest.Start();
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {state = "false", msg = "request error,please check your url or post data!"}));
                try
                {
                    StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                    var urlPara = requestContext.Request.RawUrl.Split('/');
                    var json = new JObject();
                    if (requestContext.Request.HttpMethod == "POST")
                    {
                        var info = sr.ReadToEnd();
                        json = Newtonsoft.Json.Linq.JObject.Parse(info);
                    }

                    if (urlPara.Length > 1)
                    {
                        var method = urlPara[1];
                        if (method == "deploy")
                        {
                            var coinType = urlPara[2];
                            string address = string.Empty;
                            string priKey = string.Empty;
                            switch (coinType)
                            {
                                case "btc":
                                    SendNep5Btc(json);
                                    break;
                                case "eth":
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void SendNep5Btc(JObject json)
        {
            var wif = Console.ReadLine();
            var prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(wif);
            var pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            var address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            var scriptHash = ThinNeo.Helper.GetPublicKeyHashFromAddress(address);

            byte[] script;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + json["address"]);
                array.AddArrayValue("(int)" + json["value"] + "00000000"); //value
                sb.EmitParamJson(array); //参数倒序入
                sb.EmitPushString("deploy"); //参数倒序入
                sb.EmitAppCall(new Hash160("07bc2c1398e1a472f3841a00e7e7e02029b8b38b")); //nep5脚本
                script = sb.ToArray();
                Console.WriteLine(ThinNeo.Helper.Bytes2HexString(script));
            }

            SendTransWithoutUtxo(prikey, pubkey, address, script);

        }

        private static void SendTransWithoutUtxo(byte[] prikey, byte[] pubkey, string address, byte[] script)
        {
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
            var url = MakeRpcUrlPost(api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(rawdata));
            var result = HttpPost(url, postdata);
            Console.WriteLine(txid);
            MyJson.JsonNode_Object resJO = (MyJson.JsonNode_Object)MyJson.Parse(result);
            Console.WriteLine(resJO.ToString());
        }

        public static string MakeRpcUrlPost(string url, string method, out byte[] data, params MyJson.IJsonNode[] _params)
        {
            //if (url.Last() != '/')
            //    url = url + "/";
            var json = new MyJson.JsonNode_Object();
            json["id"] = new MyJson.JsonNode_ValueNumber(1);
            json["jsonrpc"] = new MyJson.JsonNode_ValueString("2.0");
            json["method"] = new MyJson.JsonNode_ValueString(method);
            StringBuilder sb = new StringBuilder();
            var array = new MyJson.JsonNode_Array();
            for (var i = 0; i < _params.Length; i++)
            {

                array.Add(_params[i]);
            }
            json["params"] = array;
            data = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            return url;
        }
        public static string MakeRpcUrl(string url, string method, params MyJson.IJsonNode[] _params)
        {
            StringBuilder sb = new StringBuilder();
            if (url.Last() != '/')
                url = url + "/";

            sb.Append(url + "?jsonrpc=2.0&id=1&method=" + method + "&params=[");
            for (var i = 0; i < _params.Length; i++)
            {
                _params[i].ConvertToString(sb);
                if (i != _params.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }
        public static string HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = wc.UploadData(url, "POST", data);
            return System.Text.Encoding.UTF8.GetString(retdata);
        }
    }
}
