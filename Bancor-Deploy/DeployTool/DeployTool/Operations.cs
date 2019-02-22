using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThinNeo;

namespace DeployTool
{
    //public class SetMathContract : IOperation
    //{
    //    public string Name => "setMathContract";

    //    public string ID => "0";

    //    public void Start()
    //    {
    //        var wif = Config.bancorAdmin;
    //        Console.WriteLine("Please input math contract hash:");
    //        var mathHash = Console.ReadLine();
    //        var array = new JArray();
    //        array.Add("(hex160)" + mathHash);
    //        var result = MyHelper.SendrawTransaction(Config.bancorHash, "setMathContract", array, wif);
    //        Console.WriteLine(result);
    //    }
    //}

    public class SetWhiteList : IOperation
    {
        public string Name => "添加交易列表";

        public string ID => "1";

        public void Start()
        {
            Console.WriteLine("请输入总管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 BCP hash:");
            var connectAssetHash = Console.ReadLine();
            Console.WriteLine("请输入代币管理员 address:");
            var adminAddress = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(hex160)" + connectAssetHash);
            array.Add("(addr)" + adminAddress);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "setWhiteList", array, wif);
            Console.WriteLine(result);
        }
    }

    public class SetConnectWeight : IOperation
    {
        public string Name => "设置 ConnectWeight";

        public string ID => "2";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 ConnectWeight 值 (0-100000):");
            var connectWeight = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(int)" + connectWeight);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "setConnectWeight", array, wif);
            Console.WriteLine(result);
        }
    }

    public class SetMaxConnectWeight : IOperation
    {
        public string Name => "设置 MaxConnectWeight";

        public string ID => "3";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 MaxConnectWeight 值 (0-100000):");
            var maxConnectWeight = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(int)" + maxConnectWeight);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "setMaxConnectWeight", array, wif);
            Console.WriteLine(result);
        }
    }

    public class SetConnectBalanceIn : IOperation
    {
        public string Name => "充值连接器代币 BCP";

        public string ID => "4";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 BCP 转账的 txid:");
            var txid = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(hex256)" + txid);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "setConnectBalanceIn", array, wif);
            Console.WriteLine(result);
        }
    }

    public class SetSmartTokenSupplyIn : IOperation
    {
        public string Name => "充值智能代币";

        public string ID => "5";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入代币转账的 txid:");
            var txid = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(hex256)" + txid);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "setSmartTokenSupplyIn", array, wif);
            Console.WriteLine(result);
        }
    }

    public class GetConnectBalanceBack : IOperation
    {
        public string Name => "取出连接器代币 BCP";

        public string ID => "6";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入要取回 BCP 的数量:");
            var value = Console.ReadLine();
            var amount = Math.Round(decimal.Parse(value) * 100000000, 0);
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(int)" + amount);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "getConnectBalanceBack", array, wif);
            Console.WriteLine(result);
        }
    }

    public class GetSmartTokenSupplyBack : IOperation
    {
        public string Name => "取回智能代币";

        public string ID => "7";

        public void Start()
        {
            Console.WriteLine("请输入管理员账户 wif:");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入要取回智能代币的数量:");
            var value = Console.ReadLine();
            var amount = Math.Round(decimal.Parse(value) * 100000000, 0);
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(int)" + amount);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "getSmartTokenSupplyBack", array, wif);
            Console.WriteLine(result);
        }
    }

    public class Purchase : IOperation
    {
        public string Name => "用 BCP 购买智能代币";

        public string ID => "8";

        public void Start()
        {
            Console.WriteLine("请输入用户 wif(扣 gas 费使用):");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 BCP 转账的 txid:");
            var txid = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(hex256)" + txid);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "purchase", array, wif);
            Console.WriteLine(result);
        }
    }

    public class Sale : IOperation
    {
        public string Name => "卖出智能代币换回 BCP";

        public string ID => "9";

        public void Start()
        {
            Console.WriteLine("请输入用户 wif(扣 gas 费使用):");
            var wif = Console.ReadLine();
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入智能代币转账的 txid:");
            var txid = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(hex256)" + txid);
            var result = MyHelper.SendrawTransaction(Config.bancorHash, "sale", array, wif);
            Console.WriteLine(result);
        }
    }


    public class GetMathContract : IOperation
    {
        public string Name => "getMathContract";

        public string ID => "10";

        public void Start()
        {
            var array = new JArray();
            array.Add("(int)" + 1);
            var result = MyHelper.CallInvokescript(Config.bancorHash, "getMathContract", array);
            Console.WriteLine(result);
        }
    }

    public class GetWhiteList : IOperation
    {
        public string Name => "获取交易列表";

        public string ID => "11";

        public void Start()
        {
            var array = new JArray();
            array.Add("(int)" + 1);
            var result = MyHelper.CallInvokescript(Config.bancorHash, "getWhiteList", array);
            var stack = ((JObject.Parse(result)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
            var value = stack["value"] as JArray;
            List<WhiteList> whiteLists = new List<WhiteList>();
            foreach (var jo in value)
            {
                var whiteObj = new WhiteList();
                whiteObj.assetHash = Helper_NEO
                    .GetScriptHash_FromAddress(Helper_NEO.GetAddress_FromScriptHash(MyHelper.HexString2Bytes(jo["key"]["value"].ToString())))
                    .ToString();
                whiteObj.adminAddress =
                    Helper_NEO.GetAddress_FromScriptHash(MyHelper.HexString2Bytes(jo["value"]["value"].ToString()));
                whiteLists.Add(whiteObj);
            }

            Console.WriteLine(JsonConvert.SerializeObject(whiteLists));
        }
    }

    public class GetAssetInfo : IOperation
    {
        public string Name => "获取交易代币的配置信息";

        public string ID => "12";

        public void Start()
        {
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            var result = MyHelper.CallInvokescript(Config.bancorHash, "getAssetInfo", array);
            var stack = ((JObject.Parse(result)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
            var value = stack["value"] as JArray;
            Console.WriteLine(stack);
            var assetInfo = new AssetInfo();
            assetInfo.connectAssetHash = Helper_NEO
                .GetScriptHash_FromAddress(
                    Helper_NEO.GetAddress_FromScriptHash(MyHelper.HexString2Bytes(value[0]["value"].ToString())))
                .ToString();
            assetInfo.adminAddress =
                Helper_NEO.GetAddress_FromScriptHash(MyHelper.HexString2Bytes(value[1]["value"].ToString()));
            assetInfo.connectWeight = (int) new BigInteger(MyHelper.HexString2Bytes(value[2]["value"].ToString()));
            assetInfo.maxConnectWeight = (int) new BigInteger(MyHelper.HexString2Bytes(value[3]["value"].ToString()));
            assetInfo.connectBalance = decimal.Parse(value[4]["value"].ToString()) / 100000000;
            assetInfo.smartTokenBalance = decimal.Parse(value[5]["value"].ToString()) / 100000000;
            Console.WriteLine(JsonConvert.SerializeObject(assetInfo));
        }
    }

    public class CalculatePurchaseReturn : IOperation
    {
        public string Name => "查询一定数量 BCP 可兑换智能代币的数量";

        public string ID => "13";

        public void Start()
        {
            Console.WriteLine("请输入代币 hash:");
            var assetHash = Console.ReadLine();
            Console.WriteLine("请输入 BCP 数量:");
            var value = Console.ReadLine();
            var amount = Math.Round(decimal.Parse(value) * 100000000, 0);
            var array = new JArray();
            array.Add("(hex160)" + assetHash);
            array.Add("(int)" + amount);
            var result = MyHelper.CallInvokescript(Config.bancorHash, "calculatePurchaseReturn", array);
            var stack = ((JObject.Parse(result)["result"] as JArray)[0]["stack"] as JArray)[0] as JObject;
            Console.WriteLine(decimal.Parse(stack["value"].ToString()) / 100000000);
        }
    }

    public class WhiteList
    {
        public string assetHash;
        public string adminAddress;
    }

    public class AssetInfo
    {
        public string connectAssetHash; //连接器hash
        public string adminAddress; //管理员
        public int connectWeight; //连接器权重
        public int maxConnectWeight; //最大权重
        public decimal connectBalance; //连接器代币余额
        public decimal smartTokenBalance; //智能代币余额
    }
}
