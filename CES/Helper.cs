﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace CES
{
    public class Helper
    {
        //获取地址的utxo来得出地址的资产  
        public static async Task<Dictionary<string, List<Utxo>>> GetBalanceByAddressAsync(string api, string _addr)
        {
            MyJson.JsonNode_Object response = (MyJson.JsonNode_Object)MyJson.Parse(await Helper.HttpGet(api + "?method=getutxo&id=1&params=['" + _addr + "']"));
            MyJson.JsonNode_Array resJA = (MyJson.JsonNode_Array)response["result"];
            Dictionary<string, List<Utxo>> _dir = new Dictionary<string, List<Utxo>>();
            foreach (MyJson.JsonNode_Object j in resJA)
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

        public static ThinNeo.Transaction makeTran(List<Utxo> utxos, List<string> usedUtxoList, string targetaddr, ThinNeo.Hash256 assetid, decimal sendCount, decimal gasfee)
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
            for (var i = utxos.Count - 1; i >= 0; i--)
            {
                if (usedUtxoList.Contains(utxos[i].txid.ToString() + utxos[i].n))
                {
                    utxos.Remove(utxos[i]);
                    continue;
                }
                ThinNeo.TransactionInput input = new ThinNeo.TransactionInput();
                input.hash = utxos[i].txid;
                input.index = (ushort)utxos[i].n;
                list_inputs.Add(input);
                count += utxos[i].value;
                scraddr = utxos[i].addr;
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
                    output.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(targetaddr);
                    list_outputs.Add(output);
                }

                //找零
                var change = count - sendCount - gasfee;
                if (change > decimal.Zero)
                {
                    var num = change;
                    int i = 0;
                    while (num > 3)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(scraddr);
                        outputchange.value = 3;
                        outputchange.assetId = assetid;
                        list_outputs.Add(outputchange);
                        num -= 3;
                        i += 1;
                        if (i >= 10)
                        {
                            break;
                        }
                    }

                    if (num > 0)
                    {
                        ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                        outputchange.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(scraddr);
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

        public static string MakeRpcUrlPost(string url, string method, out byte[] data, params MyJson.IJsonNode[] _params)
        {
            //if (url.Last() != '/')
            //    url = url + "/";
            var json = new MyJson.JsonNode_Object();
            json["id"] = new MyJson.JsonNode_ValueNumber(1);
            json["jsonrpc"] = new MyJson.JsonNode_ValueString("2.0");
            json["method"] = new MyJson.JsonNode_ValueString(method);
            StringBuilder sb = new StringBuilder();
            var array = new MyJson.JsonNode_Array();
            for (var i = 0; i < _params.Length; i++)
            {

                array.Add(_params[i]);
            }
            json["params"] = array;
            data = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            return url;
        }

        public static string MakeRpcUrl(string url, string method, params MyJson.IJsonNode[] _params)
        {
            StringBuilder sb = new StringBuilder();
            if (url.Last() != '/')
                url = url + "/";

            sb.Append(url + "?jsonrpc=2.0&id=1&method=" + method + "&params=[");
            for (var i = 0; i < _params.Length; i++)
            {
                _params[i].ConvertToString(sb);
                if (i != _params.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
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

    }
}
