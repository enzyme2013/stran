using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stran
{
	public partial class RaidOptForm : Form
	{
		public RaidOptForm()
		{
			InitializeComponent();
		}

		public int[] Troops { get; set; }
		public MUI mui { get; set; }

		private void RaidOptForm_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);

		}
	}
}
