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
	public partial class ProduceTroopSetting : Form
	{
		private DateTime actionAt = DateTime.MinValue;
		private int minimumInterval = 0;
		public List<TroopInfo> CanProduce { get; set; }
		public MUI mui { get; set; }

		public ProduceTroopQueue Result;

		public ProduceTroopSetting()
		{
			InitializeComponent();
		}

		private void buttonTiming_Click(object sender, EventArgs e)
		{
			ActionTiming tt = new ActionTiming
			{
				ActionAt = actionAt,
				MinimumInterval = minimumInterval,
				ActionTime = 0,
				mui = mui
			};

			if(tt.ShowDialog() == DialogResult.OK)
			{
				actionAt = tt.ActionAt;
				minimumInterval = tt.MinimumInterval;
			}

		}

		private void ProduceTroopSetting_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			if(CanProduce != null)
				foreach(var p in CanProduce)
					listBox1.Items.Add(p);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(numericUpDown1.Value == 0)
				return;
			Result = new ProduceTroopQueue
			{
				Aid = (listBox1.SelectedItem as TroopInfo).Aid,
				Amount = Convert.ToInt32(numericUpDown1.Value),
				MaxCount = Convert.ToInt32(numericUpDownTransferCount.Value),
				MinimumInterval = minimumInterval,
				NextExec = actionAt
			};
		}
	}

	public class TroopInfo
	{
		public int Aid { get; set; }
		public string Name { get; set; }
		public override string ToString()
		{
			return string.Format("{0} - {1}", Aid, Name);
		}
	}
}
