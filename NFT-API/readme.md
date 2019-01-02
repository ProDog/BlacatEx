# NFT证书 合约说明

## 介绍
 NFT 合约实现了 BlaCat 合伙人推广的证书功能

证书属性包括：
证书 ID，所有者，等级，贡献值，邀请者证书 ID。

每个 NFT 证书有唯一 ID；所有特权属于证书，证书拥有者可以享有特权。

主要功能：

* 初始发行：为首批证书持有者发行，内部使用；
* 购买：每个地址只能拥有一个 NFT 证书，购买时需要填写邀请人地址，每个证书只有一个邀请人；
* 升级：证书可以根据邀请的人数增加贡献值，积分达到升级要求后可以选择升级，总共有四个等级；
* 交易：证书可以转卖给其他人，转卖后证书的积分和等级特权等会跟着转移；
* 加分：证书持有者邀请一个普通会员可以获得相应的贡献值。

辅助功能有：
* 设置各项参数，比如各等级邀请人所得贡献值、升级所需贡献值和 BCT、购买价格等；
* 数量记录，统计已卖出的 NFT 证书总量和各等级的证书数量；
* 交易信息，记录每次购买或交易的信息，提供查询接口；
* NFT 证书信息，查询 NFT 证书所有信息的接口。

## 调用接口

### buy 
购买发行：POST，传入付钱的 txid 和邀请者 address，接口和参数如下：

http://xxx.xxx:xxxx/buy
```
{
    "txid":"0x1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9",
    "inviter":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi"
}
```
返回 txid：

```
{
    "state":true,
    "msg":{
    "txid":"0x604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f"
    }
}
```
### upgrade
升级接口：POST，传入付钱的 txid，接口信息如下：

http://xxx.xxx:xxxx/upgrade
```
{
    "txid":"0x1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9"
}
```
返回 txid：
```
{
    "state":true,
    "msg":{
    "txid":"0x604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f"
}
```
### addpoint
加分接口：POST，接口信息如下：

http://xxx.xxx:xxxx/addpoint
```
{
    "address":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi"
}
```
返回 txid：
```
{
    "state":true,
    "msg": {
    "txid":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
    }
}
```

### exchange 
转手交易：POST，接口和参数如下：

http://xxx.xxx:xxxx/exchange
```
{
    "from":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi",
    "to":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi"
}
```
返回 txid：
```
{
    "state":true,
    "msg":{
        "txid":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
    }
}
```
### getnftinfo 
根据 address 查询 NFT 证书信息：POST，接口和参数如下：

http://xxx.xxx:xxxx/getnftinfo
```
{
    "address":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi"
}
```
返回 json 格式的证书信息：

```
{
    "state":true,
    "msg":{
        "NFTInfo":{
            "TokenId":"604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f", //tokenid 证书ID
            "Owner":"AMWc2Q9EcrytKN7Qi7FpB56zbhtXkBmRAp", //所有者 address
            "Rank":1, //等级
            "ContributionPoint":500, //贡献值
            "InviterTokenId":"c21ff5eaf70375350ead6cd31140c63a159b395eae59f0ff1c368e8cc352b5f6" //邀请者证书ID
        }
    }
}
```
### getnftinfobyid 
根据 tokenId 查询 NFT 证书信息：POST，接口和参数如下：

http://xxx.xxx:xxxx/getnftinfo
```
{
    "tokenId":"0x9fad4cfa9b87dcf442d3768c2d229d99db436a6ea2b86927c49fa426f3676266"
}
```
返回 json 格式的证书信息：

```
{
    "state":true,
    "msg":{
        "NFTInfo":{
            "TokenId":"604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f", //tokenid 证书ID
            "Owner":"AMWc2Q9EcrytKN7Qi7FpB56zbhtXkBmRAp", //所有者 address
            "Rank":1, //等级
            "ContributionPoint":500, //贡献值
            "InviterTokenId":"c21ff5eaf70375350ead6cd31140c63a159b395eae59f0ff1c368e8cc352b5f6" //邀请者证书ID
        }
    }
}
```
### gettxinfo 
查询证书交易信息，buy、exchange 时会产生交易信息：POST，接口和参数如下：

http://xxx.xxx:xxxx/gettxinfo
```
{
    "txid":"0x1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9"
}
```
返回 json 格式的交易信息：

```
{
    "state":true,
    "msg":{
        "ExchangeInfo":{
            "from":"AMWc2Q9EcrytKN7Qi7FpB56zbhtXkBmRAp",
            "to":"AUkVH4k8gPowAEpvQVAmNEkriX96CrKzk9",
            "tokenId":"1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9"
        }
    }
}
```
### getcount 
查询已发行证书数量：POST，接口：http://xxx.xxx:xxxx/getcount
返回 json 格式的数量信息：

```
{
    "state":true,
    "msg":{
        "NftCount":{
            "AllCount":20, //总数量
            "SilverCount":5, //白银数量
            "GoldCount":5, //黄金数量
            "PlatinumCount":5, //铂金数量
            "DiamondCount":5 //钻石数量
        }
    }
}
```
### getnotify
根据 txid 获取一次交易产生的 notify。

产生 notify 的接口：

* buy 会产生 addpoint 和 exchange 的 notify；
* upgrade 会产生 addpoint 和 upgrade 的 notify；
* exchange 会产生 exchange 的 notify；
* addpoint 会产生 addpoint 的 notify。

POST，接口：http://xxx.xxx:xxxx/getnotify
```
{
    "txid":"0x6c5e42da352bec914448fd07981c833f287049e985bad19114a226ec227bf526"
}
```
返回 json 格式的 notify 信息：
比如下面是一笔 buy 交易产生的 notify，有 addpoint 和 exchange 两种。
```
{
    "state": true,
    "msg": {
        "NotifyType": "buy", //交易类型，有 buy、upgrade、exchange、addpoint 四种
        "ExchangeFrom": null, //exchange 的 form
        "ExchangeTo": "AdsNmzKPPG7HfmQpacZ4ixbv9XJHJs2ACz", //exchange 的 to
        "ExchangeTokenId": "1f5861ebb7ef64c3d5b358bb8fc8392b66e7126c82f4ab983696ab393b03efbb", //exchange 的 tokenId
        "UpgradeTokenId": null, //upgrade 的 tokenId
        "UpgradeAddress": null, //upgrade 的 证书所有者 address
        "UpgradeLastRank": 0, //upgrade 的 升级前等级
        "UpgradenowRank": 0, //upgrade 的 升级后等级
        "AddPointTokenId": "9fad4cfa9b87dcf442d3768c2d229d99db436a6ea2b86927c49fa426f3676266", //addpoint 的 tokenId
        "AddPointAddress": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx", //addpoint 的 证书所有者 address
        "AddPointValue": 100 //addpoint 的 加分值
    }
}
```
下面是一笔 upgrade 交易产生的 notify
```
{
    "state": true,
    "msg": {
        "NotifyType": "upgrade",
        "ExchangeFrom": null,
        "ExchangeTo": null,
        "ExchangeTokenId": null,
        "UpgradeTokenId": "7cd39e8f1929f14c7a306727bb8943d0cacb8b60988d09ebdd8fb2132ffff44f",
        "UpgradeAddress": "AeriJeuFS5EKHWmCgLmEiQx94C7yhPXLhc",
        "UpgradeLastRank": 1,
        "UpgradenowRank": 2,
        "AddPointTokenId": "9fad4cfa9b87dcf442d3768c2d229d99db436a6ea2b86927c49fa426f3676266",
        "AddPointAddress": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx",
        "AddPointValue": 1000
    }
}
```
