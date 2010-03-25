namespace Stran
{
    partial class BalanceForm
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
        	this.label2 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.label5 = new System.Windows.Forms.Label();
        	this.Btn_OK = new System.Windows.Forms.Button();
        	this.Btn_Cel = new System.Windows.Forms.Button();
        	this.label6 = new System.Windows.Forms.Label();
        	this.label7 = new System.Windows.Forms.Label();
        	this.numID = new System.Windows.Forms.NumericUpDown();
        	this.numMarketTime = new System.Windows.Forms.NumericUpDown();
        	this.numReadyTime = new System.Windows.Forms.NumericUpDown();
        	this.numMinSendResource = new System.Windows.Forms.NumericUpDown();
        	this.numMaxSendResource = new System.Windows.Forms.NumericUpDown();
        	this.cbxMarket = new System.Windows.Forms.ComboBox();
        	this.textDescription = new System.Windows.Forms.TextBox();
        	((System.ComponentModel.ISupportInitialize)(this.numID)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMarketTime)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numReadyTime)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMinSendResource)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMaxSendResource)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(23, 25);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(125, 22);
        	this.label1.TabIndex = 0;
        	this.label1.Tag = "groupid";
        	this.label1.Text = "GroupID";
        	this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(23, 65);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(125, 22);
        	this.label2.TabIndex = 1;
        	this.label2.Tag = "ignoremarket";
        	this.label2.Text = "IgnoreMarket";
        	this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label3
        	// 
        	this.label3.Location = new System.Drawing.Point(23, 105);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(125, 22);
        	this.label3.TabIndex = 2;
        	this.label3.Tag = "ignoretime";
        	this.label3.Text = "IgnoreTime";
        	this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label4
        	// 
        	this.label4.Location = new System.Drawing.Point(23, 145);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(125, 22);
        	this.label4.TabIndex = 3;
        	this.label4.Tag = "description";
        	this.label4.Text = "Description";
        	this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label5
        	// 
        	this.label5.Location = new System.Drawing.Point(23, 185);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(125, 22);
        	this.label5.TabIndex = 4;
        	this.label5.Tag = "readytime";
        	this.label5.Text = "ReadyTime";
        	this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// Btn_OK
        	// 
        	this.Btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
        	this.Btn_OK.Location = new System.Drawing.Point(34, 305);
        	this.Btn_OK.Name = "Btn_OK";
        	this.Btn_OK.Size = new System.Drawing.Size(90, 30);
        	this.Btn_OK.TabIndex = 10;
        	this.Btn_OK.Tag = "ok";
        	this.Btn_OK.Text = "OK";
        	this.Btn_OK.UseVisualStyleBackColor = true;
        	this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
        	// 
        	// Btn_Cel
        	// 
        	this.Btn_Cel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        	this.Btn_Cel.Location = new System.Drawing.Point(182, 305);
        	this.Btn_Cel.Name = "Btn_Cel";
        	this.Btn_Cel.Size = new System.Drawing.Size(90, 30);
        	this.Btn_Cel.TabIndex = 11;
        	this.Btn_Cel.Tag = "cancel";
        	this.Btn_Cel.Text = "Cancel";
        	this.Btn_Cel.UseVisualStyleBackColor = true;
        	// 
        	// label6
        	// 
        	this.label6.Location = new System.Drawing.Point(23, 225);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(125, 22);
        	this.label6.TabIndex = 12;
        	this.label6.Tag = "minsendresource";
        	this.label6.Text = "MinSendResource";
        	this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label7
        	// 
        	this.label7.Location = new System.Drawing.Point(23, 265);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(125, 22);
        	this.label7.TabIndex = 14;
        	this.label7.Tag = "maxsendresource";
        	this.label7.Text = "MaxSendResource";
        	this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// numID
        	// 
        	this.numID.Location = new System.Drawing.Point(152, 27);
        	this.numID.Maximum = new decimal(new int[] {
        	        	        	20,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numID.Name = "numID";
        	this.numID.Size = new System.Drawing.Size(120, 22);
        	this.numID.TabIndex = 16;
        	// 
        	// numMarketTime
        	// 
        	this.numMarketTime.Increment = new decimal(new int[] {
        	        	        	30,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numMarketTime.Location = new System.Drawing.Point(152, 107);
        	this.numMarketTime.Maximum = new decimal(new int[] {
        	        	        	4320,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numMarketTime.Name = "numMarketTime";
        	this.numMarketTime.Size = new System.Drawing.Size(120, 22);
        	this.numMarketTime.TabIndex = 17;
        	// 
        	// numReadyTime
        	// 
        	this.numReadyTime.Increment = new decimal(new int[] {
        	        	        	30,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numReadyTime.Location = new System.Drawing.Point(152, 187);
        	this.numReadyTime.Maximum = new decimal(new int[] {
        	        	        	120,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numReadyTime.Name = "numReadyTime";
        	this.numReadyTime.Size = new System.Drawing.Size(120, 22);
        	this.numReadyTime.TabIndex = 18;
        	// 
        	// numMinSendResource
        	// 
        	this.numMinSendResource.Location = new System.Drawing.Point(152, 227);
        	this.numMinSendResource.Name = "numMinSendResource";
        	this.numMinSendResource.Size = new System.Drawing.Size(120, 22);
        	this.numMinSendResource.TabIndex = 19;
        	// 
        	// numMaxSendResource
        	// 
        	this.numMaxSendResource.Location = new System.Drawing.Point(152, 267);
        	this.numMaxSendResource.Name = "numMaxSendResource";
        	this.numMaxSendResource.Size = new System.Drawing.Size(120, 22);
        	this.numMaxSendResource.TabIndex = 20;
        	// 
        	// cbxMarket
        	// 
        	this.cbxMarket.FormattingEnabled = true;
        	this.cbxMarket.Items.AddRange(new object[] {
        	        	        	"NO",
        	        	        	"YES"});
        	this.cbxMarket.Location = new System.Drawing.Point(151, 66);
        	this.cbxMarket.Name = "cbxMarket";
        	this.cbxMarket.Size = new System.Drawing.Size(121, 22);
        	this.cbxMarket.TabIndex = 21;
        	// 
        	// textDescription
        	// 
        	this.textDescription.Location = new System.Drawing.Point(151, 146);
        	this.textDescription.Name = "textDescription";
        	this.textDescription.Size = new System.Drawing.Size(121, 22);
        	this.textDescription.TabIndex = 22;
        	// 
        	// BalanceForm
        	// 
        	this.AcceptButton = this.Btn_OK;
        	this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.CancelButton = this.Btn_Cel;
        	this.ClientSize = new System.Drawing.Size(303, 351);
        	this.Controls.Add(this.textDescription);
        	this.Controls.Add(this.cbxMarket);
        	this.Controls.Add(this.numMaxSendResource);
        	this.Controls.Add(this.numMinSendResource);
        	this.Controls.Add(this.numReadyTime);
        	this.Controls.Add(this.numMarketTime);
        	this.Controls.Add(this.numID);
        	this.Controls.Add(this.label7);
        	this.Controls.Add(this.label6);
        	this.Controls.Add(this.Btn_Cel);
        	this.Controls.Add(this.Btn_OK);
        	this.Controls.Add(this.label5);
        	this.Controls.Add(this.label4);
        	this.Controls.Add(this.label3);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.label1);
        	this.Font = new System.Drawing.Font("Tahoma", 9F);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "BalanceForm";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Tag = "balanceform";
        	this.Text = "BalanceForm";
        	this.Load += new System.EventHandler(this.BalanceForm_Load);
        	((System.ComponentModel.ISupportInitialize)(this.numID)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMarketTime)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numReadyTime)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMinSendResource)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numMaxSendResource)).EndInit();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.ComboBox cbxMarket;
        private System.Windows.Forms.NumericUpDown numMinSendResource;
        private System.Windows.Forms.NumericUpDown numMaxSendResource;
        private System.Windows.Forms.NumericUpDown numReadyTime;
        private System.Windows.Forms.NumericUpDown numID;
        private System.Windows.Forms.NumericUpDown numMarketTime;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Button Btn_Cel;
        private System.Windows.Forms.Button Btn_OK;

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}