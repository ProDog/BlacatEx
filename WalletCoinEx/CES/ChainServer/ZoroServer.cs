using System;
using System.Numerics;
using System.Reflection;
using log4net;
using Neo.VM;
using Newtonsoft.Json.Linq;
using Zoro;
using Zoro.IO;
using Zoro.Network.P2P.Payloads;
using Zoro.Wallets;

namespace CES
{
    public class ZoroServer
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static TransResult SendNep5Rsp(JObject json, string coinType)
        {
            var transResult = new TransResult();

            UInt160 nep5Hash = UInt160.Parse(Config.tokenHashDic[coinType]);

            decimal value = Math.Round((decimal)json["value"] * (decimal)100000000.00000000, 0);
            UInt160 targetscripthash = Helper.ZoroHelper.GetPublicKeyHashFromAddress(json["address"].ToString());
            ScriptBuilder sb = new ScriptBuilder();

            KeyPair keypair = Helper.ZoroHelper.GetKeyPairFromWIF(Config.adminWifDic[coinType]);
            var adminHash = Helper.ZoroHelper.GetPublicKeyHashFromWIF(Config.adminWifDic[coinType]);

            if (coinType == "bct" || coinType == "bcp")
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", nep5Hash, adminHash, targetscripthash, new BigInteger(value));
            else
                sb.EmitAppCall(nep5Hash, "transfer", adminHash, targetscripthash, value);

            decimal gas = Helper.ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");

            InvocationTransaction tx = Helper.ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(gas), Fixed8.FromDecimal(0.0001m));
            var result = Helper.ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
            var sendTxid = tx.Hash.ToString();

            var state = (bool)(JObject.Parse(result)["result"]);
            if (state)
            {
                transResult.coinType = coinType;
                transResult.key = json["key"].ToString();
                transResult.txid = sendTxid;
            }
            else
            {
                Logger.Warn("Trans result: " + result);
                return null;
            }
            return transResult;
        }

        public static TransResult DeployMappingNep5(JObject json, string coinType)
        {
            var transResult = new TransResult();

            UInt160 nep5Hash = UInt160.Parse(Config.tokenHashDic[coinType]);

            decimal value = Math.Round((decimal)json["value"] * (decimal)100000000.00000000, 0);
            UInt160 targetscripthash = Helper.ZoroHelper.GetPublicKeyHashFromAddress(json["address"].ToString());

            KeyPair keypair = Helper.ZoroHelper.GetKeyPairFromWIF(Config.adminWifDic[coinType]);
            var adminHash = Helper.ZoroHelper.GetPublicKeyHashFromWIF(Config.adminWifDic[coinType]);

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(nep5Hash, "deploy", targetscripthash, new BigInteger(value));

            decimal gas = Helper.ZoroHelper.GetScriptGasConsumed(sb.ToArray(), "");

            InvocationTransaction tx = Helper.ZoroHelper.MakeTransaction(sb.ToArray(), keypair, Fixed8.FromDecimal(gas), Fixed8.FromDecimal(0.0001m));
            var result = Helper.ZoroHelper.SendRawTransaction(tx.ToArray().ToHexString(), "");
            var sendTxid = tx.Hash.ToString();

            if (result.Contains("Block or transaction validation failed"))
                return null;

            var state = (bool)(JObject.Parse(result)["result"]);
            if (state)
            {
                transResult.coinType = coinType;
                transResult.key = json["key"].ToString();
                transResult.txid = sendTxid;
            }
            else
            {
                Logger.Warn("Trans result: " + result);
                return null;
            }
            return transResult;
        }


    }
}
