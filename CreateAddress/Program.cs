using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using NBitcoin;
using Nethereum.Hex.HexConvertors.Extensions;

namespace CreateAddress
{
    class Program
    {
        private static string sendAddrUrl = "http://127.0.0.1:7081/addr/";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Thread HttpThread = new Thread(HttpServerStart);
            HttpThread.Start();
        }

        private static HttpListener httpGetRequest = new HttpListener();
      
        private static void HttpServerStart()
        {
            httpGetRequest.Prefixes.Add("http://+:7080/getaccount");
            httpGetRequest.Start();
            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpGetRequestHandle));
            ThrednHttpPostRequest.Start();
        }

        private static void httpGetRequestHandle()
        {
            string address = string.Empty;
            string priKey = string.Empty;
            var type = "";
            while (true)
            {
                httpGetRequest.Start();
                HttpListenerContext requestContext = httpGetRequest.GetContext();
                var info = requestContext.Request.RawUrl.Split('/');
                if (info.Length > 2)
                {
                    type = info[2];
                }

                if (!string.IsNullOrEmpty(type))
                {
                    switch (type)
                    {
                        case "btc":
                            var btcPrikey = new Key();
                            priKey = btcPrikey.GetWif(Network.Main).ToString();
                            address = btcPrikey.PubKey.GetAddress(Network.Main).ToString();
                            break;
                        case "eth":
                            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
                            var ethPrikey = ecKey.GetPrivateKeyAsBytes().ToHex();
                            priKey = ethPrikey.ToString();
                            address = new Nethereum.Web3.Accounts.Account(ethPrikey).Address;
                            break;
                        default:
                            priKey = null;
                            address = null;
                            break;
                    }
                    var sendString = "{\"type\":\"" + type + "\",\"address\":\"" + address + "\"}";
                    //SendAddress(sendString);
                }

                requestContext.Response.StatusCode = 200;
                requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                requestContext.Response.ContentType = "application/json";
                requestContext.Response.ContentEncoding = Encoding.UTF8;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new{priKey, address}));
                requestContext.Response.ContentLength64 = buffer.Length;
                var output = requestContext.Response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private static void SendAddress(string address)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sendAddrUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] data = System.Text.Encoding.Default.GetBytes(address);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                var result = reader.ReadToEnd();
            }
        }
    }
}
