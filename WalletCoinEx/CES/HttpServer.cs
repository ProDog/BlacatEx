using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;
using Neo.VM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zoro;

namespace CES
{
    public class HttpServer
    {
        private static HttpListener httpListener = new HttpListener();
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Http 服务接口
        /// </summary>
        public static void Start()
        {
            Logger.Info("Http Server Start!");
            httpListener.Prefixes.Add(Config.apiDic["http"]);
            while (true)
            {
                httpListener.Start();
                HttpListenerContext requestContext = httpListener.GetContext();

                byte[] buffer = new byte[] { };
                RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };

                try
                {
                    //获取客户端传递的参数
                    StreamReader sr = new StreamReader(requestContext.Request.InputStream);
                    var reqMethod = requestContext.Request.RawUrl.Replace("/", "");
                    var data = sr.ReadToEnd();

                    var json = new JObject();
                    if (!string.IsNullOrEmpty(data))
                        json = JObject.Parse(data);

                    Logger.Info($"Have a request:{reqMethod}; post data:{data}");

                    rspInfo = GetResponse(reqMethod, json);

                    var rsp = JsonConvert.SerializeObject(rspInfo);
                    buffer = Encoding.UTF8.GetBytes(rsp);
                }

                catch (Exception e)
                {
                    var rsp = JsonConvert.SerializeObject(new RspInfo() { state = false, msg = e.Message });
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

        private static RspInfo GetResponse(string reqMethod, JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            switch (reqMethod)
            {
                case "getBalance":
                    rspInfo = GetBalanceRsp(json["coinType"].ToString());
                    break;
                case "getAccount":
                    rspInfo = GetAccountRsp(json["coinType"].ToString());
                    break;
                case "deployNep5":
                    rspInfo = DeployNep5Rsp(json);
                    break;
                case "addAddress":
                    rspInfo = AddAddressRsp(json);
                    break;
                case "gatherCoin":
                    rspInfo = GatherCoinRsp(json);
                    break;
                case "exchange":
                    rspInfo = ExchangeCoinRsp(json);
                    break;
                default:
                    break;
            }          

            return rspInfo;
        }

        private static RspInfo DeployNep5Rsp(JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };
            var transResult = new TransResult();
            string coinType = json["coinType"].ToString();

            if (coinType == "bct" || coinType == "bcp")
            {
                return ExchangeCoinRsp(json);
            }

            string key = json["key"].ToString();
            string deployTxid = Helper.DbHelper.GetDeployTxidByTxid(coinType, key);

            if (string.IsNullOrEmpty(deployTxid)) //没有发放NEP5 BTC/ETH
            {
                var deployResult = ZoroServer.DeployMappingNep5(json, coinType);

                if (deployResult != null)
                {
                    Helper.DbHelper.SaveDeployInfo(deployResult.txid, key, coinType);
                    rspInfo.state = true;
                    rspInfo.msg = deployResult;
                }
                else
                {
                    rspInfo.state = true;
                    rspInfo.msg = "Transfer error.";
                }

            }
            else
            {
                transResult.key = key;transResult.txid = deployTxid;transResult.coinType = coinType;
                rspInfo.state = true;
                rspInfo.msg = transResult;
            }
            return rspInfo;

        }

        private static RspInfo ExchangeCoinRsp(JObject json)
        {
            RspInfo rspInfo = new RspInfo() { state = false, msg = "Input data error!" };

            var coinType = json["coinType"].ToString();
            
            string sendTxid = Helper.DbHelper.GetSendTxid(json["key"].ToString());

            if (string.IsNullOrEmpty(sendTxid))
            {
                var exchangeResult = ZoroServer.SendNep5Rsp(json, coinType);
                Helper.DbHelper.SaveExchangeInfo(exchangeResult.key, exchangeResult.txid);
                rspInfo.state = true;
                rspInfo.msg = exchangeResult;
            }
            else
            {
                rspInfo = new RspInfo()
                {
                    state = true,
                    msg = new TransResult() { coinType = coinType, txid = sendTxid, key = json["key"].ToString() }
                };
            }
            return rspInfo;
        }

        private static RspInfo GatherCoinRsp(JObject json)
        {
            var msg = "";
            switch (json["coinType"].ToString())
            {
                case "btc":
                    msg = BtcServer.SendBtcTrans(json);
                    break;
                case "eth":
                    msg = EthServer.SendEthTrans(json).Result;
                    break;
                default:
                    msg = "Error: error coin type";
                    break;
            }

            if (msg.Contains("Error"))
                return new RspInfo { state = false, msg = msg };
            else
                return new RspInfo { state = true, msg = msg };
        }

        private static RspInfo AddAddressRsp(JObject json)
        {
            string coinType = json["coinType"].ToString();
            string address = json["address"].ToString();
            if (coinType == "btc" && !Config.btcAddrList.Contains(address))
            {
                Config.btcAddrList.Add(address);
                Helper.DbHelper.SaveAddress(coinType, address);
            }

            if (coinType == "eth" && !Config.ethAddrList.Contains(address))
            {
                Config.ethAddrList.Add(address);
                Helper.DbHelper.SaveAddress(coinType, address);
            }

            Logger.Info("Add a new " + coinType + " address: " + address);
            return new RspInfo { state = true, msg = "Add a new " + coinType + " address: " + address };
        }

        private static RspInfo GetAccountRsp(string coinType)
        {
            RspInfo rspInfo = new RspInfo();
            var accountInfo = new AccountInfo();
            switch (coinType)
            {
                case "btc":
                    accountInfo = BtcServer.GetBtcAccount();
                    rspInfo.msg = accountInfo;
                    break;
                case "eth":
                    accountInfo = EthServer.GetEthAccount();
                    rspInfo.msg = accountInfo;
                    break;
                default:
                    rspInfo.msg = null;
                    break;
            }
            rspInfo.state = true;
            return rspInfo;
        }

        private static RspInfo GetBalanceRsp(string coinType)
        {
            UInt160 adminHash = Helper.ZoroHelper.GetPublicKeyHashFromWIF(Config.adminWifDic[coinType]);
            string tokenHash = Config.tokenHashDic[coinType];
            UInt160 nep5Hash = UInt160.Parse(tokenHash);

            ScriptBuilder sb = new ScriptBuilder();

            if (coinType == "bct" || coinType == "bcp")
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "BalanceOf", nep5Hash, adminHash);
            else
                sb.EmitAppCall(nep5Hash, "balanceOf", adminHash);

            var info = Helper.ZoroHelper.InvokeScript(sb.ToArray(), "");

            JObject json = JObject.Parse(info);
            if (json.ContainsKey("result"))
            {
                JObject json_result = json["result"] as JObject;
                JArray stack = json_result["stack"] as JArray;

                string result = Helper.ZoroHelper.GetJsonValue(stack[0] as JObject);
                decimal value = Math.Round(decimal.Parse(result) / (decimal)100000000.00000000, 0);

                return new RspInfo { state = true, msg = new CoinInfon() { coinType = coinType, balance = value } };
            }
            else
                return new RspInfo { state = true, msg = json.ToString() };

        }

    }

}
