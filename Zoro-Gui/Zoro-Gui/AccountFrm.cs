using System;
using System.Windows.Forms;
using Zoro.Wallets;
using Zoro;
using Zoro.Ledger;
using Neo.VM;
using Newtonsoft.Json.Linq;

namespace Zoro_Gui
{
    public partial class AccountFrm : UserControl
    {
        public string wif;
        public KeyPair keypair;
        public UInt160 addressHash;
        public string address;

        public AccountFrm()
        {
            InitializeComponent();
        }

        private void tbxAccountWif_TextChanged(object sender, EventArgs e)
        {
            if (GetAccount())
            {
                GetBalance();
            }
        }

        private bool GetAccount()
        {
            try
            {
                keypair = ZoroHelper.GetKeyPairFromWIF(tbxAccountWif.Text);
                addressHash = ZoroHelper.GetPublicKeyHashFromWIF(tbxAccountWif.Text);
                tbxAccountAddress.Text = ZoroHelper.GetAddressFromScriptHash(addressHash);
                wif = tbxAccountWif.Text;
                address = ZoroHelper.GetAddressFromScriptHash(addressHash);
            }
            catch
            {
                MessageBox.Show("钱包 Wif 密钥格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void GetBalance()
        {
            UInt160 bcpAssetId = Genesis.BcpContractAddress;
            UInt160 bctAssetId = Genesis.BctContractAddress;
            UInt160 bcsAssetId = UInt160.Parse("0xbca3d3be47bd966fddd2702ac0dac1a3bdaf317e");

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "BalanceOf", bcpAssetId, addressHash);
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Decimals", bcpAssetId);

                var info = ZoroHelper.InvokeScript(sb.ToArray(), "");
                var value = GetBalanceFromJson(info);

                lblBcpBalance.Text = value;

            }

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "BalanceOf", bctAssetId, addressHash);
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Decimals", bctAssetId);

                var info = ZoroHelper.InvokeScript(sb.ToArray(), "");
                var value = GetBalanceFromJson(info);

                lblBctBalance.Text = value;

            }

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "BalanceOf", bcsAssetId, addressHash);
                sb.EmitSysCall("Zoro.NativeNEP5.Call", "Decimals", bcsAssetId);

                var info = ZoroHelper.InvokeScript(sb.ToArray(), "");
                var value = GetBalanceFromJson(info);

                lblBcsBalance.Text = value;

            }
        }

        private string GetBalanceFromJson(string info)
        {
            string result = "";
            JObject json = JObject.Parse(info);

            if (json.ContainsKey("result"))
            {
                JObject json_result = json["result"] as JObject;
                JArray stack = json_result["stack"] as JArray;

                if (stack != null && stack.Count >= 2)
                {
                    string balance = ZoroHelper.GetJsonValue(stack[0] as JObject);
                    string decimals = ZoroHelper.GetJsonValue(stack[1] as JObject);

                    Decimal value = Decimal.Parse(balance) / new Decimal(Math.Pow(10, int.Parse(decimals)));
                    string fmt = "{0:N" + decimals + "}";
                    result = string.Format(fmt, value);
                }
            }
            else if (json.ContainsKey("error"))
            {
                JObject json_error_obj = json["error"] as JObject;
                result = json_error_obj.ToString();
            }

            return result;
        }

        private void btnAccountRefresh_Click(object sender, EventArgs e)
        {
            if (GetAccount())
                GetBalance();
        }
    }
}
