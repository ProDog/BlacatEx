using System;
using System.Collections.Generic;
using System.Text;

namespace CoinExchangeWatcher
{
    public class BtcTransResponse
    {
        public string address;  //地址
        public string coinType="btc"; //币种
        public string txid;  //txid
        public decimal value;  //金额
        public int confirmcount;  //确认次数
    }

    public class EthTransResponse
    {
        public string address;  //地址
        public string coinType = "eth"; //币种
        public string txid;  //txid
        public decimal value;  //金额
        public int confirmcount;  //确认次数
    }
}
