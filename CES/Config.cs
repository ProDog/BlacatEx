using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public static int neoTransHeight;

        public static List<string> btcAddrList = new List<string>(); //BTC 监听地址列表
        public static List<string> ethAddrList = new List<string>();  //ETH 监听地址列表

        public static Network nettype = Network.TestNet;

        public static void Init(string configPath)
        {
            ConfigJObject = JObject.Parse(File.ReadAllText(configPath));
            neoIndex = getIndex("neo");
            ethIndex = getIndex("eth");
            btcIndex = getIndex("btc");

            confirmCountDic = getIntDic("confirm_count");
            apiDic = getStringDic("api");
            myAccountDic = getStringDic("my_account");
            minerFeeDic = getDecimalDic("miner_fee");
            adminWifDic = getStringDic("admin");
            tokenHashDic = getStringDic("token");
            factorDic = getDecimalDic("factor");

            neoTransHeight = GetNeoHeightAsync().Result;

            btcAddrList = DbHelper.GetBtcAddr();
            ethAddrList = DbHelper.GetEthAddr();
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
            return DbHelper.GetIndex(name);
        }

        public static async System.Threading.Tasks.Task<int> GetNeoHeightAsync()
        {
            var url = apiDic["neo"] + "?method=getblockcount&id=1&params=[]";
            var result = await MyHelper.HttpGet(url);
            var res = Newtonsoft.Json.Linq.JObject.Parse(result)["result"] as Newtonsoft.Json.Linq.JArray;
            int height = (int)res[0]["blockcount"];
            return height;
        }
        

    }
}
