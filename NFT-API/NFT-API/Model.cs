using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NFT_API
{
    public class Utxo
    {
        //txid[n] 是utxo的属性
        public ThinNeo.Hash256 txid;
        public int n;
        //asset资产、addr 属于谁，value数额，这都是查出来的
        public string addr;
        public string asset;
        public decimal value;

        public Utxo(string _addr, ThinNeo.Hash256 _txid, string _asset, decimal _value, int _n)
        {
            this.addr = _addr;
            this.txid = _txid;
            this.asset = _asset;
            this.value = _value;
            this.n = _n;
        }
    }

    public class RspInfo
    {
        public bool state;
        public dynamic msg;
    }

    public class Txid
    {
        public string txid;
    }

    public class Error
    {
        public string error;
    }

    //已发行数量
    public class NftCount
    {
        public BigInteger AllCount; //总数量
        public BigInteger SilverCount; //白银数量
        public BigInteger GoldCount; //黄金数量
        public BigInteger PlatinumCount; //铂金数量
        public BigInteger DiamondCount; //钻石数量
    }

    public class NFTInfo
    {
        public string TokenId; //tokenid 证书ID
        public string Owner; //所有者 address
        public int Rank; //等级
        public int ContributionPoint; //贡献值
        public string InviterTokenId; //邀请者证书ID
    }

    public class ExchangeInfo
    {
        public string from;
        public string to;
        public string tokenId;
    }

    public class NotifyInfo
    {
        public string NotifyType;
        public string ExchangeFrom;
        public string ExchangeTo;
        public string ExchangeTokenId;

        public string UpgradeTokenId;
        public string UpgradeAddress;
        public BigInteger UpgradeLastRank;
        public BigInteger UpgradenowRank;

        public string AddPointTokenId;
        public string AddPointAddress;
        public BigInteger AddPointValue;
    }

    //配置
    public class ConfigInfo
    {
        public BigInteger SilverPrice; //白银购买价格
        public BigInteger GoldPrice; //升级黄金价格
        public BigInteger PlatinumPrice; //升级铂金价格
        public BigInteger DiamondPrice; //升级钻石价格

        public BigInteger LeaguerInvitePoint; //邀请普通会员所得贡献值
        public BigInteger SilverInvitePoint; //邀请白银所得贡献值

        public BigInteger GoldInvitePoint; //被邀请者升级黄金时邀请者所得贡献值
        public BigInteger PlatinumInvitePoint; //被邀请者升级铂金时邀请者所得贡献值
        public BigInteger DiamondInvitePoint; //被邀请者升级钻石时邀请者所得贡献值

        public BigInteger GoldUpgradePoint; //升级黄金所需贡献值
        public BigInteger PlatinumUpgradePoint; //升级铂金所需贡献值
        public BigInteger DiamondUpgradePoint; //升级钻石所需贡献值

        public string GatheringAddress; //收钱地址
    }

    public enum ContractState
    {
        None,
        Active,
        Inactive,
        AllStop
    }

}
