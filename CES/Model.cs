using System;
using System.Collections.Generic;
using System.Text;

namespace CoinExchangeService
{
    public class TransResponse
    {
        public string netType = "testnet";//网络  testnet  mainnet
        public string coinType; //币种
        public int confirmcount;  //确认次数
        public int height; //高度
        public string address;  //收款地址
        public string txid;  //txid
        public decimal value;  //金额
        
    }
}
