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

            Thread BtcThread = new Thread(BtcWatcher.BtcWatcherStartAsync);
            Thread EthThread = new Thread(EthWatcher.EthWatcherStartAsync);
            Thread NeoThread = new Thread(NeoWatcher.NeoWatcherStart);
            Thread HttpThread = new Thread(HttpHelper.HttpServerStart);
            BtcThread.Start();
            EthThread.Start();
            NeoThread.Start();
            HttpThread.Start();

            //var btcTask = Task.Run(() => BtcWatcher.BtcWatcherStartAsync());
            //var ethTask = Task.Run(() => EthWatcher.EthWatcherStartAsync());
            //var neoTask = new Task(async () => NeoWatcher.NeoWatcherStart());
            //var httpTask = new Task(async () => HttpHelper.HttpServerStart());

            ////btcTask.Start();
            ////ethTask.Start();
            //neoTask.Start();
            //httpTask.Start();

            //while (true)
            //{
            //    string comm = Console.ReadLine();
            //    switch (comm)
            //    {
            //        case "btc exit":
            //            if (httpTask.Status == TaskStatus.RanToCompletion)
            //            {
            //                btcTask.Wait();
            //                Console.WriteLine(comm);
            //            }

            //            break;
            //        case "eth exit":
            //            if (ethTask.Status == TaskStatus.RanToCompletion)
            //            {
            //                ethTask.Dispose();
            //                Console.WriteLine(comm);
            //            }
            //            break;
            //        case "neo exit":
            //            if (neoTask.Status == TaskStatus.RanToCompletion)
            //            {
            //                neoTask.Dispose();
            //                Console.WriteLine(comm);
            //            }

            //            break;
            //        case "http exit":
            //            if (httpTask.Status == TaskStatus.RanToCompletion)
            //            {
            //                httpTask.Dispose();
            //                Console.WriteLine(comm);
            //            }

            //            break;
            //    }
            //}
        }

    }
}
