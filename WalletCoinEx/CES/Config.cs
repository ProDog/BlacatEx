using System.Collections.Generic;
using System.IO;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CES
{
    public class Config
    {
        public static int neoIndex;
        public static int ethIndex;
        public static int btcIndex;
        public static Dictionary<string, int> confirmCountDic;
        public static Dictionary<string, string> apiDic;
        public static Dictionary<string, string> myAccountDic;
        public static Dictionary<string, decimal> minerFeeDic;
        public static Dictionary<string, string> adminWifDic;
        public static Dictionary<string, string> tokenHashDic;
        public static Dictionary<string, decimal> factorDic;
        public static JObject ConfigJObject = null;
        
        public static List<string> btcAddrList = new List<string>(); //BTC 监听地址列表
        public static List<string> ethAddrList = new List<string>();  //ETH 监听地址列表

        public static Network nettype;

        public static void Init(string configPath)
        {
            ConfigJObject = JObject.Parse(File.ReadAllText(configPath));
            neoIndex = getIndex("neo") + 1;
            ethIndex = getIndex("eth") + 1;
            btcIndex = getIndex("btc") + 1;
            //btcIndex = 1447085 + 1;
            confirmCountDic = getIntDic("confirmCount");
            apiDic = getStringDic("api");
            myAccountDic = getStringDic("gatherAddress");
            minerFeeDic = getDecimalDic("gasFee");
            adminWifDic = getStringDic("adminWif");
            tokenHashDic = getStringDic("tokenHash");
            factorDic = getDecimalDic("factor");

            var net = (string)getValue("netType");
            if (net == "mainnet")
                nettype = Network.Main;
            if (net == "testnet")
                nettype = Network.TestNet;

            btcAddrList = Helper.DbHelper.GetBtcAddr();
            ethAddrList = Helper.DbHelper.GetEthAddr();    
        }

        private static dynamic getValue(string name)
        {
            return ConfigJObject.GetValue(name);
        }

        private static Dictionary<string, int> getIntDic(string name)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(ConfigJObject[name].ToString());
        }

        private static Dictionary<string, decimal> getDecimalDic(string name)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, decimal>>(ConfigJObject[name].ToString());
        }

        private static Dictionary<string, string> getStringDic(string name)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigJObject[name].ToString());
        }

        private static int getIndex(string name)
        {
            return Helper.DbHelper.GetIndex(name);
        }

        public static int  GetNeoHeight()
        {
            var url = apiDic["neo"] + "?method=getblockcount&id=1&params=[]";
            var result = Helper.Helper.HttpGet(url);
            var res = JObject.Parse(result)["result"];
            int height = int.Parse(res[0]["blockcount"].ToString());
            return height;
        }
        

    }
}
