using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using ThinNeo;

namespace MultiTransfer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            cbxRpc.SelectedIndex = 0;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbxFromWif.Text))
            {
                MessageBox.Show("请输入钱包账户 Wif！");
                return;
            }
            if (string.IsNullOrEmpty(this.rtbxToAddress.Text))
            {
                MessageBox.Show("请输入收款地址！");
                return;
            }

            try
            {
                SendTransaction(tbxFromWif.Text, rtbxToAddress.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错了：" + ex.ToString());
            }
        }

        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbxFromWif.Text))
            {
                MessageBox.Show("请输入钱包账户 Wif！");
                return;
            }
            decimal decimals = 100000000;           

            GetBalance(decimals);

        }

        private void GetBalance(decimal decimals)
        {
            string api = cbxRpc.Text;
            try
            {
                byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(tbxFromWif.Text);
                byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
                string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

                tbxAddress.Text = address;

                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    JArray array = new JArray();
                    array.Add("(addr)" + address);
                    sb.EmitParamJson(array);
                    sb.EmitPushString("balanceOf");
                    sb.EmitAppCall(new Hash160(cbxHash.Text)); //合约脚本hash
                    byte[] data = sb.ToArray();

                    decimal balance = 0;
                    string script = Helper.Bytes2HexString(data);
                    var result = Helper.HttpGet($"{api}?method=invokescript&id=1&params=[\"{script}\"]");
                    if (JObject.Parse(result)["result"] is JArray res && res.Count > 0)
                    {
                        var stack = (res[0]["stack"] as JArray)[0] as JObject;
                        var vBanlance = new BigInteger(Helper.HexString2Bytes((string)stack["value"]));
                        balance = (decimal)vBanlance / decimals;
                    }

                    this.tbxBalance.Text = balance.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错了：" + ex.ToString());
            }
        }

        private static object logLock = new object();

        private void SendTransaction(string wif, string toAddress)
        {
            string path = Path.Combine($"{DateTime.Now:yyyy-MM-dd}.txt");

            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(wif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            string address = Helper_NEO.GetAddress_FromPublicKey(pubkey);
            var toAddrArray = toAddress.Split(new string[] { "\n" }, StringSplitOptions.None);
            decimal decimals = 100000000;            

            //if (toAddrArray.Length > 20)
            //{
            //    MessageBox.Show("一次最多支持20个地址！");
            //    return;
            //}

            foreach (var str in toAddrArray)
            {
                if (str.Length < 1)
                    continue;

                //ScriptBuilder sb = new ScriptBuilder();
                JArray array = new JArray();

                int index = str.IndexOf(";");
                string addr = str.Substring(0, index);
                string valueStr = str.Substring(index + 1);

                decimal amount = Math.Round(decimal.Parse(valueStr) * decimals, 0);

                array.Add("(addr)" + address); //from
                array.Add("(addr)" + addr); //to
                array.Add("(int)" + amount); //value
                //sb.EmitParamJson(array);
                //sb.EmitPushString("transfer");
                //sb.EmitAppCall(new Hash160(cbxHash.Text));//合约脚本hash

                string result = Helper.SendTransWithoutUtxo(prikey, cbxRpc.Text, cbxHash.Text, "transfer", array);

                //byte[] data = null;
                //data = sb.ToArray();
                //var result = SendrawTransaction(wif, data);

                if (result != null && result.Contains("result"))
                {
                    var res = JObject.Parse(result)["result"] as JArray;
                    var sendTxid = (string)res[0]["txid"];
                    if (!string.IsNullOrEmpty(sendTxid))
                        rtbxResult.Text += $"{addr} :交易发送成功; txid:{sendTxid}\n";
                    else
                    {
                        rtbxResult.Text += $"{addr} :交易发送失败; 返回:{result.ToString()}\n";
                        lock (logLock)
                        {                            
                            File.AppendAllLines(path, new[] { addr });
                        }
                    }
                }
                else
                {
                    rtbxResult.Text += $"{addr} :交易发送失败; 返回:{result.ToString()}\n";
                    lock (logLock)
                    {
                        File.AppendAllLines(path, new[] { addr });
                    }
                }
            }
        }

        private static Dictionary<string, string> usedUtxoDic = new Dictionary<string, string>();
        private static Dictionary<string, List<Utxo>> dic_UTXO = new Dictionary<string, List<Utxo>>();
        private static List<Utxo> list_Gas = new List<Utxo>();
        
        private static string gas_Id = "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7";

        public string SendrawTransaction(string wif, byte[] data)
        {
            string api = cbxRpc.Text;
            byte[] prikey = Helper_NEO.GetPrivateKeyFromWIF(wif);
            byte[] pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(prikey);
            var address = Helper_NEO.GetAddress_FromPublicKey(pubkey);

            //dic_UTXO = Helper.GetBalanceByAddress(api, address);

            if (!dic_UTXO.ContainsKey(gas_Id)||dic_UTXO[gas_Id].Count == 0)
            {
                dic_UTXO = Helper.GetBalanceByAddress(api, address, ref usedUtxoDic);
            }

            if (dic_UTXO.ContainsKey(gas_Id) == false)
            {
                throw new Exception("no gas.");
            }

            list_Gas = dic_UTXO[gas_Id];
            Transaction tran = Helper.makeTran(ref list_Gas, usedUtxoDic, new Hash256(gas_Id), (decimal)0.001);

            tran.type = ThinNeo.TransactionType.InvocationTransaction;
            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = data;
            idata.gas = 0;

            var signdata = Helper_NEO.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            var txid = tran.GetHash().ToString();

            foreach (var item in tran.inputs)
            {
                usedUtxoDic[((Hash256)item.hash).ToString() + item.index] = txid;
            }

            string inputStr = @"{
	            'jsonrpc': '2.0',
                'method': 'sendrawtransaction',
	            'params': ['#'],
	            'id': '1'
            }";

            inputStr = inputStr.Replace("#", strtrandata);
            string result = Helper.Post(api, inputStr, System.Text.Encoding.UTF8, 1);

            return result;
        }

        private void tbxFromWif_TextChanged_1(object sender, EventArgs e)
        {
            GetBalance(100000000);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.rtbxResult.Clear();
        }

        private void CbxRpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxHash.SelectedIndex = cbxRpc.SelectedIndex;
        }

        private void CbxHash_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxHash.SelectedIndex = cbxRpc.SelectedIndex;
        }
    }
}