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
        private TVillage CV = null;
        private bool initialized = false;
        
        public List<TroopInfo> CanProduce { get; set; }
		public MUI mui { get; set; }

        public Data TravianData { get; set; }
        public int RUVillageID { get; set; }
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
            if (TravianData == null)
                return;
            CV = TravianData.Villages[RUVillageID];
            TResAmount TroopRes = new TResAmount(0,0,0,0);
        	if (!initialized)
        	{
        		this.buttonOK.Enabled = false;
         		numericUpDown1.Value = 0;
                numericUpDownTransferCount.Value = 1;
                numericUpDown1.Enabled = numericUpDownTransferCount.Enabled = checkBox1.Enabled = listBox1.Enabled = true;
         		listBox1.Items.Clear();
                if (CanProduce != null)
                    foreach (var p in CanProduce)
                        if (p.Researched | checkBox1.Checked)
                            listBox1.Items.Add(p);
	            initialized = true;
        	}
        	if (listBox1.SelectedIndices.Count == 1 || checkBox3.Checked)
            {
            	int Aid = checkBox3.Checked ? 10 : (listBox1.SelectedItem as TroopInfo).Aid;
				int key = (TravianData.Tribe - 1) * 10 + Aid;
				int Amount = Convert.ToInt32(numericUpDown1.Value);
				if (Aid == 9 || Aid == 10)
				{
					if (Amount > 1)
					{
						Amount = 1;
						numericUpDown1.Value = 1;
					}
					TroopRes = Buildings.TroopCost[key] * Amount;
				}
				else
					TroopRes = Buildings.TroopCost[key] * Amount * (checkBox2.Checked ? 3 : 1);
            }

        	if (checkBox3.Checked)
            {
        		listBox1.Items.Clear();
                numericUpDown1.Value = 1;
                numericUpDownTransferCount.Value = 0;
                numericUpDown1.Enabled = false;
                numericUpDownTransferCount.Enabled = false;
                checkBox1.Enabled = false;
                listBox1.Enabled = false;
                TroopRes = Buildings.TroopCost[(TravianData.Tribe - 1) * 10 + 10];
        	}
        	var ResRes = CV.ResourceCurrAmount - TroopRes;
        	this.labelA.ForeColor = this.labelB.ForeColor = this.labelC.ForeColor = this.labelD.ForeColor = Color.FromArgb(0, 0, 0);
        	if (ResRes.Resources[0] <= 0)
        		this.labelA.ForeColor = Color.FromArgb(255, 0, 0);
        	if (ResRes.Resources[1] <= 0)
        		this.labelB.ForeColor = Color.FromArgb(255, 0, 0);
        	if (ResRes.Resources[2] <= 0)
        		this.labelC.ForeColor = Color.FromArgb(255, 0, 0);
        	if (ResRes.Resources[3] <= 0)
        		this.labelD.ForeColor = Color.FromArgb(255, 0, 0);
        	this.labelA.Text = string.Format("{2}/{1} \r\n {0}", TroopRes.Resources[0].ToString(), CV.ResourceCurrAmount.Resources[0].ToString(), ResRes.Resources[0].ToString());
        	this.labelB.Text = string.Format("{2}/{1} \r\n {0}", TroopRes.Resources[1].ToString(), CV.ResourceCurrAmount.Resources[1].ToString(), ResRes.Resources[1].ToString());
        	this.labelC.Text = string.Format("{2}/{1} \r\n {0}", TroopRes.Resources[2].ToString(), CV.ResourceCurrAmount.Resources[2].ToString(), ResRes.Resources[2].ToString());
        	this.labelD.Text = string.Format("{2}/{1} \r\n {0}", TroopRes.Resources[3].ToString(), CV.ResourceCurrAmount.Resources[3].ToString(), ResRes.Resources[3].ToString());
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (numericUpDown1.Value == 0)
				return;
            if (listBox1.SelectedItem == null && checkBox3.Checked == false)
				return;
			bool St = checkBox3.Checked;
            Result = new ProduceTroopQueue
			{
				Aid = St ? 10 : (listBox1.SelectedItem as TroopInfo).Aid,
                GRt = checkBox2.Checked,
                Amount = Convert.ToInt32(numericUpDown1.Value),
				MaxCount = Convert.ToInt32(numericUpDownTransferCount.Value),
				MinimumInterval = minimumInterval,
				NextExec = actionAt,
			};
		}									 

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			initialized = false;
			ProduceTroopSetting_Load(sender, e);
		}

        private void buttonlimit_Click(object sender, EventArgs e)
        {
            ResourceLimit limit = new ResourceLimit()
            {
                Village = this.CV,
                Description = this.mui._("TResourceLimit"),
                Limit = this.CV.Troop.ResLimit == null ? new TResAmount(0, 0, 0, 0) : this.CV.Troop.ResLimit,
                mui = this.mui
            };

            if (limit.ShowDialog() == DialogResult.OK && limit.Return != null)
            {
                this.CV.Troop.ResLimit = limit.Return;
                TravianData.Dirty = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
        	this.buttonOK.Enabled = false;
        	if (!checkBox3.Checked)
        		initialized = false;
        	if (checkBox3.Checked)
        		this.buttonOK.Enabled = true;
            ProduceTroopSetting_Load(sender, e);
        }
		
		private void NumericUpDown1ValueChanged(object sender, EventArgs e)
		{
        	if (listBox1.SelectedIndices.Count == 1)
        		ProduceTroopSetting_Load(sender, e);
		}
		
		private void NumericUpDownTransferCountValueChanged(object sender, EventArgs e)
		{
        	if (listBox1.SelectedIndices.Count == 1)
        		ProduceTroopSetting_Load(sender, e);
		}
		
		private void ListBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			this.buttonOK.Enabled = false;
        	if (listBox1.SelectedIndices.Count == 1)
        	{
        		ProduceTroopSetting_Load(sender, e);
        		this.buttonOK.Enabled = true;
        	}
		}
		private void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			ProduceTroopSetting_Load(sender, e);
		}
	}

	public class TroopInfo
	{
		public int Aid { get; set; }
		public string Name { get; set; }
		public bool Researched { get; set; }
		public override string ToString()
		{
			return string.Format("{0} - {1}", Aid, Name);
		}
	}
}
