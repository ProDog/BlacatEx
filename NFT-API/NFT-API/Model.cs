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

    public class NFTInfo
    {
        public string TokenId; //tokenid 证书ID
        public string Owner; //所有者 address
        public int Rank; //等级
        public int AllPoint; //贡献值
        public int AvailablePoint; //可用贡献值
        public string InviterTokenId; //邀请者证书ID
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
        //public string ownerAddress;
        //public BigInteger buyCount;
        //public long payValue;
        public List<string> tokenIdList;
    }

    public class BuyNftLog
    {
        public List<AddPointLog> addPointLogs;
        public CreateNftLog createNftLogs;
    }

    public class BlockDataHeight
    {
        public BigInteger NotifyDataHeight;
        public BigInteger Nep5DataHeight;
    }

    public enum ContractState
    {
        None,
        Active,
        Inactive,
        AllStop
    }

}
