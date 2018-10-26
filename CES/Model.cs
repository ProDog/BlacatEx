﻿using System;
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

    public class DeployInfo
    {
        public string coinType; //币种
        public string address;  //收款地址
        public string txid;  //txid
        public decimal value;  //金额
        public string deployTxid;//发行txid
        public string deployTime; //发行时间
    }

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
}
