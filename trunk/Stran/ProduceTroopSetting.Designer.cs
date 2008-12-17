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
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).BeginInit();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 14;
			this.listBox1.Location = new System.Drawing.Point(12, 12);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(120, 200);
			this.listBox1.TabIndex = 0;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(200, 224);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 32);
			this.buttonCancel.TabIndex = 20;
			this.buttonCancel.Tag = "cancel";
			this.buttonCancel.Text = "取消";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(102, 224);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 32);
			this.buttonOK.TabIndex = 19;
			this.buttonOK.Tag = "ok";
			this.buttonOK.Text = "确定";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericUpDown1.Location = new System.Drawing.Point(152, 12);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(100, 22);
			this.numericUpDown1.TabIndex = 21;
			this.numericUpDown1.ThousandsSeparator = true;
			// 
			// buttonTiming
			// 
			this.buttonTiming.Location = new System.Drawing.Point(172, 180);
			this.buttonTiming.Name = "buttonTiming";
			this.buttonTiming.Size = new System.Drawing.Size(80, 32);
			this.buttonTiming.TabIndex = 22;
			this.buttonTiming.Tag = "Timing";
			this.buttonTiming.Text = "定时";
			this.buttonTiming.UseVisualStyleBackColor = true;
			this.buttonTiming.Click += new System.EventHandler(this.buttonTiming_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(138, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(142, 22);
			this.label3.TabIndex = 30;
			this.label3.Tag = "transfercnt";
			this.label3.Text = "transfercnt";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownTransferCount
			// 
			this.numericUpDownTransferCount.Location = new System.Drawing.Point(219, 113);
			this.numericUpDownTransferCount.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDownTransferCount.Name = "numericUpDownTransferCount";
			this.numericUpDownTransferCount.Size = new System.Drawing.Size(61, 22);
			this.numericUpDownTransferCount.TabIndex = 28;
			this.numericUpDownTransferCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numericUpDownTransferCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// ProduceTroopSetting
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(294, 270);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownTransferCount);
			this.Controls.Add(this.buttonTiming);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBox1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ProduceTroopSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProduceTroopSetting";
			this.Load += new System.EventHandler(this.ProduceTroopSetting_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferCount)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button buttonTiming;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownTransferCount;
	}
}