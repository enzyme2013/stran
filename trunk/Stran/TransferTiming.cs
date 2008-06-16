using System;
using System.Windows.Forms;
using libTravian;

namespace Stran
{
	public partial class TransferTiming : Form
	{
		public DateTime TransferAt { get; set; }
		public int MinimumInterval { get; set; }
		public int TransferTime { get; set; }
		public MUI mui { get; set; }

		public TransferTiming()
		{
			InitializeComponent();
		}

		private void TransferTiming_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			if (this.TransferAt > DateTime.Now)
			{
				this.radioDelayed.Checked = true;
				this.dateTimeTransferAt.Value = this.TransferAt;
			}
			else
			{
				this.radioImmediate.Checked = true;
				this.dateTimeTransferAt.Value = DateTime.Now;
			}

			this.numericUpDown1.Value = this.MinimumInterval / 60;
			this.CalculateArrivalTime();
		}

		private void radioDelayed_CheckedChanged(object sender, EventArgs e)
		{
			this.dateTimeTransferAt.Enabled  = this.radioDelayed.Checked;
			if (this.radioImmediate.Checked)
			{
				this.dateTimeTransferAt.Value = DateTime.Now;
			}
		}

		private void dateTimeTransferAt_ValueChanged(object sender, EventArgs e)
		{
			this.TransferAt = this.dateTimeTransferAt.Value;
			this.CalculateArrivalTime();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			this.MinimumInterval = Convert.ToInt32(this.numericUpDown1.Value) * 60;
		}

		private void CalculateArrivalTime()
		{
			if (this.TransferTime > 0)
			{
				this.labelDetail.Text = this.TransferAt.AddSeconds(this.TransferTime).ToString("dddd MMM dd yyyy hh:mm");
			}
			else
			{
				this.labelDetail.Text = "N/A";
			}
		}
	}
}
