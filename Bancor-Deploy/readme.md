# Zoro 交易所对接指南

## 简介
Zoro 交易所内部交易机制采用 Bancor 自动化市商方案，代币交易的对手方是 Bancor 合约，
实现了程序化的自动议价，其可靠性已得到 [Bancor](https://www.bancor.network/tokens) 项目的验证。

详细原理机制可以参考 Bancor [白皮书](https://storage.googleapis.com/website-bancor/2018/04/01ba8253-bancor_protocol_whitepaper_en.pdf)。

该交易所支持的代币均为 NEO nep5 标准代币，精度为 8 位小数，关于 nep5 说明参考 [nep5](nep5.md)。

## Zoro 交易所
Zoro 交易所目前由一个 Bancor 数学计算合约 [BancorMath](https://github.com/ZoroChain/Zoro-Contracts/tree/master/Bancor/BancorMath)
(hash:9f8d9b7dd380c187dadb887a134bf56e3e1d3453)
和一个交易对管理合约 [BancorCommon](https://github.com/ZoroChain/Zoro-Contracts/tree/master/Bancor/BancorCommon)
(hash:0ca406aea638e0fed8580f00eb8b6e1dcb3d95da)
构成，新代币添加到交易所时要调用 BancorCommon 合约提供的接口实现交易前的配置。

## 对接流程

### 1、设置交易列表
开发者将已发行的代币 hash 和交易所要使用的代币管理员地址发给我们，由我们添加到交易所交易列表中。
后续从交易所充提代币和设置交易参数均需要该管理员签名。

设置完成后可以使用 getWhiteList 接口来查询是否增加成功。

### 2、设置交易参数
Bancor 协议通过 ConnectWeight(权重)来控制两种代币之间的兑换率变化幅度，新发行的代币称为智能代币，交易所通用代币称为连接器代币(Zoro 交易所中为 BCP)
，开发者需要设置 ConnectWeight 和 MaxConnectWeight 两个参数，

* 权重 = 连接器代币余额 / 智能代币总价值；

* 这里用设置的 ConnectWeight 和 MaxConnectWeight 来表示权重 CW, CW = ConnectWeight / MaxConnectWeight；

* 代币单价 = 连接器代币余额 / (智能代币结余供应量 × CW)；

比如我设置的 ConnectWeight 为 60000，MaxConnectWeight 为 100000，则 CW = 0.6；
交易所 BCP 余额 1000，新代币余额 2000，则用 BCP 购买新代币的单价为：1000 / (2000 * 0.6) = 0.83333。意味着 1 BCP 可以购买 1 / 0.8333 = 1.2 个新代币。

设置完成后可以使用 getAssetInfo 接口查询是否设置成功。
### 3、充值连接器代币(BCP)
开发者要先使用**管理员账户**往交易所(address:AbhdjnniQ7LMK3aYRmP2Na99qVmcrirdLZ)转账 BCP，
转账金额根据交易体量自行决定，转账完成后，用代币 hash 和转账的 txid 作为参数，调用
BancorCommon 的 setConnectBalanceIn 接口，完成充值操作。

设置完成后可以使用 getAssetInfo 接口查询是否充值成功。
### 4、充值智能代币
开发者先使用**管理员账户**往交易所(address:AbhdjnniQ7LMK3aYRmP2Na99qVmcrirdLZ)转账新代币，转账金额由开发者自定，
转账完成后，以代币 hash 和转账的 txid 作为参数，用 BancorCommon 的 setSmartTokenSupplyIn 接口，完成充值操作。

设置完成后可以使用 getAssetInfo 接口查询是否充值成功。

以上 2、3、4 步均可以使用 DeployTool 工具来完成，完成后代币已经可以在 Zoro 交易所与 BCP 兑换了，其他查询和
测试交易功能在 DeployTool 工具中均有提供，可以查看代币配置信息和测试交易、提取代币。


## DeployTool 工具使用
下载 [DeployTool.zip](https://github.com/ZoroChain/Zoro-Exchange/releases)，解压后使用 `dotnet run DeployTool.dll` 命令启动，然后按照菜单提示操作即可。 
