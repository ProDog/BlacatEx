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
            this.rtbxToAddress = new System.Windows.Forms.RichTextBox();
            this.tbxFromWif = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxTokenHash = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxBalance = new System.Windows.Forms.TextBox();
            this.btnGetBalance = new System.Windows.Forms.Button();
            this.rtbxResult = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rtbxToAddress
            // 
            this.rtbxToAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxToAddress.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbxToAddress.Location = new System.Drawing.Point(37, 411);
            this.rtbxToAddress.Name = "rtbxToAddress";
            this.rtbxToAddress.Size = new System.Drawing.Size(348, 400);
            this.rtbxToAddress.TabIndex = 0;
            this.rtbxToAddress.Text = "";
            // 
            // tbxFromWif
            // 
            this.tbxFromWif.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxFromWif.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxFromWif.Location = new System.Drawing.Point(34, 154);
            this.tbxFromWif.Name = "tbxFromWif";
            this.tbxFromWif.Size = new System.Drawing.Size(495, 22);
            this.tbxFromWif.TabIndex = 1;
            this.tbxFromWif.TextChanged += new System.EventHandler(this.tbxFromWif_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(31, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "钱包 Wif:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(33, 371);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(260, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "收款地址(多个地址用换行分隔):";
            // 
            // btnTransfer
            // 
            this.btnTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTransfer.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTransfer.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnTransfer.Location = new System.Drawing.Point(213, 314);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(57, 26);
            this.btnTransfer.TabIndex = 4;
            this.btnTransfer.Text = "转  账";
            this.btnTransfer.UseVisualStyleBackColor = false;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(31, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "转账金额:";
            // 
            // tbxValue
            // 
            this.tbxValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxValue.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxValue.Location = new System.Drawing.Point(33, 315);
            this.tbxValue.Name = "tbxValue";
            this.tbxValue.Size = new System.Drawing.Size(172, 23);
            this.tbxValue.TabIndex = 5;
            this.tbxValue.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(33, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 24);
            this.label4.TabIndex = 8;
            this.label4.Text = "代币 Hash(BCP/BCT):";
            // 
            // tbxTokenHash
            // 
            this.tbxTokenHash.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxTokenHash.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxTokenHash.Location = new System.Drawing.Point(35, 72);
            this.tbxTokenHash.Name = "tbxTokenHash";
            this.tbxTokenHash.Size = new System.Drawing.Size(495, 23);
            this.tbxTokenHash.TabIndex = 7;
            this.tbxTokenHash.Text = "0x04e31cee0443bb916534dad2adf508458920e66d";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(33, 203);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 24);
            this.label5.TabIndex = 11;
            this.label5.Text = "钱包余额:";
            // 
            // tbxBalance
            // 
            this.tbxBalance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxBalance.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxBalance.Location = new System.Drawing.Point(35, 240);
            this.tbxBalance.Name = "tbxBalance";
            this.tbxBalance.Size = new System.Drawing.Size(172, 23);
            this.tbxBalance.TabIndex = 10;
            this.tbxBalance.Text = "1";
            // 
            // btnGetBalance
            // 
            this.btnGetBalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnGetBalance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGetBalance.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetBalance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnGetBalance.Location = new System.Drawing.Point(213, 239);
            this.btnGetBalance.Name = "btnGetBalance";
            this.btnGetBalance.Size = new System.Drawing.Size(57, 26);
            this.btnGetBalance.TabIndex = 12;
            this.btnGetBalance.Text = "刷  新";
            this.btnGetBalance.UseVisualStyleBackColor = false;
            this.btnGetBalance.Click += new System.EventHandler(this.btnGetBalance_Click);
            // 
            // rtbxResult
            // 
            this.rtbxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxResult.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbxResult.Location = new System.Drawing.Point(599, 72);
            this.rtbxResult.Name = "rtbxResult";
            this.rtbxResult.Size = new System.Drawing.Size(574, 739);
            this.rtbxResult.TabIndex = 13;
            this.rtbxResult.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(606, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 24);
            this.label6.TabIndex = 14;
            this.label6.Text = "转账结果：";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 841);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rtbxResult);
            this.Controls.Add(this.btnGetBalance);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxBalance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxTokenHash);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxValue);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxFromWif);
            this.Controls.Add(this.rtbxToAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
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
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxTokenHash;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxBalance;
        private System.Windows.Forms.Button btnGetBalance;
        private System.Windows.Forms.RichTextBox rtbxResult;
        private System.Windows.Forms.Label label6;
    }
}

