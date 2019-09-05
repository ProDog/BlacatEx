using Neo.VM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Zoro;
using Zoro.Ledger;
using Zoro.Network.P2P.Payloads;
using Zoro.SmartContract;
using Zoro.Wallets;

namespace Zoro_Gui
{
    public partial class FrmZoroGui : Form
    {
        private byte[] contractScript;
        decimal bcpFee = 10000;

        public FrmZoroGui()
        {
            InitializeComponent();
        }

        private void FrmZoroGui_Load(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(tbxContractPath.Text))
            {
                GetContract();
            }

            lblBcpFee.Text = bcpFee.ToString();

            cmbxTokenType.SelectedIndex = 0;
            cmbMutiSigCoinType.SelectedIndex = 0;
            cbxLockMethod.SelectedIndex = 0;
        }

        //转账交易
        private void btnSendTransaction_Click(object sender, EventArgs e)
        {
            UInt160 assetId;
            string api = transAccountFrm.RpcUrl;
            if (cmbxTokenType.Text == "ZORO")
            {
                assetId = Genesis.BcpContractAddress;
            }
            else if (cmbxTokenType.Text == "BCT")
            {
                assetId = Genesis.BctContractAddress;
            }
            //else if (cmbxTokenType.Text == "BCS")
            //{
            //    assetId = transAccountFrm.bcsAssetId;
            //}
            else
            {
                MessageBox.Show("请选择币种！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(transAccountFrm.addressHash.ToString()))
            {
                MessageBox.Show("请打开钱包！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(tbxValue.Text))
            {
                MessageBox.Show("请输入金额！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxTargetAddress.Text))
            {
                MessageBox.Show("请输入接收地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Decimal value = Decimal.Parse(tbxValue.Text, NumberStyles.Float) * new Decimal(Math.Pow(10, 8));
                UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(tbxTargetAddress.Text);

                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", assetId, transAccountFrm.addressHash, targetscripthash, new BigInteger(value));

                    decimal gasLimit = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");

                    gasLimit = Math.Max(decimal.Parse(tbxGasLimit.Text), gasLimit);

                    decimal gasPrice = decimal.Parse(tbxGasPrice.Text);

                    var tx = ZoroHelper.MakeTransaction(sb.ToArray(), transAccountFrm.keypair, Fixed8.FromDecimal(gasLimit), Fixed8.FromDecimal(gasPrice));

                    //var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), transAccountFrm.keypair, "", Fixed8.FromDecimal(1000), Fixed8.FromDecimal(gasPrice));

                    rtbxTranResult.Text = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + gasLimit + "\r\n txid: " + tx.Hash;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //发布合约
        private void btnPublish_Click(object sender, EventArgs e)
        {
            string api = publishAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(publishAccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxContractPath.Text))
            {
                MessageBox.Show("请输入合约文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] parameter__list = ZoroHelper.HexString2Bytes(tbxParameterType.Text);
            byte[] return_type = ZoroHelper.HexString2Bytes("05");
            int need_storage = cbxNeedStorge.Checked == true ? 1 : 0;
            int need_nep4 = cbxNeedNep4.Checked == true ? 2 : 0;
            int need_canCharge = cbxNeedCharge.Checked == true ? 4 : 0;

            try
            {
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    var ss = need_storage | need_nep4 | need_canCharge;
                    sb.EmitPush(tbxDescri.Text);
                    sb.EmitPush(tbxEmail.Text);
                    sb.EmitPush(tbxAuthor.Text);
                    sb.EmitPush(tbxVersion.Text);
                    sb.EmitPush(tbxContractName.Text);
                    sb.EmitPush(ss);
                    sb.EmitPush(return_type);
                    sb.EmitPush(parameter__list);
                    sb.EmitPush(contractScript);
                    sb.EmitSysCall("Zoro.Contract.Create");

                    var tx = ZoroHelper.MakeTransaction(sb.ToArray(), publishAccountFrm.keypair, Fixed8.Zero, Fixed8.FromDecimal(0.0001m));
                    bcpFee = ZoroHelper.EstimateGas(api, tx, "");

                    lblBcpFee.Text = bcpFee.ToString();

                    tx = ZoroHelper.MakeTransaction(sb.ToArray(), publishAccountFrm.keypair, Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(0.0001m));

                    //var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), publishAccountFrm.keypair, "", Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(0.0001m));

                    var result = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + bcpFee + "\r\n txid: " + tx.Hash;

                    rtbxPublishReturn.Text = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //Invoke
        private void btnInvoke_Click(object sender, EventArgs e)
        {
            string api = invokeAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(tbxContractScriptHash.Text))
            {
                MessageBox.Show("合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxMethodName.Text))
            {
                MessageBox.Show("调用接口不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();

            if (!string.IsNullOrEmpty(rtbxParameterJson.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxParameterJson.Text);
                    sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text);
            }

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                rtbxReturnJson.Text = info;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //SendRaw
        private void btnSendRaw_Click(object sender, EventArgs e)
        {
            string api = invokeAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(invokeAccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxContractScriptHash.Text))
            {
                MessageBox.Show("合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();

            if (!string.IsNullOrEmpty(rtbxParameterJson.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxParameterJson.Text);
                    sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text);
            }

            try
            {
                decimal gasPrice = decimal.Parse(tbxGasPrice.Text);
                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), invokeAccountFrm.keypair, Fixed8.Zero, Fixed8.FromDecimal(gasPrice));
                bcpFee = ZoroHelper.EstimateGas(api, tx, "");

                tbxGasLimit.Text = bcpFee.ToString();

                tx = ZoroHelper.MakeTransaction(sb.ToArray(), invokeAccountFrm.keypair, Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(gasPrice));

                var result = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + bcpFee + "\r\n txid: " + tx.Hash;

                rtbxReturnJson.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnEstimateGas_Click(object sender, EventArgs e)
        {
            string api = invokeAccountFrm.RpcUrl;

            if (string.IsNullOrEmpty(tbxContractScriptHash.Text))
            {
                MessageBox.Show("合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();

            if (!string.IsNullOrEmpty(rtbxParameterJson.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxParameterJson.Text);
                    sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                sb.EmitAppCall(UInt160.Parse(tbxContractScriptHash.Text), tbxMethodName.Text);
            }

            try
            {
                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), invokeAccountFrm.keypair, Fixed8.Zero, Fixed8.FromDecimal(0.0001m));
                bcpFee = ZoroHelper.EstimateGas(api, tx, "");

                tbxGasLimit.Text = bcpFee.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //加载合约
        private void btnLoadContract_Click(object sender, EventArgs e)
        {
            GetContract();
        }

        //取消
        private void btnCancelTran_Click(object sender, EventArgs e)
        {
            //tbxValue.Text = string.Empty;
            //tbxTargetAddress.Text = string.Empty;
        }

        private bool GetContract()
        {
            var contractPath = tbxContractPath.Text;
            tbxContractName.Text = contractPath.Replace(".avm", "");
            if (!System.IO.File.Exists(contractPath))
            {
                MessageBox.Show("合约文件路径无效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxContractHash.Text = string.Empty;
                return false;
            }
            contractScript = System.IO.File.ReadAllBytes(contractPath);
            var contractHash = contractScript.ToScriptHash();
            tbxContractHash.Text = contractHash.ToString();
            return true;
        }

        private void cbxNeedNep4_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxNeedNep4.CheckState == CheckState.Checked)
                bcpFee += (decimal)50000;
            else
                bcpFee -= (decimal)50000;
            lblBcpFee.Text = bcpFee.ToString();
        }

        private void cbxNeedStorge_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxNeedStorge.CheckState == CheckState.Checked)
                bcpFee += (decimal)40000;
            else
                bcpFee -= (decimal)40000;
            lblBcpFee.Text = bcpFee.ToString();
        }

        private List<dynamic> GetParameterArray(string str)
        {
            List<dynamic> paraList = new List<dynamic>();

            string[] parameterArray = str.Split(';');
            for (int i = 0; i < parameterArray.Length; i++)
            {
                paraList.Add(ZoroHelper.GetParamBytes(parameterArray[i]));
            }

            return paraList;
        }

        private void tbxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsPunctuation(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;//消除不合适字符
            }
            else if (Char.IsPunctuation(e.KeyChar))
            {
                if (e.KeyChar != '.')//小数点
                {
                    e.Handled = true;
                }
            }
        }

        private void btnNep5Refresh_Click(object sender, EventArgs e)
        {
            string api = nep5AccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(tbxNep5Hash.Text))
            {
                MessageBox.Show("合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxMyAddress.Text))
            {
                MessageBox.Show("查询地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var addressHash = ZoroHelper.GetPublicKeyHashFromAddress(tbxMyAddress.Text);
                ScriptBuilder sb = new ScriptBuilder();

                if (tbxNep5Hash.Text == "0x0000000000000000000000000000000000000001" || tbxNep5Hash.Text == "0x0000000000000000000000000000000000000002")
                {
                    sb.EmitSysCall("Zoro.NativeNEP5.Call", "BalanceOf", UInt160.Parse(tbxNep5Hash.Text), addressHash);
                }
                else
                    sb.EmitAppCall(UInt160.Parse(tbxNep5Hash.Text), "balanceOf", addressHash);

                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    decimal value = Math.Round(decimal.Parse(result) / (decimal)Math.Pow(10, 8), 8);

                    lblNep5Balance.Text = value.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnNep5SendTran_Click(object sender, EventArgs e)
        {
            string api = nep5AccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(nep5AccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxNep5Hash.Text))
            {
                MessageBox.Show("Nep5 合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxNep5ToAddress.Text))
            {
                MessageBox.Show("接收地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxNep5Value.Text))
            {
                MessageBox.Show("转账金额不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbxNep5GasPrice.Text))
            {
                MessageBox.Show("Gas Price 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();
            try
            {
                var toAddressHash = ZoroHelper.GetPublicKeyHashFromAddress(tbxNep5ToAddress.Text);
                decimal value = Math.Round(decimal.Parse(tbxNep5Value.Text) * (decimal)Math.Pow(10, 8), 0);

                Fixed8 gasPrice = Fixed8.FromDecimal(decimal.Parse(tbxNep5GasPrice.Text));

                sb.EmitAppCall(UInt160.Parse(tbxNep5Hash.Text), "transfer", nep5AccountFrm.addressHash, toAddressHash, new BigInteger(value));

                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), nep5AccountFrm.keypair, Fixed8.Zero, gasPrice);
                bcpFee = ZoroHelper.EstimateGas(api, tx, "");

                tx = ZoroHelper.MakeTransaction(sb.ToArray(), nep5AccountFrm.keypair, Fixed8.FromDecimal(bcpFee), gasPrice);

                var result = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + bcpFee + "\r\n txid: " + tx.Hash;

                rtbxNep5Result.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnMutiSigSendTran_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxMutiSigRpcUrl.Text))
            {
                MessageBox.Show("RpcUrl 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(tbxMutiSigValue.Text))
            {
                MessageBox.Show("金额不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(tbxMutiSigRecAddress.Text))
            {
                MessageBox.Show("接收地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UInt160 assetId;
            string api = tbxMutiSigRpcUrl.Text;
            if (cmbMutiSigCoinType.Text == "ZORO")
            {
                assetId = Genesis.BcpContractAddress;
            }
            else if (cmbMutiSigCoinType.Text == "BCT")
            {
                assetId = Genesis.BctContractAddress;
            }
            else
            {
                MessageBox.Show("请选择币种！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Decimal value = Decimal.Parse(tbxMutiSigValue.Text, NumberStyles.Float) * new Decimal(Math.Pow(10, 8));

                var wifArray = rtbxMutiSigWifs.Text.Split(';');
                KeyPair[] keypairs = wifArray.Select(p => ZoroHelper.GetKeyPairFromWIF(p)).ToArray();
                int m = keypairs.Length - (keypairs.Length - 1) / 3;

                UInt160 scriptHash = ZoroHelper.GetMultiSigRedeemScriptHash(m, keypairs);

                UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(tbxMutiSigRecAddress.Text);

                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    if (cmbMutiSigCoinType.Text == "ZORO")
                        sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", assetId, scriptHash, targetscripthash, new BigInteger(value));
                    if (cmbMutiSigCoinType.Text == "BCT")
                        sb.EmitSysCall("Zoro.NativeNEP5.Call", "MintToken", assetId, targetscripthash, new BigInteger(value));

                    decimal gasLimit = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");

                    gasLimit = Math.Max(decimal.Parse(tbxGasLimit.Text), gasLimit);

                    InvocationTransaction tx = ZoroHelper.MakeMultiSignatureTransaction(sb.ToArray(), m, keypairs, Fixed8.FromDecimal(gasLimit), Fixed8.FromDecimal(0.0001m));

                    rtbxMutiSigResult.Text = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + gasLimit + "\r\n txid: " + tx.Hash;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnCallEstimateGas_Click(object sender, EventArgs e)
        {
            string api = otherCallAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(otherCallAccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(tbxCallInterface.Text))
            {
                MessageBox.Show("调用接口不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();
            if (!string.IsNullOrEmpty(rtbxCallParams.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxCallParams.Text);

                    sb.EmitSysCall(tbxCallInterface.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                bcpFee = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");

                tbxCallGasLimit.Text = bcpFee.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnCallSendRaw_Click(object sender, EventArgs e)
        {
            string api = otherCallAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(otherCallAccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(tbxCallInterface.Text))
            {
                MessageBox.Show("调用接口不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();
            if (!string.IsNullOrEmpty(rtbxCallParams.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxCallParams.Text);
                    sb.EmitSysCall(tbxCallInterface.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                bcpFee = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");
                tbxCallGasLimit.Text = bcpFee.ToString();

                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), otherCallAccountFrm.keypair, Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(0.0001m));

                var result = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + bcpFee + "\r\n txid: " + tx.Hash;

                rtbxCallResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnCallInvoke_Click(object sender, EventArgs e)
        {
            string api = otherCallAccountFrm.RpcUrl;
            if (string.IsNullOrEmpty(otherCallAccountFrm.wif))
            {
                MessageBox.Show("请输入钱包 wif ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(tbxCallInterface.Text))
            {
                MessageBox.Show("调用接口不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptBuilder sb = new ScriptBuilder();
            if (!string.IsNullOrEmpty(rtbxCallParams.Text))
            {
                try
                {
                    List<dynamic> paraList = GetParameterArray(rtbxCallParams.Text);
                    sb.EmitSysCall(tbxCallInterface.Text, paraList.ToArray());
                }
                catch
                {
                    MessageBox.Show("参数格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");
                rtbxCallResult.Text = info;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void BatchSend_Click(object sender, EventArgs e)
        {
            UInt160 assetId;
            string api = BatchaccountFrm.RpcUrl;
            if (BatchCbxCoinType.Text == "ZORO")
            {
                assetId = Genesis.BcpContractAddress;
            }
            else if (BatchCbxCoinType.Text == "BCT")
            {
                assetId = Genesis.BctContractAddress;
            }
            //else if (BatchCbxCoinType.Text == "BCS")
            //{
            //    assetId = BatchaccountFrm.bcsAssetId;
            //}
            else
            {
                MessageBox.Show("请选择币种！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(BatchaccountFrm.addressHash.ToString()))
            {
                MessageBox.Show("请打开钱包！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(BatchTbxValue.Text))
            {
                MessageBox.Show("请输入金额！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(BatchToAddress.Text))
            {
                MessageBox.Show("请输入接收地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Decimal value = Decimal.Parse(BatchTbxValue.Text, NumberStyles.Float) * new Decimal(Math.Pow(10, 8));

                ScriptBuilder sb = new ScriptBuilder();

                foreach (var address in BatchToAddress.Lines)
                {
                    UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(address.ToString());

                    sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", assetId, BatchaccountFrm.addressHash, targetscripthash, new BigInteger(value));
                }

                decimal gasLimit = 1010 * (decimal)BatchToAddress.Lines.Count();

                BatchGasLimit.Text = gasLimit.ToString();

                decimal gasPrice = decimal.Parse(tbxGasPrice.Text);

                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), BatchaccountFrm.keypair, Fixed8.FromDecimal(gasLimit), Fixed8.FromDecimal(gasPrice));

                //var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), transAccountFrm.keypair, "", Fixed8.FromDecimal(1000), Fixed8.FromDecimal(gasPrice));

                BatchResult.Text = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + gasLimit + "\r\n txid: " + tx.Hash;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CbxLockMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLockMethod.Text == "解锁提现")
            {
                label56.Visible = true;
                tbxWithdrawAddress.Visible = true;
                btnLockSendRaw.Visible = true;
                btnLockSendRaw.Enabled = true;
                btnLockInvoke.Enabled = false;
            }
            else
            {
                label56.Visible = false;
                tbxWithdrawAddress.Visible = false;
                btnLockSendRaw.Visible = false;
                btnLockSendRaw.Enabled = false;
                btnLockInvoke.Enabled = true;
            }
        }

        private void BtnLockInvoke_Click(object sender, EventArgs e)
        {
            switch (cbxLockMethod.Text)
            {
                case "可解锁金额":
                    QueryUnlockAmount("getCanWithdrawAmount");
                    break;
                case "已锁仓金额":
                    QueryLockAmount("getLockAomunt");
                    break;
                case "上次解锁时间":
                    GetLockTimestamp("getLockTimestamp");
                    break;
                case "解锁时间间隔":
                    GetUnlockInterval("getUnlockInterval");
                    break;
                case "每次解锁金额":
                    GetPerUnlockAmount("getUnlockAmount");                   
                    break;
            }
        }

        private void Withdraw(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();

            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress, ZoroHelper.GetPublicKeyHashFromAddress(tbxWithdrawAddress.Text),(new BigInteger(1)).ToByteArray());

            try
            {
                decimal gasPrice = 0.0001m;
                var tx = ZoroHelper.MakeTransaction(sb.ToArray(), accountFrmLock.keypair, Fixed8.Zero, Fixed8.FromDecimal(gasPrice));
                bcpFee = ZoroHelper.EstimateGas(api, tx, "");                               

                tx = ZoroHelper.MakeTransaction(sb.ToArray(), accountFrmLock.keypair, Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(gasPrice));

                var result = ZoroHelper.SendRawTransaction(api, tx, "") + " gas_consumed: " + bcpFee + "\r\n txid: " + tx.Hash;

                rtbxLockReturnResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void GetPerUnlockAmount(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress);

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    decimal value = Math.Round(decimal.Parse(result) / (decimal)Math.Pow(10, 8), 8);

                    rtbxLockReturnResult.Text = "每次解锁金额：" + value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void GetUnlockInterval(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress);

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    long value = long.Parse(result);

                    TimeSpan timeSpan = new TimeSpan(value * 10000000);

                    rtbxLockReturnResult.Text = "解锁时间间隔：" + timeSpan.ToString() + " S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void GetLockTimestamp(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress);

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    long value = long.Parse(result);

                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    long lTime = long.Parse(value + "0000000");
                    TimeSpan toNow = new TimeSpan(lTime);
                    DateTime targetDt = dtStart.Add(toNow);

                    rtbxLockReturnResult.Text = "上次解锁时间：" + targetDt.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void QueryUnlockAmount(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress);

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    decimal value = Math.Round(decimal.Parse(result) / (decimal)Math.Pow(10, 8), 8);

                    rtbxLockReturnResult.Text = "金额：" + value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void QueryLockAmount(string method)
        {
            string api = accountFrmLock.RpcUrl;

            if (!CheckLockParameter())
                return;

            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitAppCall(UInt160.Parse(tbxLockContract.Text), method, ZoroHelper.GetPublicKeyHashFromAddress(tbxLockerAddress.Text), Genesis.BcpContractAddress);

            try
            {
                var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

                JObject json = JObject.Parse(info);
                if (json.ContainsKey("result"))
                {
                    JObject json_result = json["result"] as JObject;
                    JArray stack = json_result["stack"] as JArray;

                    string result = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    decimal value = Math.Round(decimal.Parse(result) / (decimal)Math.Pow(10, 8), 8);

                    rtbxLockReturnResult.Text = "金额：" + value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool CheckLockParameter()
        {
            if (string.IsNullOrEmpty(tbxLockContract.Text))
            {
                MessageBox.Show("合约 Hash 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(cbxLockMethod.Text))
            {
                MessageBox.Show("调用接口不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(tbxLockerAddress.Text))
            {
                MessageBox.Show("锁仓地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (tbxWithdrawAddress.Visible)
            {
                if (string.IsNullOrEmpty(tbxWithdrawAddress.Text))
                {
                    MessageBox.Show("收款地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (string.IsNullOrEmpty(accountFrmLock.wif))
                {
                    MessageBox.Show("锁仓地址钱包 Wif 不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void BtnLockSendRaw_Click(object sender, EventArgs e)
        {
            Withdraw("withdraw");
        }
    }
}
