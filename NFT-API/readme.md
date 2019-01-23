# NFT证书 合约说明

## 介绍
 NFT 合约实现了 BlaCat 合伙人推广的证书功能

证书属性包括：
证书 ID，所有者，等级，累计贡献值，可用贡献值，邀请者证书 ID。

每个 NFT 证书有唯一 ID；所有特权属于证书，证书拥有者可以享有特权。

主要功能：

* 购买：每个地址可以拥有多个 NFT 证书，购买时需要填写邀请人证书地址，每个证书只有一个邀请人；
* 升级：证书可以根据邀请的人数增加贡献值，积分达到升级要求后可以选择升级，总共有四个等级；
* 交易：证书可以转卖给其他人，转卖后证书的积分和等级特权等会跟着转移；
* 加分：证书持有者邀请一个普通会员可以获得相应的贡献值。

辅助功能有：
* NFT 证书信息，查询 NFT 证书所有信息的接口;
* 获取某地址下所有的证书 ID。

## 调用接口

### buy 
购买发行：POST，传入付钱的 txid 和邀请者 TokenId，购买数量 count

http://xxx.xxx:xxxx/buy
```
{
    "txid":"0x1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9",
    "count":10,
    "inviterTokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
返回 txid：

```
{
    "state":true,
    "msg":"0x604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f"
}
```
### upgrade
升级接口：POST，传入付钱的 txid，tokenId，当前等级 nowGrade，接口信息如下：

http://xxx.xxx:xxxx/upgrade
```
{
    "txid":"0x1e23bdfa643c2c9595766d93ddebbee46446d2acd6f67d005989db1ade883fc9",
    "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418",
    "nowGrade":2
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f"
}
```
### bind
绑定接口：POST：

http://xxx.xxx:xxxx/bind
```
{
    "address":"AcQLYjGbQU2bEQ8RKFXUcf8XvromfUQodq",
    "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x604c5a37520a3a114015026735faddae6d16c4d972d75309519a3bcb0545847f"
}
```
### addPoint
加分接口：POST，接口信息如下：

http://xxx.xxx:xxxx/addPoint
```
{
   "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
}
```

### exchange 
转手交易：POST，接口和参数如下：

http://xxx.xxx:xxxx/exchange
```
{
    "from":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi",
    "to":"AQzB8XJAwRQqBGFe3fsM5Leq6U8eAQ2kZi",
    "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
}
```
### reduceGrade
降级：POST，接口信息如下：

http://xxx.xxx:xxxx/reduceGrade
```
{
   "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
}
```
### reducePoint
扣分：POST，接口信息如下：

http://xxx.xxx:xxxx/reduceGrade
```
{
    "tokenId":"e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"，
    "pointValue":20
}
```
返回 txid：
```
{
    "state":true,
    "msg":"0x618ff2755cbd2c3c5145d23b53abe9d98acc44b665e94d9a2297d8d0066b80de"
}
```

### getNftInfo
根据 tokenId 查询 NFT 证书信息：POST，接口和参数如下：

http://xxx.xxx:xxxx/getNftInfo
```
{
   "tokenId":"0x9fad4cfa9b87dcf442d3768c2d229d99db436a6ea2b86927c49fa426f3676266"
}
```
返回 json 格式的证书信息：

```
{
    "state": true,
    "msg": {
        "TokenId": "e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418",
        "Owner": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx",
        "Rank": 2,
        "AllPoint": 620,
        "AvailablePoint": 220,
        "InviterTokenId": "0x9fad4cfa9b87dcf442d3768c2d229d99db436a6ea2b86927c49fa426f3676266"
    }
}
```
### getBindNft 
查询绑定的证书 ID：POST，接口和参数如下：

http://xxx.xxx:xxxx/getBindNft
```
{
    "address":"AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx"
}
```
返回：

```
{
    "state": true,
    "msg": "e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418"
}
```
### getUserNfts 
查询该地址下所有证书ID：POST，接口和参数如下：

http://xxx.xxx:xxxx/getUserNfts
```
{
    "address":"AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx"
}
```
返回：
```
{
    "state": true,
    "msg": [
        "e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418",
        "2d99161a1b53342f0f3abc6f6e93513ce09eb7b3caf63fce2d0a7e566147591a",
        "d23fe468f6e698d2957d228cc07b275a448348bbd9da542df510d61a7dda030a",
        "fcac236be6fe8dd62268df68d169be7b428396a34b6fd92f0ccbb15549101680",
        "5a89efaf7c9d51f35d5ac616c4efe0a14a66631dbe42d02b62981eba10211168",
        "000265927502c6f31b3a28d0e4cc984db8b990ac7dcaaf59f8c1191a2955de4d",
        "7f7abfd4ca8b8f8c18f17e42bfaa3265bcb7150514df8974ce99b31b3827b93f",
        "f6e18cd5220cde0c4ce985be29fa1c53744f8ce892770ea78233b9ae3cc68fcb",
        "edd8cd96fdb26e9554b5b3869fe22195a378a052aa131102fcacd33678739efe",
        "fedcd7f54241cfccccec94610ef4dce660f299468513da715fe930911212be7c",
        "98ee9f9abd5daf662175761e276dff9e12cf871a70a02a1490f80cb4da7232fc"
    ]
}
```
### getApplicationLog
根据 txid 获取一次交易产生的 ApplicationLog。

产生 ApplicationLog 的接口：

* buy 会产生 addpoint 和 create 的 ApplicationLog；
* upgrade 会产生 upgrade 的 ApplicationLog；
* exchange 会产生 exchange 的 ApplicationLog；
* addPoint 会产生 addPoint 的 ApplicationLog。

POST，接口：http://xxx.xxx:xxxx/getApplicationLog
```
{
    "txid":"0x6c5e42da352bec914448fd07981c833f287049e985bad19114a226ec227bf526",
    "method":"buy"
}
```
返回 json 格式的 ApplicationLog 信息：
比如下面是一笔 buy 交易产生的 ApplicationLog，有 addPoint 和 create 两种。
```
{
    "state": true,
    "msg": {
        "height": 21123,
        "applicationLog": {
            "addPointLogs": [
                {
                    "tokenId": "d23fe468f6e698d2957d228cc07b275a448348bbd9da542df510d61a7dda030a",
                    "ownerAddress": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx",
                    "addPoint": 400
                },
                {
                    "tokenId": "e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418",
                    "ownerAddress": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx",
                    "addPoint": 40
                }
            ],
            "createNftLogs": {
                "ownerAddress": "AcQLYjGbQU2bEQ8RKFXUcf8XvromfUQodq",
                "buyCount": 10,
                "payValue": 35991,
                "tokenIdList": [
                    "12a52e3c6d200b4a51015d4874ed332b05ea98c5073eaf01bd285765af2922a1",
                    "3912b77f223e71a6a0eb6d2fd88f2d26aec9a5f72b1e9e91ebc877cfe3b26d8d",
                    "f81c7397a631f07fd2f1378f635c4c7f43ef2e069821873cb05288f5b9e18d11",
                    "3fda1ae5a591d0a85623f740211fa028950c9b01bd5e5c2a3e0694ad749cc0c5",
                    "e3e93f04e33b4f91539fae5083c04d2622ff3faae10844dec42e07603d282afc",
                    "64ac19439580c549d50d2ce38b1f4b57f01f2dc85227764f712457dd82c344fa",
                    "d0bbe82d7a8be6fbe0f2587f9da8e6d1dee3f9b2d65e42d222c0ccdad35c9091",
                    "542867fa6a0cb3bfebc8028365e1dfe16b8ec866c65f4d793ff6ccb522b60ae2",
                    "8ce8138eaeb40c93a664bd3cc9964cac71e0a84b04a904787b489031f70bb24d",
                    "fe6864542a48be9798e9cea84143dedb74c7337d6caf3bf8b83c7c11f0d01e55"
                ]
            }
        }
    }
}
```
下面是一笔 upgrade 交易产生的 ApplicationLog
```
{
    "txid":"0x99161a1b53342f0f3abc6f6e93513ce09eb7b3caf63fce2d0a7e566147591c",
    "method":"upgrade"
}
```
```
{  
    "state": true,
    "msg": {
        "height": 17842,
        "applicationLog": {
            "tokenId": "e4658581f20649c32314326f180c7ec89587b786bc29ab2a9a8ce4b7c96d7418",
            "ownerAddress": "AbN2K2trYzgx8WMg2H7U7JHH6RQVzz2fnx",
            "lastGrade": 1,
            "nowGrade": 2
        }
    }
}

```
