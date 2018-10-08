using System;
using System.IO;
using System.Net;
using System.Text;
using Nancy;
using NBitcoin;
using Nethereum.Hex.HexConvertors.Extensions;
using Owin;

namespace CreateAccount
{
    public class CommService : NancyModule
    {
        private static string sendAddrUrl = "http://127.0.0.1:30000/addr/"; //接收新地址 url
        string _jsonString = string.Empty;
        public CommService() : base("/getaccount")
        {
            Get[@"/{type}"] = x => DoCreateAccount(x.type);
        }

        private Response DoCreateAccount(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;
            string address;
            string priKey;
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
                    return null;
            }
            _jsonString = "{\"priKey\":\"" + priKey + "\",\"address\":\"" + address + "\"}";

            var sendString = "{\"type\":\"" + type + "\",\"address\":\"" + address + "\"}";
            SendAddress(sendString);

            return Response.AsText(_jsonString, "text/html;charset=UTF-8");
        }

        private void SendAddress(string address)
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
