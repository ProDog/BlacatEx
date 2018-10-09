using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using NBitcoin;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;

namespace TransApp
{
    class Program
    {
        private static string sendTransUrl = "http://127.0.0.1:30331/trans/";
        private static string btcRpcUrl = "http://47.52.192.77:8332";  //BTC RPC url
        private static string ethRpcUrl = "http://47.52.192.77:8545/";  //ETH RPC url
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Thread HttpThread = new Thread(HttpServerStart);
            HttpThread.Start();
        }

        private static HttpListener httpPostRequest = new HttpListener();
        
        private static void HttpServerStart()
        {
            httpPostRequest.Prefixes.Add(sendTransUrl);
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
                StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                var info = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(info))
                {
                    var json = Newtonsoft.Json.Linq.JObject.Parse(info);
                    if (!json.ContainsKey("address")||!json.ContainsKey("prikey"))
                        return;
                    switch (json["type"].ToString())
                    {
                        case "btc":
                            SendBtcTrans(json);
                            break;
                        case "eth":
                            SendEthTrans(json);
                            break;
                        default:
                            return;
                    }
                    
                }

                requestContext.Response.StatusCode = 200;
                requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                requestContext.Response.ContentType = "application/json";
                requestContext.Response.ContentEncoding = Encoding.UTF8;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = "true", msg = "send success" }));
                requestContext.Response.ContentLength64 = buffer.Length;
                var output = requestContext.Response.OutputStream; output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private static void SendBtcTrans(JObject json)
        {
            var btcPriKey = new BitcoinSecret(json["prikey"].ToString());
            var network = btcPriKey.Network;
            var address = btcPriKey.GetAddress();
            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse(json["txid"].ToString());
            var transactionResponse = client.GetTransaction(transactionId).Result;

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == btcPriKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });
        }

        private static void SendEthTrans(JObject json)
        {
            var account = new ManagedAccount(json["address"].ToString(), json["prikey"].ToString());
            var web3 = new Web3(account,ethRpcUrl);
            web3.TransactionManager.SendTransactionAsync(account.Address, "", new HexBigInteger(20));
        }
    }
}
