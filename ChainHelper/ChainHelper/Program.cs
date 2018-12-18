using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;

namespace ChainHelper
{
    class Program
    {
        private static HttpListener httpPostRequest = new HttpListener();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string httpUrl = "http://+:18332/";
        private static string neoApi = "https://api.nel.group/api/testnet";

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"log4net.config"));
            GlobalContext.Properties["pname"] = Assembly.GetEntryAssembly().GetName().Name;
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            Console.OutputEncoding = Encoding.UTF8;
            httpPostRequestHandle();
        }

        private static void httpPostRequestHandle()
        {
            Logger.Info("Http Server Start!");
            httpPostRequest.Prefixes.Add(httpUrl);
            while (true)
            {
                httpPostRequest.Start();
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                //logger.Log("Have a request: " + requestContext.Request.RawUrl);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RspInfo()
                { state = false, msg = "request error,please check your url or post data!" }));
                try
                {
                    var task = Task.Run(async () => buffer = await ExecRequestAsync(requestContext));
                    task.Wait();
                }
                catch (Exception e)
                {
                    var rsp = JsonConvert.SerializeObject(new RspInfo()
                    { state = false, msg = e.Message });
                    buffer = Encoding.UTF8.GetBytes(rsp);
                    Logger.Error(rsp);
                }
                finally
                {
                    requestContext.Response.StatusCode = 200;
                    requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    requestContext.Response.ContentType = "application/json";
                    requestContext.Response.ContentEncoding = Encoding.UTF8;
                    requestContext.Response.ContentLength64 = buffer.Length;
                    var output = requestContext.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

        private static async Task<byte[]> ExecRequestAsync(HttpListenerContext requestContext)
        {
            StreamReader sr = new StreamReader(requestContext.Request.InputStream);
            var rawUrl = requestContext.Request.RawUrl;
            var urlPara = rawUrl.Split('/');
            var json = new JObject();
            string msg = string.Empty;
            object resContent = null;
            var rsp = string.Empty;
            if (requestContext.Request.HttpMethod == "POST")
            {
                JArray array = new JArray();
                JObject stack;
                var info = sr.ReadToEnd();
                string hash = string.Empty;
                if (!string.IsNullOrEmpty(info))
                    json = JObject.Parse(info);
                string method = urlPara[1].ToString();
                switch (method)
                {
                    case "GetAssetInfo":
                        hash = "0ca406aea638e0fed8580f00eb8b6e1dcb3d95da";
                        array.Add("(hex160)" + json["assetid"].ToString());
                        msg = await CallInvokescriptAsync(hash, array, "getAssetInfo");
                        stack = ((JObject.Parse(msg)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
                        resContent = BancorAssetInfoParse(stack);
                        break;
                    default:
                        break;
                }
                rsp = JsonConvert.SerializeObject(new RspInfo() { state = true, msg = resContent });
            }
            //Logger.Info("Response: " + rsp);            
            return Encoding.UTF8.GetBytes(rsp); 
        }

        private static AssetInfo BancorAssetInfoParse(JObject stack)
        {
            var value = stack["value"] as JArray;
            var assetInfo = new AssetInfo();
            if (value == null)
                return assetInfo;
            if (value[0]["value"].ToString() != "False")
                assetInfo.connectAssetHash = Helper_NEO
                    .GetScriptHash_FromAddress(
                        Helper_NEO.GetAddress_FromScriptHash(Helper.HexString2Bytes(value[0]["value"].ToString())))
                    .ToString();
            if (value[1]["value"].ToString() != "False")
                assetInfo.adminAddress =
                Helper_NEO.GetAddress_FromScriptHash(Helper.HexString2Bytes(value[1]["value"].ToString()));
            if (value[2]["value"].ToString() != "False")
                assetInfo.connectWeight = (int)new BigInteger(Helper.HexString2Bytes(value[2]["value"].ToString()));
            if (value[3]["value"].ToString() != "False")
                assetInfo.maxConnectWeight = (int)new BigInteger(Helper.HexString2Bytes(value[3]["value"].ToString()));
            if (value[4]["value"].ToString() != "False")
                assetInfo.connectBalance = decimal.Parse(value[4]["value"].ToString()) / 100000000;
            if (value[5]["value"].ToString() != "False")
                assetInfo.smartTokenBalance = decimal.Parse(value[5]["value"].ToString()) / 100000000;
            return assetInfo;
        }

        public static async Task<string> CallInvokescriptAsync(string contractHash, JArray array, string method)
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
            sb.EmitAppCall(new Hash160(contractHash));//合约脚本hash
            data = sb.ToArray();
            script = sb.ToArray();
            var strscript = ThinNeo.Helper.Bytes2HexString(script);
            var result = await Helper.HttpGet($"{neoApi}?method=invokescript&id=1&params=[\"{strscript}\"]");
            return result;
        }

    }

    public class AssetInfo
    {
        public string connectAssetHash; //连接器hash
        public string adminAddress; //管理员
        public int connectWeight; //连接器权重
        public int maxConnectWeight; //最大权重
        public decimal connectBalance; //连接器代币余额
        public decimal smartTokenBalance; //智能代币余额
    }

    public class RspInfo
    {
        public bool state;
        public dynamic msg;
    }
}
