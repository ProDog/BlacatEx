using System.IO;
using Newtonsoft.Json.Linq;

namespace DeployTool
{
    public class Config
    {
        public static string bancorHash;
        public static string neoApi;
        public static string gasId;
        private static JObject configJson = null;

        public static void Init(string configPath)
        {
            configJson = JObject.Parse(File.ReadAllText(configPath));
            bancorHash = getValue("bancorHash");
            neoApi = getValue("neoApi");
            gasId = getValue("gasId");
        }

        private static string getValue(string name)
        {
            return configJson.GetValue(name).ToString();
        }
    }
}
