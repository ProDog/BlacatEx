using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NFT_API
{
    class Config
    {
        public static string nelApi;
        public static string myApi;
        public static string gasId;
        public static string nftHash;
        public static string adminAif;
        public static decimal gasFee;
        public static string httpUrl;

        private static JObject configJson = null;

        public static void init(string configPath)
        {
            configJson = JObject.Parse(File.ReadAllText(configPath));
            nelApi = getValue("nelApi");
            myApi = getValue("myApi");
            gasId = getValue("gasId");
            nftHash = getValue("nftHash");
            adminAif = getValue("adminWif");
            gasFee = decimal.Parse(getValue("gasFee"));
            httpUrl = getValue("httpUrl");
        }

        private static string getValue(string name)
        {
            return configJson.GetValue(name).ToString();
        }
        
        public static int GetNeoHeight()
        {
            var url = nelApi + "?method=getblockcount&id=1&params=[]";
            var result = Helper.HttpGet(url).Result;
            var res = JObject.Parse(result)["result"] as JArray;
            int height = (int)res[0]["blockcount"];
            return height;
        }
    }
}
