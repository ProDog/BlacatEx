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
            this.btnEstimateGas = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.tbxGasPrice = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbxGasLimit = new System.Windows.Forms.TextBox();
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
            this.invokeAccountFrm = new Zoro_Gui.AccountFrm();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.tbxTransGasPrice = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbxTransGasLimit = new System.Windows.Forms.TextBox();
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
            this.transAccountFrm = new Zoro_Gui.AccountFrm();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label35 = new System.Windows.Forms.Label();
            this.tbxNep5GasPrice = new System.Windows.Forms.TextBox();
            this.tbxMyAddress = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.btnNep5Refresh = new System.Windows.Forms.Button();
            this.lblNep5Balance = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.tbxNep5Hash = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.btnNep5Cancel = new System.Windows.Forms.Button();
            this.btnNep5SendTran = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.rtbxNep5Result = new System.Windows.Forms.RichTextBox();
            this.tbxNep5Value = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.tbxNep5ToAddress = new System.Windows.Forms.TextBox();
            this.nep5AccountFrm = new Zoro_Gui.AccountFrm();
            this.tbcMutiTrans = new System.Windows.Forms.TabPage();
            this.label26 = new System.Windows.Forms.Label();
            this.tbxMutiSigRpcUrl = new System.Windows.Forms.TextBox();
            this.btcMutiSigCancel = new System.Windows.Forms.Button();
            this.btnMutiSigSendTran = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.rtbxMutiSigResult = new System.Windows.Forms.RichTextBox();
            this.tbxMutiSigValue = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.cmbMutiSigCoinType = new System.Windows.Forms.ComboBox();
            this.label39 = new System.Windows.Forms.Label();
            this.tbxMutiSigRecAddress = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.rtbxMutiSigWifs = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label42 = new System.Windows.Forms.Label();
            this.tbxCallMethod = new System.Windows.Forms.TextBox();
            this.btnCallEstimateGas = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.tbxCallGasPrice = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.tbxCallGasLimit = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbxCallInterface = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.rtbxCallResult = new System.Windows.Forms.RichTextBox();
            this.btnCallSendRaw = new System.Windows.Forms.Button();
            this.btnCallInvoke = new System.Windows.Forms.Button();
            this.label41 = new System.Windows.Forms.Label();
            this.rtbxCallParams = new System.Windows.Forms.RichTextBox();
            this.otherCallAccountFrm = new Zoro_Gui.AccountFrm();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label49 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.BatchGasPrice = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.BatchGasLimit = new System.Windows.Forms.TextBox();
            this.batchCancel = new System.Windows.Forms.Button();
            this.BatchSend = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.BatchResult = new System.Windows.Forms.RichTextBox();
            this.BatchTbxValue = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.BatchCbxCoinType = new System.Windows.Forms.ComboBox();
            this.label48 = new System.Windows.Forms.Label();
            this.BatchToAddress = new System.Windows.Forms.TextBox();
            this.BatchaccountFrm = new Zoro_Gui.AccountFrm();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tbxWithdrawAddress = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.tbxLockerAddress = new System.Windows.Forms.TextBox();
            this.cbxLockMethod = new System.Windows.Forms.ComboBox();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.rtbxLockReturnResult = new System.Windows.Forms.RichTextBox();
            this.btnLockSendRaw = new System.Windows.Forms.Button();
            this.btnLockInvoke = new System.Windows.Forms.Button();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.tbxLockContract = new System.Windows.Forms.TextBox();
            this.accountFrmLock = new Zoro_Gui.AccountFrm();
            this.tableControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tbcMutiTrans.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableControl
            // 
            this.tableControl.Controls.Add(this.tabPage1);
            this.tableControl.Controls.Add(this.tabPage2);
            this.tableControl.Controls.Add(this.tabPage3);
            this.tableControl.Controls.Add(this.tabPage5);
            this.tableControl.Controls.Add(this.tbcMutiTrans);
            this.tableControl.Controls.Add(this.tabPage4);
            this.tableControl.Controls.Add(this.tabPage6);
            this.tableControl.Controls.Add(this.tabPage7);
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
            this.publishAccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.publishAccountFrm.Size = new System.Drawing.Size(842, 120);
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
            this.btnLoadContract.Location = new System.Drawing.Point(355, 130);
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
            this.label10.Location = new System.Drawing.Point(147, 329);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "入参类型：";
            // 
            // tbxParameterType
            // 
            this.tbxParameterType.Location = new System.Drawing.Point(232, 325);
            this.tbxParameterType.Name = "tbxParameterType";
            this.tbxParameterType.Size = new System.Drawing.Size(170, 25);
            this.tbxParameterType.TabIndex = 24;
            this.tbxParameterType.Text = "0710";
            this.tbxParameterType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(427, 326);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 20);
            this.label11.TabIndex = 23;
            this.label11.Text = "返回类型：";
            // 
            // tbxReturnType
            // 
            this.tbxReturnType.Location = new System.Drawing.Point(512, 325);
            this.tbxReturnType.Name = "tbxReturnType";
            this.tbxReturnType.Size = new System.Drawing.Size(170, 25);
            this.tbxReturnType.TabIndex = 22;
            this.tbxReturnType.Text = "05";
            this.tbxReturnType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
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
            this.label7.Location = new System.Drawing.Point(427, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "合约名称：";
            // 
            // tbxContractName
            // 
            this.tbxContractName.Location = new System.Drawing.Point(512, 132);
            this.tbxContractName.Name = "tbxContractName";
            this.tbxContractName.Size = new System.Drawing.Size(170, 25);
            this.tbxContractName.TabIndex = 13;
            this.tbxContractName.Text = "TestContract";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "版 本：";
            // 
            // tbxVersion
            // 
            this.tbxVersion.Location = new System.Drawing.Point(232, 230);
            this.tbxVersion.Name = "tbxVersion";
            this.tbxVersion.Size = new System.Drawing.Size(170, 25);
            this.tbxVersion.TabIndex = 11;
            this.tbxVersion.Text = "1.0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 281);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "作 者：";
            // 
            // tbxAuthor
            // 
            this.tbxAuthor.Location = new System.Drawing.Point(232, 277);
            this.tbxAuthor.Name = "tbxAuthor";
            this.tbxAuthor.Size = new System.Drawing.Size(170, 25);
            this.tbxAuthor.TabIndex = 9;
            this.tbxAuthor.Text = "TestA";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "邮 箱：";
            // 
            // tbxEmail
            // 
            this.tbxEmail.Location = new System.Drawing.Point(512, 229);
            this.tbxEmail.Name = "tbxEmail";
            this.tbxEmail.Size = new System.Drawing.Size(170, 25);
            this.tbxEmail.TabIndex = 7;
            this.tbxEmail.Text = "TestA@zoro.com";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(427, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "描 述：";
            // 
            // tbxDescri
            // 
            this.tbxDescri.Location = new System.Drawing.Point(512, 277);
            this.tbxDescri.Name = "tbxDescri";
            this.tbxDescri.Size = new System.Drawing.Size(170, 25);
            this.tbxDescri.TabIndex = 5;
            this.tbxDescri.Text = "None";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hash：";
            // 
            // tbxContractHash
            // 
            this.tbxContractHash.BackColor = System.Drawing.Color.AliceBlue;
            this.tbxContractHash.Location = new System.Drawing.Point(232, 179);
            this.tbxContractHash.Name = "tbxContractHash";
            this.tbxContractHash.ReadOnly = true;
            this.tbxContractHash.Size = new System.Drawing.Size(450, 25);
            this.tbxContractHash.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "合约文件：";
            // 
            // tbxContractPath
            // 
            this.tbxContractPath.Location = new System.Drawing.Point(232, 132);
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
            this.tabPage2.Controls.Add(this.btnEstimateGas);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.tbxGasPrice);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.tbxGasLimit);
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
            this.tabPage2.Controls.Add(this.invokeAccountFrm);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(848, 508);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "调用合约";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnEstimateGas
            // 
            this.btnEstimateGas.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnEstimateGas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEstimateGas.FlatAppearance.BorderSize = 0;
            this.btnEstimateGas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstimateGas.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEstimateGas.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnEstimateGas.Location = new System.Drawing.Point(436, 292);
            this.btnEstimateGas.Name = "btnEstimateGas";
            this.btnEstimateGas.Size = new System.Drawing.Size(106, 29);
            this.btnEstimateGas.TabIndex = 33;
            this.btnEstimateGas.Text = "EstimateGas";
            this.btnEstimateGas.UseVisualStyleBackColor = false;
            this.btnEstimateGas.Click += new System.EventHandler(this.btnEstimateGas_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(224, 299);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 20);
            this.label9.TabIndex = 32;
            this.label9.Text = "GasPrice：";
            // 
            // tbxGasPrice
            // 
            this.tbxGasPrice.Location = new System.Drawing.Point(309, 294);
            this.tbxGasPrice.Name = "tbxGasPrice";
            this.tbxGasPrice.Size = new System.Drawing.Size(77, 25);
            this.tbxGasPrice.TabIndex = 31;
            this.tbxGasPrice.Text = "0.0001";
            this.tbxGasPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(39, 299);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(80, 20);
            this.label23.TabIndex = 27;
            this.label23.Text = "GasLimit：";
            // 
            // tbxGasLimit
            // 
            this.tbxGasLimit.Location = new System.Drawing.Point(124, 296);
            this.tbxGasLimit.Name = "tbxGasLimit";
            this.tbxGasLimit.Size = new System.Drawing.Size(93, 25);
            this.tbxGasLimit.TabIndex = 26;
            this.tbxGasLimit.Text = "1000";
            this.tbxGasLimit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(550, 135);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 20);
            this.label16.TabIndex = 25;
            this.label16.Text = "调用接口：";
            // 
            // tbxMethodName
            // 
            this.tbxMethodName.Location = new System.Drawing.Point(635, 132);
            this.tbxMethodName.Name = "tbxMethodName";
            this.tbxMethodName.Size = new System.Drawing.Size(167, 25);
            this.tbxMethodName.TabIndex = 24;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(39, 330);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 20);
            this.label15.TabIndex = 23;
            this.label15.Text = "返回结果：";
            // 
            // rtbxReturnJson
            // 
            this.rtbxReturnJson.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxReturnJson.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxReturnJson.Location = new System.Drawing.Point(44, 364);
            this.rtbxReturnJson.Name = "rtbxReturnJson";
            this.rtbxReturnJson.ReadOnly = true;
            this.rtbxReturnJson.Size = new System.Drawing.Size(758, 120);
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
            this.btnSendRaw.Location = new System.Drawing.Point(703, 292);
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
            this.btnInvoke.Location = new System.Drawing.Point(574, 292);
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
            this.label14.Location = new System.Drawing.Point(39, 165);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(198, 20);
            this.label14.TabIndex = 7;
            this.label14.Text = "调用参数(多个参数用 ; 分隔)：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(35, 135);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 20);
            this.label13.TabIndex = 6;
            this.label13.Text = "合约 Hash：";
            // 
            // tbxContractScriptHash
            // 
            this.tbxContractScriptHash.Location = new System.Drawing.Point(125, 132);
            this.tbxContractScriptHash.Name = "tbxContractScriptHash";
            this.tbxContractScriptHash.Size = new System.Drawing.Size(391, 25);
            this.tbxContractScriptHash.TabIndex = 5;
            // 
            // rtbxParameterJson
            // 
            this.rtbxParameterJson.BackColor = System.Drawing.Color.Azure;
            this.rtbxParameterJson.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxParameterJson.Location = new System.Drawing.Point(44, 200);
            this.rtbxParameterJson.Name = "rtbxParameterJson";
            this.rtbxParameterJson.Size = new System.Drawing.Size(758, 80);
            this.rtbxParameterJson.TabIndex = 0;
            this.rtbxParameterJson.Text = "";
            // 
            // invokeAccountFrm
            // 
            this.invokeAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.invokeAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.invokeAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.invokeAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.invokeAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.invokeAccountFrm.Name = "invokeAccountFrm";
            this.invokeAccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.invokeAccountFrm.Size = new System.Drawing.Size(842, 115);
            this.invokeAccountFrm.TabIndex = 30;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.tbxTransGasPrice);
            this.tabPage3.Controls.Add(this.label17);
            this.tabPage3.Controls.Add(this.tbxTransGasLimit);
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
            this.tabPage3.Controls.Add(this.transAccountFrm);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(848, 508);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "转账交易";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(431, 273);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 20);
            this.label12.TabIndex = 36;
            this.label12.Text = "GasPrice：";
            // 
            // tbxTransGasPrice
            // 
            this.tbxTransGasPrice.Location = new System.Drawing.Point(514, 268);
            this.tbxTransGasPrice.Name = "tbxTransGasPrice";
            this.tbxTransGasPrice.Size = new System.Drawing.Size(77, 25);
            this.tbxTransGasPrice.TabIndex = 35;
            this.tbxTransGasPrice.Text = "0.0001";
            this.tbxTransGasPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(246, 273);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(80, 20);
            this.label17.TabIndex = 34;
            this.label17.Text = "GasLimit：";
            // 
            // tbxTransGasLimit
            // 
            this.tbxTransGasLimit.Location = new System.Drawing.Point(328, 270);
            this.tbxTransGasLimit.Name = "tbxTransGasLimit";
            this.tbxTransGasLimit.Size = new System.Drawing.Size(93, 25);
            this.tbxTransGasLimit.TabIndex = 33;
            this.tbxTransGasLimit.Text = "1000";
            this.tbxTransGasLimit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // btnCancelTran
            // 
            this.btnCancelTran.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCancelTran.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancelTran.FlatAppearance.BorderSize = 0;
            this.btnCancelTran.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelTran.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancelTran.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelTran.Location = new System.Drawing.Point(463, 323);
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
            this.btnSendTransaction.Location = new System.Drawing.Point(290, 323);
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
            this.label22.Location = new System.Drawing.Point(44, 365);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 20);
            this.label22.TabIndex = 25;
            this.label22.Text = "返回结果：";
            // 
            // rtbxTranResult
            // 
            this.rtbxTranResult.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxTranResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxTranResult.Location = new System.Drawing.Point(49, 397);
            this.rtbxTranResult.Name = "rtbxTranResult";
            this.rtbxTranResult.ReadOnly = true;
            this.rtbxTranResult.Size = new System.Drawing.Size(748, 77);
            this.rtbxTranResult.TabIndex = 24;
            this.rtbxTranResult.Text = "";
            // 
            // tbxValue
            // 
            this.tbxValue.Location = new System.Drawing.Point(491, 170);
            this.tbxValue.Name = "tbxValue";
            this.tbxValue.Size = new System.Drawing.Size(185, 25);
            this.tbxValue.TabIndex = 22;
            this.tbxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(406, 173);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 20);
            this.label21.TabIndex = 23;
            this.label21.Text = "转账金额：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(141, 171);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(79, 20);
            this.label20.TabIndex = 8;
            this.label20.Text = "选择币种：";
            // 
            // cmbxTokenType
            // 
            this.cmbxTokenType.FormattingEnabled = true;
            this.cmbxTokenType.Items.AddRange(new object[] {
            "ZORO",
            "BCT",
            "BCS"});
            this.cmbxTokenType.Location = new System.Drawing.Point(226, 170);
            this.cmbxTokenType.Name = "cmbxTokenType";
            this.cmbxTokenType.Size = new System.Drawing.Size(163, 27);
            this.cmbxTokenType.TabIndex = 7;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(141, 229);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 20);
            this.label18.TabIndex = 6;
            this.label18.Text = "接收地址：";
            // 
            // tbxTargetAddress
            // 
            this.tbxTargetAddress.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbxTargetAddress.Location = new System.Drawing.Point(226, 226);
            this.tbxTargetAddress.Name = "tbxTargetAddress";
            this.tbxTargetAddress.Size = new System.Drawing.Size(450, 25);
            this.tbxTargetAddress.TabIndex = 5;
            // 
            // transAccountFrm
            // 
            this.transAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.transAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.transAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.transAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.transAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.transAccountFrm.Name = "transAccountFrm";
            this.transAccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.transAccountFrm.Size = new System.Drawing.Size(842, 117);
            this.transAccountFrm.TabIndex = 28;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label35);
            this.tabPage5.Controls.Add(this.tbxNep5GasPrice);
            this.tabPage5.Controls.Add(this.tbxMyAddress);
            this.tabPage5.Controls.Add(this.label34);
            this.tabPage5.Controls.Add(this.btnNep5Refresh);
            this.tabPage5.Controls.Add(this.lblNep5Balance);
            this.tabPage5.Controls.Add(this.label33);
            this.tabPage5.Controls.Add(this.tbxNep5Hash);
            this.tabPage5.Controls.Add(this.label31);
            this.tabPage5.Controls.Add(this.btnNep5Cancel);
            this.tabPage5.Controls.Add(this.btnNep5SendTran);
            this.tabPage5.Controls.Add(this.label19);
            this.tabPage5.Controls.Add(this.rtbxNep5Result);
            this.tabPage5.Controls.Add(this.tbxNep5Value);
            this.tabPage5.Controls.Add(this.label24);
            this.tabPage5.Controls.Add(this.label32);
            this.tabPage5.Controls.Add(this.tbxNep5ToAddress);
            this.tabPage5.Controls.Add(this.nep5AccountFrm);
            this.tabPage5.Location = new System.Drawing.Point(4, 28);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(848, 508);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Nep5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(328, 279);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(80, 20);
            this.label35.TabIndex = 58;
            this.label35.Text = "GasPrice：";
            // 
            // tbxNep5GasPrice
            // 
            this.tbxNep5GasPrice.Location = new System.Drawing.Point(411, 274);
            this.tbxNep5GasPrice.Name = "tbxNep5GasPrice";
            this.tbxNep5GasPrice.Size = new System.Drawing.Size(77, 25);
            this.tbxNep5GasPrice.TabIndex = 57;
            this.tbxNep5GasPrice.Text = "0.0001";
            this.tbxNep5GasPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // tbxMyAddress
            // 
            this.tbxMyAddress.Location = new System.Drawing.Point(124, 186);
            this.tbxMyAddress.Name = "tbxMyAddress";
            this.tbxMyAddress.Size = new System.Drawing.Size(393, 25);
            this.tbxMyAddress.TabIndex = 53;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(41, 189);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(79, 20);
            this.label34.TabIndex = 54;
            this.label34.Text = "查询地址：";
            // 
            // btnNep5Refresh
            // 
            this.btnNep5Refresh.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnNep5Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnNep5Refresh.FlatAppearance.BorderSize = 0;
            this.btnNep5Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNep5Refresh.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNep5Refresh.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNep5Refresh.Location = new System.Drawing.Point(772, 181);
            this.btnNep5Refresh.Name = "btnNep5Refresh";
            this.btnNep5Refresh.Size = new System.Drawing.Size(47, 29);
            this.btnNep5Refresh.TabIndex = 52;
            this.btnNep5Refresh.Text = "刷新";
            this.btnNep5Refresh.UseVisualStyleBackColor = false;
            this.btnNep5Refresh.Click += new System.EventHandler(this.btnNep5Refresh_Click);
            // 
            // lblNep5Balance
            // 
            this.lblNep5Balance.AutoSize = true;
            this.lblNep5Balance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblNep5Balance.Location = new System.Drawing.Point(587, 186);
            this.lblNep5Balance.Name = "lblNep5Balance";
            this.lblNep5Balance.Size = new System.Drawing.Size(140, 20);
            this.lblNep5Balance.TabIndex = 51;
            this.lblNep5Balance.Text = "00000000.00000000";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(531, 186);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(55, 20);
            this.label33.TabIndex = 50;
            this.label33.Text = "余 额：";
            // 
            // tbxNep5Hash
            // 
            this.tbxNep5Hash.Location = new System.Drawing.Point(124, 142);
            this.tbxNep5Hash.Name = "tbxNep5Hash";
            this.tbxNep5Hash.Size = new System.Drawing.Size(393, 25);
            this.tbxNep5Hash.TabIndex = 48;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(41, 145);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(85, 20);
            this.label31.TabIndex = 49;
            this.label31.Text = "合约Hash：";
            // 
            // btnNep5Cancel
            // 
            this.btnNep5Cancel.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnNep5Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnNep5Cancel.FlatAppearance.BorderSize = 0;
            this.btnNep5Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNep5Cancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNep5Cancel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNep5Cancel.Location = new System.Drawing.Point(720, 277);
            this.btnNep5Cancel.Name = "btnNep5Cancel";
            this.btnNep5Cancel.Size = new System.Drawing.Size(99, 29);
            this.btnNep5Cancel.TabIndex = 47;
            this.btnNep5Cancel.Text = "取消";
            this.btnNep5Cancel.UseVisualStyleBackColor = false;
            // 
            // btnNep5SendTran
            // 
            this.btnNep5SendTran.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnNep5SendTran.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnNep5SendTran.FlatAppearance.BorderSize = 0;
            this.btnNep5SendTran.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNep5SendTran.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNep5SendTran.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNep5SendTran.Location = new System.Drawing.Point(547, 276);
            this.btnNep5SendTran.Name = "btnNep5SendTran";
            this.btnNep5SendTran.Size = new System.Drawing.Size(99, 29);
            this.btnNep5SendTran.TabIndex = 46;
            this.btnNep5SendTran.Text = "发送";
            this.btnNep5SendTran.UseVisualStyleBackColor = false;
            this.btnNep5SendTran.Click += new System.EventHandler(this.btnNep5SendTran_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(41, 354);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 20);
            this.label19.TabIndex = 45;
            this.label19.Text = "返回结果：";
            // 
            // rtbxNep5Result
            // 
            this.rtbxNep5Result.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxNep5Result.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxNep5Result.Location = new System.Drawing.Point(45, 387);
            this.rtbxNep5Result.Name = "rtbxNep5Result";
            this.rtbxNep5Result.ReadOnly = true;
            this.rtbxNep5Result.Size = new System.Drawing.Size(748, 84);
            this.rtbxNep5Result.TabIndex = 44;
            this.rtbxNep5Result.Text = "";
            // 
            // tbxNep5Value
            // 
            this.tbxNep5Value.Location = new System.Drawing.Point(124, 274);
            this.tbxNep5Value.Name = "tbxNep5Value";
            this.tbxNep5Value.Size = new System.Drawing.Size(171, 25);
            this.tbxNep5Value.TabIndex = 42;
            this.tbxNep5Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(42, 277);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(79, 20);
            this.label24.TabIndex = 43;
            this.label24.Text = "转账金额：";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(42, 232);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(79, 20);
            this.label32.TabIndex = 39;
            this.label32.Text = "接收地址：";
            // 
            // tbxNep5ToAddress
            // 
            this.tbxNep5ToAddress.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbxNep5ToAddress.Location = new System.Drawing.Point(124, 229);
            this.tbxNep5ToAddress.Name = "tbxNep5ToAddress";
            this.tbxNep5ToAddress.Size = new System.Drawing.Size(393, 25);
            this.tbxNep5ToAddress.TabIndex = 38;
            // 
            // nep5AccountFrm
            // 
            this.nep5AccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.nep5AccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.nep5AccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nep5AccountFrm.Location = new System.Drawing.Point(3, 3);
            this.nep5AccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nep5AccountFrm.Name = "nep5AccountFrm";
            this.nep5AccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.nep5AccountFrm.Size = new System.Drawing.Size(842, 107);
            this.nep5AccountFrm.TabIndex = 0;
            // 
            // tbcMutiTrans
            // 
            this.tbcMutiTrans.Controls.Add(this.label26);
            this.tbcMutiTrans.Controls.Add(this.tbxMutiSigRpcUrl);
            this.tbcMutiTrans.Controls.Add(this.btcMutiSigCancel);
            this.tbcMutiTrans.Controls.Add(this.btnMutiSigSendTran);
            this.tbcMutiTrans.Controls.Add(this.label36);
            this.tbcMutiTrans.Controls.Add(this.rtbxMutiSigResult);
            this.tbcMutiTrans.Controls.Add(this.tbxMutiSigValue);
            this.tbcMutiTrans.Controls.Add(this.label37);
            this.tbcMutiTrans.Controls.Add(this.label38);
            this.tbcMutiTrans.Controls.Add(this.cmbMutiSigCoinType);
            this.tbcMutiTrans.Controls.Add(this.label39);
            this.tbcMutiTrans.Controls.Add(this.tbxMutiSigRecAddress);
            this.tbcMutiTrans.Controls.Add(this.label40);
            this.tbcMutiTrans.Controls.Add(this.rtbxMutiSigWifs);
            this.tbcMutiTrans.Location = new System.Drawing.Point(4, 28);
            this.tbcMutiTrans.Name = "tbcMutiTrans";
            this.tbcMutiTrans.Padding = new System.Windows.Forms.Padding(3);
            this.tbcMutiTrans.Size = new System.Drawing.Size(848, 508);
            this.tbcMutiTrans.TabIndex = 5;
            this.tbcMutiTrans.Text = "多签交易";
            this.tbcMutiTrans.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(46, 22);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(82, 20);
            this.label26.TabIndex = 51;
            this.label26.Text = "RPC地址 ：";
            // 
            // tbxMutiSigRpcUrl
            // 
            this.tbxMutiSigRpcUrl.BackColor = System.Drawing.Color.AliceBlue;
            this.tbxMutiSigRpcUrl.Location = new System.Drawing.Point(131, 20);
            this.tbxMutiSigRpcUrl.Name = "tbxMutiSigRpcUrl";
            this.tbxMutiSigRpcUrl.Size = new System.Drawing.Size(391, 25);
            this.tbxMutiSigRpcUrl.TabIndex = 50;
            this.tbxMutiSigRpcUrl.Text = "http://127.0.0.1:20333";
            // 
            // btcMutiSigCancel
            // 
            this.btcMutiSigCancel.BackColor = System.Drawing.SystemColors.Highlight;
            this.btcMutiSigCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btcMutiSigCancel.FlatAppearance.BorderSize = 0;
            this.btcMutiSigCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btcMutiSigCancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btcMutiSigCancel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btcMutiSigCancel.Location = new System.Drawing.Point(461, 315);
            this.btcMutiSigCancel.Name = "btcMutiSigCancel";
            this.btcMutiSigCancel.Size = new System.Drawing.Size(99, 29);
            this.btcMutiSigCancel.TabIndex = 49;
            this.btcMutiSigCancel.Text = "取消";
            this.btcMutiSigCancel.UseVisualStyleBackColor = false;
            // 
            // btnMutiSigSendTran
            // 
            this.btnMutiSigSendTran.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnMutiSigSendTran.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMutiSigSendTran.FlatAppearance.BorderSize = 0;
            this.btnMutiSigSendTran.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMutiSigSendTran.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMutiSigSendTran.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnMutiSigSendTran.Location = new System.Drawing.Point(288, 314);
            this.btnMutiSigSendTran.Name = "btnMutiSigSendTran";
            this.btnMutiSigSendTran.Size = new System.Drawing.Size(99, 29);
            this.btnMutiSigSendTran.TabIndex = 48;
            this.btnMutiSigSendTran.Text = "发送";
            this.btnMutiSigSendTran.UseVisualStyleBackColor = false;
            this.btnMutiSigSendTran.Click += new System.EventHandler(this.btnMutiSigSendTran_Click);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(47, 368);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(79, 20);
            this.label36.TabIndex = 47;
            this.label36.Text = "返回结果：";
            // 
            // rtbxMutiSigResult
            // 
            this.rtbxMutiSigResult.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxMutiSigResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxMutiSigResult.Location = new System.Drawing.Point(51, 400);
            this.rtbxMutiSigResult.Name = "rtbxMutiSigResult";
            this.rtbxMutiSigResult.ReadOnly = true;
            this.rtbxMutiSigResult.Size = new System.Drawing.Size(748, 74);
            this.rtbxMutiSigResult.TabIndex = 46;
            this.rtbxMutiSigResult.Text = "";
            // 
            // tbxMutiSigValue
            // 
            this.tbxMutiSigValue.Location = new System.Drawing.Point(489, 206);
            this.tbxMutiSigValue.Name = "tbxMutiSigValue";
            this.tbxMutiSigValue.Size = new System.Drawing.Size(185, 25);
            this.tbxMutiSigValue.TabIndex = 44;
            this.tbxMutiSigValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(404, 209);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(79, 20);
            this.label37.TabIndex = 45;
            this.label37.Text = "转账金额：";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(139, 207);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(79, 20);
            this.label38.TabIndex = 43;
            this.label38.Text = "选择币种：";
            // 
            // cmbMutiSigCoinType
            // 
            this.cmbMutiSigCoinType.FormattingEnabled = true;
            this.cmbMutiSigCoinType.Items.AddRange(new object[] {
            "ZORO",
            "BCT"});
            this.cmbMutiSigCoinType.Location = new System.Drawing.Point(224, 206);
            this.cmbMutiSigCoinType.Name = "cmbMutiSigCoinType";
            this.cmbMutiSigCoinType.Size = new System.Drawing.Size(163, 27);
            this.cmbMutiSigCoinType.TabIndex = 42;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(139, 265);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(79, 20);
            this.label39.TabIndex = 41;
            this.label39.Text = "接收地址：";
            // 
            // tbxMutiSigRecAddress
            // 
            this.tbxMutiSigRecAddress.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbxMutiSigRecAddress.Location = new System.Drawing.Point(224, 262);
            this.tbxMutiSigRecAddress.Name = "tbxMutiSigRecAddress";
            this.tbxMutiSigRecAddress.Size = new System.Drawing.Size(450, 25);
            this.tbxMutiSigRecAddress.TabIndex = 40;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(45, 56);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(145, 20);
            this.label40.TabIndex = 39;
            this.label40.Text = "多签 Wif (用 ; 分隔)：";
            // 
            // rtbxMutiSigWifs
            // 
            this.rtbxMutiSigWifs.BackColor = System.Drawing.Color.Azure;
            this.rtbxMutiSigWifs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxMutiSigWifs.Location = new System.Drawing.Point(49, 86);
            this.rtbxMutiSigWifs.Name = "rtbxMutiSigWifs";
            this.rtbxMutiSigWifs.Size = new System.Drawing.Size(758, 98);
            this.rtbxMutiSigWifs.TabIndex = 38;
            this.rtbxMutiSigWifs.Text = "";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label42);
            this.tabPage4.Controls.Add(this.tbxCallMethod);
            this.tabPage4.Controls.Add(this.btnCallEstimateGas);
            this.tabPage4.Controls.Add(this.label27);
            this.tabPage4.Controls.Add(this.tbxCallGasPrice);
            this.tabPage4.Controls.Add(this.label28);
            this.tabPage4.Controls.Add(this.tbxCallGasLimit);
            this.tabPage4.Controls.Add(this.label29);
            this.tabPage4.Controls.Add(this.tbxCallInterface);
            this.tabPage4.Controls.Add(this.label30);
            this.tabPage4.Controls.Add(this.rtbxCallResult);
            this.tabPage4.Controls.Add(this.btnCallSendRaw);
            this.tabPage4.Controls.Add(this.btnCallInvoke);
            this.tabPage4.Controls.Add(this.label41);
            this.tabPage4.Controls.Add(this.rtbxCallParams);
            this.tabPage4.Controls.Add(this.otherCallAccountFrm);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(848, 508);
            this.tabPage4.TabIndex = 6;
            this.tabPage4.Text = "SysCall";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(335, 143);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(79, 20);
            this.label42.TabIndex = 50;
            this.label42.Text = "调用方法：";
            // 
            // tbxCallMethod
            // 
            this.tbxCallMethod.Location = new System.Drawing.Point(416, 141);
            this.tbxCallMethod.Name = "tbxCallMethod";
            this.tbxCallMethod.Size = new System.Drawing.Size(190, 25);
            this.tbxCallMethod.TabIndex = 49;
            // 
            // btnCallEstimateGas
            // 
            this.btnCallEstimateGas.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCallEstimateGas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCallEstimateGas.FlatAppearance.BorderSize = 0;
            this.btnCallEstimateGas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallEstimateGas.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCallEstimateGas.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCallEstimateGas.Location = new System.Drawing.Point(435, 309);
            this.btnCallEstimateGas.Name = "btnCallEstimateGas";
            this.btnCallEstimateGas.Size = new System.Drawing.Size(106, 29);
            this.btnCallEstimateGas.TabIndex = 48;
            this.btnCallEstimateGas.Text = "EstimateGas";
            this.btnCallEstimateGas.UseVisualStyleBackColor = false;
            this.btnCallEstimateGas.Click += new System.EventHandler(this.btnCallEstimateGas_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(223, 316);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(80, 20);
            this.label27.TabIndex = 47;
            this.label27.Text = "GasPrice：";
            // 
            // tbxCallGasPrice
            // 
            this.tbxCallGasPrice.Location = new System.Drawing.Point(308, 311);
            this.tbxCallGasPrice.Name = "tbxCallGasPrice";
            this.tbxCallGasPrice.Size = new System.Drawing.Size(77, 25);
            this.tbxCallGasPrice.TabIndex = 46;
            this.tbxCallGasPrice.Text = "0.0001";
            this.tbxCallGasPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(38, 316);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(80, 20);
            this.label28.TabIndex = 45;
            this.label28.Text = "GasLimit：";
            // 
            // tbxCallGasLimit
            // 
            this.tbxCallGasLimit.Location = new System.Drawing.Point(123, 313);
            this.tbxCallGasLimit.Name = "tbxCallGasLimit";
            this.tbxCallGasLimit.Size = new System.Drawing.Size(93, 25);
            this.tbxCallGasLimit.TabIndex = 44;
            this.tbxCallGasLimit.Text = "1000";
            this.tbxCallGasLimit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxOnlyNumber_KeyPress);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(40, 144);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 20);
            this.label29.TabIndex = 43;
            this.label29.Text = "调用接口：";
            // 
            // tbxCallInterface
            // 
            this.tbxCallInterface.Location = new System.Drawing.Point(123, 142);
            this.tbxCallInterface.Name = "tbxCallInterface";
            this.tbxCallInterface.Size = new System.Drawing.Size(190, 25);
            this.tbxCallInterface.TabIndex = 42;
            this.tbxCallInterface.Text = "Zoro.NativeNEP5.Call";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(38, 350);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(79, 20);
            this.label30.TabIndex = 41;
            this.label30.Text = "返回结果：";
            // 
            // rtbxCallResult
            // 
            this.rtbxCallResult.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxCallResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxCallResult.Location = new System.Drawing.Point(43, 382);
            this.rtbxCallResult.Name = "rtbxCallResult";
            this.rtbxCallResult.ReadOnly = true;
            this.rtbxCallResult.Size = new System.Drawing.Size(758, 104);
            this.rtbxCallResult.TabIndex = 40;
            this.rtbxCallResult.Text = "";
            // 
            // btnCallSendRaw
            // 
            this.btnCallSendRaw.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCallSendRaw.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCallSendRaw.FlatAppearance.BorderSize = 0;
            this.btnCallSendRaw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallSendRaw.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCallSendRaw.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCallSendRaw.Location = new System.Drawing.Point(702, 309);
            this.btnCallSendRaw.Name = "btnCallSendRaw";
            this.btnCallSendRaw.Size = new System.Drawing.Size(99, 29);
            this.btnCallSendRaw.TabIndex = 39;
            this.btnCallSendRaw.Text = "SendRaw";
            this.btnCallSendRaw.UseVisualStyleBackColor = false;
            this.btnCallSendRaw.Click += new System.EventHandler(this.btnCallSendRaw_Click);
            // 
            // btnCallInvoke
            // 
            this.btnCallInvoke.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCallInvoke.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCallInvoke.FlatAppearance.BorderSize = 0;
            this.btnCallInvoke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallInvoke.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCallInvoke.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCallInvoke.Location = new System.Drawing.Point(573, 309);
            this.btnCallInvoke.Name = "btnCallInvoke";
            this.btnCallInvoke.Size = new System.Drawing.Size(99, 29);
            this.btnCallInvoke.TabIndex = 38;
            this.btnCallInvoke.Text = "Invoke";
            this.btnCallInvoke.UseVisualStyleBackColor = false;
            this.btnCallInvoke.Click += new System.EventHandler(this.btnCallInvoke_Click);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(38, 182);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(198, 20);
            this.label41.TabIndex = 37;
            this.label41.Text = "调用参数(多个参数用 ; 分隔)：";
            // 
            // rtbxCallParams
            // 
            this.rtbxCallParams.BackColor = System.Drawing.Color.Azure;
            this.rtbxCallParams.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxCallParams.Location = new System.Drawing.Point(43, 214);
            this.rtbxCallParams.Name = "rtbxCallParams";
            this.rtbxCallParams.Size = new System.Drawing.Size(758, 80);
            this.rtbxCallParams.TabIndex = 34;
            this.rtbxCallParams.Text = "";
            // 
            // otherCallAccountFrm
            // 
            this.otherCallAccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.otherCallAccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.otherCallAccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.otherCallAccountFrm.Location = new System.Drawing.Point(3, 3);
            this.otherCallAccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.otherCallAccountFrm.Name = "otherCallAccountFrm";
            this.otherCallAccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.otherCallAccountFrm.Size = new System.Drawing.Size(842, 118);
            this.otherCallAccountFrm.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label49);
            this.tabPage6.Controls.Add(this.label43);
            this.tabPage6.Controls.Add(this.BatchGasPrice);
            this.tabPage6.Controls.Add(this.label44);
            this.tabPage6.Controls.Add(this.BatchGasLimit);
            this.tabPage6.Controls.Add(this.batchCancel);
            this.tabPage6.Controls.Add(this.BatchSend);
            this.tabPage6.Controls.Add(this.label45);
            this.tabPage6.Controls.Add(this.BatchResult);
            this.tabPage6.Controls.Add(this.BatchTbxValue);
            this.tabPage6.Controls.Add(this.label46);
            this.tabPage6.Controls.Add(this.label47);
            this.tabPage6.Controls.Add(this.BatchCbxCoinType);
            this.tabPage6.Controls.Add(this.label48);
            this.tabPage6.Controls.Add(this.BatchToAddress);
            this.tabPage6.Controls.Add(this.BatchaccountFrm);
            this.tabPage6.Location = new System.Drawing.Point(4, 28);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(848, 508);
            this.tabPage6.TabIndex = 7;
            this.tabPage6.Text = "批量转账";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // label49
            // 
            this.label49.AllowDrop = true;
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(110, 205);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(131, 20);
            this.label49.TabIndex = 37;
            this.label49.Text = "(多个用换行分隔)：";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(241, 322);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(80, 20);
            this.label43.TabIndex = 36;
            this.label43.Text = "GasPrice：";
            // 
            // BatchGasPrice
            // 
            this.BatchGasPrice.Location = new System.Drawing.Point(324, 317);
            this.BatchGasPrice.Name = "BatchGasPrice";
            this.BatchGasPrice.Size = new System.Drawing.Size(77, 25);
            this.BatchGasPrice.TabIndex = 35;
            this.BatchGasPrice.Text = "0.0001";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(56, 322);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(80, 20);
            this.label44.TabIndex = 34;
            this.label44.Text = "GasLimit：";
            // 
            // BatchGasLimit
            // 
            this.BatchGasLimit.Location = new System.Drawing.Point(138, 319);
            this.BatchGasLimit.Name = "BatchGasLimit";
            this.BatchGasLimit.Size = new System.Drawing.Size(93, 25);
            this.BatchGasLimit.TabIndex = 33;
            this.BatchGasLimit.Text = "000";
            // 
            // batchCancel
            // 
            this.batchCancel.BackColor = System.Drawing.SystemColors.Highlight;
            this.batchCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.batchCancel.FlatAppearance.BorderSize = 0;
            this.batchCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.batchCancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.batchCancel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.batchCancel.Location = new System.Drawing.Point(696, 317);
            this.batchCancel.Name = "batchCancel";
            this.batchCancel.Size = new System.Drawing.Size(99, 29);
            this.batchCancel.TabIndex = 27;
            this.batchCancel.Text = "取消";
            this.batchCancel.UseVisualStyleBackColor = false;
            // 
            // BatchSend
            // 
            this.BatchSend.BackColor = System.Drawing.SystemColors.Highlight;
            this.BatchSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BatchSend.FlatAppearance.BorderSize = 0;
            this.BatchSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BatchSend.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BatchSend.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.BatchSend.Location = new System.Drawing.Point(529, 316);
            this.BatchSend.Name = "BatchSend";
            this.BatchSend.Size = new System.Drawing.Size(99, 29);
            this.BatchSend.TabIndex = 26;
            this.BatchSend.Text = "发送";
            this.BatchSend.UseVisualStyleBackColor = false;
            this.BatchSend.Click += new System.EventHandler(this.BatchSend_Click);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(44, 365);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(79, 20);
            this.label45.TabIndex = 25;
            this.label45.Text = "返回结果：";
            // 
            // BatchResult
            // 
            this.BatchResult.BackColor = System.Drawing.Color.AliceBlue;
            this.BatchResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BatchResult.Location = new System.Drawing.Point(49, 397);
            this.BatchResult.Name = "BatchResult";
            this.BatchResult.ReadOnly = true;
            this.BatchResult.Size = new System.Drawing.Size(748, 77);
            this.BatchResult.TabIndex = 24;
            this.BatchResult.Text = "";
            // 
            // BatchTbxValue
            // 
            this.BatchTbxValue.Location = new System.Drawing.Point(512, 131);
            this.BatchTbxValue.Name = "BatchTbxValue";
            this.BatchTbxValue.Size = new System.Drawing.Size(185, 25);
            this.BatchTbxValue.TabIndex = 22;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(427, 134);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(79, 20);
            this.label46.TabIndex = 23;
            this.label46.Text = "转账金额：";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(161, 133);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(79, 20);
            this.label47.TabIndex = 8;
            this.label47.Text = "选择币种：";
            // 
            // BatchCbxCoinType
            // 
            this.BatchCbxCoinType.FormattingEnabled = true;
            this.BatchCbxCoinType.Items.AddRange(new object[] {
            "ZORO",
            "BCT",
            "BCS"});
            this.BatchCbxCoinType.Location = new System.Drawing.Point(247, 131);
            this.BatchCbxCoinType.Name = "BatchCbxCoinType";
            this.BatchCbxCoinType.Size = new System.Drawing.Size(163, 27);
            this.BatchCbxCoinType.TabIndex = 7;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(161, 175);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(65, 20);
            this.label48.TabIndex = 6;
            this.label48.Text = "接收地址";
            // 
            // BatchToAddress
            // 
            this.BatchToAddress.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BatchToAddress.Location = new System.Drawing.Point(247, 172);
            this.BatchToAddress.Multiline = true;
            this.BatchToAddress.Name = "BatchToAddress";
            this.BatchToAddress.Size = new System.Drawing.Size(450, 123);
            this.BatchToAddress.TabIndex = 5;
            // 
            // BatchaccountFrm
            // 
            this.BatchaccountFrm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BatchaccountFrm.Dock = System.Windows.Forms.DockStyle.Top;
            this.BatchaccountFrm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BatchaccountFrm.Location = new System.Drawing.Point(3, 3);
            this.BatchaccountFrm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BatchaccountFrm.Name = "BatchaccountFrm";
            this.BatchaccountFrm.RpcUrl = "http://127.0.0.1:20332";
            this.BatchaccountFrm.Size = new System.Drawing.Size(842, 117);
            this.BatchaccountFrm.TabIndex = 28;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.tbxWithdrawAddress);
            this.tabPage7.Controls.Add(this.label56);
            this.tabPage7.Controls.Add(this.tbxLockerAddress);
            this.tabPage7.Controls.Add(this.cbxLockMethod);
            this.tabPage7.Controls.Add(this.label52);
            this.tabPage7.Controls.Add(this.label53);
            this.tabPage7.Controls.Add(this.rtbxLockReturnResult);
            this.tabPage7.Controls.Add(this.btnLockSendRaw);
            this.tabPage7.Controls.Add(this.btnLockInvoke);
            this.tabPage7.Controls.Add(this.label54);
            this.tabPage7.Controls.Add(this.label55);
            this.tabPage7.Controls.Add(this.tbxLockContract);
            this.tabPage7.Controls.Add(this.accountFrmLock);
            this.tabPage7.Location = new System.Drawing.Point(4, 28);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(848, 508);
            this.tabPage7.TabIndex = 8;
            this.tabPage7.Text = "锁仓操作";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tbxWithdrawAddress
            // 
            this.tbxWithdrawAddress.Location = new System.Drawing.Point(125, 246);
            this.tbxWithdrawAddress.Name = "tbxWithdrawAddress";
            this.tbxWithdrawAddress.Size = new System.Drawing.Size(391, 25);
            this.tbxWithdrawAddress.TabIndex = 52;
            this.tbxWithdrawAddress.Visible = false;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(39, 249);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(79, 20);
            this.label56.TabIndex = 51;
            this.label56.Text = "收款地址：";
            this.label56.Visible = false;
            // 
            // tbxLockerAddress
            // 
            this.tbxLockerAddress.Location = new System.Drawing.Point(125, 191);
            this.tbxLockerAddress.Name = "tbxLockerAddress";
            this.tbxLockerAddress.Size = new System.Drawing.Size(391, 25);
            this.tbxLockerAddress.TabIndex = 50;
            // 
            // cbxLockMethod
            // 
            this.cbxLockMethod.FormattingEnabled = true;
            this.cbxLockMethod.Items.AddRange(new object[] {
            "可解锁金额",
            "已锁仓金额",
            "上次解锁时间",
            "解锁时间间隔",
            "每次解锁金额",
            "解锁提现"});
            this.cbxLockMethod.Location = new System.Drawing.Point(620, 134);
            this.cbxLockMethod.Name = "cbxLockMethod";
            this.cbxLockMethod.Size = new System.Drawing.Size(163, 27);
            this.cbxLockMethod.TabIndex = 49;
            this.cbxLockMethod.SelectedIndexChanged += new System.EventHandler(this.CbxLockMethod_SelectedIndexChanged);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(546, 138);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(79, 20);
            this.label52.TabIndex = 43;
            this.label52.Text = "操作方法：";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(39, 335);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(79, 20);
            this.label53.TabIndex = 41;
            this.label53.Text = "返回结果：";
            // 
            // rtbxLockReturnResult
            // 
            this.rtbxLockReturnResult.BackColor = System.Drawing.Color.AliceBlue;
            this.rtbxLockReturnResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxLockReturnResult.Location = new System.Drawing.Point(44, 369);
            this.rtbxLockReturnResult.Name = "rtbxLockReturnResult";
            this.rtbxLockReturnResult.ReadOnly = true;
            this.rtbxLockReturnResult.Size = new System.Drawing.Size(758, 120);
            this.rtbxLockReturnResult.TabIndex = 40;
            this.rtbxLockReturnResult.Text = "";
            // 
            // btnLockSendRaw
            // 
            this.btnLockSendRaw.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnLockSendRaw.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLockSendRaw.FlatAppearance.BorderSize = 0;
            this.btnLockSendRaw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLockSendRaw.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLockSendRaw.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLockSendRaw.Location = new System.Drawing.Point(620, 240);
            this.btnLockSendRaw.Name = "btnLockSendRaw";
            this.btnLockSendRaw.Size = new System.Drawing.Size(99, 29);
            this.btnLockSendRaw.TabIndex = 39;
            this.btnLockSendRaw.Text = "提取";
            this.btnLockSendRaw.UseVisualStyleBackColor = false;
            this.btnLockSendRaw.Visible = false;
            this.btnLockSendRaw.Click += new System.EventHandler(this.BtnLockSendRaw_Click);
            // 
            // btnLockInvoke
            // 
            this.btnLockInvoke.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnLockInvoke.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLockInvoke.FlatAppearance.BorderSize = 0;
            this.btnLockInvoke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLockInvoke.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLockInvoke.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLockInvoke.Location = new System.Drawing.Point(620, 185);
            this.btnLockInvoke.Name = "btnLockInvoke";
            this.btnLockInvoke.Size = new System.Drawing.Size(99, 29);
            this.btnLockInvoke.TabIndex = 38;
            this.btnLockInvoke.Text = "查询";
            this.btnLockInvoke.UseVisualStyleBackColor = false;
            this.btnLockInvoke.Click += new System.EventHandler(this.BtnLockInvoke_Click);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(39, 194);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(79, 20);
            this.label54.TabIndex = 37;
            this.label54.Text = "锁仓地址：";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(39, 140);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(79, 20);
            this.label55.TabIndex = 36;
            this.label55.Text = "锁仓合约：";
            // 
            // tbxLockContract
            // 
            this.tbxLockContract.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbxLockContract.Location = new System.Drawing.Point(125, 137);
            this.tbxLockContract.Name = "tbxLockContract";
            this.tbxLockContract.ReadOnly = true;
            this.tbxLockContract.Size = new System.Drawing.Size(391, 25);
            this.tbxLockContract.TabIndex = 35;
            this.tbxLockContract.Text = "0x7c99961fb3c6c9bb8b8b8f4deab1c055755e78f8";
            // 
            // accountFrmLock
            // 
            this.accountFrmLock.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.accountFrmLock.Dock = System.Windows.Forms.DockStyle.Top;
            this.accountFrmLock.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.accountFrmLock.Location = new System.Drawing.Point(3, 3);
            this.accountFrmLock.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.accountFrmLock.Name = "accountFrmLock";
            this.accountFrmLock.RpcUrl = "http://127.0.0.1:20332";
            this.accountFrmLock.Size = new System.Drawing.Size(842, 115);
            this.accountFrmLock.TabIndex = 31;
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
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tbcMutiTrans.ResumeLayout(false);
            this.tbcMutiTrans.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
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
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbxGasLimit;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.RichTextBox rtbxPublishReturn;
        private AccountFrm publishAccountFrm;
        private AccountFrm invokeAccountFrm;
        private System.Windows.Forms.TabPage tabPage5;
        private AccountFrm nep5AccountFrm;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxGasPrice;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbxTransGasPrice;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbxTransGasLimit;
        private AccountFrm transAccountFrm;
        private System.Windows.Forms.Button btnEstimateGas;
        private System.Windows.Forms.TextBox tbxNep5Hash;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Button btnNep5Cancel;
        private System.Windows.Forms.Button btnNep5SendTran;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.RichTextBox rtbxNep5Result;
        private System.Windows.Forms.TextBox tbxNep5Value;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox tbxNep5ToAddress;
        private System.Windows.Forms.Label lblNep5Balance;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Button btnNep5Refresh;
        private System.Windows.Forms.TextBox tbxMyAddress;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox tbxNep5GasPrice;
        private System.Windows.Forms.TabPage tbcMutiTrans;
        private System.Windows.Forms.Button btcMutiSigCancel;
        private System.Windows.Forms.Button btnMutiSigSendTran;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.RichTextBox rtbxMutiSigResult;
        private System.Windows.Forms.TextBox tbxMutiSigValue;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.ComboBox cmbMutiSigCoinType;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox tbxMutiSigRecAddress;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.RichTextBox rtbxMutiSigWifs;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbxMutiSigRpcUrl;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnCallEstimateGas;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbxCallGasPrice;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbxCallGasLimit;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox tbxCallInterface;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.RichTextBox rtbxCallResult;
        private System.Windows.Forms.Button btnCallSendRaw;
        private System.Windows.Forms.Button btnCallInvoke;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.RichTextBox rtbxCallParams;
        private AccountFrm otherCallAccountFrm;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox tbxCallMethod;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox BatchGasPrice;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox BatchGasLimit;
        private System.Windows.Forms.Button batchCancel;
        private System.Windows.Forms.Button BatchSend;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.RichTextBox BatchResult;
        private System.Windows.Forms.TextBox BatchTbxValue;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.ComboBox BatchCbxCoinType;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.TextBox BatchToAddress;
        private AccountFrm BatchaccountFrm;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TextBox tbxWithdrawAddress;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.TextBox tbxLockerAddress;
        private System.Windows.Forms.ComboBox cbxLockMethod;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.RichTextBox rtbxLockReturnResult;
        private System.Windows.Forms.Button btnLockSendRaw;
        private System.Windows.Forms.Button btnLockInvoke;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.TextBox tbxLockContract;
        private AccountFrm accountFrmLock;
    }
}

