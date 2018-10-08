using System;
using System.IO;
using System.Net;
using System.Threading;
using Nethereum.Hex.HexConvertors.Extensions;

namespace GetAddress
{
    class Program
    {
        private static string getAddrUrl = "http://127.0.0.1:30332/newaccount/"; //
        static void Main(string[] args)
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            HttpServerStart();
            
        }


        private static HttpListener httpGetRequest = new HttpListener();
        /// <summary>
        /// 新地址接收
        /// </summary>
        private static void HttpServerStart()
        {
            httpGetRequest.Prefixes.Add(getAddrUrl);
            httpGetRequest.Start();
            Thread ThrednHttpGetRequest = new Thread(new ThreadStart(httpGetRequestHandle));
            ThrednHttpGetRequest.Start();
        }

        private static void httpGetRequestHandle()
        {
            while (true)
            {
                httpGetRequest.Start();
                HttpListenerContext requestContext = httpGetRequest.GetContext();
                var type = requestContext.Request.RawUrl.Split('/')[2].ToString();
                StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                var info = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(info))
                {
                    var json = Newtonsoft.Json.Linq.JObject.Parse(info);
                    if (!json.ContainsKey("address"))
                        return;
                    switch (json["type"].ToString())
                    {
                        case "btc":
                           
                            break;
                        case "eth":
                           
                            break;
                        default:
                            return;
                    }

                    //DbHelper.SaveAddress(json);
                    Console.WriteLine("Add a new " + json["type"].ToString() + " address: " + json["address"].ToString());
                }
                httpGetRequest.Stop();
            }
        }
    }
}
