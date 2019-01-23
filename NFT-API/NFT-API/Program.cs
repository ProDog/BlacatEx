﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace NFT_API
{
    public class Program
    {
        private static HttpListener httpListener = new HttpListener();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            DbHelper.CreateDb();
            Config.init("config.json");

            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"log4net.config"));
            Console.OutputEncoding = Encoding.UTF8;

            HttpServerStart();
        }

        private static void HttpServerStart()
        {
            Logger.Info("Http Server Start!");
            httpListener.Prefixes.Add(Config.getStrValue("httpAddress"));
            while (true)
            {
                httpListener.Start();

                HttpListenerContext requestContext = httpListener.GetContext();
                //logger.Log("Have a request: " + requestContext.Request.RawUrl);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RspInfo()
                { state = false, msg = "" }));
                try
                {
                    buffer = NftServer.ExecRequest(requestContext);
                }
                catch (Exception e)
                {
                    var rsp = JsonConvert.SerializeObject(new RspInfo()
                    { state = false, msg = e.Message });
                    buffer = Encoding.UTF8.GetBytes(rsp);
                    Logger.Error(rsp);
                }
                finally
                {
                    requestContext.Response.StatusCode = 200;
                    requestContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    requestContext.Response.ContentType = "application/json";
                    requestContext.Response.ContentEncoding = Encoding.UTF8;
                    requestContext.Response.ContentLength64 = buffer.Length;
                    var output = requestContext.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

    }

}