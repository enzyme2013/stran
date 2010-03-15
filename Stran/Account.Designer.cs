namespace Stran
{
    partial class NewAccount
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
			this.Btn_OK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
			this.Btn_Cancel = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.lblProxy = new System.Windows.Forms.Label();
            this.txtProxy = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
			this.Btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Btn_OK.Location = new System.Drawing.Point(77, 231);
			this.Btn_OK.Name = "Btn_OK";
			this.Btn_OK.Size = new System.Drawing.Size(80, 32);
			this.Btn_OK.TabIndex = 5;
			this.Btn_OK.Tag = "save";
			this.Btn_OK.Text = "OK";
			this.Btn_OK.UseVisualStyleBackColor = true;
			this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 14);
            this.label4.TabIndex = 9;
            this.label4.Tag = "password";
			this.label4.Text = "PWD";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 14);
            this.label5.TabIndex = 8;
            this.label5.Tag = "username";
			this.label5.Text = "ID";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(24, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 14);
            this.label6.TabIndex = 7;
            this.label6.Tag = "server";
			this.label6.Text = "Server";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(135, 93);
            this.textBox1.Name = "textBox1";
            this.textBox1.PasswordChar = 'X';
            this.textBox1.Size = new System.Drawing.Size(180, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.UseSystemPasswordChar = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(135, 58);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(180, 22);
            this.textBox2.TabIndex = 1;
            // 
            // button2
            // 
			this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Btn_Cancel.Location = new System.Drawing.Point(187, 231);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new System.Drawing.Size(80, 32);
			this.Btn_Cancel.TabIndex = 6;
			this.Btn_Cancel.Tag = "cancel";
			this.Btn_Cancel.Text = "Cancel";
			this.Btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(135, 128);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(180, 22);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 14);
            this.label1.TabIndex = 10;
            this.label1.Tag = "tribe";
			this.label1.Text = "Race";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(135, 23);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(180, 22);
            this.textBox3.TabIndex = 0;
            this.textBox3.Leave += new System.EventHandler(this.textBox3_Leave);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 14);
            this.label2.TabIndex = 11;
            this.label2.Tag = "displang";
			this.label2.Text = "Language";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(135, 164);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(180, 22);
            this.textBox4.TabIndex = 4;
            // 
            // lblProxy
            // 
            this.lblProxy.Location = new System.Drawing.Point(24, 202);
            this.lblProxy.Name = "lblProxy";
            this.lblProxy.Size = new System.Drawing.Size(93, 14);
            this.lblProxy.TabIndex = 13;
            this.lblProxy.Tag = "proxy";
            this.lblProxy.Text = "¥˙¿Ìµÿ÷∑";
            this.lblProxy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxy
            // 
            this.txtProxy.Location = new System.Drawing.Point(135, 199);
            this.txtProxy.Name = "txtProxy";
            this.txtProxy.Size = new System.Drawing.Size(180, 22);
            this.txtProxy.TabIndex = 5;
            // 
            // NewAccount
            // 
			this.AcceptButton = this.Btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Btn_Cancel;
			this.ClientSize = new System.Drawing.Size(344, 270);
            this.Controls.Add(this.lblProxy);
            this.Controls.Add(this.txtProxy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.Btn_Cancel);
			this.Controls.Add(this.Btn_OK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewAccount";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "account";
			this.Text = "Acccount";
            this.Load += new System.EventHandler(this.NewAccount_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		private System.Windows.Forms.Button Btn_Cancel;
		private System.Windows.Forms.Button Btn_OK;

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label lblProxy;
        private System.Windows.Forms.TextBox txtProxy;

    }
}