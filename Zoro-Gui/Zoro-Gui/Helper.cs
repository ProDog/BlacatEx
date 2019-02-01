using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;

namespace Zoro_Gui
{
    class Helper
    {

        public static string HttpPost(string url, byte[] data)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers["content-type"] = "text/plain;charset=UTF-8";
                byte[] retdata = wc.UploadData(url, "POST", data);
                return System.Text.Encoding.UTF8.GetString(retdata);
            }
        }

        public static string GetJsonString(JObject item)
        {
            var type = item["type"].ToString();
            var value = item["value"];
            if (type == "ByteArray")
            {
                var bt = ZoroHelper.HexString2Bytes(value.ToString());
                string str = System.Text.Encoding.ASCII.GetString(bt);
                return str;

            }
            return "";
        }

        public static string GetJsonBigInteger(JObject item)
        {
            var type = item["type"].ToString();
            var value = item["value"];
            if (type == "ByteArray")
            {
                var bt = ZoroHelper.HexString2Bytes(value.ToString());
                var num = new BigInteger(bt);
                return num.ToString();

            }
            else if (type == "Integer")
            {
                return value.ToString();
            }
            return "";
        }

        public static string GetJsonInteger(JObject item)
        {
            var type = item["type"].ToString();
            var value = item["value"];
            if (type == "Integer")
            {
                return value.ToString();

            }
            return "";
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

        public static string HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }

        public static string Post(string url, string data, Encoding encoding, int type = 3)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            Stream reqStream = null;

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
                req.ContinueTimeout = 10;

                byte[] postData = encoding.GetBytes(data);
                reqStream = req.GetRequestStreamAsync().Result;
                reqStream.Write(postData, 0, postData.Length);

                rsp = (HttpWebResponse) req.GetResponse();
                string result = GetResponseAsString(rsp, encoding);

                return result;
            }
            catch (Exception)
            {
                throw;
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
    }
}
