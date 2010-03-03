namespace Stran
{
	partial class ProduceTroopSetting
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
			if(disposing && (components != null))
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.buttonTiming = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownTransferCount = new System.Windows.Forms.NumericUpDown();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.buttonlimit = new System.Windows.Forms.Button();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelA = new System.Windows.Forms.Label();
			this.labelB = new System.Windows.Forms.Label();
			this.labelC = new System.Windows.Forms.Label();
			this.labelD = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 14;
			this.listBox1.Location = new System.Drawing.Point(14, 113);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(120, 200);
			this.listBox1.TabIndex = 0;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1SelectedIndexChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(202, 322);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 32);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Tag = "cancel";
			this.buttonCancel.Text = "取消";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(108, 322);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 32);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Tag = "ok";
			this.buttonOK.Text = "确定";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Increment = new decimal(new int[] {
									50,
									0,
									0,
									0});
			this.numericUpDown1.Location = new System.Drawing.Point(202, 138);
			this.numericUpDown1.Maximum = new decimal(new int[] {
									1000,
									0,
									0,
									0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(80, 22);
			this.numericUpDown1.TabIndex = 1;
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown1.ThousandsSeparator = true;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
			// 
			// buttonTiming
			// 
			this.buttonTiming.Location = new System.Drawing.Point(202, 281);
			this.buttonTiming.Name = "buttonTiming";
			this.buttonTiming.Size = new System.Drawing.Size(80, 32);
			this.buttonTiming.TabIndex = 6;
			this.buttonTiming.Tag = "Timing";
			this.buttonTiming.Text = "定时";
			this.buttonTiming.UseVisualStyleBackColor = true;
			this.buttonTiming.Click += new System.EventHandler(this.buttonTiming_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(140, 161);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(142, 22);
			this.label3.TabIndex = 10;
			this.label3.Tag = "transfercnt";
			this.label3.Text = "transfercnt";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownTransferCount
			// 
			this.numericUpDownTransferCount.Location = new System.Drawing.Point(221, 186);
			this.numericUpDownTransferCount.Maximum = new decimal(new int[] {
									50,
									0,
									0,
									0});
			this.numericUpDownTransferCount.Name = "numericUpDownTransferCount";
			this.numericUpDownTransferCount.Size = new System.Drawing.Size(61, 22);
			this.numericUpDownTransferCount.TabIndex = 2;
			this.numericUpDownTransferCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownTransferCount.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.numericUpDownTransferCount.ValueChanged += new System.EventHandler(this.NumericUpDownTransferCountValueChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(140, 252);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(97, 18);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Tag = "showallarmy";
			this.checkBox1.Text = "ShowAllArmy";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(140, 229);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(146, 18);
			this.checkBox2.TabIndex = 4;
			this.checkBox2.Tag = "Great";
			this.checkBox2.Text = "Great Barracks&&Stable";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// buttonlimit
			// 
			this.buttonlimit.Location = new System.Drawing.Point(14, 322);
			this.buttonlimit.Name = "buttonlimit";
			this.buttonlimit.Size = new System.Drawing.Size(80, 32);
			this.buttonlimit.TabIndex = 7;
			this.buttonlimit.Tag = "ResourceLimit";
			this.buttonlimit.Text = "资源限制";
			this.buttonlimit.UseVisualStyleBackColor = true;
			this.buttonlimit.Click += new System.EventHandler(this.buttonlimit_Click);
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(140, 205);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(68, 18);
			this.checkBox3.TabIndex = 3;
			this.checkBox3.Tag = "Settlers";
			this.checkBox3.Text = "Settlers";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(140, 113);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 22);
			this.label1.TabIndex = 11;
			this.label1.Tag = "troopamount";
			this.label1.Text = "troopamount";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelA
			// 
			this.labelA.BackColor = System.Drawing.Color.LightGreen;
			this.labelA.Location = new System.Drawing.Point(14, 11);
			this.labelA.Name = "labelA";
			this.labelA.Size = new System.Drawing.Size(120, 44);
			this.labelA.TabIndex = 12;
			this.labelA.Text = "labelA";
			this.labelA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelB
			// 
			this.labelB.BackColor = System.Drawing.Color.LightSalmon;
			this.labelB.Location = new System.Drawing.Point(147, 11);
			this.labelB.Name = "labelB";
			this.labelB.Size = new System.Drawing.Size(120, 44);
			this.labelB.TabIndex = 13;
			this.labelB.Text = "labelB";
			this.labelB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelC
			// 
			this.labelC.BackColor = System.Drawing.Color.LightSteelBlue;
			this.labelC.Location = new System.Drawing.Point(14, 60);
			this.labelC.Name = "labelC";
			this.labelC.Size = new System.Drawing.Size(120, 44);
			this.labelC.TabIndex = 14;
			this.labelC.Text = "labelC";
			this.labelC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelD
			// 
			this.labelD.BackColor = System.Drawing.Color.LightYellow;
			this.labelD.Location = new System.Drawing.Point(147, 60);
			this.labelD.Name = "labelD";
			this.labelD.Size = new System.Drawing.Size(120, 44);
			this.labelD.TabIndex = 15;
			this.labelD.Text = "labelD";
			this.labelD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelD);
			this.groupBox1.Controls.Add(this.labelC);
			this.groupBox1.Controls.Add(this.labelB);
			this.groupBox1.Controls.Add(this.labelA);
			this.groupBox1.Location = new System.Drawing.Point(7, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(282, 110);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			// 
			// ProduceTroopSetting
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(304, 362);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.buttonlimit);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownTransferCount);
			this.Controls.Add(this.buttonTiming);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProduceTroopSetting";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProduceTroopSetting";
			this.Load += new System.EventHandler(this.ProduceTroopSetting_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelD;
		private System.Windows.Forms.Label labelC;
		private System.Windows.Forms.Label labelB;
		private System.Windows.Forms.Label labelA;
		private System.Windows.Forms.Label label1;

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button buttonTiming;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownTransferCount;
		private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button buttonlimit;
        private System.Windows.Forms.CheckBox checkBox3;
	}
}