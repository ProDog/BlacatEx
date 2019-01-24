using System.Collections.Generic;
using System.Numerics;

namespace NFT_API
{
    public class RspInfo
    {
        public bool state;
        public dynamic msg;
    }

    public class TransResult
    {
        public string txid;
        public string key;
    }

    public class SendRawResult
    {
        public string txid;
        public string nftHash;
    }

    public class NFTInfo
    {
        public string tokenId; //tokenid 证书ID
        public string owner; //所有者 address
        public int grade; //等级
        public int allPoint; //贡献值
        public int availablePoint; //可用贡献值
        public string inviterTokenId; //邀请者证书ID
    }

    public class ApplicationLog
    {
        public int height;
        public dynamic applicationLog;
    }

    public class UpgradeLog
    {
        public string tokenId;
        public string ownerAddress;
        public int lastGrade;
        public int nowGrade;
    }

    public class BindLog
    {
        public string ownerAddress;
        public string tokenId;
    }

    public class ExchangeLog
    {
        public string from;
        public string to;
        public string tokenId;
    }

    public class AddPointLog
    {
        public string tokenId;
        public string ownerAddress;
        public BigInteger addPoint;
    }

    public class CreateNftLog
    {
        public string ownerAddress;
        public BigInteger buyCount;
        public long payValue;
        public List<string> tokenIdList;
    }

    public class BuyNftLog
    {
        public List<AddPointLog> addPointLog;
        public CreateNftLog createNftLog;
    }

    public class AddGradeLog
    {
        public UpgradeLog upgradeLog;
        public AddPointLog addPointLog;
    }


    public enum ContractState
    {
        None,
        Active,
        Inactive,
        AllStop
    }

}
