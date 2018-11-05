## BTC ETH 地址监控程序说明
本程序实现了币币兑换过程中的钱包地址监控功能，设计方案如下：
* 实现一个生产 BTC、ETH 等主流币钱包账户的程序/接口，请求一次返回相应的钱包地址和密钥；
* 每个用户注册平台账户时，自动/手动为其创建各主流币种钱包地址，与平台账户一一对应，钱包私钥和地址离线保存，并将地址发送给监控程序进行监控；
* 监控程序监控 BTC、ETH 等主流币种的最新区块，解析每个区块，将收款地址与收到的监控地址进行对比，如果区块中有交易的 vout 地址是监控的地址，说明该用户往兑换平台充值了，发送充值交易的信息给兑换平台服务器；
* 监控程序持续监控并更新确认次数，当充值交易确认次数达到要求时完成该监控；
* 每次监控的区块高度需要记录，下次重启后从该高度开始，避免重复/遗漏交易；
* 监控账户地址需要实时更新，有新地址自动添加到监控列表并本地存储，程序启动时先获取存储的地址；

* 收到用户转账的 BTC/ETH 后，兑换成等额的 Nep5 BTC，ETH，之后在钱包中的转账交易等均使用该 Nep5 BTC/ETH 代替，提高效率节省费用；
* 使用 Nep5 BTC/ETH 兑换(购买)其他 Nep5 资产，提供相应的购买转账接口。

## 实现接口说明
### 发送交易信息
当监控到一笔符合条件的交易时，发送交易信息给兑换平台，发送方式：POST，发送数据格式：
`confirmcount 是确认次数，超过设定确认次数后不再监控，如果交易在确认中时被取消，确认数变为 0`.
```
[
    {
        "netType":"testnet",
        "address":"3LpBqiC2cGbj1QHwyN81aYpUNakbxjk8xJ",
        "coinType":"btc",
        "confirmcount":5,
        "height":544573,
        "txid":"74d63daa4df7b0791e6a6df816765f59018f6eab314e3e82b343299789153d9b",
        "value":2.60891635
    },
    {
        "netType":"testnet",
        "address":"1CbPtXDSsVmSRKTCbhwMH9MEj7Hfmwt2LS",
        "coinType":"btc",
        "confirmcount":4,
        "height":544574,
        "txid":"aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c",
        "value":0.80051573
    },
    {
        "netType":"testnet",
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
为用户创建各币种的钱包，调用接口：http://xx.xx.xx.xx:xxxx/getaccount/{type}，  参数 type 是币种简称，如 btc，eth 等，调用方式：GET，返回格式：
```
{
    "priKey":"Ky9hKMaG2cg6fMvDju91K5PUrnm8boQcRojQ84xGYid9KxCkrWu8",
    "address":"1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc"
}
```

### 发送交易
发送转账交易，调用接口：http://xx.xx.xx.xx:xxxx/trans/，  参数 type 是币种简称，如 btc，eth 等，txid 是充值 BTC 的 txid，支持多个 txid，调用方式：POST，发送数据格式：
```
{
    "type": btc,
    "account": "1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc",
    "txid" : "73d82f176a2b357256138bd0a6aeddd2131829dadccdee18f98a65da19282228","aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c",
    "priKey": "Ky9hKMaG2cg6fMvDju91K5PUrnm8boQcRojQ84xGYid9KxCkrWu8"
}
```
返回 txid
```
{
    "state": "true",
    "txid": "aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c"
}
```

### 发送新地址
当有用户创建了新地址时，发送给监控程序，url：http://xx.xx.xx.xx:xxxx/addr/，  发送方式：POST，type 是币种简称，如 btc，eth 等，发送数据格式：
```
{
    "type":"btc",
    "address":"1PweQ2GtDzregsXshCyU2Vj8QWMb8T5tmc"
}
```

### 发行 Nep5 BTC/ETH 代币
钱包中使用 Nep5 Token 代替 BTC、ETH 进行交易兑换等操作，提高效率节省费用，收到 BTC ETH 后请求发行，发行请求：url：http://xx.xx.xx.xx:xxxx/deploy/{coinType}，coinType 是币种简称，如 btc，eth 等,发送数据格式：
txid 为收到 BTC ETH 转账的 txid，用来判断是否已经发行过相应的 nep5 BTC ETH
```
{
    "txid":"74d63daa4df7b0791e6a6df816765f59018f6eab314e3e82b343299789153d9b",
    "address":"AUWYsHRi1xv584DswcQKkz1UXJf8G3se4Y",
    "value":0.3005
}
```
返回 txid
```
{
    "state": "true",
    "txid": "aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c"
}
```

### 购买 Nep5 资产的转账接口
钱包中将 BTC ETH 等兑换成 Nep5 资产，请求 url：http://xx.xx.xx.xx:xxxx/exchange，发送数据格式：
txid 为购买时 nep5 BTC ETH 付款的 txid，用来检测是否已经发放了购买的 token
```
{
    "txid": "aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c",
    "type":"gas",
    "address":"AWN6jngST5ytpNnY1dhBQG7QHd7V8SqSCp",
    "value":360.23
}
```
返回 txid
```
{
    "state": "true",
    "txid": "aeb32459b4c5d979690838f99927165c0ce92c75ed71f3783e68d9f89600fe2c"
}
```