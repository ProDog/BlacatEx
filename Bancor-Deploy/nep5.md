# 在 NEO 上发布代币合约
NEO 系统中有 UTXO 模型的代币 NEO 和 GAS，但如果基于 NEO 开发 Dapp 或其他区块链项目时我们仍然需要能实现自己的代币功能，这里就来总结一下如何用智能合约在 NEO 上发布代币。
与比特币的 BIP、以太坊的 ERC 等类似，NEO 也有 NEPs: NEO Enhancement Proposals  NEO加强/改进提议 ，它描述的是 NEO 平台的标准，包括核心协议规范，客户端 API 和合约标准。
在 NEO 公链上发布代币是 Nep5 即 NEO 5 号改进提案中提出的，所以我们发布的代币统称 Nep5 代币，在 Nep5 中，规定了代币合约必须实现的接口、返回结果等标准，所以我们只需按照标准就可以开发自己的代币，然后发布合约即可，下面就分别讲讲合约编程和发布。
合约的基本介绍和开发前准备可以参考 NEO 开发文档中的智能合约部分。

## nep5：
### 概述：
nep5 提案描述了 neo 区块链的 token 标准，它为 token 类的智能合约提供了系统的通用交互机制、定义了这种机制和每种特性、并提供了开发模板和示例。
动机：随着 neo 区块链生态的发展，智能合约的部署和调用变得越来越重要，如果没有标准的交互方法，无论合约间有没有相似性，系统就需要为每个智能合约维护一套单独的 api。
token 类合约的操作机制其实基本都是相同的，因此需要这样一套标准。这些与 token 交互的标准方案使整个生态系统免于维护每个使用 token 的智能合约的 pai。
### 规范：
在下面的方法中，我们提供了 nep5 代币合约中函数的定义方式及参数调用。

方法：

* totalSupply  
public static BigInteger totalSupply（）
返回 token 总量。
* name  
public static string name()
返回 token 名称
每次调用时此方法必须返回相同的值。
* symbol  
public static string symbol()
返回此合约中管理的 token 的简称，3-8 字符、限制为大写英文字母；
每次调用时此方法必须返回相同的值。
* decimals  
public static byte decimals()
返回 token 使用的小数位数；
每次调用时此方法必须返回相同的值。
* balanceOf  
public static BigInteger balanceOf(byte[] account)
返回 token 余额
参数 account 应该是一个 20 字节的地址；
如果 account 是未使用的地址，则此方法必须返回 0。
* transfer  
public static bool transfer(byte[] from, byte[] to, BigInteger amount)
将 amount 数量的 token 从 from 账户转到 to 账户；
参数 amount 必须大于或等于 0；
如果 from 帐户余额没有足够的 token 可用，则该函数必须返回 false；
如果该方法成功，它必须触发 transfer 事件，并且必须返回 true，即使 amount 是 0，或 from 与 to 相同；
函数应该检查 from 地址是否等于合约调用者 hash，如果是，应该处理转账; 如果不是，该函数应该使用 SYSCALL Neo.Runtime.CheckWitness 来验证交易；
如果 to 地址是已部署的合约地址，则该函数应该检查该合约的 payable 标志以决定是否应该将 token 转移到该合约地址；如果未处理转账，则函数应该返回 false。

## 开发合约
下面就是一个 Nep5 代币合约的具体实现：

```
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;

namespace ABCContract
{
    public class ABC : SmartContract
    {
        public delegate void deleTransfer(byte[] from, byte[] to, BigInteger value);
        [DisplayName("transfer")]
        public static event deleTransfer Transferred;        

		//发币管理员账户，改成自己测试用的的
        private static readonly byte[] superAdmin = Helper.ToScriptHash("APVdDEtthapuaPedMHCgrDR5Vyc22fns9m");

        public static string name()
        {
            return "ABC Coin";//名称
        }

        public static string symbol()
        {
            return "ABC";//简称
        }

        private const ulong factor = 100000000;//精度
        private const ulong totalCoin =  100000000 * factor;//总量 要乘以精度，NEO 系统中没有小数，所有数字类型都转为 BigInteger 处理

        public static byte decimals()
        {
            return 8;
        }

        public static object Main(string method, object[] args)
        {
            var magicstr = "abc-test";
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return false;
            }
            else if (Runtime.Trigger == TriggerType.VerificationR)
            {
                return true;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                //开始时取到调用该合约的脚本hash
                var callscript = ExecutionEngine.CallingScriptHash;

                if (method == "totalSupply")
                    return totalSupply();
                if (method == "name")
                    return name();
                if (method == "symbol")
                    return symbol();
                if (method == "decimals")
                    return decimals();
                //发行，合约发布后由管理员发行代币
                if (method == "deploy")
                {
                    if (!Runtime.CheckWitness(superAdmin))
                        return false;
                    byte[] total_supply = Storage.Get(Storage.CurrentContext, "totalSupply");
                    if (total_supply.Length != 0)
                        return false;
                    var keySuperAdmin = new byte[] {0x11}.Concat(superAdmin);
                    Storage.Put(Storage.CurrentContext, keySuperAdmin, totalCoin);
                    Storage.Put(Storage.CurrentContext, "totalSupply", totalCoin);

                    Transferred(null, superAdmin, totalCoin);
                }

                //获取余额
                if (method == "balanceOf")
                {
                    if (args.Length != 1)
                        return 0;
                    byte[] who = (byte[]) args[0];
                    if (who.Length != 20)
                        return false;
                    return balanceOf(who);
                }

                //转账接口
                if (method == "transfer")
                {
                    if (args.Length != 3)
                        return false;
                    byte[] from = (byte[]) args[0];
                    byte[] to = (byte[]) args[1];
                    if (from == to)
                        return true;
                    if (from.Length != 20 || to.Length != 20)
                        return false;
                    BigInteger value = (BigInteger) args[2];
                    if (!Runtime.CheckWitness(from))
                        return false;
                        //禁止跳板调用
                    if (ExecutionEngine.EntryScriptHash.AsBigInteger() != callscript.AsBigInteger())
                        return false;
                    if (!IsPayable(to))
                        return false;
                    return transfer(from, to, value);
                }

                //合约脚本的转账接口、弥补没有跳板调用
                if (method == "transfer_app")
                {
                    if (args.Length != 3)
                        return false;
                    byte[] from = (byte[]) args[0];
                    byte[] to = (byte[]) args[1];
                    BigInteger value = (BigInteger) args[2];

                    if (from.AsBigInteger() != callscript.AsBigInteger())
                        return false;
                    return transfer(from, to, value);
                }

                //获取交易信息
                if (method == "getTxInfo")
                {
                    if (args.Length != 1)
                        return 0;
                    byte[] txid = (byte[]) args[0];
                    return getTxInfo(txid);
                }

            }

            return false;

        }

        //获取总量
        private static object totalSupply()
        {
            return Storage.Get(Storage.CurrentContext, "totalSupply").AsBigInteger();
        }

        //交易
        private static bool transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (value <= 0)
                return false;
            if (from == to)
                return true;
            if (from.Length > 0)
            {
                var keyFrom = new byte[] {0x11}.Concat(from);
                BigInteger from_value = Storage.Get(Storage.CurrentContext, keyFrom).AsBigInteger();
                if (from_value < value)
                    return false;
                if (from_value == value)
                    Storage.Delete(Storage.CurrentContext, keyFrom);
                else
                {
                    Storage.Put(Storage.CurrentContext, keyFrom, from_value - value);
                }
            }

            if (to.Length > 0)
            {
                var keyTo = new byte[] {0x11}.Concat(to);
                BigInteger to_value = Storage.Get(Storage.CurrentContext, keyTo).AsBigInteger();
                Storage.Put(Storage.CurrentContext, keyTo, to_value + value);
            }

            setTxInfo(from, to, value);
            Transferred(from, to, value);
            return true;
        }

        private static void setTxInfo(byte[] from, byte[] to, BigInteger value)
        {
            TransferInfo info = new TransferInfo();
            info.@from = from;
            info.to = to;
            info.value = value;
            byte[] txInfo = Helper.Serialize(info);
            var txid = (ExecutionEngine.ScriptContainer as Transaction).Hash;
            var keyTxid = new byte[] {0x13}.Concat(txid);
            Storage.Put(Storage.CurrentContext, keyTxid, txInfo);
        }

        private static object balanceOf(byte[] who)
        {
            var keyAddress = new byte[] {0x11}.Concat(who);
            return Storage.Get(Storage.CurrentContext, keyAddress).AsBigInteger();
        }

        private static TransferInfo getTxInfo(byte[] txid)
        {
            byte[] keyTxid=new byte[] {0x13}.Concat(txid);
            byte[] v = Storage.Get(Storage.CurrentContext, keyTxid);
            if (v.Length == 0)
                return null;
            return Helper.Deserialize(v) as TransferInfo;
        }

        public static bool IsPayable(byte[] to)
        {
            var c = Blockchain.GetContract(to);
            if (c.Equals(null))
                return true;
            return c.IsPayable;
        }
    }
    public class TransferInfo
    {
        public byte[] from;
        public byte[] to;
        public BigInteger value;
    }
}

```

## 发布合约
写好的合约编译后生成 avm 文件，接下来就可以拿着 avm 文件去发布合约了，这里有完整的发布合约教程可以参考，此处就不展开了：[NEO 智能合约发布和升级](https://my.oschina.net/u/3869289/blog/1834874)。

合约发布成功后，就要使用合约中预置的管理员来发行代币，可以参考下面代码来调用合约的 deploy 接口发行代币：
```
//Helper相关方法是引用了https://www.nuget.org/packages/Neo.sdk.thin
private static void DeployNep5Token(byte[] prikey)
    {
        var array = new JArray();
        array.Add("(int)" + 1); //deploy接口不需要参数，这里传个1
        sb.EmitParamJson(array); //参数倒序入
        sb.EmitPushString("deploy");
        sb.EmitAppCall(new Hash160("hash");//hash是刚才发布的合约的hash
        script = sb.ToArray();

        //私钥用合约中预留管理员的私钥、deploy 接口需要验证管理员签名
        byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
        string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

        Transaction tran = new Transaction();
        tran.inputs = new TransactionInput[0];
        tran.outputs = new TransactionOutput[0];
        tran.attributes = new ThinNeo.Attribute[1];
        tran.attributes[0] = new ThinNeo.Attribute();
        tran.attributes[0].usage = TransactionAttributeUsage.Script;
        tran.attributes[0].data = pubkey;
        tran.version = 1;
        tran.type = TransactionType.InvocationTransaction;

        var idata = new InvokeTransData();
        tran.extdata = idata;
        idata.script = script;
        idata.gas = 0;

        byte[] msg = tran.GetMessage();
        string msgstr = Helper.Bytes2HexString(msg);
        byte[] signdata = Helper_NEO.Sign(msg, prikey);
        tran.AddWitness(signdata, pubkey, address);
        string txid = tran.GetHash().ToString();
        byte[] data = tran.GetRawData();
        string rawdata = Helper.Bytes2HexString(data);
        //neoapi是neo cli节点的url
        var result = HttpGet($"{neoapi}?method=sendrawtransaction&id=1&params=[\"{rawdata}\"]");
        var json = JObject.Parse(result);
    }
```
到此为止已经在 NEO 上发行了自己的代币，可以使用 [ApplicationLogs](http://docs.neo.org/zh-cn/node/plugin.html#%E6%8F%92%E4%BB%B6%E4%B8%AD%E7%9A%84-api-%E6%8E%A5%E5%8F%A3) 提供的 API 来查询 Nep5 资产的转账信息了。