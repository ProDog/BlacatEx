using Neo.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Windows.Forms;
using Zoro;
using Zoro.Ledger;
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
        }

        //转账交易
        private void btnSendTransaction_Click(object sender, EventArgs e)
        {
            UInt160 assetId;
            string api = transAccountFrm.RpcUrl;
            if (cmbxTokenType.Text == "BCP")
            {
                assetId = Genesis.BcpContractAddress;
            }
            else if (cmbxTokenType.Text == "BCT")
            {
                assetId = Genesis.BctContractAddress;
            }
            else if(cmbxTokenType.Text == "BCS")
            {
                assetId = transAccountFrm.bcsAssetId;
            }
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

            Decimal value = Decimal.Parse(tbxValue.Text, NumberStyles.Float) * new Decimal(Math.Pow(10, 8));
            UInt160 targetscripthash = ZoroHelper.GetPublicKeyHashFromAddress(tbxTargetAddress.Text);

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Transfer", assetId, transAccountFrm.addressHash, targetscripthash, new BigInteger(value));

                decimal gasLimit = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");

                gasLimit = Math.Max(decimal.Parse(tbxGasLimit.Text), gasLimit);

                decimal gasPrice = decimal.Parse(tbxGasPrice.Text);

                var result = ZoroHelper.SendInvocationTransaction(api,sb.ToArray(), transAccountFrm.keypair, "", Fixed8.FromDecimal(1000), Fixed8.FromDecimal(gasPrice));

                rtbxTranResult.Text = result;
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

                bcpFee = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");

                lblBcpFee.Text = bcpFee.ToString();

                var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), publishAccountFrm.keypair, "", Fixed8.FromDecimal(bcpFee), Fixed8.FromDecimal(0.0001m));

                rtbxPublishReturn.Text = result;
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
                    List<dynamic> paraList = GetParameterArray();
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

            var info = ZoroHelper.InvokeScript(api, sb.ToArray(), "");

            rtbxReturnJson.Text = info;
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
                    List<dynamic> paraList = GetParameterArray();
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

            decimal gasLimit = ZoroHelper.GetScriptGasConsumed(api, sb.ToArray(), "");
            gasLimit = Math.Max(decimal.Parse(tbxGasLimit.Text), gasLimit);

            decimal gasPrice= decimal.Parse(tbxGasPrice.Text);

            //var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), invokeAccountFrm.keypair, "", Fixed8.FromDecimal(gasLimit), Fixed8.FromDecimal(gasPrice));

            var result = ZoroHelper.SendInvocationTransaction(api, sb.ToArray(), invokeAccountFrm.keypair, "", Fixed8.FromDecimal(gasPrice));

            rtbxReturnJson.Text = result;

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

        private List<dynamic> GetParameterArray()
        {
            List<dynamic> paraList = new List<dynamic>();

            string[] parameterArray = rtbxParameterJson.Text.Split(';');
            for (int i = 0; i < parameterArray.Length; i++)
            {
                paraList.Add(ZoroHelper.GetParamBytes(parameterArray[i]));
            }

            return paraList;
        }

        private void tbxGasFee_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsPunctuation(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;//消除不合适字符
            }
            else if (Char.IsPunctuation(e.KeyChar))
            {
                if (e.KeyChar != '.' )//小数点
                {
                    e.Handled = true;
                }
            }
        }
    }
}
