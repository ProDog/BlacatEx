namespace Zoro_Gui
{
    partial class FrmZoroGui
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmZoroGui));
            this.tableControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.publishAccountFrm = new Zoro_Gui.AccountFrm();
            this.label25 = new System.Windows.Forms.Label();
            this.rtbxPublishReturn = new System.Windows.Forms.RichTextBox();
            this.btnLoadContract = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbxParameterType = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbxReturnType = new System.Windows.Forms.TextBox();
            this.btnPublish = new System.Windows.Forms.Button();
            this.lblBcpFee = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxNeedStorge = new System.Windows.Forms.CheckBox();
            this.cbxNeedCharge = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxContractName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxVersion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxAuthor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxDescri = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxContractHash = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxContractPath = new System.Windows.Forms.TextBox();
            this.cbxNeedNep4 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.invokeAccountFrm = new Zoro_Gui.AccountFrm();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tbxGasFee = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tbxMethodName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.rtbxReturnJson = new System.Windows.Forms.RichTextBox();
            this.btnSendRaw = new System.Windows.Forms.Button();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbxContractScriptHash = new System.Windows.Forms.TextBox();
            this.rtbxParameterJson = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.transAccountFrm = new Zoro_Gui.AccountFrm();
            this.btnCancelTran = new System.Windows.Forms.Button();
            this.btnSendTransaction = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.rtbxTranResult = new System.Windows.Forms.RichTextBox();
            this.tbxValue = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.cmbxTokenType = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbxTargetAddress = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.rtbxMutiSign = new System.Windows.Forms.RichTextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tableControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableControl
            // 
            this.tableControl.Controls.Add(this.tabPage1);
            this.tableControl.Controls.Add(this.tabPage2);
            this.tableControl.Controls.Add(this.tabPage3);
            this.tableControl.Controls.Add(this.tabPage4);
            this.tableControl.Controls.Add(this.tabPage5);
            this.tableControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableControl.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableControl.Location = new System.Drawing.Point(0, 0);
            this.tableControl.Name = "tableControl";
            this.tableControl.SelectedIndex = 0;
            this.tableControl.Size = new System.Drawing.Size(856, 540);
            this.tableControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.Controls.Add(this.publishAccountFrm);
            this.tabPage1.Controls.Add(this.label25);
            this.tabPage1.Controls.Add(this.rtbxPublishReturn);
            this.tabPage1.Controls.Add(this.btnLoadContract);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.tbxParameterType);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.tbxReturnType);
            this.tabPage1.Controls.Add(this.btnPublish);
            this.tabPage1.Controls.Add(this.lblBcpFee);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.cbxNeedStorge);
            this.tabPage1.Controls.Add(this.cbxNeedCharge);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.tbxContractName);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.tbxVersion);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.tbxAuthor);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.tbxEmail);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tbxDescri);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.tbxContractHash);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.tbxContractPath);
            this.tabPage1.Controls.Add(this.cbxNeedNep4);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(848, 508);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "发布合约";
            // 
            // publishAccountFrm
            // 
            this.publishAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.publishAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.publishAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.publishAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.publishAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.publishAccountFrm.Name = "publishAccountFrm";
            this.publishAccountFrm.Size = new System.Drawing.Size(842, 98);
            this.publishAccountFrm.TabIndex = 29;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(39, 398);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(79, 20);
            this.label25.TabIndex = 28;
            this.label25.Text = "返回结果：";
            // 
            // rtbxPublishReturn
            // 
            this.rtbxPublishReturn.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxPublishReturn.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxPublishReturn.Location = new System.Drawing.Point(44, 430);
            this.rtbxPublishReturn.Name = "rtbxPublishReturn";
            this.rtbxPublishReturn.ReadOnly = true;
            this.rtbxPublishReturn.Size = new System.Drawing.Size(758, 58);
            this.rtbxPublishReturn.TabIndex = 27;
            this.rtbxPublishReturn.Text = "";
            // 
            // btnLoadContract
            // 
            this.btnLoadContract.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnLoadContract.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLoadContract.FlatAppearance.BorderSize = 0;
            this.btnLoadContract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadContract.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLoadContract.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLoadContract.Location = new System.Drawing.Point(355, 117);
            this.btnLoadContract.Name = "btnLoadContract";
            this.btnLoadContract.Size = new System.Drawing.Size(47, 29);
            this.btnLoadContract.TabIndex = 26;
            this.btnLoadContract.Text = "加载";
            this.btnLoadContract.UseVisualStyleBackColor = false;
            this.btnLoadContract.Click += new System.EventHandler(this.btnLoadContract_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(147, 316);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "入参类型：";
            // 
            // tbxParameterType
            // 
            this.tbxParameterType.Location = new System.Drawing.Point(232, 312);
            this.tbxParameterType.Name = "tbxParameterType";
            this.tbxParameterType.Size = new System.Drawing.Size(170, 25);
            this.tbxParameterType.TabIndex = 24;
            this.tbxParameterType.Text = "0710";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(427, 313);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 20);
            this.label11.TabIndex = 23;
            this.label11.Text = "返回类型：";
            // 
            // tbxReturnType
            // 
            this.tbxReturnType.Location = new System.Drawing.Point(512, 312);
            this.tbxReturnType.Name = "tbxReturnType";
            this.tbxReturnType.Size = new System.Drawing.Size(170, 25);
            this.tbxReturnType.TabIndex = 22;
            this.tbxReturnType.Text = "05";
            // 
            // btnPublish
            // 
            this.btnPublish.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnPublish.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPublish.FlatAppearance.BorderSize = 0;
            this.btnPublish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPublish.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPublish.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnPublish.Location = new System.Drawing.Point(583, 363);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(99, 29);
            this.btnPublish.TabIndex = 19;
            this.btnPublish.Text = "确认部署";
            this.btnPublish.UseVisualStyleBackColor = false;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // lblBcpFee
            // 
            this.lblBcpFee.AutoSize = true;
            this.lblBcpFee.ForeColor = System.Drawing.Color.Red;
            this.lblBcpFee.Location = new System.Drawing.Point(521, 368);
            this.lblBcpFee.Name = "lblBcpFee";
            this.lblBcpFee.Size = new System.Drawing.Size(44, 20);
            this.lblBcpFee.TabIndex = 18;
            this.lblBcpFee.Text = "90.00";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(428, 368);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 20);
            this.label8.TabIndex = 17;
            this.label8.Text = "花费 BCP：";
            // 
            // cbxNeedStorge
            // 
            this.cbxNeedStorge.AutoSize = true;
            this.cbxNeedStorge.Location = new System.Drawing.Point(245, 366);
            this.cbxNeedStorge.Name = "cbxNeedStorge";
            this.cbxNeedStorge.Size = new System.Drawing.Size(104, 24);
            this.cbxNeedStorge.TabIndex = 16;
            this.cbxNeedStorge.Text = "使用 Storge";
            this.cbxNeedStorge.UseVisualStyleBackColor = true;
            this.cbxNeedStorge.CheckedChanged += new System.EventHandler(this.cbxNeedStorge_CheckedChanged);
            // 
            // cbxNeedCharge
            // 
            this.cbxNeedCharge.AutoSize = true;
            this.cbxNeedCharge.Location = new System.Drawing.Point(357, 366);
            this.cbxNeedCharge.Name = "cbxNeedCharge";
            this.cbxNeedCharge.Size = new System.Drawing.Size(70, 24);
            this.cbxNeedCharge.TabIndex = 15;
            this.cbxNeedCharge.Text = "可收款";
            this.cbxNeedCharge.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(427, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "合约名称：";
            // 
            // tbxContractName
            // 
            this.tbxContractName.Location = new System.Drawing.Point(512, 119);
            this.tbxContractName.Name = "tbxContractName";
            this.tbxContractName.Size = new System.Drawing.Size(170, 25);
            this.tbxContractName.TabIndex = 13;
            this.tbxContractName.Text = "TestContract";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 221);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "版 本：";
            // 
            // tbxVersion
            // 
            this.tbxVersion.Location = new System.Drawing.Point(232, 217);
            this.tbxVersion.Name = "tbxVersion";
            this.tbxVersion.Size = new System.Drawing.Size(170, 25);
            this.tbxVersion.TabIndex = 11;
            this.tbxVersion.Text = "1.0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 268);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "作 者：";
            // 
            // tbxAuthor
            // 
            this.tbxAuthor.Location = new System.Drawing.Point(232, 264);
            this.tbxAuthor.Name = "tbxAuthor";
            this.tbxAuthor.Size = new System.Drawing.Size(170, 25);
            this.tbxAuthor.TabIndex = 9;
            this.tbxAuthor.Text = "TestA";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "邮 箱：";
            // 
            // tbxEmail
            // 
            this.tbxEmail.Location = new System.Drawing.Point(512, 216);
            this.tbxEmail.Name = "tbxEmail";
            this.tbxEmail.Size = new System.Drawing.Size(170, 25);
            this.tbxEmail.TabIndex = 7;
            this.tbxEmail.Text = "TestA@zoro.com";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(427, 265);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "描 述：";
            // 
            // tbxDescri
            // 
            this.tbxDescri.Location = new System.Drawing.Point(512, 264);
            this.tbxDescri.Name = "tbxDescri";
            this.tbxDescri.Size = new System.Drawing.Size(170, 25);
            this.tbxDescri.TabIndex = 5;
            this.tbxDescri.Text = "None";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hash：";
            // 
            // tbxContractHash
            // 
            this.tbxContractHash.BackColor = System.Drawing.Color.AliceBlue;
            this.tbxContractHash.Location = new System.Drawing.Point(232, 166);
            this.tbxContractHash.Name = "tbxContractHash";
            this.tbxContractHash.ReadOnly = true;
            this.tbxContractHash.Size = new System.Drawing.Size(450, 25);
            this.tbxContractHash.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "合约文件：";
            // 
            // tbxContractPath
            // 
            this.tbxContractPath.Location = new System.Drawing.Point(232, 119);
            this.tbxContractPath.Name = "tbxContractPath";
            this.tbxContractPath.Size = new System.Drawing.Size(117, 25);
            this.tbxContractPath.TabIndex = 1;
            // 
            // cbxNeedNep4
            // 
            this.cbxNeedNep4.AutoSize = true;
            this.cbxNeedNep4.Location = new System.Drawing.Point(153, 366);
            this.cbxNeedNep4.Name = "cbxNeedNep4";
            this.cbxNeedNep4.Size = new System.Drawing.Size(84, 24);
            this.cbxNeedNep4.TabIndex = 0;
            this.cbxNeedNep4.Text = "动态调用";
            this.cbxNeedNep4.UseVisualStyleBackColor = true;
            this.cbxNeedNep4.CheckedChanged += new System.EventHandler(this.cbxNeedNep4_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.invokeAccountFrm);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.tbxGasFee);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.tbxMethodName);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.rtbxReturnJson);
            this.tabPage2.Controls.Add(this.btnSendRaw);
            this.tabPage2.Controls.Add(this.btnInvoke);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.tbxContractScriptHash);
            this.tabPage2.Controls.Add(this.rtbxParameterJson);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(848, 508);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "调用合约";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // invokeAccountFrm
            // 
            this.invokeAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.invokeAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.invokeAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.invokeAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.invokeAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.invokeAccountFrm.Name = "invokeAccountFrm";
            this.invokeAccountFrm.Size = new System.Drawing.Size(842, 99);
            this.invokeAccountFrm.TabIndex = 30;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(183, 289);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(36, 20);
            this.label24.TabIndex = 29;
            this.label24.Text = "BCP";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(39, 289);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(75, 20);
            this.label23.TabIndex = 27;
            this.label23.Text = "Gas费用：";
            // 
            // tbxGasFee
            // 
            this.tbxGasFee.Location = new System.Drawing.Point(118, 286);
            this.tbxGasFee.Name = "tbxGasFee";
            this.tbxGasFee.Size = new System.Drawing.Size(62, 25);
            this.tbxGasFee.TabIndex = 26;
            this.tbxGasFee.Text = "10";
            this.tbxGasFee.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxGasFee_KeyPress);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(550, 115);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 20);
            this.label16.TabIndex = 25;
            this.label16.Text = "调用接口：";
            // 
            // tbxMethodName
            // 
            this.tbxMethodName.Location = new System.Drawing.Point(635, 112);
            this.tbxMethodName.Name = "tbxMethodName";
            this.tbxMethodName.Size = new System.Drawing.Size(167, 25);
            this.tbxMethodName.TabIndex = 24;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(39, 320);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 20);
            this.label15.TabIndex = 23;
            this.label15.Text = "返回结果：";
            // 
            // rtbxReturnJson
            // 
            this.rtbxReturnJson.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxReturnJson.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxReturnJson.Location = new System.Drawing.Point(44, 352);
            this.rtbxReturnJson.Name = "rtbxReturnJson";
            this.rtbxReturnJson.ReadOnly = true;
            this.rtbxReturnJson.Size = new System.Drawing.Size(758, 132);
            this.rtbxReturnJson.TabIndex = 22;
            this.rtbxReturnJson.Text = "";
            // 
            // btnSendRaw
            // 
            this.btnSendRaw.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSendRaw.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSendRaw.FlatAppearance.BorderSize = 0;
            this.btnSendRaw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendRaw.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSendRaw.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSendRaw.Location = new System.Drawing.Point(671, 289);
            this.btnSendRaw.Name = "btnSendRaw";
            this.btnSendRaw.Size = new System.Drawing.Size(99, 29);
            this.btnSendRaw.TabIndex = 21;
            this.btnSendRaw.Text = "SendRaw";
            this.btnSendRaw.UseVisualStyleBackColor = false;
            this.btnSendRaw.Click += new System.EventHandler(this.btnSendRaw_Click);
            // 
            // btnInvoke
            // 
            this.btnInvoke.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnInvoke.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnInvoke.FlatAppearance.BorderSize = 0;
            this.btnInvoke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInvoke.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnInvoke.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnInvoke.Location = new System.Drawing.Point(495, 289);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(99, 29);
            this.btnInvoke.TabIndex = 20;
            this.btnInvoke.Text = "Invoke";
            this.btnInvoke.UseVisualStyleBackColor = false;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(39, 158);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(198, 20);
            this.label14.TabIndex = 7;
            this.label14.Text = "调用参数(多个参数用 ; 分隔)：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(35, 115);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 20);
            this.label13.TabIndex = 6;
            this.label13.Text = "合约 Hash：";
            // 
            // tbxContractScriptHash
            // 
            this.tbxContractScriptHash.Location = new System.Drawing.Point(125, 112);
            this.tbxContractScriptHash.Name = "tbxContractScriptHash";
            this.tbxContractScriptHash.Size = new System.Drawing.Size(391, 25);
            this.tbxContractScriptHash.TabIndex = 5;
            // 
            // rtbxParameterJson
            // 
            this.rtbxParameterJson.BackColor = System.Drawing.Color.Azure;
            this.rtbxParameterJson.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxParameterJson.Location = new System.Drawing.Point(44, 191);
            this.rtbxParameterJson.Name = "rtbxParameterJson";
            this.rtbxParameterJson.Size = new System.Drawing.Size(758, 80);
            this.rtbxParameterJson.TabIndex = 0;
            this.rtbxParameterJson.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.transAccountFrm);
            this.tabPage3.Controls.Add(this.btnCancelTran);
            this.tabPage3.Controls.Add(this.btnSendTransaction);
            this.tabPage3.Controls.Add(this.label22);
            this.tabPage3.Controls.Add(this.rtbxTranResult);
            this.tabPage3.Controls.Add(this.tbxValue);
            this.tabPage3.Controls.Add(this.label21);
            this.tabPage3.Controls.Add(this.label20);
            this.tabPage3.Controls.Add(this.cmbxTokenType);
            this.tabPage3.Controls.Add(this.label18);
            this.tabPage3.Controls.Add(this.tbxTargetAddress);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(848, 508);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "转账交易";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // transAccountFrm
            // 
            this.transAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.transAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.transAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.transAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.transAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.transAccountFrm.Name = "transAccountFrm";
            this.transAccountFrm.Size = new System.Drawing.Size(842, 95);
            this.transAccountFrm.TabIndex = 28;
            // 
            // btnCancelTran
            // 
            this.btnCancelTran.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCancelTran.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancelTran.FlatAppearance.BorderSize = 0;
            this.btnCancelTran.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelTran.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancelTran.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelTran.Location = new System.Drawing.Point(463, 271);
            this.btnCancelTran.Name = "btnCancelTran";
            this.btnCancelTran.Size = new System.Drawing.Size(99, 29);
            this.btnCancelTran.TabIndex = 27;
            this.btnCancelTran.Text = "取消";
            this.btnCancelTran.UseVisualStyleBackColor = false;
            this.btnCancelTran.Click += new System.EventHandler(this.btnCancelTran_Click);
            // 
            // btnSendTransaction
            // 
            this.btnSendTransaction.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSendTransaction.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSendTransaction.FlatAppearance.BorderSize = 0;
            this.btnSendTransaction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendTransaction.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSendTransaction.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSendTransaction.Location = new System.Drawing.Point(290, 271);
            this.btnSendTransaction.Name = "btnSendTransaction";
            this.btnSendTransaction.Size = new System.Drawing.Size(99, 29);
            this.btnSendTransaction.TabIndex = 26;
            this.btnSendTransaction.Text = "发送";
            this.btnSendTransaction.UseVisualStyleBackColor = false;
            this.btnSendTransaction.Click += new System.EventHandler(this.btnSendTransaction_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(44, 313);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 20);
            this.label22.TabIndex = 25;
            this.label22.Text = "返回结果：";
            // 
            // rtbxTranResult
            // 
            this.rtbxTranResult.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxTranResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxTranResult.Location = new System.Drawing.Point(49, 345);
            this.rtbxTranResult.Name = "rtbxTranResult";
            this.rtbxTranResult.ReadOnly = true;
            this.rtbxTranResult.Size = new System.Drawing.Size(748, 77);
            this.rtbxTranResult.TabIndex = 24;
            this.rtbxTranResult.Text = "";
            // 
            // tbxValue
            // 
            this.tbxValue.Location = new System.Drawing.Point(491, 160);
            this.tbxValue.Name = "tbxValue";
            this.tbxValue.Size = new System.Drawing.Size(185, 25);
            this.tbxValue.TabIndex = 22;
            this.tbxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxGasFee_KeyPress);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(406, 163);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 20);
            this.label21.TabIndex = 23;
            this.label21.Text = "转账金额：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(141, 161);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(79, 20);
            this.label20.TabIndex = 8;
            this.label20.Text = "选择币种：";
            // 
            // cmbxTokenType
            // 
            this.cmbxTokenType.FormattingEnabled = true;
            this.cmbxTokenType.Items.AddRange(new object[] {
            "BCP",
            "BCT"});
            this.cmbxTokenType.Location = new System.Drawing.Point(226, 160);
            this.cmbxTokenType.Name = "cmbxTokenType";
            this.cmbxTokenType.Size = new System.Drawing.Size(163, 27);
            this.cmbxTokenType.TabIndex = 7;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(141, 219);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 20);
            this.label18.TabIndex = 6;
            this.label18.Text = "接收地址：";
            // 
            // tbxTargetAddress
            // 
            this.tbxTargetAddress.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbxTargetAddress.Location = new System.Drawing.Point(226, 216);
            this.tbxTargetAddress.Name = "tbxTargetAddress";
            this.tbxTargetAddress.Size = new System.Drawing.Size(450, 25);
            this.tbxTargetAddress.TabIndex = 5;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button1);
            this.tabPage4.Controls.Add(this.button2);
            this.tabPage4.Controls.Add(this.label27);
            this.tabPage4.Controls.Add(this.richTextBox2);
            this.tabPage4.Controls.Add(this.textBox1);
            this.tabPage4.Controls.Add(this.label28);
            this.tabPage4.Controls.Add(this.label29);
            this.tabPage4.Controls.Add(this.comboBox1);
            this.tabPage4.Controls.Add(this.label30);
            this.tabPage4.Controls.Add(this.textBox2);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.rtbxMutiSign);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(848, 508);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "多签交易";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Highlight;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button1.Location = new System.Drawing.Point(461, 277);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 29);
            this.button1.TabIndex = 37;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Highlight;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.Location = new System.Drawing.Point(288, 276);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 29);
            this.button2.TabIndex = 36;
            this.button2.Text = "发送";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(47, 330);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(79, 20);
            this.label27.TabIndex = 35;
            this.label27.Text = "返回结果：";
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Location = new System.Drawing.Point(51, 362);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(748, 53);
            this.richTextBox2.TabIndex = 34;
            this.richTextBox2.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(489, 168);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(185, 25);
            this.textBox1.TabIndex = 32;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(404, 171);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(79, 20);
            this.label28.TabIndex = 33;
            this.label28.Text = "转账金额：";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(139, 169);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 20);
            this.label29.TabIndex = 31;
            this.label29.Text = "选择币种：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "BCP",
            "BCT"});
            this.comboBox1.Location = new System.Drawing.Point(224, 168);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(163, 27);
            this.comboBox1.TabIndex = 30;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(139, 227);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(79, 20);
            this.label30.TabIndex = 29;
            this.label30.Text = "接收地址：";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox2.Location = new System.Drawing.Point(224, 224);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(450, 25);
            this.textBox2.TabIndex = 28;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(41, 18);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(145, 20);
            this.label26.TabIndex = 9;
            this.label26.Text = "多签 Wif (用 ; 分隔)：";
            // 
            // rtbxMutiSign
            // 
            this.rtbxMutiSign.BackColor = System.Drawing.Color.Azure;
            this.rtbxMutiSign.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxMutiSign.Location = new System.Drawing.Point(49, 48);
            this.rtbxMutiSign.Name = "rtbxMutiSign";
            this.rtbxMutiSign.Size = new System.Drawing.Size(758, 98);
            this.rtbxMutiSign.TabIndex = 8;
            this.rtbxMutiSign.Text = "";
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 28);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(848, 508);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "创建地址";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // FrmZoroGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(856, 540);
            this.Controls.Add(this.tableControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmZoroGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zoro-Gui";
            this.Load += new System.EventHandler(this.FrmZoroGui_Load);
            this.tableControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tableControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox cbxNeedStorge;
        private System.Windows.Forms.CheckBox cbxNeedCharge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxContractName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxAuthor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxEmail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxDescri;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxContractHash;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxContractPath;
        private System.Windows.Forms.CheckBox cbxNeedNep4;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblBcpFee;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbxParameterType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbxReturnType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbxContractScriptHash;
        private System.Windows.Forms.RichTextBox rtbxParameterJson;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnSendRaw;
        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RichTextBox rtbxReturnJson;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbxMethodName;
        private System.Windows.Forms.ComboBox cmbxTokenType;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbxTargetAddress;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnCancelTran;
        private System.Windows.Forms.Button btnSendTransaction;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.RichTextBox rtbxTranResult;
        private System.Windows.Forms.TextBox tbxValue;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button btnLoadContract;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbxGasFee;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.RichTextBox rtbxPublishReturn;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.RichTextBox rtbxMutiSign;
        private AccountFrm publishAccountFrm;
        private AccountFrm invokeAccountFrm;
        private AccountFrm transAccountFrm;
        private System.Windows.Forms.TabPage tabPage5;
    }
}

