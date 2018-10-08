using System;
using Microsoft.Owin.Hosting;

namespace CreateAccount
{
    class Program
    {
        private static IDisposable _webApp;
        static void Main(string[] args)
        {
            var host = "http://+:7080";
            _webApp = WebApp.Start<Startup>(host);
            Console.WriteLine("Start:" + host);
            Console.ReadKey();
        }
    }
}
