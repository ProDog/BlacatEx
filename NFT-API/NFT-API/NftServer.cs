using log4net;
using Neo.VM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using Zoro;
using Zoro.IO;
using Zoro.Network.P2P.Payloads;
using Zoro.Wallets;

namespace NFT_API
{
    public class NftServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static byte[] ExecRequest(HttpListenerContext requestContext)
        {
            //获取客户端传递的参数
            StreamReader sr = new StreamReader(requestContext.Request.InputStream);
            var reqMethod = requestContext.Request.RawUrl.Replace("/", "");
            var data = sr.ReadToEnd();
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RspInfo() { }));
            Logger.Warn($"Have a request:{reqMethod}; post data:{data}");
            var json = new JObject();
            if (!string.IsNullOrEmpty(data))
                json = JObject.Parse(data);

            RspInfo rspInfo = GetRsp(reqMethod, json);

            var JSRspInfo = JsonConvert.SerializeObject(rspInfo);

            Logger.Info($"result: {JSRspInfo}");

            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rspInfo));

            return buffer;
        }

        private static RspInfo GetRsp(string reqMethod, JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            ScriptBuilder sb = new ScriptBuilder();
            decimal gas = 15;
            switch (reqMethod)
            {
                case "getState":
                    return GetStateRsp();
                case "getBindNft":
                    return GetBindNftRsp(json);
                case "getNftInfo":
                    return GetNftInfoRsp(json);
                case "getUserNftCount":
                    return GetUserNftsCountRsp(json);
                case "getAllNftCount":
                    return GetNftCount();
                case "getActivatedCount":
                    return GetActivatedCount();

                case "getMoney":
                    return SendMoney(json);

                case "getApplicationLog":
                    return GetApplicationLog(json);

                case "addPoint":
                    return AddPointRsp(json);

                case "reduceGrade":
                    sb = ReduceGradeBuilder(json);
                    break;
                case "reducePoint":
                    sb = ReducePointBuilder(json);
                    break;
                case "bind":
                    sb = BindBulider(json);
                    break;
                case "buy":
                    sb = BuyBulider(json, ref gas);
                    break;
                case "activate":
                    sb = ActivateBuilder(json);
                    gas = 10;
                    break;
                case "upgrade":
                    sb = UpgradeBuilder(json);
                    break;
                case "exchange":
                    sb = ExchangeBulider(json);
                    gas = 10;
                    break;
                default:
                    return new RspInfo() { state = false, msg = "Input data error!" };
            }

            rspInfo = ExecuTransaction(sb, gas);

            return rspInfo;
        }

        private static RspInfo GetActivatedCount()
        {
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getActivatedCount");
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;

            return new RspInfo() { state = true, msg = Helper.GetJsonBigInteger(stack) };
        }

        private static RspInfo GetNftCount()
        {
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getAllNftCount");
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;

            return new RspInfo() { state = true, msg = Helper.GetJsonBigInteger(stack) };
        }

        private static RspInfo SendMoney(JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            UInt160 nep5Hash;
            decimal value = 0;
            string txid = DbHelper.GetSendMoneyTxid(json);
            if (string.IsNullOrEmpty(txid))
            {
                if (json["coinType"].ToString() == "bct")
                    nep5Hash = UInt160.Parse(Config.getStrValue("bctHash"));
                else if (json["coinType"].ToString() == "bcp")
                    nep5Hash = UInt160.Parse(Config.getStrValue("bcpHash"));
                else
                    return rspInfo;

                value = Math.Round((decimal)json["value"] * (decimal)100000000.00000000, 0);
                UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(json["address"].ToString());
                ScriptBuilder sb = new ScriptBuilder();

                KeyPair keypair = ZoroHelper.GetKeyPairFromWIF(Config.getStrValue("adminWif"));
                var adminHash = ZoroHelper.GetPublicKeyHashFromWIF(Config.getStrValue("adminWif"));

                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", nep5Hash, adminHash, targetscripthash, new BigInteger(value));
                decimal gas = ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");
                InvocationTransaction tx = ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(gas), Fixed8.FromDecimal(0.0001m));
                var result = ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
                txid = tx.Hash.ToString();

                var state = (bool)(JObject.Parse(result)["result"]);
                if (state)
                {
                    rspInfo = new RspInfo()
                    {
                        state = true,
                        msg = new TransResult() { txid = txid, key = json["key"].ToString() }
                    };
                    DbHelper.SaveSendMoneyResult(json["coinType"].ToString(), json["key"].ToString(), txid, json["address"].ToString(), (decimal)json["value"]);
                }
                else
                {
                    rspInfo = new RspInfo() { state = false, msg = result };
                }
                return rspInfo;
            }
            else
            {
                rspInfo = new RspInfo()
                {
                    state = true,
                    msg = new TransResult() { txid = txid, key = json["key"].ToString() }
                };
            }

            return rspInfo;
        }

        private static RspInfo AddPointRsp(JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            string opType = "addPoint";
            string txid = DbHelper.GetOpRecordTxid(opType, json["key"].ToString());
            if (string.IsNullOrEmpty(txid))
            {
                ScriptBuilder sb = new ScriptBuilder();
                List<dynamic> paraList = new List<dynamic>();
                paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));
                paraList.Add(ZoroHelper.GetParamBytes("(int)" + Config.getIntValue("memberPoint")));
                sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "addPoint", paraList.ToArray());

                KeyPair keypair = ZoroHelper.GetKeyPairFromWIF(Config.getStrValue("adminWif"));
                //gas = ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");
                InvocationTransaction tx = ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(15 * 1000), Fixed8.FromDecimal(0.0001m));
                txid = tx.Hash.ToString();
                var result = ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
                try
                {
                    var state = (bool)(JObject.Parse(result)["result"]);
                    if (state)
                    {
                        rspInfo = new RspInfo() { state = true, msg = new SendRawResult() { txid = txid, nftHash = Config.nftHash } };
                        DbHelper.SaveOpRecordResult(opType, json["key"].ToString(), txid);
                    }
                    else
                        rspInfo = new RspInfo() { state = false, msg = result };
                }
                catch
                {
                    Logger.Error("Trans Error: " + result);
                    rspInfo = new RspInfo() { state = false, msg = result };
                }
            }
            else
            {
                rspInfo= new RspInfo() { state = true, msg = new SendRawResult() { txid = txid, nftHash = Config.nftHash } };
            }
            return rspInfo;
        }

        private static RspInfo ExecuTransaction(ScriptBuilder sb, decimal gas)
        {
            RspInfo rspInfo;
            KeyPair keypair = ZoroHelper.GetKeyPairFromWIF(Config.getStrValue("adminWif"));

            //gas = ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");
            InvocationTransaction tx = ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(gas * 1000), Fixed8.FromDecimal(0.0001m));
            var txid = tx.Hash.ToString();
            var result = ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
            try
            {
                var state = (bool)(JObject.Parse(result)["result"]);
                if (state)
                    rspInfo = new RspInfo() { state = true, msg = new SendRawResult() { txid = txid, nftHash = Config.nftHash } };
                else
                    rspInfo = new RspInfo() { state = false, msg = result };
            }
            catch
            {
                Logger.Error("Trans Error: " + result);
                rspInfo = new RspInfo() { state = false, msg = result };
            }

            return rspInfo;
        }

        private static RspInfo GetApplicationLogs(JObject json)
        {
            var paramsArr = json["params"] as JArray;
            List<ApplicationLog> applicationLogList = new List<ApplicationLog>();

            for (int i = 0; i < paramsArr.Count; i++)
            {
                var txid= paramsArr[i]["txid"].ToString();
                var method = paramsArr[i]["method"].ToString();

                ApplicationLog applicationLog = new ApplicationLog();
                applicationLog.txid = txid;
                applicationLog.method = method;
                applicationLog.height = ZoroHelper.GetHeight();

                string url = Config.getStrValue("myApi") + $"?jsonrpc=2.0&id=1&method=getapplicationlog&params=['',\"{txid}\"]";
                //string url = "http://127.0.0.1:20333" + $"?jsonrpc=2.0&id=1&method=getapplicationlog&params=['',\"{txid}\"]";
                var result = Helper.HttpGet(url);
                if (result.Contains("error"))
                {
                    applicationLog.transExisted = false;
                }
                else
                {
                    try
                    {
                        applicationLog.transExisted = true;
                        var executions = (JObject.Parse(result)["result"]["executions"] as JArray)[0] as JObject;
                        applicationLog.vmstate = executions["vmstate"].ToString();
                        applicationLog.gas_consumed = int.Parse(executions["gas_consumed"].ToString());

                        var notificationsArray = executions["notifications"] as JArray;
                        switch (method)
                        {
                            case "buy":
                                applicationLog.applicationLog = GetBuyLog(notificationsArray[0] as JObject);
                                break;
                            case "activate":
                                applicationLog.applicationLog = GetActivateLog(notificationsArray);
                                break;
                            case "bind":
                                applicationLog.applicationLog = GetBindLog(notificationsArray[0] as JObject);
                                break;
                            case "exchange":
                                applicationLog.applicationLog = GetExchange(notificationsArray[0] as JObject);
                                break;
                            case "upgrade":
                                applicationLog.applicationLog = GetUpgradeLog(notificationsArray);
                                break;
                            case "addPoint":
                                applicationLog.applicationLog = GetAddPointLog(notificationsArray[0] as JObject);
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Parse applicationLog error: " + ex.Message);
                        Logger.Warn("ApplicationLog result: " + result);
                        applicationLog.applicationLog = null;
                    }
                }

                applicationLogList.Add(applicationLog);
            }
            return new RspInfo() { state = true, msg = applicationLogList };
        }

        private static RspInfo GetApplicationLog(JObject json)
        {
            var txid = json["txid"].ToString();
            var method = json["method"].ToString();

            ApplicationLog applicationLog = new ApplicationLog();
            applicationLog.height = ZoroHelper.GetHeight();
            applicationLog.txid = txid;
            applicationLog.method = method;
            string url = Config.getStrValue("myApi") + $"?jsonrpc=2.0&id=1&method=getapplicationlog&params=['',\"{txid}\"]";
            var result = Helper.HttpGet(url);
            if (result.Contains("error"))
            {
                applicationLog.transExisted = false;
            }
            else
            {
                try
                {
                    applicationLog.transExisted = true;
                    var executions = (JObject.Parse(result)["result"]["executions"] as JArray)[0] as JObject;
                    applicationLog.vmstate = executions["vmstate"].ToString();
                    applicationLog.gas_consumed = int.Parse(executions["gas_consumed"].ToString());
                    var notificationsArray = executions["notifications"] as JArray;

                    switch (method)
                    {
                        case "buy":
                            applicationLog.applicationLog = GetBuyLog(notificationsArray[0] as JObject);
                            break;
                        case "activate":
                            applicationLog.applicationLog = GetActivateLog(notificationsArray);
                            break;
                        case "bind":
                            applicationLog.applicationLog = GetBindLog(notificationsArray[0] as JObject);
                            break;
                        case "exchange":
                            applicationLog.applicationLog = GetExchange(notificationsArray[0] as JObject);
                            break;
                        case "upgrade":
                            applicationLog.applicationLog = GetUpgradeLog(notificationsArray);
                            break;
                        case "addPoint":
                            applicationLog.applicationLog = GetAddPointLog(notificationsArray[0] as JObject);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Parse applicationLog error: " + ex.Message);
                    Logger.Warn("ApplicationLog result: " + result);
                    applicationLog.applicationLog = null;
                }
            }
            return new RspInfo() { state = true, msg = applicationLog };
        }

        private static RspInfo GetUserNftsCountRsp(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            var addr = ZoroHelper.GetParamBytes("(addr)" + json["address"].ToString());
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getUserNftCount", addr);
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;

            return new RspInfo() { state = true, msg = Helper.GetJsonBigInteger(stack) };
        }

        private static List<string> GetTokenList(JObject stack)
        {
            List<string> nftList = new List<string>();
            var value = stack["value"] as JArray;
            if (value.Count > 0)
            {
                foreach (var nft in value)
                {
                    nftList.Add(nft["key"]["value"].ToString());
                }
            }
            return nftList;
        }

        private static RspInfo GetNftInfoRsp(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            var tokenId = ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString());
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getNftInfo", tokenId);
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;

            var nftInfo = new NFTInfo();
            var value = stack["value"] as JArray;

            if (value != null && value.Count > 5)
            {
                if (value[0]["type"].ToString() == "ByteArray")
                    nftInfo.tokenId = value[0]["value"].ToString();

                nftInfo.owner = Helper.GetJsonAddress((JObject)value[1]);
                nftInfo.grade = Helper.GetJsonInteger((JObject)value[2]);
                nftInfo.allPoint = Helper.GetJsonInteger((JObject)value[3]);
                nftInfo.availablePoint = Helper.GetJsonInteger((JObject)value[4]);

                if (value[5]["type"].ToString() == "ByteArray")
                    nftInfo.inviterTokenId = value[5]["value"].ToString();

                nftInfo.IsActivated = string.IsNullOrEmpty(((JObject)value[6])["value"].ToString()) ? false : true;
            }
            return new RspInfo() { state = true, msg = nftInfo };
        }

        private static RspInfo GetBindNftRsp(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            var addr = ZoroHelper.GetParamBytes("(addr)" + json["address"].ToString());
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getBindNft", addr);
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;

            return new RspInfo() { state = true, msg = stack["value"].ToString() };
        }

        private static RspInfo GetStateRsp()
        {
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "getState");
            var result = ZoroHelper.InvokeScript(sb.ToArray(), "");
            var stack = (JObject.Parse(result)["result"]["stack"] as JArray)[0] as JObject;
            return new RspInfo() { state = true, msg = stack["value"].ToString() };
        }

        private static AddGradeLog GetUpgradeLog(JArray notificationsArray)
        {
            var addGradeLog = new AddGradeLog();
            var upgradeLog = new UpgradeLog();
            var notification = notificationsArray[0] as JObject;
            var jValue = notification["state"]["value"] as JArray;
            upgradeLog.tokenId = jValue[1]["value"].ToString();
            upgradeLog.ownerAddress = Helper.GetJsonAddress((JObject)jValue[2]);
            upgradeLog.lastGrade= (int)jValue[3]["value"];
            upgradeLog.nowGrade = (int)jValue[4]["value"];

            addGradeLog.upgradeLog = upgradeLog;
            addGradeLog.addPointLog = GetAddPointLog(notificationsArray[1] as JObject);

            return addGradeLog;
        }

        private static ExchangeLog GetExchange(JObject notification)
        {
            var exchangeLog = new ExchangeLog();
            var jValue = notification["state"]["value"] as JArray;
            exchangeLog.from = Helper.GetJsonAddress((JObject)jValue[1]);
            exchangeLog.to = Helper.GetJsonAddress((JObject)jValue[2]);
            exchangeLog.tokenId = jValue[3]["value"].ToString();
            return exchangeLog;
        }

        private static ActivateLog GetActivateLog(JArray notificationsArray)
        {
            ActivateLog activateLog = new ActivateLog();
            activateLog.addPointLog = new List<AddPointLog>();
            if (notificationsArray.Count == 2)
            {
                activateLog.addPointLog.Add(GetAddPointLog(notificationsArray[0] as JObject));
                activateLog.tokenId = GetActivateTokenId(notificationsArray[1] as JObject);
            }
            else if (notificationsArray.Count == 3)
            {
                activateLog.addPointLog.Add(GetAddPointLog(notificationsArray[0] as JObject));
                activateLog.addPointLog.Add(GetAddPointLog(notificationsArray[1] as JObject));
                activateLog.tokenId = GetActivateTokenId(notificationsArray[2] as JObject);
            }
            else
                throw new Exception("ApplicationLog error.");
            return activateLog;
        }

        private static string GetActivateTokenId(JObject notification)
        {
            var jValue = notification["state"]["value"] as JArray;           
            return jValue[2]["value"].ToString();
        }

        private static BindLog GetBindLog(JObject notification)
        {
            var bindLog = new BindLog();
            var jValue = notification["state"]["value"] as JArray;
            bindLog.ownerAddress = Helper.GetJsonAddress((JObject)jValue[1]);
            bindLog.tokenId= jValue[2]["value"].ToString();
            return bindLog;
        }

        private static BuyNftLog GetBuyLog(JObject notification)
        {
            var buyNftLog = new BuyNftLog();
            var jValue = notification["state"]["value"] as JArray;
            buyNftLog.ownerAddress = Helper.GetJsonAddress((JObject)jValue[1]);
            buyNftLog.inviterTokenId = jValue[2]["value"].ToString();
            buyNftLog.buyCount = Helper.GetJsonBigInteger((JObject)jValue[3]);
            buyNftLog.payValue = (long)Helper.GetJsonBigInteger((JObject)jValue[4]) / 100000000;

            buyNftLog.tokenIdList = GetTokenList((JObject)jValue[5]);
            
            return buyNftLog;
        }

        private static AddPointLog GetAddPointLog(JObject notification)
        {
            var addPointLog = new AddPointLog();
            var jValue = notification["state"]["value"] as JArray;
            addPointLog.tokenId = jValue[1]["value"].ToString();
            addPointLog.ownerAddress = Helper.GetJsonAddress((JObject)jValue[2]);
            addPointLog.addPoint = Helper.GetJsonBigInteger((JObject)jValue[3]);

            return addPointLog;
        }

        private static ScriptBuilder ExchangeBulider(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();
            paraList.Add(ZoroHelper.GetParamBytes("(addr)" + json["from"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(addr)" + json["to"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));

            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "exchange", paraList.ToArray());

            return sb;
        }

        private static ScriptBuilder UpgradeBuilder(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();

            int nowGrade = int.Parse(json["nowGrade"].ToString());

            long receivableValue;
            int needPoint;
            GetUpgradeParams(out receivableValue, out needPoint, nowGrade);

            if (needPoint == 0) return null;

            if (long.Parse(json["transferValue"].ToString()) * 100000000 < receivableValue)
                return null;
            if (json["gatherAddress"].ToString() != Config.getStrValue("gatherAddress"))
                return null;

            paraList.Add(ZoroHelper.GetParamBytes("(hex160)" + Config.getStrValue("bctHash")));
            paraList.Add(ZoroHelper.GetParamBytes("(hex256)" + json["txid"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + receivableValue.ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + needPoint.ToString()));

            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "upgrade", paraList.ToArray());

            return sb;
        }

        private static ScriptBuilder ActivateBuilder(JObject json)
        {
            int pointValue = Config.getIntValue("silverPoint");
            int twoLevelInviterPoint = pointValue * Config.getIntValue("twoLevelPercent") / 100;

            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();
            string tokenId = json["tokenId"].ToString();

            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + tokenId));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + pointValue));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + twoLevelInviterPoint));

            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "activate", paraList.ToArray());
            return sb;
        }

        private static ScriptBuilder BuyBulider(JObject json, ref decimal gas)
        {
            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();
            string inviterTokenId = json["inviterTokenId"].ToString();

            int count = int.Parse(json["count"].ToString());
            gas = 100;
            if (count > 2) gas += 2 * (count - 2);

            long receivableValue = GetReceivableValue(count);
            if (decimal.Parse(json["transferValue"].ToString()) * 100000000 < receivableValue)
                return null;
            if (json["gatherAddress"].ToString() != Config.getStrValue("gatherAddress"))
                return null;

            paraList.Add(ZoroHelper.GetParamBytes("(hex160)" + Config.getStrValue("bctHash")));
            paraList.Add(ZoroHelper.GetParamBytes("(hex256)" + json["txid"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + json["count"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + inviterTokenId));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + receivableValue.ToString()));

            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "buy", paraList.ToArray());
            return sb;
        }

        private static ScriptBuilder BindBulider(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();
            paraList.Add(ZoroHelper.GetParamBytes("(addr)" + json["address"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "bind", paraList.ToArray());
            return sb;
        }

        private static ScriptBuilder ReducePointBuilder(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            List<dynamic> paraList = new List<dynamic>();
            paraList.Add(ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));
            paraList.Add(ZoroHelper.GetParamBytes("(int)" + "-" + json["pointValue"].ToString()));
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "addPoint", paraList.ToArray());
            return sb;
        }

        private static ScriptBuilder ReduceGradeBuilder(JObject json)
        {
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(Config.getStrValue("nftHash")), "reduceGrade", ZoroHelper.GetParamBytes("(bytes)" + json["tokenId"].ToString()));
            return sb;
        }

        private static void GetUpgradeParams(out long receivableValue, out int needPoint, int nowGrade)
        {
            if (nowGrade == 1)
            {
                receivableValue = Config.getLongValue("goldPrice");
                needPoint = Config.getIntValue("goldUpgradePoint");
            }

            else if (nowGrade == 2)
            {
                receivableValue = Config.getLongValue("platinumPrice");
                needPoint = Config.getIntValue("platinumUpgradePoint");
            }

            else if (nowGrade == 3)
            {
                receivableValue = Config.getLongValue("diamondPrice");
                needPoint = Config.getIntValue("diamondUpgradePoint");
            }

            else
            {
                receivableValue = 0;
                needPoint = 0;
            }
        }

        private static long GetReceivableValue(int count)
        {
            long receivableValue = Config.getLongValue("silverPrice") * count;

            //if (count >= Config.getIntValue("threeDiscountCount"))
            //    return receivableValue * Config.getIntValue("threeDiscountPercent") / 100;
            if (count >= Config.getIntValue("twoDiscountCount"))
                return receivableValue * Config.getIntValue("twoDiscountPercent") / 100;
            else if (count >= Config.getIntValue("oneDiscountCount"))
                return receivableValue * Config.getIntValue("oneDiscountPercent") / 100;
            else
                return receivableValue;
        }

    }
}
