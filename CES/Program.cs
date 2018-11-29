using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NBitcoin;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;

namespace CES
{
    class Program
    {

        static void Main(string[] args)
        {
            DbHelper.CreateDb("MonitorData.db");
            Config.Init("config.json");

            AppStart();

            Console.ReadKey();
        }

        private static void AppStart()
        {
            var btcTask = Task.Run(() => BtcWatcher.BtcWatcherStartAsync());
            var ethTask = Task.Run(() => EthWatcher.EthWatcherStartAsync());
            var neoTask = Task.Run(() => NeoWatcher.NeoWatcherStartAsync());
            var httpTask = Task.Run(() => HttpHelper.HttpServerStart());

            while (true)
            {
                string comm = Console.ReadLine();
                switch (comm)
                {
                    case "btc exit":
                        if (httpTask.Status == TaskStatus.RanToCompletion)
                        {
                            btcTask.Wait();
                            Console.WriteLine(comm);
                        }

                        break;
                    case "eth exit":
                        if (ethTask.Status == TaskStatus.RanToCompletion)
                        {
                            ethTask.Dispose();
                            Console.WriteLine(comm);
                        }

                        break;
                    case "neo exit":
                        if (neoTask.Status == TaskStatus.RanToCompletion)
                        {
                            neoTask.Dispose();
                            Console.WriteLine(comm);
                        }

                        break;
                    case "http exit":
                        if (httpTask.Status == TaskStatus.RanToCompletion)
                        {
                            httpTask.Dispose();
                            Console.WriteLine(comm);
                        }

                        break;
                }
            }
        }

    }
}
