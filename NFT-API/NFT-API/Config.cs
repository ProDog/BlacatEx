using System.IO;
using Newtonsoft.Json.Linq;
using Zoro;
using Zoro.Wallets;

namespace NFT_API
{
    class Config
    {
        public static JObject configJson = null;

        public static Fixed8 GasPrice = Fixed8.One;

        public static decimal GasNEP5Transfer = (decimal)4.5;

        public static void init(string configPath)
        {
            configJson = JObject.Parse(File.ReadAllText(configPath));
        }

        public static string getStrValue(string name)
        {
            return configJson.GetValue(name).ToString();
        }

        public static int getIntValue(string name)
        {
            return int.Parse(configJson.GetValue(name).ToString());
        }

        public static long getLongValue(string name)
        {
            return long.Parse(configJson.GetValue(name).ToString());
        }
    }
}
