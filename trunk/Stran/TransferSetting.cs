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
		public Data TravianData { get; set; }
		public int FromVillageID { get; set; }
		public MUI mui { get; set; }
		//public TResAmount ResAmount { get; private set; }
		//public Point TargetPos { get; private set; }
		public TransferOption Return { get; private set; }
		private int[] TargetResource = new int[4];
		private TVillage CV = null;
		private TVillage TV = null;
		public TransferSetting()
		{
			InitializeComponent();
		}

		private void TransferSetting_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			if(TravianData == null)
				return;
			CV = TravianData.Villages[FromVillageID];
			foreach(var v in TravianData.Villages)
				if(v.Key != FromVillageID)
					comboBox1.Items.Add(v.Key + " " + v.Value.Coord + " " + v.Value.Name);
				else
					comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
			numericUpDown1.Increment =
				numericUpDown2.Increment =
				numericUpDown3.Increment =
				numericUpDown4.Increment = CV.Market.SingleCarry;
			numericUpDown1.Maximum =
				numericUpDown2.Maximum =
				numericUpDown3.Maximum =
				numericUpDown4.Maximum = CV.Market.SingleCarry * CV.Market.ActiveMerchant;
			numericUpDown5.Maximum = CV.Market.ActiveMerchant;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int p1, p2;
			if(!int.TryParse(textBox1.Text, out p1))
				return;
			if(!int.TryParse(textBox2.Text, out p2))
				return;
			Return = new TransferOption()
			{
				ResourceAmount = new TResAmount(
				 Convert.ToInt32(numericUpDown1.Value),
				 Convert.ToInt32(numericUpDown2.Value),
				 Convert.ToInt32(numericUpDown3.Value),
				 Convert.ToInt32(numericUpDown4.Value)),
				TargetPos = new TPoint(p1, p2),
				VillageID = FromVillageID
			};
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Enabled = textBox2.Enabled = comboBox1.SelectedIndex == 0;
			if(comboBox1.SelectedIndex > 0) // && checkBox1.Checked
			{
				var vid = Convert.ToInt32((comboBox1.SelectedItem as string).Split(' ')[0]);
				if(!TravianData.Villages.ContainsKey(vid))
					return;
				TV = TravianData.Villages[vid];
				textBox1.Text = TV.X.ToString();
				textBox2.Text = TV.Y.ToString();
				var distance = CV.Coord * TV.Coord;
				if(TravianData.MarketSpeed == 0)
					TravianData.MarketSpeed = 24;
				var timecost = distance / TravianData.MarketSpeed;
				if(TV.isBuildingInitialized == 2)
				{
					TargetResource = new int[4];
					if(radioNormalTarget.Checked)
						for(int i = 0; i < 4; i++)
							TargetResource[i] =
								Convert.ToInt32(TV.Resource[i].Capacity - TV.Resource[i].CurrAmount - TV.Resource[i].Produce * timecost);
					else if(radioNormalMe.Checked)
						for(int i = 0; i < 4; i++)
							TargetResource[i] = CV.Resource[i].CurrAmount;
					if(checkBox1.Checked)
						TargetResource[3] = -2000000;
					numericUpDown5_ValueChanged(sender, e);
					return;
				}
			}
			TV = null;
			TargetResource = new int[] { 0, 0, 0, 0 };
			if(checkBox1.Checked)
				TargetResource[3] = -2000000;
			numericUpDown5_ValueChanged(sender, e);
		}
		static int[] calc(int[] TargetResource, int MCount, int SingleCarry)
		{
			int all;
			int[] Result = new int[4];
			all = 0;

			int min = Math.Min(TargetResource[0],
				Math.Min(TargetResource[1],
				Math.Min(TargetResource[2], TargetResource[3])));
			for(int i = 0; i < 4; i++)
				Result[i] = TargetResource[i] - min;
			foreach(var x in Result)
				all += x;
			if(all > MCount * SingleCarry)
			{
				do
				{
					int zerocount = 0;
					foreach(var x in Result)
						if(x == 0)
							zerocount++;
					int singleminus = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(all - MCount * SingleCarry) / (4 - zerocount)));
					Result[0] = Math.Max(0, Result[0] - singleminus);
					Result[1] = Math.Max(0, Result[1] - singleminus);
					Result[2] = Math.Max(0, Result[2] - singleminus);
					Result[3] = Math.Max(0,
						MCount * SingleCarry -
						Result[0] -
						Result[1] -
						Result[2]
						);
					all = 0;
					foreach(var x in Result)
						all += x;
				} while(all > MCount * SingleCarry);
			}
			else
			{
				int singleadd = (SingleCarry * MCount - all) / 4;
				Result[0] = Result[0] + singleadd;
				Result[1] = Result[1] + singleadd;
				Result[2] = Result[2] + singleadd;
				Result[3] = Result[3] + SingleCarry * MCount - all - singleadd * 3;
			}
			return Result;
		}
		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			//try
			//{
			int[] Result = calc(/*new int[]{
					Convert.ToInt32(numericUpDown1.Value),
					Convert.ToInt32(numericUpDown2.Value),
					Convert.ToInt32(numericUpDown3.Value),
					Convert.ToInt32(numericUpDown4.Value)},*/
				TargetResource,
				CV.Market.SingleCarry,
				Convert.ToInt32(numericUpDown5.Value));
			numericUpDown1.Value = Result[0];
			numericUpDown2.Value = Result[1];
			numericUpDown3.Value = Result[2];
			numericUpDown4.Value = Result[3];
			//}
			//catch(Exception)
			//{
			//}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			comboBox1_SelectedIndexChanged(sender, e);
		}

		private void numericUpDown1234_ValueChanged(object sender, EventArgs e)
		{
			var num = new int[]{
					Convert.ToInt32(numericUpDown1.Value),
					Convert.ToInt32(numericUpDown2.Value),
					Convert.ToInt32(numericUpDown3.Value),
					Convert.ToInt32(numericUpDown4.Value)};

			StringBuilder sb = new StringBuilder();
			if(TV != null)
			{
				int max = Math.Max(num[0],
					Math.Max(num[1],
					Math.Max(num[2], num[3])));

				int length1 = Math.Max(CV.Resource[0].Capacity, CV.Resource[3].Capacity).ToString().Length;
				int length2 = max.ToString().Length;
				int length3 = Math.Max(TV.Resource[0].Capacity, TV.Resource[3].Capacity).ToString().Length;

				string format = "{0," + length1.ToString() + "}/{1," + length1.ToString() + "} {2,3}% -> {3," + length2.ToString() + "} -> {4," + length3.ToString() + "}/{5," + length3.ToString() + "} {6,3}%";
				for(int i = 0; i < 4; i++)
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
			for(int i = 0; i < 4; i++)
				all += num[i];
			if(CV.Market.SingleCarry == 0)
				CV.Market.SingleCarry = 750;
			sb.AppendFormat(mui._("merchantsformat"), Convert.ToInt32(Math.Ceiling(Convert.ToDouble(all) / CV.Market.SingleCarry)), CV.Market.ActiveMerchant);
			label3.Text = sb.ToString();
		}

		private void radio_CheckedChanged(object sender, EventArgs e)
		{
			comboBox1_SelectedIndexChanged(sender, e);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				linkLabel1.LinkVisited = true;
				Process.Start("http://traplug.5d6d.com/thread-417-1-1.html");
			}
			catch(Exception)
			{
			}
		}
	}
}
