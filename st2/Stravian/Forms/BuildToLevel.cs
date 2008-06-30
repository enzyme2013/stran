using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stravian
{
	public partial class BuildToLevel : Form
	{
		public int Return { private set; get; }
		public string BuildingName { set; private get; }
		public int CurrentLevel { set; private get; }
		public int TargetLevel { set; private get; }
		public BuildToLevel()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Return = (int)comboBox1.SelectedItem;
		}

		private void BuildToLevel_Load(object sender, EventArgs e)
		{
			if(CurrentLevel >= TargetLevel)
			{
				Return = -1;
				this.Close();
			}
			label1.Text = BuildingName;
			for(int i = CurrentLevel; i < TargetLevel; i++)
			{
				comboBox1.Items.Add(i + 1);
			}
			comboBox1.SelectedIndex = 0;
		}
	}
}
