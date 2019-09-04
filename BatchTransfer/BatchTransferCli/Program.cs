using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ThinNeo;

namespace BatchTransferCli
{
    class Program
    {
        private static List<string> addrList;
        private static string contractHash;
        private static int timeInterval;
        private static string rpcUrl;
        private static string wif;

        static void Main(string[] args)
        {
            Console.WriteLine("Batch Transfer Start!");

            Console.WriteLine("Please input asset hash:");
            contractHash = Console.ReadLine();

            Console.WriteLine("Please input your wif:");
            wif = Console.ReadLine();

            Console.WriteLine("Please input time interval S:");
            timeInterval = int.Parse(Console.ReadLine()) * 1000;

            Console.WriteLine("Please select net type, 0 testnet; 1 mainnet:");

            var aa = int.Parse(Console.ReadLine());
            if (aa == 1)
                rpcUrl = "https://api.nel.group/api/mainnet";
            if (aa == 0)
                rpcUrl = "https://api.nel.group/api/testnet";

            addrList = new List<string>();

            LoadFromAddress("address.txt");

            SendTransaction();

            Console.WriteLine("All send out!");

            Console.ReadKey();
        }

        protected static void LoadFromAddress(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} not exist!");
                return;
            }

            StreamReader sr = new StreamReader(filename, Encoding.Default);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                addrList.Add(line.ToString());
            }
          
        }

        private static object logLock = new object();
        private static void SendTransaction()
        {
            string path = Path.Combine($"{DateTime.Now:yyyy-MM-dd}.txt");
            string errPath = Path.Combine($"{DateTime.Now:yyyy-MM-dd}_err.txt");

            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(wif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);
            
            decimal decimals = 100000000;

            foreach (var str in addrList)
            {
                if (str.Length < 1)
                    continue;
                JArray array = new JArray();

                int index = str.IndexOf(";");
                string addr = str.Substring(0, index);
                string valueStr = str.Substring(index + 1);

                decimal amount = Math.Round(decimal.Parse(valueStr) * decimals, 0);

                array.Add("(addr)" + address); //from
                array.Add("(addr)" + addr); //to
                array.Add("(int)" + amount); //value

                string result = Helper.SendTransWithoutUtxo(prikey, rpcUrl, contractHash, "transfer", array);

                if (result != null && result.Contains("result"))
                {
                    var res = JObject.Parse(result)["result"] as JArray;
                    var sendTxid = (string)res[0]["txid"];
                    if (!string.IsNullOrEmpty(sendTxid))
                    {
                        Console.WriteLine($"{addr} :交易发送成功; txid:{sendTxid},value:{valueStr}");
                        lock (logLock)
                        {
                            File.AppendAllLines(path, new[] { addr + ":交易发送成功; txid:" + sendTxid });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{addr} :交易发送失败; 返回:{result.ToString()}");
                        lock (logLock)
                        {
                            File.AppendAllLines(errPath, new[] { addr + ":交易发送失败; 返回:" + result.ToString()});
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{addr} :交易发送失败; 返回:{result.ToString()}");
                    lock (logLock)
                    {
                        File.AppendAllLines(errPath, new[] { addr + ":交易发送失败; 返回:" + result.ToString()});
                    }
                }

                Thread.Sleep(timeInterval);
            }
        }
    }
}
