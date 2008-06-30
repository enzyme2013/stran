namespace test
{
	partial class Form1
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
			this.syntaxHighlighting1 = new libScriptEngine.SyntaxHighlighting();
			this.SuspendLayout();
			// 
			// syntaxHighlighting1
			// 
			this.syntaxHighlighting1.AcceptsTab = true;
			this.syntaxHighlighting1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.syntaxHighlighting1.Font = new System.Drawing.Font("Lucida Console", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.syntaxHighlighting1.Location = new System.Drawing.Point(0, 0);
			this.syntaxHighlighting1.MaxUndoRedoSteps = 20;
			this.syntaxHighlighting1.Name = "syntaxHighlighting1";
			this.syntaxHighlighting1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.syntaxHighlighting1.Size = new System.Drawing.Size(679, 601);
			this.syntaxHighlighting1.TabIndex = 0;
			this.syntaxHighlighting1.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 601);
			this.Controls.Add(this.syntaxHighlighting1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private libScriptEngine.SyntaxHighlighting syntaxHighlighting1;

	}
}