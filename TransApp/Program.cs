using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using NBitcoin;
using NBitcoin.Protocol;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace TransApp
{
    class Program
    {
        private static string sendTransUrl = "http://127.0.0.1:30331/trans/";
        private static string btcRpcUrl = "http://47.52.192.77:8332";  //BTC RPC url
        private static string ethRpcUrl = "http://47.52.192.77:8545/";  //ETH RPC url
        const int UNLOCK_TIMEOUT = 2 * 60; // 2 minutes (arbitrary)

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
            var uri = new Uri(btcRpcUrl);

            var btcPriKey = new BitcoinSecret(json["prikey"].ToString());
            var network = btcPriKey.Network;
            var address = btcPriKey.GetAddress();
            var client = new QBitNinjaClient(uri, network);
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
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            BitcoinAddress receiveAddress = new BitcoinPubKeyAddress("address", network);
            var transaction = Transaction.Create(network);
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            var minerFee = txInAmount.ToDecimal(MoneyUnit.BTC) * (decimal)0.02;

            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Coins(txInAmount.ToDecimal(MoneyUnit.BTC)-minerFee),
                ScriptPubKey = receiveAddress.ScriptPubKey
            });

            transaction.Inputs[0].ScriptSig = btcPriKey.ScriptPubKey;
            transaction.Sign(btcPriKey,false);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(transaction.GetHash());
            }
        }

        private static async System.Threading.Tasks.Task SendEthTrans(JObject json)
        {
            //var account = new ManagedAccount(json["address"].ToString(), json["prikey"].ToString());
            //var web3 = new Web3(account,ethRpcUrl);
            //await web3.TransactionManager.SendTransactionAsync(account.Address, "", new HexBigInteger(20));
            var web3 = new Web3(ethRpcUrl);
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(json["address"].ToString());
            var balanceEther = Web3.Convert.FromWei(balanceWei);

            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(json["address"].ToString(), json["prikey"].ToString(), UNLOCK_TIMEOUT);
            var sendTxHash = await web3.Eth.TransactionManager.SendTransactionAsync(json["address"].ToString(), "toAddress", new HexBigInteger(balanceWei));
        }
    }
}
