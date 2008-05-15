using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Stran.DockingPanel
{
	public partial class BuildingList : DockContent
	{
		public MainFrame UpCall { get; set; }
		
		public BuildingList()
		{
			InitializeComponent();
		}

		private void BuildingList_Load(object sender, EventArgs e)
		{
			listViewBuilding.ContextMenuStrip = UpCall.contextMenuBuilding;
			UpCall.mui.RefreshLanguage(this);
			TabText = Text;
		}
	}
}
