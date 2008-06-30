using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libTravian;

namespace Stran
{
	public partial class RaidOptForm : Form
	{
		public RaidOptForm()
		{
			InitializeComponent();
		}
		public int Tribe { get; set; }
		public int[] Troops { get; set; }
		public MUI mui { get; set; }
		public DisplayLang dl { get; set; }
		public NumericUpDown[] Nums = new NumericUpDown[11];
		public RaidOption Return { get; private set; }

		private void RaidOptForm_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			// 
			// numericUpDown1
			// 
			SuspendLayout();
			for(int i = 0; i < Nums.Length; i++)
			{
				NumericUpDown nud = new NumericUpDown();
				Nums[i] = nud;
				tableLayoutPanel1.Controls.Add(nud, 1, i + 1);
				nud.Dock = DockStyle.Fill;
				nud.TabIndex = i;
				nud.Maximum = Troops[i];
				nud.Minimum = -1;
				nud.ThousandsSeparator = true;
				nud.Increment = Math.Max(1, (decimal)(Troops[i] / 20));
				Label l1 = new Label();
				tableLayoutPanel1.Controls.Add(l1, 0, i + 1);
				l1.Dock = DockStyle.Fill;
				l1.Text = dl.GetAidLang(Tribe, i + 1);
				l1.AutoSize = false;
				l1.TextAlign = ContentAlignment.MiddleCenter;
				Label l2 = new Label();
				tableLayoutPanel1.Controls.Add(l2, 2, i + 1);
				l2.Dock = DockStyle.Fill;
				l2.Text = Troops[i].ToString();
				l2.AutoSize = false;
				l2.TextAlign = ContentAlignment.MiddleCenter;
			}
			ResumeLayout();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int[] troops = new int[11];
			for(int i = 0; i < 11; i++)
				troops[i] = Convert.ToInt32(Nums[i].Value);
			Return = new RaidOption
			{
				Troops = troops,
				Targets = null
			};
			throw new NotImplementedException();
		}
	}
}
