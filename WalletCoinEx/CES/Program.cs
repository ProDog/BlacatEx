using System;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace CES
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"log4net.config"));
            GlobalContext.Properties["pname"] = Assembly.GetEntryAssembly().GetName().Name;
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            Console.OutputEncoding = Encoding.UTF8;

            DbHelper.CreateDb("MonitorData.db");

            Config.Init("config.json");
            Logger.Info("CES Start.");
            AppStart();
            
            Console.ReadKey();
        }

        private static void AppStart()
        {
            var btcTask = Task.Run(() => BtcWatcher.Start());
            var ethTask = Task.Run(() => EthWatcher.Start());
            var neoTask = Task.Run(() => NeoWatcher.Start());
            var httpTask = Task.Run(() => HttpHelper.Start());

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
