using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace NFT_API
{
    public class Program
    {
        private static HttpListener httpPostRequest = new HttpListener();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Config.init("config.json");
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"log4net.config"));
            GlobalContext.Properties["pname"] = Assembly.GetEntryAssembly().GetName().Name;
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            Console.OutputEncoding = Encoding.UTF8;
            httpPostRequestHandle();

            Console.ReadKey();
        }

        private static void httpPostRequestHandle()
        {
            Logger.Info("Http Server Start!");
            httpPostRequest.Prefixes.Add(Config.httpUrl);
            while (true)
            {
                httpPostRequest.Start();

                HttpListenerContext requestContext = httpPostRequest.GetContext();
                //logger.Log("Have a request: " + requestContext.Request.RawUrl);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RspInfo()
                { state = false, msg = new Error() }));
                try
                {
                    var task = Task.Run(async () => buffer = await ExecRequest(requestContext));
                    task.Wait();
                }
                catch (Exception e)
                {
                    var rsp = JsonConvert.SerializeObject(new RspInfo()
                        {state = false, msg = new Error() {error = e.Message}});
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

        private static async Task<byte[]> ExecRequest(HttpListenerContext requestContext)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                RspInfo()
                {state = false, msg = new Error() {error = "request error,please check your url or post data!"}}));
            StreamReader sr = new StreamReader(requestContext.Request.InputStream);
            var rawUrl = requestContext.Request.RawUrl;
            var urlPara = rawUrl.Split('/');
            var json = new JObject();
            string msg = string.Empty;
            object resContent = null;
            var rsp = string.Empty;
            if (requestContext.Request.HttpMethod == "POST")
            {
                var info = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(info))
                    json = JObject.Parse(info);
                if (urlPara.Length > 1)
                {
                    Logger.Info($"Have a request,url:{rawUrl}; post data:{json}");
                    JArray array = new JArray();
                    string method = urlPara[1].ToString();
                    if (method == "deploy" || method == "addpoint" || method == "buy" || method == "upgrade" ||
                        method == "exchange")
                    {
                        switch (method)
                        {
                            case "deploy":
                            case "addpoint":
                                array.Add("(addr)" + json["address"].ToString());
                                break;
                            case "buy":
                                array.Add("(hex256)" + json["txid"].ToString());
                                array.Add("(addr)" + json["inviter"].ToString());
                                break;
                            case "upgrade":
                                array.Add("(hex256)" + json["txid"].ToString());
                                break;
                            case "exchange":
                                array.Add("(addr)" + json["from"].ToString());
                                array.Add("(addr)" + json["to"].ToString());
                                break;
                        }
                        msg = await Controller.SendrawTransactionAsync(array, method);
                        var state = (bool)(JObject.Parse(msg)["result"] as JArray)[0]["sendrawtransactionresult"];
                        if (state)
                        {
                            resContent = new Txid()
                                {txid = (JObject.Parse(msg)["result"] as JArray)[0]["txid"].ToString()};
                            rsp = JsonConvert.SerializeObject(new RspInfo() { state = true, msg = resContent });
                        }
                        else
                        {
                            resContent = new Error()
                                { error = (JObject.Parse(msg)["result"] as JArray)[0]["errorMessage"].ToString() };
                            rsp = JsonConvert.SerializeObject(new RspInfo() { state = false, msg = resContent });
                        }
                    }
                    else
                    {
                        JObject stack;
                        switch (method)
                        {
                            case "getnftinfo":
                                array.Add("(addr)" + json["address"].ToString());
                                msg = await Controller.CallInvokescriptAsync(array, method);
                                stack = ((JObject.Parse(msg)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
                                resContent = CountNftInfoParse(stack);
                                break;

                            case "gettxinfo":
                                array.Add("(hex256)" + json["txid"].ToString());
                                msg = await Controller.CallInvokescriptAsync(array, method);
                                stack = ((JObject.Parse(msg)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
                                resContent = TxInfoParse(stack);
                                break;

                            case "getcount":
                                array.Add("(int)" + "1");
                                msg = await Controller.CallInvokescriptAsync(array, method);
                                stack = ((JObject.Parse(msg)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
                                resContent = CountParse(stack);
                                break;

                            case "getconfig":
                                array.Add("(int)" + "1");
                                msg = await Controller.CallInvokescriptAsync(array, method);
                                stack = ((JObject.Parse(msg)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
                                resContent = ConfigParse(stack);
                                break;
                        }
                        rsp = JsonConvert.SerializeObject(new RspInfo() { state = true, msg = resContent });
                    }

                    Logger.Info("Response: " + rsp);
                    buffer = Encoding.UTF8.GetBytes(rsp);
                }
            }

            return buffer;
        }
        
        private static object ConfigParse(JObject stack)
        {
            var config = new ConfigInfo();
            var value = stack["value"] as JArray;
            if (value == null || value.Count < 12)
                return config;
            if (value[0]["type"].ToString() == "ByteArray")
                config.SilverPrice = new BigInteger(Helper.HexString2Bytes((string) value[0]["value"])) / 10000;
            if (value[1]["type"].ToString() == "ByteArray")
                config.GoldPrice = new BigInteger(Helper.HexString2Bytes((string)value[1]["value"])) / 10000;
            if (value[2]["type"].ToString() == "ByteArray")
                config.PlatinumPrice = new BigInteger(Helper.HexString2Bytes((string)value[2]["value"])) / 10000;
            if (value[3]["type"].ToString() == "ByteArray")
                config.DiamondPrice = new BigInteger(Helper.HexString2Bytes((string)value[3]["value"])) / 10000;
            if (value[4]["type"].ToString() == "ByteArray")
                config.LeaguerInvitePoint = new BigInteger(Helper.HexString2Bytes((string)value[4]["value"]));
            if (value[5]["type"].ToString() == "ByteArray")
                config.SilverInvitePoint = new BigInteger(Helper.HexString2Bytes((string)value[5]["value"]));
            if (value[6]["type"].ToString() == "ByteArray")
                config.GoldInvitePoint = new BigInteger(Helper.HexString2Bytes((string)value[6]["value"]));
            if (value[7]["type"].ToString() == "ByteArray")
                config.PlatinumInvitePoint = new BigInteger(Helper.HexString2Bytes((string)value[7]["value"]));
            if (value[8]["type"].ToString() == "ByteArray")
                config.DiamondInvitePoint = new BigInteger(Helper.HexString2Bytes((string)value[8]["value"]));
            if (value[9]["type"].ToString() == "ByteArray")
                config.GoldUpgradePoint = new BigInteger(Helper.HexString2Bytes((string)value[9]["value"]));
            if (value[10]["type"].ToString() == "ByteArray")
                config.PlatinumUpgradePoint = new BigInteger(Helper.HexString2Bytes((string)value[10]["value"]));
            if (value[11]["type"].ToString() == "ByteArray")
                config.DiamondUpgradePoint = new BigInteger(Helper.HexString2Bytes((string)value[11]["value"]));
            return config;
        }

        private static ExchangeInfo TxInfoParse(JObject stack)
        {
            var txInfo = new ExchangeInfo();
            var value = stack["value"] as JArray;
            if (value == null || value.Count < 3)
                return txInfo;
            if (!string.IsNullOrEmpty(value[0]["value"].ToString()))
                txInfo.@from =
                    Helper_NEO.GetAddress_FromScriptHash(ThinNeo.Helper.HexString2Bytes(value[0]["value"].ToString()));
            if (!string.IsNullOrEmpty(value[1]["value"].ToString()))
                txInfo.to = Helper_NEO.GetAddress_FromScriptHash(ThinNeo.Helper.HexString2Bytes(value[1]["value"].ToString()));
           if (!string.IsNullOrEmpty(value[2]["value"].ToString()))
                txInfo.tokenId= value[2]["value"].ToString();
            return txInfo;
        }

        private static NFTInfo CountNftInfoParse(JObject stack)
        {
            var nftInfo = new NFTInfo();
            var value = stack["value"] as JArray;
            if (value == null || value.Count < 5)
                return nftInfo;
            if (!string.IsNullOrEmpty(value[0]["value"].ToString()))
                nftInfo.TokenId = value[0]["value"].ToString();
            if (!string.IsNullOrEmpty(value[1]["value"].ToString()))
                nftInfo.Owner = Helper_NEO.GetAddress_FromScriptHash(ThinNeo.Helper.HexString2Bytes(value[1]["value"].ToString()));
            if (value[2]["type"].ToString() == "Integer")
                nftInfo.Rank = Convert.ToInt32(value[2]["value"]);
            if (value[3]["type"].ToString() == "Integer")
                nftInfo.ContributionPoint = Convert.ToInt32(value[3]["value"]);
            if (!string.IsNullOrEmpty(value[4]["value"].ToString()))
                nftInfo.InviterTokenId = value[4]["value"].ToString();
            return nftInfo;
        }

        private static NftCount CountParse(JObject stack)
        {
            var count = new NftCount();
            var value = stack["value"] as JArray;
            if (value == null || value.Count < 5)
                return count;
            if (value[0]["type"].ToString() == "Integer")
                count.AllCount = Convert.ToInt32(value[0]["value"]);
            if (value[1]["type"].ToString() == "Integer")
                count.SilverCount = Convert.ToInt32(value[1]["value"]);
            if (value[2]["type"].ToString() == "Integer")
                count.GoldCount = Convert.ToInt32(value[2]["value"]);
            if (value[3]["type"].ToString() == "Integer")
                count.PlatinumCount = Convert.ToInt32(value[3]["value"]);
            if (value[4]["type"].ToString() == "Integer")
                count.DiamondCount = Convert.ToInt32(value[4]["value"]);
            return count;
        }

    }

}