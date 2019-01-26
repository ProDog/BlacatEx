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

            Helper.DbHelper.CreateDb();

            Config.Init("config.json");
            
            AppStart();
            
            Console.ReadKey();
        }

        private static void AppStart()
        {
            var btcTask = Task.Run(() => BtcServer.Start());
            var ethTask = Task.Run(() => EthServer.Start());
            var neoTask = Task.Run(() => NeoServer.Start());
            var httpTask = Task.Run(() => HttpServer.Start());

            Logger.Info("CES Start.");
        }

    }
}
