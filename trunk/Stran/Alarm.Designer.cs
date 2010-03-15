namespace Stran
{
    partial class Alarm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpTargets = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.tbTrust = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRecv = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.grpTargets.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(21, 30);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(43, 14);
        	this.label1.TabIndex = 0;
        	this.label1.Tag = "mailuser";
        	this.label1.Text = "发件人";
        	// 
        	// tbFrom
        	// 
        	this.tbFrom.Location = new System.Drawing.Point(115, 22);
        	this.tbFrom.Name = "tbFrom";
        	this.tbFrom.Size = new System.Drawing.Size(144, 22);
        	this.tbFrom.TabIndex = 1;
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(267, 27);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(18, 14);
        	this.label2.TabIndex = 2;
        	this.label2.Text = "@";
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Items.AddRange(new object[] {
        	        	        	"gmail.com"});
        	this.comboBox1.Location = new System.Drawing.Point(287, 23);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(91, 22);
        	this.comboBox1.TabIndex = 2;
        	this.comboBox1.Text = "gmail.com";
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(21, 66);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(31, 14);
        	this.label3.TabIndex = 4;
        	this.label3.Tag = "mailpass";
        	this.label3.Text = "密码";
        	// 
        	// tbPass
        	// 
        	this.tbPass.Location = new System.Drawing.Point(115, 63);
        	this.tbPass.Name = "tbPass";
        	this.tbPass.PasswordChar = '*';
        	this.tbPass.Size = new System.Drawing.Size(144, 22);
        	this.tbPass.TabIndex = 5;
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.tbPort);
        	this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbServer);
            this.groupBox1.Controls.Add(this.label5);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Controls.Add(this.label3);
        	this.groupBox1.Controls.Add(this.tbPass);
        	this.groupBox1.Controls.Add(this.tbFrom);
        	this.groupBox1.Controls.Add(this.comboBox1);
        	this.groupBox1.Controls.Add(this.label2);
        	this.groupBox1.Location = new System.Drawing.Point(14, 14);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(418, 143);
        	this.groupBox1.TabIndex = 7;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Tag = "mailsender";
        	this.groupBox1.Text = "邮件服务器";
        	// 
        	// tbPort
        	// 
        	this.tbPort.Location = new System.Drawing.Point(322, 99);
        	this.tbPort.Name = "tbPort";
        	this.tbPort.ReadOnly = true;
        	this.tbPort.Size = new System.Drawing.Size(56, 22);
        	this.tbPort.TabIndex = 9;
        	this.tbPort.Text = "587";
        	// 
        	// label6
        	// 
        	this.label6.AutoSize = true;
        	this.label6.Location = new System.Drawing.Point(267, 103);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(31, 14);
        	this.label6.TabIndex = 8;
        	this.label6.Tag = "serverport";
        	this.label6.Text = "端口";
        	// 
        	// tbServer
        	// 
        	this.tbServer.Location = new System.Drawing.Point(115, 99);
        	this.tbServer.Name = "tbServer";
        	this.tbServer.ReadOnly = true;
        	this.tbServer.Size = new System.Drawing.Size(144, 22);
        	this.tbServer.TabIndex = 7;
        	this.tbServer.Text = "smtp.gmail.com";
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(23, 103);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(43, 14);
        	this.label5.TabIndex = 6;
        	this.label5.Tag = "smtpserver";
        	this.label5.Text = "服务器";
        	// 
        	// grpTargets
        	// 
            this.grpTargets.Controls.Add(this.label9);
        	this.grpTargets.Controls.Add(this.numericUpDown1);
        	this.grpTargets.Controls.Add(this.label8);
        	this.grpTargets.Controls.Add(this.tbTrust);
        	this.grpTargets.Controls.Add(this.label7);
        	this.grpTargets.Controls.Add(this.label4);
        	this.grpTargets.Controls.Add(this.tbRecv);
        	this.grpTargets.Location = new System.Drawing.Point(14, 181);
        	this.grpTargets.Name = "grpTargets";
        	this.grpTargets.Size = new System.Drawing.Size(418, 192);
        	this.grpTargets.TabIndex = 69;
        	this.grpTargets.TabStop = false;
        	this.grpTargets.Tag = "recvsettings";
        	this.grpTargets.Text = "发送设置";
        	// 
        	// label9
        	// 
        	this.label9.AutoSize = true;
        	this.label9.Location = new System.Drawing.Point(153, 156);
        	this.label9.Name = "label9";
        	this.label9.Size = new System.Drawing.Size(31, 14);
        	this.label9.TabIndex = 68;
        	this.label9.Tag = "Minutes";
        	this.label9.Text = "分钟";
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Location = new System.Drawing.Point(7, 154);
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(140, 22);
        	this.numericUpDown1.TabIndex = 67;
        	// 
        	// label8
        	// 
        	this.label8.AutoSize = true;
        	this.label8.Location = new System.Drawing.Point(5, 136);
        	this.label8.Name = "label8";
        	this.label8.Size = new System.Drawing.Size(67, 14);
        	this.label8.TabIndex = 66;
        	this.label8.Tag = "ActionInterval";
        	this.label8.Text = "时间间隔：";
        	// 
        	// tbTrust
        	// 
        	this.tbTrust.Location = new System.Drawing.Point(7, 96);
        	this.tbTrust.Name = "tbTrust";
        	this.tbTrust.Size = new System.Drawing.Size(403, 22);
        	this.tbTrust.TabIndex = 65;
        	// 
        	// label7
        	// 
        	this.label7.AutoSize = true;
        	this.label7.Location = new System.Drawing.Point(8, 78);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(55, 14);
        	this.label7.TabIndex = 64;
        	this.label7.Tag = "trustfuluser";
        	this.label7.Text = "信任帐号";
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(8, 24);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(43, 14);
        	this.label4.TabIndex = 63;
        	this.label4.Tag = "recvaccounts";
        	this.label4.Text = "收件人";
        	// 
        	// tbRecv
        	// 
        	this.tbRecv.Location = new System.Drawing.Point(7, 45);
        	this.tbRecv.Name = "tbRecv";
        	this.tbRecv.Size = new System.Drawing.Size(403, 22);
        	this.tbRecv.TabIndex = 62;
        	// 
        	// btnCancel
        	// 
        	this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        	this.btnCancel.Location = new System.Drawing.Point(267, 392);
        	this.btnCancel.Name = "btnCancel";
        	this.btnCancel.Size = new System.Drawing.Size(93, 35);
        	this.btnCancel.TabIndex = 71;
        	this.btnCancel.Tag = "cancel";
        	this.btnCancel.Text = "cancel";
        	this.btnCancel.UseVisualStyleBackColor = true;
        	this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        	// 
        	// btnOk
        	// 
        	this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        	this.btnOk.Location = new System.Drawing.Point(97, 392);
        	this.btnOk.Name = "btnOk";
        	this.btnOk.Size = new System.Drawing.Size(93, 35);
        	this.btnOk.TabIndex = 70;
        	this.btnOk.Tag = "ok";
        	this.btnOk.Text = "ok";
        	this.btnOk.UseVisualStyleBackColor = true;
        	this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
        	// 
        	// Alarm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(443, 447);
        	this.Controls.Add(this.btnCancel);
        	this.Controls.Add(this.btnOk);
        	this.Controls.Add(this.grpTargets);
        	this.Controls.Add(this.groupBox1);
        	this.Font = new System.Drawing.Font("Tahoma", 9F);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "Alarm";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Tag = "CMBAlarm";
        	this.Text = "警报邮件";
        	this.Load += new System.EventHandler(this.Alarm_Load);
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
            this.grpTargets.ResumeLayout(false);
            this.grpTargets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.GroupBox grpTargets;
        private System.Windows.Forms.TextBox tbRecv;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTrust;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}