namespace Zoro_Gui
{
    partial class AccountFrm
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAccountRefresh = new System.Windows.Forms.Button();
            this.lblBctBalance = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.lblBcpBalance = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.tbxAccountWif = new System.Windows.Forms.TextBox();
            this.tbxAccountAddress = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAccountRefresh
            // 
            this.btnAccountRefresh.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAccountRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAccountRefresh.FlatAppearance.BorderSize = 0;
            this.btnAccountRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccountRefresh.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAccountRefresh.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnAccountRefresh.Location = new System.Drawing.Point(756, 34);
            this.btnAccountRefresh.Name = "btnAccountRefresh";
            this.btnAccountRefresh.Size = new System.Drawing.Size(47, 29);
            this.btnAccountRefresh.TabIndex = 41;
            this.btnAccountRefresh.Text = "刷新";
            this.btnAccountRefresh.UseVisualStyleBackColor = false;
            // 
            // lblBctBalance
            // 
            this.lblBctBalance.AutoSize = true;
            this.lblBctBalance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblBctBalance.Location = new System.Drawing.Point(587, 61);
            this.lblBctBalance.Name = "lblBctBalance";
            this.lblBctBalance.Size = new System.Drawing.Size(140, 20);
            this.lblBctBalance.TabIndex = 40;
            this.lblBctBalance.Text = "00000000.00000000";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(518, 61);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(77, 20);
            this.label44.TabIndex = 39;
            this.label44.Text = "BCT余额：";
            // 
            // lblBcpBalance
            // 
            this.lblBcpBalance.AutoSize = true;
            this.lblBcpBalance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblBcpBalance.Location = new System.Drawing.Point(587, 22);
            this.lblBcpBalance.Name = "lblBcpBalance";
            this.lblBcpBalance.Size = new System.Drawing.Size(140, 20);
            this.lblBcpBalance.TabIndex = 38;
            this.lblBcpBalance.Text = "00000000.00000000";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(518, 21);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(78, 20);
            this.label46.TabIndex = 37;
            this.label46.Text = "BCP余额：";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(39, 62);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(79, 20);
            this.label47.TabIndex = 36;
            this.label47.Text = "钱包地址：";
            // 
            // tbxAccountWif
            // 
            this.tbxAccountWif.Location = new System.Drawing.Point(122, 17);
            this.tbxAccountWif.Name = "tbxAccountWif";
            this.tbxAccountWif.PasswordChar = '*';
            this.tbxAccountWif.Size = new System.Drawing.Size(391, 25);
            this.tbxAccountWif.TabIndex = 33;
            this.tbxAccountWif.TextChanged += new System.EventHandler(this.tbxAccountWif_TextChanged);
            // 
            // tbxAccountAddress
            // 
            this.tbxAccountAddress.BackColor = System.Drawing.Color.AliceBlue;
            this.tbxAccountAddress.Location = new System.Drawing.Point(122, 59);
            this.tbxAccountAddress.Name = "tbxAccountAddress";
            this.tbxAccountAddress.ReadOnly = true;
            this.tbxAccountAddress.Size = new System.Drawing.Size(391, 25);
            this.tbxAccountAddress.TabIndex = 35;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(39, 20);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(78, 20);
            this.label48.TabIndex = 34;
            this.label48.Text = "钱包 Wif：";
            // 
            // AccountFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.btnAccountRefresh);
            this.Controls.Add(this.lblBctBalance);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.lblBcpBalance);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.tbxAccountWif);
            this.Controls.Add(this.tbxAccountAddress);
            this.Controls.Add(this.label48);
            this.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "AccountFrm";
            this.Size = new System.Drawing.Size(853, 107);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAccountRefresh;
        private System.Windows.Forms.Label lblBctBalance;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label lblBcpBalance;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TextBox tbxAccountWif;
        private System.Windows.Forms.TextBox tbxAccountAddress;
        private System.Windows.Forms.Label label48;
    }
}
