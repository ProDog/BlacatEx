using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class MyHelper
    {
        //获取地址的utxo来得出地址的资产  
        public static async Task<Dictionary<string, List<Utxo>>> GetBalanceByAddressAsync(string api, string _addr)
        {
            JObject response = JObject.Parse(await MyHelper.HttpGet(api + "?method=getutxo&id=1&params=['" + _addr + "']"));
            JArray resJA = (JArray)response["result"];
            Dictionary<string, List<Utxo>> _dir = new Dictionary<string, List<Utxo>>();
            foreach (JObject j in resJA)
            {
                Utxo utxo = new Utxo(j["addr"].ToString(), new ThinNeo.Hash256(j["txid"].ToString()), j["asset"].ToString(), decimal.Parse(j["value"].ToString()), int.Parse(j["n"].ToString()));
                if (_dir.ContainsKey(j["asset"].ToString()))
                {
                    _dir[j["asset"].ToString()].Add(utxo);
                }
                else
                {
                    List<Utxo> l = new List<Utxo>();
                    l.Add(utxo);
                    _dir[j["asset"].ToString()] = l;
                }

            }
            return _dir;
        }

        public static ThinNeo.Transaction makeTran(List<Utxo> utxos, ref List<string> usedUtxoList, string targetaddr, ThinNeo.Hash256 assetid, decimal sendCount, decimal gasfee)
        {
            if (sendCount == 0) //sendCount==0,说明是合约交易，gasfee做sendCount
            {
                sendCount = gasfee;
                gasfee = 0;
            }

            var tran = new ThinNeo.Transaction();
            tran.type = ThinNeo.TransactionType.ContractTransaction;
            tran.version = 0;//0 or 1
            
            tran.attributes = new ThinNeo.Attribute[0];
            var scraddr = "";
            utxos.Sort((a, b) =>
            {
                if (a.value > b.value)
                    return 1;
                else if (a.value < b.value)
                    return -1;
                else
                    return 0;
            });
            decimal count = decimal.Zero;
            List<ThinNeo.TransactionInput> list_inputs = new List<ThinNeo.TransactionInput>();
            for (var i = 0; i < utxos.Count; i++)
            {
                if (usedUtxoList.Contains(utxos[i].txid.ToString() + utxos[i].n))
                {
                    continue;
                }
                ThinNeo.TransactionInput input = new ThinNeo.TransactionInput();
                input.hash = utxos[i].txid;
                input.index = (ushort)utxos[i].n;
                list_inputs.Add(input);
                count += utxos[i].value;
                scraddr = utxos[i].addr;
                usedUtxoList.Add(utxos[i].txid.ToString() + utxos[i].n);
                if (count >= sendCount)
                {
                    break;
                }
            }
            tran.inputs = list_inputs.ToArray();
            if (count >= sendCount)//输入大于等于输出
            {
                List<ThinNeo.TransactionOutput> list_outputs = new List<ThinNeo.TransactionOutput>();
                //输出
                if (sendCount > decimal.Zero && targetaddr != null)
                {
                    ThinNeo.TransactionOutput output = new ThinNeo.TransactionOutput();
                    output.assetId = assetid;
                    output.value = sendCount;
                    output.toAddress = Helper_NEO.GetScriptHash_FromAddress(targetaddr);
                    list_outputs.Add(output);
                }

                //找零
                var change = count - sendCount - gasfee;
                if (change > decimal.Zero)
                {
                    var num = change;
                    int i = 0;
                    decimal splitvalue = 3;
                    while (num > splitvalue && utxos.Count - usedUtxoList.Count < 100)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = Helper_NEO.GetScriptHash_FromAddress(scraddr);
                        outputchange.value = splitvalue;
                        outputchange.assetId = assetid;
                        list_outputs.Add(outputchange);
                        num -= splitvalue;
                        i += 1;
                        if (i >= 100)
                        {
                            break;
                        }
                    }

                    if (num > 0)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = Helper_NEO.GetScriptHash_FromAddress(scraddr);
                        outputchange.value = num;
                        outputchange.assetId = assetid;
                        list_outputs.Add(outputchange);
                    }

                }

                tran.outputs = list_outputs.ToArray();
            }
            else
            {
                throw new Exception("no enough money.");
            }
            return tran;
        }

        #region NEO 接口，有了 CNEO 后弃用

        //public static Transaction makeUtxoTran(Dictionary<string, List<Utxo>> dic_UTXO, List<string> usedUtxoList, string targetAddr, Dictionary<string, string> tokenHashDic, string type, decimal sendCount, decimal gasfee)
        //{
        //    var tran = new ThinNeo.Transaction();
        //    tran.type = ThinNeo.TransactionType.ContractTransaction;
        //    tran.version = 0;//0 or 1
        //    var assetid = new Hash256(tokenHashDic[type]);
        //    var utxos = dic_UTXO[tokenHashDic[type]];

        //    tran.attributes = new ThinNeo.Attribute[0];
        //    var scraddr = "";
        //    utxos.Sort((a, b) =>
        //    {
        //        if (a.value > b.value)
        //            return 1;
        //        else if (a.value < b.value)
        //            return -1;
        //        else
        //            return 0;
        //    });
        //    decimal count = decimal.Zero;
        //    List<ThinNeo.TransactionInput> list_inputs = new List<ThinNeo.TransactionInput>();
        //    for (var i = utxos.Count - 1; i >= 0; i--)
        //    {
        //        if (usedUtxoList.Contains(utxos[i].txid.ToString() + utxos[i].n))
        //        {
        //            utxos.Remove(utxos[i]);
        //            continue;
        //        }

        //        ThinNeo.TransactionInput input = new ThinNeo.TransactionInput();
        //        input.hash = utxos[i].txid;
        //        input.index = (ushort) utxos[i].n;
        //        list_inputs.Add(input);
        //        count += utxos[i].value;
        //        scraddr = utxos[i].addr;
        //        if (count >= sendCount)
        //        {
        //            break;
        //        }
        //    }

        //    tran.inputs = list_inputs.ToArray();
        //    if (count >= sendCount)//输入大于等于输出
        //    {
        //        List<ThinNeo.TransactionOutput> list_outputs = new List<ThinNeo.TransactionOutput>();
        //        //输出
        //        if (sendCount > decimal.Zero && targetAddr != null)
        //        {
        //            ThinNeo.TransactionOutput output = new ThinNeo.TransactionOutput();
        //            output.assetId = assetid;
        //            output.value = sendCount;
        //            output.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(targetAddr);
        //            list_outputs.Add(output);
        //        }

        //        //找零
        //        var change = count - sendCount - gasfee;
        //        if (change > decimal.Zero)
        //        {

        //            var num = change;
        //            int i = 0;
        //            while (num > 3)
        //            {
        //                ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
        //                outputchange.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(scraddr);
        //                outputchange.value = 3;
        //                outputchange.assetId = assetid;
        //                list_outputs.Add(outputchange);
        //                num -= 3;
        //                i += 1;
        //                if (i >= 10)
        //                {
        //                    break;
        //                }
        //            }

        //            if (num > 0)
        //            {
        //                ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
        //                outputchange.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(scraddr);
        //                outputchange.value = num;
        //                outputchange.assetId = assetid;
        //                list_outputs.Add(outputchange);
        //            }

        //        }

        //        tran.outputs = list_outputs.ToArray();
        //    }
        //    else
        //    {
        //        throw new Exception("no enough money.");
        //    }
        //    return tran;
        //}

        #endregion

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

        public static async Task<string> HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return await wc.DownloadStringTaskAsync(url);
        }

        public static async Task<string> HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = await wc.UploadDataTaskAsync(url, "POST", data);
            return System.Text.Encoding.UTF8.GetString(retdata);
        }

        /// <summary>
        /// 发送交易数据
        /// </summary>
        /// <param name="transRspList">交易数据列表</param>
        public static async Task SendTransInfoAsync(List<TransactionInfo> transRspList, Logger logger)
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

                    logger.Log("SendTransInfo : " + Encoding.UTF8.GetString(data));
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponseAsync().Result;
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = reader.ReadToEnd();
                        var rjson = JObject.Parse(result);
                        logger.Log("rsp: " + result);
                        if (Convert.ToInt32(rjson["r"]) == 0)
                        {
                            Thread.Sleep(5000);
                            await SendTransInfoAsync(transRspList, logger);
                        }

                        if (Convert.ToInt32(rjson["r"]) == 1)
                        {
                            //保存交易信息
                            await DbHelper.SaveTransInfoAsync(transRspList);
                        }

                    }

                }
                catch (Exception ex)
                {
                    logger.Log("send error:" + ex.ToString());
                    return;
                }

            }

        }

    }
}
