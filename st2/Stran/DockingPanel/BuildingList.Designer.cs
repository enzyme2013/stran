namespace Stran.DockingPanel
{
	partial class BuildingList
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
			this.listViewBuilding = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// listViewBuilding
			// 
			this.listViewBuilding.AllowColumnReorder = true;
			this.listViewBuilding.BackColor = System.Drawing.SystemColors.Window;
			this.listViewBuilding.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4});
			this.listViewBuilding.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewBuilding.FullRowSelect = true;
			this.listViewBuilding.GridLines = true;
			this.listViewBuilding.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewBuilding.Location = new System.Drawing.Point(0, 0);
			this.listViewBuilding.Name = "listViewBuilding";
			this.listViewBuilding.Size = new System.Drawing.Size(392, 368);
			this.listViewBuilding.TabIndex = 4;
			this.listViewBuilding.UseCompatibleStateImageBehavior = false;
			this.listViewBuilding.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Tag = "bid";
			this.columnHeader1.Text = "坑号";
			this.columnHeader1.Width = 50;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Tag = "gname";
			this.columnHeader2.Text = "建筑名";
			this.columnHeader2.Width = 200;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Tag = "lefttimetobuild";
			this.columnHeader4.Text = "剩余时间";
			this.columnHeader4.Width = 100;
			// 
			// BuildingList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(392, 368);
			this.CloseButton = false;
			this.Controls.Add(this.listViewBuilding);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "BuildingList";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.TabText = "村庄建筑";
			this.Tag = "villagestatus";
			this.Text = "村庄建筑";
			this.Load += new System.EventHandler(this.BuildingList_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListView listViewBuilding;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader4;
	}
}