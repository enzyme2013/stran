using System;
using System.Windows.Forms;

namespace Stravian
{
	public partial class NewBuilding : Form
	{
		public int OutBid, OutGid;
		//Building[] b;
		libServerLang svrlang;
		public NewBuilding(Building[] b, libServerLang svrlang)
		{
			//this.b = Array.c
			this.svrlang = svrlang;
			InitializeComponent();
			int[] s = Array.ConvertAll<Building, int>(b, new Converter<Building, int>(B2I));
			// add things to combo-boxes
			for(int i = 0; i < svrlang.Building.Count; i++)
				if(i == 10 || i == 11 || (Array.IndexOf<int>(s, i) < 0))
					comboBox1.Items.Add(i + ". " + svrlang.Building[i]);
			for(int i = 19; i < 39; i++)
				if(b[i] == null)
					comboBox2.Items.Add(i);
			if(comboBox2.Items.Count != 0)
				comboBox2.SelectedIndex = 0;
			else
				button1.Enabled = false;
		}
		public static int B2I(Building b)
		{
			if(b != null)
				return b.gid;
			else
				return 0;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OutGid = Convert.ToInt32(comboBox1.SelectedItem.ToString().Split('.')[0]);
			OutBid = (int)comboBox2.SelectedItem;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int gid = Convert.ToInt32(comboBox1.SelectedItem.ToString().Split('.')[0]);
			if(!AI.preferpos.ContainsKey(gid))
				return;
			int[] preferpos = AI.preferpos[gid];
			if(preferpos == null)
				return;
			foreach(int pos in preferpos)
				if(comboBox2.Items.Contains(pos))
				{
					comboBox2.SelectedIndex = comboBox2.Items.IndexOf(pos);
					return;
				}
			if(comboBox2.Items.Count != 0)
				comboBox2.SelectedIndex = 0;
		}
	}
}
