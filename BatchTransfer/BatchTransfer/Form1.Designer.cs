namespace MultiTransfer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.rtbxToAddress = new System.Windows.Forms.RichTextBox();
            this.tbxFromWif = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxBalance = new System.Windows.Forms.TextBox();
            this.btnGetBalance = new System.Windows.Forms.Button();
            this.rtbxResult = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxAddress = new System.Windows.Forms.TextBox();
            this.cbxRpc = new System.Windows.Forms.ComboBox();
            this.cbxHash = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // rtbxToAddress
            // 
            this.rtbxToAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxToAddress.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbxToAddress.Location = new System.Drawing.Point(24, 275);
            this.rtbxToAddress.Name = "rtbxToAddress";
            this.rtbxToAddress.Size = new System.Drawing.Size(386, 540);
            this.rtbxToAddress.TabIndex = 0;
            this.rtbxToAddress.Text = "AUVu2WHbpJX5xB9yFEoWRq9VNS5qQkT1jF;1.0";
            // 
            // tbxFromWif
            // 
            this.tbxFromWif.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxFromWif.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxFromWif.Location = new System.Drawing.Point(25, 122);
            this.tbxFromWif.Name = "tbxFromWif";
            this.tbxFromWif.PasswordChar = '*';
            this.tbxFromWif.Size = new System.Drawing.Size(385, 18);
            this.tbxFromWif.TabIndex = 1;
            this.tbxFromWif.TextChanged += new System.EventHandler(this.tbxFromWif_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(22, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "钱包 Wif:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(24, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(286, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "收款地址(多个地址用换行分隔，地址和金额用;分隔):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(24, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "代币 Hash(ZORO):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(24, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "钱包余额:";
            // 
            // tbxBalance
            // 
            this.tbxBalance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxBalance.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxBalance.Location = new System.Drawing.Point(26, 215);
            this.tbxBalance.Name = "tbxBalance";
            this.tbxBalance.Size = new System.Drawing.Size(172, 18);
            this.tbxBalance.TabIndex = 10;
            this.tbxBalance.Text = "1";
            // 
            // btnGetBalance
            // 
            this.btnGetBalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnGetBalance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGetBalance.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetBalance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnGetBalance.Location = new System.Drawing.Point(204, 214);
            this.btnGetBalance.Name = "btnGetBalance";
            this.btnGetBalance.Size = new System.Drawing.Size(57, 22);
            this.btnGetBalance.TabIndex = 12;
            this.btnGetBalance.Text = "刷  新";
            this.btnGetBalance.UseVisualStyleBackColor = false;
            this.btnGetBalance.Click += new System.EventHandler(this.btnGetBalance_Click);
            // 
            // rtbxResult
            // 
            this.rtbxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxResult.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbxResult.Location = new System.Drawing.Point(432, 47);
            this.rtbxResult.Name = "rtbxResult";
            this.rtbxResult.Size = new System.Drawing.Size(445, 768);
            this.rtbxResult.TabIndex = 13;
            this.rtbxResult.Text = "";
            this.rtbxResult.WordWrap = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(431, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "转账结果：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(24, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 17);
            this.label7.TabIndex = 16;
            this.label7.Text = "RpcUrl:";
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSend.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSend.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSend.Location = new System.Drawing.Point(353, 213);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(57, 22);
            this.btnSend.TabIndex = 17;
            this.btnSend.Text = "转 账";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClear.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnClear.Location = new System.Drawing.Point(516, 14);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 22);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "清 除";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(22, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 17);
            this.label3.TabIndex = 20;
            this.label3.Text = "Address:";
            // 
            // tbxAddress
            // 
            this.tbxAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxAddress.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxAddress.Location = new System.Drawing.Point(25, 166);
            this.tbxAddress.Name = "tbxAddress";
            this.tbxAddress.Size = new System.Drawing.Size(385, 18);
            this.tbxAddress.TabIndex = 19;
            // 
            // cbxRpc
            // 
            this.cbxRpc.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxRpc.FormattingEnabled = true;
            this.cbxRpc.Items.AddRange(new object[] {
            "https://api.nel.group/api/testnet",
            "https://api.nel.group/api/mainnet"});
            this.cbxRpc.Location = new System.Drawing.Point(27, 26);
            this.cbxRpc.Name = "cbxRpc";
            this.cbxRpc.Size = new System.Drawing.Size(383, 25);
            this.cbxRpc.TabIndex = 21;
            this.cbxRpc.SelectedIndexChanged += new System.EventHandler(this.CbxRpc_SelectedIndexChanged);
            // 
            // cbxHash
            // 
            this.cbxHash.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxHash.FormattingEnabled = true;
            this.cbxHash.Items.AddRange(new object[] {
            "0x6ac01fb3dfe0509fb31d27a49ec0d3dc553b4ec6",
            "0x7e2b538aa6015e06b0a036f2bfdc07077c5368b4"});
            this.cbxHash.Location = new System.Drawing.Point(27, 74);
            this.cbxHash.Name = "cbxHash";
            this.cbxHash.Size = new System.Drawing.Size(383, 25);
            this.cbxHash.TabIndex = 22;
            this.cbxHash.SelectedIndexChanged += new System.EventHandler(this.CbxHash_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(909, 841);
            this.Controls.Add(this.cbxHash);
            this.Controls.Add(this.cbxRpc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxAddress);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rtbxResult);
            this.Controls.Add(this.btnGetBalance);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxBalance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxFromWif);
            this.Controls.Add(this.rtbxToAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "批量转账";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbxToAddress;
        private System.Windows.Forms.TextBox tbxFromWif;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxBalance;
        private System.Windows.Forms.Button btnGetBalance;
        private System.Windows.Forms.RichTextBox rtbxResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxAddress;
        private System.Windows.Forms.ComboBox cbxRpc;
        private System.Windows.Forms.ComboBox cbxHash;
    }
}

