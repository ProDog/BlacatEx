## 币币兑换地址监控程序说明
本程序实现了币币兑换过程中的钱包地址监控功能，设计方案如下：
* 实现一个生产 BTC、ETH 等主流币钱包账户的程序/接口，请求一次返回相应的钱包地址和密钥；
* 每个用户注册平台账户时，自动/手动为其创建各主流币种钱包地址，与平台账户一一对应，钱包私钥和地址离线保存，并将地址发送给监控程序进行监控；
* 监控程序监控 BTC、ETH 等主流币种的最新区块，解析每个区块，将收款地址与收到的监控地址进行对比，如果区块中有交易的 vout 地址是监控的地址，说明该用户往兑换平台充值了，发送充值交易的信息给兑换平台服务器；
* 监控程序持续监控并更新确认次数，当充值交易确认次数达到要求时完成该监控；
* 每次监控的区块高度需要记录，下次重启后从该高度开始，避免重复/遗漏交易；
* 监控账户地址需要实时更新，有新地址自动添加到监控列表并本地存储，程序启动时先获取存储的地址；
## 实现接口说明
### 发送交易信息
当监控到一笔符合条件的交易时，发送交易信息给兑换平台，发送方式：POST，发送数据格式：
`confirmcount 是确认次数，超过设定确认次数后不再监控，如果交易在确认中时被取消，确认数变为 0`.
```
[
    {
        "address":"3LpBqiC2cGbj1QHwyN81aYpUNakbxjk8xJ",
        "coinType":"btc",
        "confirmcount":5,
        "height":544573,
        "txid":"74d63daa4df7b0791e6a6df816765f59018f6eab314e3e82b343299789153d9b",
        "value":2.60891635
    },
    {
        "address":"1CbPtXDSsVmSRKTCbhwMH9MEj7Hfmwt2LS",
        "coinType":"btc",
        "confirmcount":4,
        "height":544574,
        "txid":"aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c",
        "value":0.80051573
    },
    {
        "address":"3K2bY9qitYsSd2USgZ22fP7w7Cqyet2uMv",
        "coinType":"btc",
        "confirmcount":3,
        "height":544575,
        "txid":"73d82f176a2b357256138bd0a6aeddd2131829dadccdee18f98a65da19282228",
        "value":0.0005
    }
]
```

### 创建钱包
为用户创建各币种的钱包，调用接口：http://xx.xx.xx.xx:7080/getaccount/{type}，  参数 type 是币种简称，如 btc，eth 等，调用方式：GET，返回格式：
```
{
    "priKey":"Ky9hKMaG2cg6fMvDju91K5PUrnm8boQcRojQ84xGYid9KxCkrWu8",
    "address":"1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc"
}
```

### 获取余额
获取用户账户下的币种余额，调用接口：http://xx.xx.xx.xx:7080/getbalance/{type}/{address}，  参数 type 是币种简称，如 btc，eth 等，调用方式：GET，返回格式：
```
{
       "balance": 23.15365
}
```

### 发送交易
发送转账交易，调用接口：http://xx.xx.xx.xx:7080/trans/，  参数 type 是币种简称，如 btc，eth 等，调用方式：POST，发送数据格式：
```
{
       "type": btc,
       "account": "1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc",
       "priKey": "Ky9hKMaG2cg6fMvDju91K5PUrnm8boQcRojQ84xGYid9KxCkrWu8",
       "to": "1CbPtXDSsVmSRKTCbhwMH9MEj7Hfmwt2LS",
       "amount": 23.256
}
```

### 发送新地址
当有用户创建了新地址时，发送给监控程序，url：http://127.0.0.1:30000/addr/，  发送方式：POST，type 是币种简称，如 btc，eth 等，发送数据格式：
```
{
    "type":"btc",
    "address":"1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc"
}
```

