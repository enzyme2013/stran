/*
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * The Initial Developer of the Original Code is [MeteorRain <msg7086@gmail.com>].
 * Copyright (C) MeteorRain 2007, 2008. All Rights Reserved.
 * Contributor(s): [MeteorRain].
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libTravian;
using System.Diagnostics;

namespace Stran
{
	public partial class TransferSetting : Form
	{
		private TVillage CV = null;
		private TVillage TV = null;
		private int targetVillageID = 0;

		public Data TravianData { get; set; }
		public int FromVillageID { get; set; }
		public MUI mui { get; set; }
		public TransferOption Return { get; private set; }

		public TransferSetting()
		{
			InitializeComponent();
		}

		private void TransferSetting_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			if (TravianData == null)
				return;
			CV = TravianData.Villages[FromVillageID];
			foreach (var v in TravianData.Villages)
				if (v.Key != FromVillageID)
					comboBoxTargetVillage.Items.Add(v.Key + " " + v.Value.Coord + " " + v.Value.Name);
				else
					comboBoxTargetVillage.SelectedIndex = comboBoxTargetVillage.Items.Count - 1;

			numericUpDown1.Increment =
				numericUpDown2.Increment =
				numericUpDown3.Increment =
				numericUpDown4.Increment = CV.Market.SingleCarry;
			numericUpDown1.Maximum =
				numericUpDown2.Maximum =
				numericUpDown3.Maximum =
				numericUpDown4.Maximum = CV.Market.SingleCarry * CV.Market.MaxMerchant;
			numericUpDownMechantCount.Maximum = CV.Market.MaxMerchant;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.Return = this.GetTransferOption();
		}

		private void comboBoxTargetVillage_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.TV = null;
			if (comboBoxTargetVillage.SelectedIndex == 0)
			{
				this.txtX.Enabled = this.txtY.Enabled = true;
			}
			else
			{
				this.txtX.Enabled = this.txtY.Enabled = false;
				this.targetVillageID = Convert.ToInt32((comboBoxTargetVillage.SelectedItem as string).Split(' ')[0]);
				if (TravianData.Villages.ContainsKey(this.targetVillageID))
				{
					TVillage village = TravianData.Villages[this.targetVillageID];
					this.txtX.Text = village.X.ToString();
					this.txtY.Text = village.Y.ToString();
					if (village.isBuildingInitialized == 2)
					{
						this.TV = village;
					}
				}
			}

			if (this.TV == null)
			{
				this.radioNormalTarget.Enabled = false;
				if (this.radioNormalTarget.Checked)
				{
					this.radioNormalMe.Checked = true;
				}
			}
			else
			{
				this.radioNormalTarget.Enabled = true;
			}

			numericUpDownMechantCount_ValueChanged(sender, e);
		}

		private void numericUpDownMechantCount_ValueChanged(object sender, EventArgs e)
		{
			TransferOption option = this.GetTransferOption();
			if (option != null && option.Distribution != ResourceDistributionType.None)
			{
				int total = this.CV.Market.SingleCarry * Convert.ToInt32(this.numericUpDownMechantCount.Value);
				option.ResourceAmount = new TResAmount(0, 0, 0, total);
				option.CalculateResourceAmount(this.TravianData, this.CV.ID);
				this.numericUpDown1.Value = option.ResourceAmount.Resources[0];
				this.numericUpDown2.Value = option.ResourceAmount.Resources[1];
				this.numericUpDown3.Value = option.ResourceAmount.Resources[2];
				this.numericUpDown4.Value = option.ResourceAmount.Resources[3];
			}
		}

		private void checkBoxNoCrop_CheckedChanged(object sender, EventArgs e)
		{
			comboBoxTargetVillage_SelectedIndexChanged(sender, e);
		}

		private void numericUpDown1234_ValueChanged(object sender, EventArgs e)
		{
			var num = new int[]{
					Convert.ToInt32(numericUpDown1.Value),
					Convert.ToInt32(numericUpDown2.Value),
					Convert.ToInt32(numericUpDown3.Value),
					Convert.ToInt32(numericUpDown4.Value)};

			StringBuilder sb = new StringBuilder();
			if (TV != null)
			{
				int max = Math.Max(num[0],
					Math.Max(num[1],
					Math.Max(num[2], num[3])));

				int length1 = Math.Max(CV.Resource[0].Capacity, CV.Resource[3].Capacity).ToString().Length;
				int length2 = max.ToString().Length;
				int length3 = Math.Max(TV.Resource[0].Capacity, TV.Resource[3].Capacity).ToString().Length;

				string format = "{0," + length1.ToString() + "}/{1," + length1.ToString() + "} {2,3}% -> {3," + length2.ToString() + "} -> {4," + length3.ToString() + "}/{5," + length3.ToString() + "} {6,3}%";
				for (int i = 0; i < 4; i++)
				{
					sb.AppendFormat(format, //"{0}/{1} {2}% ->\t{3} ->\t{4}/{5} {6}%",
						CV.Resource[i].CurrAmount,
						CV.Resource[i].Capacity,
						(CV.Resource[i].CurrAmount - num[i]) * 100 / CV.Resource[i].Capacity,
						num[i],
						TV.Resource[i].CurrAmount,
						TV.Resource[i].Capacity,
						(num[i] + TV.Resource[i].CurrAmount) * 100 / TV.Resource[i].Capacity
						);
					sb.AppendLine();
				}
			}
			int all = 0;
			for (int i = 0; i < 4; i++)
				all += num[i];
			if (CV.Market.SingleCarry == 0)
				CV.Market.SingleCarry = 750;
			sb.AppendFormat(mui._("merchantsformat"), Convert.ToInt32(Math.Ceiling(Convert.ToDouble(all) / CV.Market.SingleCarry)), CV.Market.ActiveMerchant);
			labelDetail.Text = sb.ToString();
		}

		private void radio_CheckedChanged(object sender, EventArgs e)
		{
			comboBoxTargetVillage_SelectedIndexChanged(sender, e);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				linkLabel1.LinkVisited = true;
				Process.Start("http://traplug.5d6d.com/thread-417-1-1.html");
			}
			catch
			{
			}
		}

		/// <summary>
		/// Assemble a TransferOption object using current control values
		/// </summary>
		/// <returns></returns>
		private TransferOption GetTransferOption()
		{
			TransferOption option = new TransferOption();

			try
			{
				option.MaxCount = Convert.ToInt32(this.numericUpDownTransferCount.Value);
				option.TargetPos = new TPoint(Convert.ToInt32(this.txtX.Text), Convert.ToInt32(this.txtY.Text));
				option.ResourceAmount = new TResAmount(
					 Convert.ToInt32(this.numericUpDown1.Value),
					 Convert.ToInt32(this.numericUpDown2.Value),
					 Convert.ToInt32(this.numericUpDown3.Value),
					 Convert.ToInt32(this.numericUpDown4.Value));
				option.TargetVillageID = this.targetVillageID;

				if (this.radioNormalTarget.Checked)
				{
					option.Distribution = ResourceDistributionType.BalanceTarget;
					if (this.TV == null)
					{
						return null;
					}
				}
				else if (radioNormalMe.Checked)
				{
					option.Distribution = ResourceDistributionType.BalanceSource;
				}

				if (checkBoxNoCrop.Checked)
				{
					option.NoCrop = true;
				}
			}
			catch
			{
				// Parse failure
				return null;
			}

			return option;
		}

		private void numericUpDown1234_Enter(object sender, EventArgs e)
		{
			this.radioNoNormal.Checked = true;
		}
	}
}
