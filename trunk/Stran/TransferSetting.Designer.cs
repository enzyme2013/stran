﻿namespace Stran
{
	partial class TransferSetting
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
			this.comboBoxTargetVillage = new System.Windows.Forms.ComboBox();
			this.txtX = new System.Windows.Forms.TextBox();
			this.txtY = new System.Windows.Forms.TextBox();
			this.numericUpDownMechantCount = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelDetail = new System.Windows.Forms.TextBox();
			this.radioNormalTarget = new System.Windows.Forms.RadioButton();
			this.radioNormalMe = new System.Windows.Forms.RadioButton();
			this.radioNoNormal = new System.Windows.Forms.RadioButton();
			this.checkBoxNoCrop = new System.Windows.Forms.CheckBox();
			this.numericUpDownTransferCount = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.radioUniform = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMechantCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(217, 315);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 32);
			this.buttonCancel.TabIndex = 31;
			this.buttonCancel.Tag = "cancel";
			this.buttonCancel.Text = "取消";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(97, 315);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 32);
			this.buttonOK.TabIndex = 30;
			this.buttonOK.Tag = "ok";
			this.buttonOK.Text = "确定";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(12, 70);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(70, 22);
			this.numericUpDown1.TabIndex = 8;
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1234_ValueChanged);
			this.numericUpDown1.Enter += new System.EventHandler(this.numericUpDown1234_Enter);
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(12, 98);
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(70, 22);
			this.numericUpDown2.TabIndex = 9;
			this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown1234_ValueChanged);
			this.numericUpDown2.Enter += new System.EventHandler(this.numericUpDown1234_Enter);
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(12, 126);
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(70, 22);
			this.numericUpDown3.TabIndex = 10;
			this.numericUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown1234_ValueChanged);
			this.numericUpDown3.Enter += new System.EventHandler(this.numericUpDown1234_Enter);
			// 
			// numericUpDown4
			// 
			this.numericUpDown4.Location = new System.Drawing.Point(12, 154);
			this.numericUpDown4.Name = "numericUpDown4";
			this.numericUpDown4.Size = new System.Drawing.Size(70, 22);
			this.numericUpDown4.TabIndex = 11;
			this.numericUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown4.ValueChanged += new System.EventHandler(this.numericUpDown1234_ValueChanged);
			this.numericUpDown4.Enter += new System.EventHandler(this.numericUpDown1234_Enter);
			// 
			// comboBoxTargetVillage
			// 
			this.comboBoxTargetVillage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTargetVillage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxTargetVillage.FormattingEnabled = true;
			this.comboBoxTargetVillage.Items.AddRange(new object[] {
            "-"});
			this.comboBoxTargetVillage.Location = new System.Drawing.Point(12, 36);
			this.comboBoxTargetVillage.MaxDropDownItems = 12;
			this.comboBoxTargetVillage.Name = "comboBoxTargetVillage";
			this.comboBoxTargetVillage.Size = new System.Drawing.Size(370, 21);
			this.comboBoxTargetVillage.TabIndex = 0;
			this.comboBoxTargetVillage.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetVillage_SelectedIndexChanged);
			// 
			// txtX
			// 
			this.txtX.Location = new System.Drawing.Point(273, 106);
			this.txtX.Name = "txtX";
			this.txtX.Size = new System.Drawing.Size(50, 22);
			this.txtX.TabIndex = 13;
			// 
			// txtY
			// 
			this.txtY.Location = new System.Drawing.Point(332, 106);
			this.txtY.Name = "txtY";
			this.txtY.Size = new System.Drawing.Size(50, 22);
			this.txtY.TabIndex = 14;
			// 
			// numericUpDownMechantCount
			// 
			this.numericUpDownMechantCount.Location = new System.Drawing.Point(321, 134);
			this.numericUpDownMechantCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numericUpDownMechantCount.Name = "numericUpDownMechantCount";
			this.numericUpDownMechantCount.Size = new System.Drawing.Size(61, 22);
			this.numericUpDownMechantCount.TabIndex = 15;
			this.numericUpDownMechantCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numericUpDownMechantCount.ValueChanged += new System.EventHandler(this.numericUpDownMechantCount_ValueChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(227, 134);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 22);
			this.label1.TabIndex = 16;
			this.label1.Tag = "merchantcnt";
			this.label1.Text = "merchantcnt";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(246, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 14);
			this.label2.TabIndex = 17;
			this.label2.Tag = "coord";
			this.label2.Text = "label2";
			// 
			// labelDetail
			// 
			this.labelDetail.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDetail.Location = new System.Drawing.Point(12, 190);
			this.labelDetail.Multiline = true;
			this.labelDetail.Name = "labelDetail";
			this.labelDetail.ReadOnly = true;
			this.labelDetail.Size = new System.Drawing.Size(370, 119);
			this.labelDetail.TabIndex = 19;
			this.labelDetail.TabStop = false;
			// 
			// radioNormalTarget
			// 
			this.radioNormalTarget.AutoSize = true;
			this.radioNormalTarget.Checked = true;
			this.radioNormalTarget.Location = new System.Drawing.Point(115, 70);
			this.radioNormalTarget.Name = "radioNormalTarget";
			this.radioNormalTarget.Size = new System.Drawing.Size(96, 18);
			this.radioNormalTarget.TabIndex = 20;
			this.radioNormalTarget.TabStop = true;
			this.radioNormalTarget.Tag = "autonormalizetarget";
			this.radioNormalTarget.Text = "radioButton1";
			this.radioNormalTarget.UseVisualStyleBackColor = true;
			this.radioNormalTarget.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioNormalMe
			// 
			this.radioNormalMe.AutoSize = true;
			this.radioNormalMe.Location = new System.Drawing.Point(115, 94);
			this.radioNormalMe.Name = "radioNormalMe";
			this.radioNormalMe.Size = new System.Drawing.Size(96, 18);
			this.radioNormalMe.TabIndex = 21;
			this.radioNormalMe.TabStop = true;
			this.radioNormalMe.Tag = "autonormalizeme";
			this.radioNormalMe.Text = "radioButton1";
			this.radioNormalMe.UseVisualStyleBackColor = true;
			this.radioNormalMe.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioNoNormal
			// 
			this.radioNoNormal.AutoSize = true;
			this.radioNoNormal.Location = new System.Drawing.Point(115, 142);
			this.radioNoNormal.Name = "radioNoNormal";
			this.radioNoNormal.Size = new System.Drawing.Size(96, 18);
			this.radioNoNormal.TabIndex = 23;
			this.radioNoNormal.TabStop = true;
			this.radioNoNormal.Tag = "noautonormalize";
			this.radioNoNormal.Text = "radioButton2";
			this.radioNoNormal.UseVisualStyleBackColor = true;
			this.radioNoNormal.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// checkBoxNoCrop
			// 
			this.checkBoxNoCrop.AutoSize = true;
			this.checkBoxNoCrop.Location = new System.Drawing.Point(115, 166);
			this.checkBoxNoCrop.Name = "checkBoxNoCrop";
			this.checkBoxNoCrop.Size = new System.Drawing.Size(85, 18);
			this.checkBoxNoCrop.TabIndex = 24;
			this.checkBoxNoCrop.Tag = "nocrop";
			this.checkBoxNoCrop.Text = "checkBox1";
			this.checkBoxNoCrop.UseVisualStyleBackColor = true;
			this.checkBoxNoCrop.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// numericUpDownTransferCount
			// 
			this.numericUpDownTransferCount.Location = new System.Drawing.Point(321, 162);
			this.numericUpDownTransferCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownTransferCount.Name = "numericUpDownTransferCount";
			this.numericUpDownTransferCount.Size = new System.Drawing.Size(61, 22);
			this.numericUpDownTransferCount.TabIndex = 16;
			this.numericUpDownTransferCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numericUpDownTransferCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(230, 162);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 22);
			this.label3.TabIndex = 26;
			this.label3.Tag = "transfercnt";
			this.label3.Text = "transfercnt";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioUniform
			// 
			this.radioUniform.AutoSize = true;
			this.radioUniform.Location = new System.Drawing.Point(115, 118);
			this.radioUniform.Name = "radioUniform";
			this.radioUniform.Size = new System.Drawing.Size(126, 18);
			this.radioUniform.TabIndex = 22;
			this.radioUniform.TabStop = true;
			this.radioUniform.Tag = "uniformdistribution";
			this.radioUniform.Text = "uniformdistribution";
			this.radioUniform.UseVisualStyleBackColor = true;
			// 
			// TransferSetting
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(394, 370);
			this.Controls.Add(this.radioUniform);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownTransferCount);
			this.Controls.Add(this.checkBoxNoCrop);
			this.Controls.Add(this.radioNoNormal);
			this.Controls.Add(this.radioNormalMe);
			this.Controls.Add(this.radioNormalTarget);
			this.Controls.Add(this.labelDetail);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDownMechantCount);
			this.Controls.Add(this.txtY);
			this.Controls.Add(this.txtX);
			this.Controls.Add(this.comboBoxTargetVillage);
			this.Controls.Add(this.numericUpDown4);
			this.Controls.Add(this.numericUpDown3);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TransferSetting";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Tag = "transfer";
			this.Text = "TransferSetting";
			this.Load += new System.EventHandler(this.TransferSetting_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMechantCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.NumericUpDown numericUpDown3;
		private System.Windows.Forms.NumericUpDown numericUpDown4;
		private System.Windows.Forms.ComboBox comboBoxTargetVillage;
		private System.Windows.Forms.TextBox txtX;
		private System.Windows.Forms.TextBox txtY;
		private System.Windows.Forms.NumericUpDown numericUpDownMechantCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox labelDetail;
		private System.Windows.Forms.RadioButton radioNormalTarget;
		private System.Windows.Forms.RadioButton radioNormalMe;
		private System.Windows.Forms.RadioButton radioUniform;
		private System.Windows.Forms.RadioButton radioNoNormal;
		private System.Windows.Forms.CheckBox checkBoxNoCrop;
		private System.Windows.Forms.NumericUpDown numericUpDownTransferCount;
		private System.Windows.Forms.Label label3;
	}
}