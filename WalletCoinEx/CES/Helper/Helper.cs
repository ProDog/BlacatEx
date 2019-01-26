using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CES.Helper
{
    class Helper
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }

        public static string HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = wc.UploadData(url, "POST", data);
            return Encoding.UTF8.GetString(retdata);
        }

        public static string MakeRpcUrlPost(string url, string method, out byte[] data, JArray postArray)
        {
            var json = new JObject();
            json["id"] = new JValue(1);
            json["jsonrpc"] = new JValue("2.0");
            json["method"] = new JValue(method);
            var array = new JArray();
            for (var i = 0; i < postArray.Count; i++)
            {
                array.Add(postArray[i]);
            }
            json["params"] = array;
            data = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            return url;
        }

        public static async Task<string> PostAsync(string url, string data, Encoding encoding, int type = 3)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            Stream reqStream = null;
            //Stream resStream = null;

            try
            {
                req = WebRequest.CreateHttp(new Uri(url));
                if (type == 1)
                {
                    req.ContentType = "application/json;charset=utf-8";
                }
                else if (type == 2)
                {
                    req.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                }

                req.Method = "POST";
                //req.Accept = "text/xml,text/javascript";
                req.ContinueTimeout = 60000;

                byte[] postData = encoding.GetBytes(data);
                reqStream = await req.GetRequestStreamAsync();
                reqStream.Write(postData, 0, postData.Length);
                //reqStream.Dispose();

                rsp = (HttpWebResponse)req.GetResponseAsync().Result;
                string result = GetResponseAsString(rsp, encoding);

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return e.Message;
            }
            finally
            {
                // 释放资源
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream = null;
                }
                if (rsp != null)
                {
                    rsp.Close();
                    rsp = null;
                }
                if (req != null)
                {
                    req.Abort();

                    req = null;
                }
            }
        }

        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null)
                    reader.Close();
                if (stream != null)
                    stream.Close();

                reader = null;
                stream = null;

            }
        }

        /// <summary>
        /// 发送交易数据
        /// </summary>
        /// <param name="transRspList">交易数据列表</param>
        public static void SendTransInfo(List<TransactionInfo> transRspList)
        {
            if (transRspList.Count > 0)
            {
                try
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(transRspList.GetType());
                    MemoryStream meStream = new MemoryStream();
                    serializer.WriteObject(meStream, transRspList);
                    byte[] dataBytes = new byte[meStream.Length];
                    meStream.Position = 0;
                    meStream.Read(dataBytes, 0, (int)meStream.Length);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Config.apiDic["blacat"]);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = dataBytes;
                    req.ContentLength = data.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }

                    Logger.Info("SendTransInfo : " + Encoding.UTF8.GetString(data));
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponseAsync().Result;
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = reader.ReadToEnd();
                        var rjson = JObject.Parse(result);
                        Logger.Info("rsp: " + result);
                        if (Convert.ToInt32(rjson["r"]) == 0)
                        {
                            Logger.Warn("Send fail:" + rjson.ToString());
                            Thread.Sleep(5000);
                            SendTransInfo(transRspList);
                        }

                        if (Convert.ToInt32(rjson["r"]) == 1)
                        {
                            //保存交易信息
                            DbHelper.SaveTransInfo(transRspList);
                        }

                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Send error:" + ex.Message);
                    return;
                }

            }

        }

    }
}
