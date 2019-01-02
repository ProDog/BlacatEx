using System.IO;
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
        public static string httpAddress;

        public static string bctHash;
        public static string bcpHash;

        private static JObject configJson = null;

        public static void Init(string configPath)
        {
            configJson = JObject.Parse(File.ReadAllText(configPath));
            nelApi = getValue("nelApi");
            myApi = getValue("myApi");
            gasId = getValue("gasId");
            nftHash = getValue("nftHash");
            adminAif = getValue("adminWif");
            gasFee = decimal.Parse(getValue("gasFee"));
            httpAddress = getValue("httpAddress");

            bctHash = getValue("bctHash");
            bcpHash = getValue("bcpHash");
        }

        private static string getValue(string name)
        {
            return configJson.GetValue(name).ToString();
        }

    }
}
