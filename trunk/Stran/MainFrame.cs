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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using libTravian;
using System.Threading;
using System.Net;
using Stran.DockingPanel;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;

namespace Stran
{
	public partial class MainFrame : UserControl
	{
		public TLoginInfo LoginInfo { get; set; }
		public TabPage UpTP { get; set; }
		public Data TravianData;

		private BuildingList m_buildinglist = new BuildingList();
		private InBuildingList m_inbuildinglist = new InBuildingList();
		private QueueList m_queuelist = new QueueList();
		private ResearchStatus m_researchstatus = new ResearchStatus();
		private ResourceShow m_resourceshow = new ResourceShow();
		private TransferStatus m_transferstatus = new TransferStatus();
		private VillageList m_villagelist = new VillageList();

		private delegate void StatusEvent_d(object sender, Travian.StatusChanged e);
		private delegate void LogEvent_d(TDebugInfo e);
		private delegate void Void_d();

		private static Color[] ResColor = new Color[] { Color.ForestGreen, Color.Chocolate, Color.SlateGray, Color.Gold };
		private static readonly Color RedBGColor = Color.FromArgb(255, 192, 192);
		private static readonly Color YellowBGColor = Color.FromArgb(255, 255, 192);
		public static string[] typelist = new string[] { "资源田", "建筑", "拆除", "攻击", "防御", "研究", "活动", "运输", "平仓" };

		private static object QueueLock = new object();

		private DisplayLang dl;
		public MUI mui { get; set; }

		ResourceLabel[] reslabel;
		Travian tr = null;
		int QueueCount = 0;
		int SelectVillage = 0;

		public MainFrame()
		{
			InitializeComponent();
			reslabel = new ResourceLabel[] { m_resourceshow.resourceLabel1, m_resourceshow.resourceLabel2, m_resourceshow.resourceLabel3, m_resourceshow.resourceLabel4 };
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
			//Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
			for (int i = 0; i < 7; i ++)
			{
				var lvi = m_inbuildinglist.listViewInBuilding.Items.Add(typelist[i]);
				lvi.SubItems.Add("");
			}
		}

		public void Login()
		{
			if(tr != null)
				tr = null;
			TravianData = new Data()
			{
				Username = LoginInfo.Username,
				Password = LoginInfo.Password,
				Tribe = LoginInfo.Tribe,
				Server = LoginInfo.Server
			};
			if(MainForm.Options.ContainsKey("proxy"))
			{
				string proxy = MainForm.Options["proxy"];
				TravianData.Proxy = new WebProxy(proxy);
			}
			tr = new Travian(TravianData, MainForm.Options);
			dl = new DisplayLang(LoginInfo.Language);
			tr.StatusUpdate += new EventHandler<Travian.StatusChanged>(tr_StatusUpdate);
			tr.OnError += new EventHandler<LogArgs>(tr_OnError);

			m_villagelist.listViewVillage.Items.Clear();
			m_buildinglist.listViewBuilding.Items.Clear();
			tr.FetchVillages();
			UpTP.Text = string.Format("{0} @ {1}", LoginInfo.Username, LoginInfo.Server.Replace("travian.", ""));
		}

		public void RefreshLanguage()
		{
			foreach(Control x in this.Controls)
			{
				if(x.Tag is string)
					x.Text = mui._(x.Tag as string);
			}
		}

		void tr_OnError(object sender, LogArgs e)
		{
			try
			{
				Invoke(new LogEvent_d(DebugWriteError), new object[] { e.DebugInfo });
			}
			catch(Exception)
			{ }
		}

		void tr_StatusUpdate(object sender, Travian.StatusChanged e)
		{
			try
			{
				Invoke(new StatusEvent_d(Local_StatusUpdate), new object[] { sender, e });
			}
			catch(Exception)
			{ }
		}

		void Local_StatusUpdate(object sender, Travian.StatusChanged e)
		{
			if(e.ChangedData == Travian.ChangedType.Villages)
			{
				if(e.VillageID == -1)
				{
					MessageBox.Show("Login failed.", "Stran");
					return;
				}
				if(LoginInfo.Tribe == 0 && LoginInfo.Tribe != TravianData.Tribe)
					LoginInfo.Tribe = TravianData.Tribe;

				//m_villagelist.listViewVillage.Items.Clear();
				int i = 0;
				List<int> f = new List<int>();
				foreach(ListViewItem x in m_villagelist.listViewVillage.Items)
				{
					f.Add(Convert.ToInt32(x.SubItems[0].Text));
				}

				foreach(var x in TravianData.Villages)
				{
					if(f.Contains(x.Key))
					{
						var xkey = m_villagelist.listViewVillage.Items[f.IndexOf(x.Value.ID)];
						if(xkey.SubItems[2].Text != x.Value.Name)
							xkey.SubItems[2].Text = x.Value.Name;
					}
					else
					{
						var lvi = m_villagelist.listViewVillage.Items.Add(x.Value.ID.ToString());
						string qcount = x.Value.GetStatus(this.tr.userdb);
						lvi.SubItems.Add(qcount);
						lvi.SubItems.Add(x.Value.Name);
						lvi.SubItems.Add(x.Value.Coord.ToString());
						lvi.SubItems.Add(x.Value.Role);
						if(e.VillageID == x.Key)
							i = m_villagelist.listViewVillage.Items.Count - 1;
					}
				}
				if(m_villagelist.listViewVillage.SelectedIndices.Count <= 0 && i >= 0)
					m_villagelist.listViewVillage.Items[i].Selected = true;
			}
			else if(e.ChangedData == Travian.ChangedType.Stop)
			{
				m_resourceshow.label5.BackColor = e.Param == 0 ? Color.Ivory : RedBGColor;
			}
			else if(e.ChangedData == Travian.ChangedType.Queue && e.Param == -1)
			{
				RestoreQueue(e.VillageID);

			}
			else if(e.VillageID == SelectVillage)
				switch(e.ChangedData)
				{
					case Travian.ChangedType.Buildings:
						DisplayBuildings();
						DisplayResource();
						DisplayInBuilding();
						RefreshBuildings();
						break;
					case Travian.ChangedType.Research:
						DisplayUpgrade();
						break;
					case Travian.ChangedType.Queue:
						RefreshQueue(e.Param);
						break;
				}
		}

		/// <summary>
		/// Extract CV.Queue from userdb[v#vid#Queue]
		/// </summary>
		/// <param name="VillageID">Village unqiue id</param>
		private void RestoreQueue(int VillageID)
		{
			TVillage CV = TravianData.Villages[VillageID];
			CV.RestoreQueue(this.tr.userdb);
		}

		private void RefreshBuildings()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			Color c_color;
			string c_text;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				foreach(ListViewItem lvi in m_buildinglist.listViewBuilding.Items)
				{
					int Bid = Convert.ToInt32(lvi.SubItems[0].Text);
					if(!CV.Buildings.ContainsKey(Bid))
						continue;
					var y = CV.Buildings[Bid];
					if(y == null)
						continue;
					if(!Buildings.CheckLevelFull(y.Gid, y.Level, CV.isCapital))
					{
						var timecost = CV.TimeCost(Buildings.Cost(y.Gid, y.Level + 1));
						if(timecost > 0)
						{
							c_color = RedBGColor;
							c_text = TimeToString(timecost);
						}
						else
						{
							c_color = YellowBGColor;
							c_text = mui._("available");
						}
						if(lvi.SubItems[1].BackColor != c_color)
							lvi.SubItems[1].BackColor = lvi.SubItems[2].BackColor = c_color;
						if(lvi.SubItems[2].Text != c_text)
							lvi.SubItems[2].Text = c_text;
					}
				}
			}
		}
		private void DisplayBuildings()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 0)
				CV.InitializeBuilding();
			else if(CV.isBuildingInitialized == 2)
			{
				m_buildinglist.listViewBuilding.Items.Clear();
				// Parse queue onto building
				int[] TL = new int[45]; // Less than 0 => CurrLevel-TL[], Greater than 0 => TL[], 0 => Destroy

				foreach(var Q in CV.Queue)
					if(Q.QueueType == TQueueType.Building && Q.Bid > 0) // Not AI
					{
						if(TL[Q.Bid] == -1024)
							continue;
						if(Q.QueueType == TQueueType.Destroy)
							TL[Q.Bid] = -1024;
						else if(Q.TargetLevel == 0)
							if(TL[Q.Bid] > 0)
								TL[Q.Bid]++;
							else
								TL[Q.Bid]--;
						else // build to level
							TL[Q.Bid] = Q.TargetLevel;
					}
				SortedDictionary<int, ListViewItem> lvid = new SortedDictionary<int, ListViewItem>();
				foreach(var x in CV.Buildings)
				{
					if(x.Value.Gid > 4)
						break;
					if(!Buildings.CheckLevelFull(x.Value.Gid, x.Value.Level, CV.isCapital))
					{
						var lvi = new ListViewItem(x.Key.ToString());
						lvi.UseItemStyleForSubItems = false;
						if(TL[x.Key] < 0 && TL[x.Key] > -1000)
							TL[x.Key] = x.Value.Level - TL[x.Key];
						string text;
						if(TL[x.Key] == 0)
							text = string.Format("{0} {1}", dl.GetGidLang(x.Value.Gid), x.Value.Level);
						//destroy
						else
						{
							if(TL[x.Key] == -1024)
								TL[x.Key] = 0;
							else if(TL[x.Key] < 0)
								TL[x.Key] = x.Value.Level - TL[x.Key];
							text = string.Format("{0} {1} => {2}", dl.GetGidLang(x.Value.Gid), x.Value.Level, TL[x.Key]);
						}
						if(x.Value.InBuilding)
							text += " <--";
						lvi.SubItems.Add(text);
						lvi.SubItems.Add("");
						if(x.Value.Gid <= 4)
						{
							lvi.SubItems[0].BackColor = ResColor[x.Value.Gid - 1];
							lvi.SubItems[0].ForeColor = Color.White;
						}
						lvid.Add((x.Value.Gid << 16) + (x.Value.Level << 8) + x.Key, lvi);
					}
				}
				foreach(var x in lvid)
					m_buildinglist.listViewBuilding.Items.Add(x.Value);
				foreach(var x in CV.Buildings)
				{
					if(x.Value.Gid <= 4)
						continue;
					var lvi = m_buildinglist.listViewBuilding.Items.Add(x.Key.ToString());
					lvi.UseItemStyleForSubItems = false;
					if(TL[x.Key] < 0 && TL[x.Key] > -1000)
						TL[x.Key] = x.Value.Level - TL[x.Key];
					string text;
					if(TL[x.Key] == 0)
						text = string.Format("{0} {1}", dl.GetGidLang(x.Value.Gid), x.Value.Level);
					//destroy
					else
					{
						if(TL[x.Key] == -1024)
							TL[x.Key] = 0;
						else if(TL[x.Key] < 0)
							TL[x.Key] = x.Value.Level - TL[x.Key];
						text = string.Format("{0} {1} => {2}", dl.GetGidLang(x.Value.Gid), x.Value.Level, TL[x.Key]);
					}
					if(x.Value.InBuilding)
						text += " <--";
					lvi.SubItems.Add(text);
					lvi.SubItems.Add("");
				}
			}
		}
		private void RefreshQueue(int QueueID)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				lock(QueueLock)
				{
					if(CV.Queue.Count == m_queuelist.listViewQueue.Items.Count - 1)
						m_queuelist.listViewQueue.Items.RemoveAt(QueueID);
				}
			}
		}

		/// <summary>
		/// Update Queue list view
		/// </summary>
		private void DisplayQueue()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;

			var CV = TravianData.Villages[SelectVillage];
			lock(QueueLock)
			{
				if(CV.Queue.Count != m_queuelist.listViewQueue.Items.Count)
				{
					m_queuelist.listViewQueue.Items.Clear();
					foreach(var x in CV.Queue)
					{
						ListViewItem lvi;
						if(x.Bid == TQueue.AIBID)
						{
							lvi = m_queuelist.listViewQueue.Items.Add("*");
							lvi.SubItems.Add("AI");
							if(x.Gid == 0)
								lvi.SubItems.Add("Amount");
							else
								lvi.SubItems.Add("Level");
						}
						else
						{
							lvi = m_queuelist.listViewQueue.Items.Add(x.Bid <= 0 ? "*" : x.Bid.ToString());
							lvi.SubItems.Add(typelist[x.Type]);
							if (x.Type < 3)
							{
								lvi.SubItems.Add(dl.GetGidLang(x.Gid));
							}
							else if (x.Type < 6)
							{
								lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, x.Bid));
							}
							else if (x.Type == (int) TQueueType.Party)
							{
								lvi.SubItems.Add(x.Bid == 1 ? "500" : "2000");
							}
							else if (x.Type == (int)TQueueType.Transfer)
							{
								TransferOption transferOption = TransferOption.FromString(x.ExtraOptions);
								lvi.SubItems.Add(transferOption.GetTitle(TravianData));
								x.Status = transferOption.Status;
							}
							else if (x.Type == (int)TQueueType.NpcTrade)
							{
								NpcTradeOption npcTradeOption = NpcTradeOption.FromString(x.ExtraOptions);
								lvi.SubItems.Add(npcTradeOption.Title);
								x.Status = npcTradeOption.Status;
							}
						}

						lvi.SubItems.Add(x.Status);
						lvi.SubItems.Add("");//tr.GetDelay(SelectVillage, x).ToString());
					}
				}
				else
				{
					bool[] status = new bool[10];
					for(int i = 0; i < CV.Queue.Count; i++)
					{
						TQueue x = CV.Queue[i];
						var lvi = m_queuelist.listViewQueue.Items[i];
						if(lvi.SubItems[3].Text != x.Status)
							lvi.SubItems[3].Text = x.Status;
						int ntype = TravianData.isRomans ? x.Type : 0;
						if(x.Type >= 2)
							ntype = x.Type;

						string delayStr = String.Empty;
						if (x.Paused)
						{
							delayStr = "||";
						}
						else if(x.QueueType == TQueueType.Transfer || ! status[ntype])
						{
							int n = tr.GetDelay(SelectVillage, x);
							if (n > 0)
							{
								delayStr = this.TimeToString(n);
							}

							status[ntype] = true;
						}

						if (lvi.SubItems[4].Text != delayStr)
						{
							lvi.SubItems[4].Text = delayStr;
						}
					}
				}
			}
		}

		private void DisplayInBuilding()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			string c_text;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				for(int i = 0; i < CV.InBuilding.Length; i++)
				{
					var x = TravianData.isRomans || i >= 2 ? CV.InBuilding[i] : CV.InBuilding[0];
					if(x == null || x.FinishTime < DateTime.Now || !TravianData.isRomans && i < 2 && i == (x.Gid > 5 ? 0 : 1))
						c_text = "";
					else
					{
						TimeSpan ts = x.FinishTime.Subtract(DateTime.Now);

						if(i < 3)
						{
							c_text = dl.GetGidLang(x.Gid);
							//if(x.ABid != 0)
							//	c_text += " [" + x.ABid.ToString() + "]";
						}
						else if(i < 6)
							c_text = dl.GetAidLang(TravianData.Tribe, x.ABid);
						else
							c_text = "";
						c_text += string.Format(" {0} {1:0}:{2:00}:{3:00} -> {4}",
							x.Level,
							Math.Floor(ts.TotalHours), ts.Minutes, ts.Seconds,
							x.FinishTime.ToLongTimeString());
					}
					//int j = i;
					//if(x != null&&i < 2 && x.Gid > 4 && !TravianData.isRomans)
					//	j = 1;
					if(m_inbuildinglist.listViewInBuilding.Items[i].SubItems[1].Text != c_text)
					{
						m_inbuildinglist.listViewInBuilding.Items[i].SubItems[1].Text = c_text;
						//if(!TravianData.isRomans && m_inbuildinglist.listViewInBuilding.Items[(j + 1) % 2].SubItems[1].Text != "")
						//	m_inbuildinglist.listViewInBuilding.Items[(j + 1) % 2].SubItems[1].Text = "";
					}
				}
			}
		}
		private void DisplayResource()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CVRes = TravianData.Villages[SelectVillage].Resource;
			if(TravianData.Villages[SelectVillage].isBuildingInitialized == 2)
				for(int i = 0; i < 4; i++)
				{
					reslabel[i].Display(CVRes[i]);
					/*
					reslabel[i].Text = string.Format("{0}/{1}\n({2:0}, {3}:{4:00}:{5:00})\n({6}, {7:F2}%)",
						CVRes[i].CurrAmount,
						CVRes[i].Capacity,
						CVRes[i].Produce,
						Math.Floor(CVRes[i].LeftTime.TotalHours),
						CVRes[i].LeftTime.Minutes,
						CVRes[i].LeftTime.Seconds,
						CVRes[i].Capacity - CVRes[i].CurrAmount,
						CVRes[i].CurrAmount * 100.0 / CVRes[i].Capacity
						);
					*/
				}
		}
		private void DisplayUpgrade()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			m_researchstatus.listViewUpgrade.Items.Clear();
			if(CV.isUpgradeInitialized != 2)
				return;
			foreach(var x in CV.Upgrades)
			{
				var lvi = m_researchstatus.listViewUpgrade.Items.Add(dl.GetAidLang(TravianData.Tribe, x.Key));
				lvi.UseItemStyleForSubItems = false;
				#region Research
				if(x.Value.Researched)
				{
					lvi.SubItems.Add(mui._("finished"));
					lvi.SubItems[1].BackColor = Color.White;
				}
				else if(x.Value.CanResearch)
				{
					if(x.Value.InUpgrading)
					{
						lvi.SubItems.Add(mui._("upgrading"));
						lvi.SubItems[1].BackColor = Color.White;
					}
					else
					{
						int TimeCost = CV.TimeCost(Buildings.ResearchCost[(TravianData.Tribe - 1) * 10 + x.Key]);
						if(TimeCost > 0)
						{
							lvi.SubItems.Add(TimeToString(TimeCost));
							lvi.SubItems[1].BackColor = RedBGColor;
						}
						else
						{
							lvi.SubItems.Add(mui._("available"));
							lvi.SubItems[1].BackColor = YellowBGColor;
						}
					}
				}
				else
				{
					lvi.SubItems.Add(mui._("notavailable"));
					lvi.SubItems[1].BackColor = Color.White;
					lvi.SubItems[1].ForeColor = Color.DarkRed;
				}
				#endregion
				#region Attack
				if(x.Value.AttackLevel >= 20)
				{
					lvi.SubItems.Add(mui._("finished"));
					lvi.SubItems[2].BackColor = Color.White;
				}
				else if(x.Value.Researched && x.Value.AttackLevel < CV.BlacksmithLevel && x.Value.AttackLevel >= 0)
				{
					int TimeCost = CV.TimeCost(Buildings.UpCost[(TravianData.Tribe - 1) * 10 + x.Key][x.Value.AttackLevel]);
					if(TimeCost > 0)
					{
						lvi.SubItems.Add(TimeToString(TimeCost));
						lvi.SubItems[2].BackColor = RedBGColor;
					}
					else
					{
						lvi.SubItems.Add(mui._("available"));
						lvi.SubItems[2].BackColor = YellowBGColor;
					}
				}
				else
				{
					lvi.SubItems.Add(mui._("notavailable"));
					lvi.SubItems[2].BackColor = Color.White;
					lvi.SubItems[2].ForeColor = Color.DarkRed;
				}
				if(x.Value.AttackLevel >= 0 && x.Value.AttackLevel < 20)
					lvi.SubItems[2].Text = x.Value.AttackLevel.ToString() + " " + lvi.SubItems[2].Text;
				#endregion
				#region Defense
				if(x.Value.DefenceLevel >= 20)
				{
					lvi.SubItems.Add(mui._("finished"));
					lvi.SubItems[3].BackColor = Color.White;
				}
				else if(x.Value.Researched && x.Value.DefenceLevel < CV.ArmouryLevel && x.Value.DefenceLevel >= 0)
				{
					int TimeCost = CV.TimeCost(Buildings.UpCost[(TravianData.Tribe - 1) * 10 + x.Key][x.Value.DefenceLevel]);
					if(TimeCost > 0)
					{
						lvi.SubItems.Add(TimeToString(TimeCost));
						lvi.SubItems[3].BackColor = RedBGColor;
					}
					else
					{
						lvi.SubItems.Add(mui._("available"));
						lvi.SubItems[3].BackColor = YellowBGColor;
					}
				}
				else
				{
					lvi.SubItems.Add(mui._("notavailable"));
					lvi.SubItems[3].BackColor = Color.White;
					lvi.SubItems[3].ForeColor = Color.DarkRed;
				}
				if(x.Value.DefenceLevel >= 0 && x.Value.DefenceLevel < 20)
					lvi.SubItems[3].Text = x.Value.DefenceLevel.ToString() + " " + lvi.SubItems[3].Text;
				#endregion
			}
		}
		private void DisplayMarket()
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized != 2)
				return;
			m_transferstatus.listViewMarket.Items.Clear();
			m_transferstatus.listViewMarket.SuspendLayout();
			foreach(var x in CV.Market.MarketInfo)
			{
				var lvi = m_transferstatus.listViewMarket.Items.Add(TimeToString(Convert.ToInt32(x.FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5));
				lvi.SubItems.Add(x.CarryAmount.ToString());
				lvi.SubItems.Add(x.VillageName);
				lvi.SubItems.Add(x.Coord.ToString());
				lvi.SubItems.Add(x.MType.ToString());
			}
			m_transferstatus.listViewMarket.ResumeLayout();
		}
		private string TimeToString(long timecost)
		{
			if(timecost >= 86400)
				return "∞";
			TimeSpan ts = new TimeSpan(timecost * 10000000);
			return ts.ToString();
		}

		public void listViewVillage_Changed(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(m_villagelist.listViewVillage.SelectedIndices.Count == 1)
				SelectVillage = Convert.ToInt32(m_villagelist.listViewVillage.SelectedItems[0].Text);
			if(TravianData.Villages == null)
				return;
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			m_buildinglist.listViewBuilding.Items.Clear();
			m_queuelist.listViewQueue.Items.Clear();
			foreach(var l in reslabel)
				l.Clear();
			var CV = TravianData.Villages[SelectVillage];
			m_resourceshow.label5.Text = string.Format("{0} ({1}|{2})", CV.Name, CV.Coord.X, CV.Coord.Y);
			DisplayBuildings();
			DisplayResource();
			DisplayInBuilding();
			RefreshBuildings();
			DisplayQueue();
			DisplayUpgrade();
			DisplayMarket();
		}

		public void listViewVillage_Click(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(SelectVillage < 0)
				return;
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			listViewVillage_Changed(sender, e);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if(tr != null)
			{
				DisplayInBuilding();
				DisplayResource();
				RefreshBuildings();
				DisplayQueue();
				DisplayMarket();
				tr.Tick();
				int QCount = 0;
				foreach(var x in TravianData.Villages)
					if(x.Value.isBuildingInitialized > 1)
						QCount += x.Value.Queue.Count;
				if(QueueCount != QCount)
				{
					QueueCount = QCount;
					UpTP.Text = string.Format("{0} @ {1} ({2})", LoginInfo.Username, LoginInfo.Server.Replace("travian.", ""), QueueCount);
				}
			}

			foreach(ListViewItem x in m_villagelist.listViewVillage.Items)
			{
				int index = Convert.ToInt32(x.SubItems[0].Text);
				if(TravianData.Villages.ContainsKey(index))
				{
					string qcount = this.TravianData.Villages[index].GetStatus(this.tr.userdb);
					if (x.SubItems[1].Text != qcount)
						x.SubItems[1].Text = qcount;
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(tr == null ||
				QueueCount == 0 ||
				MessageBox.Show(mui._("reallyclosetext"), mui._("reallyclosecap"),
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				(UpTP.Parent as TabControl).TabPages.Remove(UpTP);
				timer1.Enabled = false;
				UpTP.Dispose();
			}
		}

		private void DebugWriteError(TDebugInfo DB)
		{
			string str = string.Format("[{0} {1}][{2}]{3,18}@{4,-35}:{5,-3} {6}",
				DB.Time.Day,
				DB.Time.ToLongTimeString(),
				DB.Level.ToString(),
				DB.MethodName,
				DB.Filename.Substring(16),
				DB.Line,
				DB.Text);
			textBox1.AppendText(str + "\r\n");
			LastDebug.Text = str;
		}

		void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("请您复制错误信息并稍后张贴到论坛，谢谢。");
			sb.AppendLine("Please copy the error message and post to developers' forum later.");
			sb.AppendLine(DateTime.Now.ToString());
			sb.AppendLine(ex.Message);
			sb.AppendLine(ex.StackTrace);
			sb.AppendLine(MainForm.VERSION);

			foreach(var DB in tr.DebugList)
			{
				sb.AppendFormat("[{0}][{1}]{2} of {3}:{4}\t{5}\r\n",
					DB.Time.ToString(),
					DB.Level,
					DB.MethodName,
					DB.Filename.Replace(@"H:\app\Stravian\", ""),
					DB.Line,
					DB.Text);
			}

			MsgBox fe = new MsgBox() { message = sb.ToString() };
			fe.ShowDialog();
		}

		private void MainFrame_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			//throw new Exception("a");
			
			m_buildinglist.UpCall =
				m_queuelist.UpCall =
				m_researchstatus.UpCall =
				m_transferstatus.UpCall =
				m_resourceshow.UpCall =
				m_inbuildinglist.UpCall =
				m_villagelist.UpCall = this;


			string fn = "style\\" + LoginInfo.Username + "@" + LoginInfo.Server + "!style.xml";
			if(!File.Exists(fn))
				fn = "style\\default!style.xml";
			SuspendLayout();
			if(File.Exists(fn))
				dockPanel1.LoadFromXml(fn, new DeserializeDockContent(FindDocument));
			else
			{
				m_resourceshow.Show(dockPanel1);
				m_inbuildinglist.Show(dockPanel1);
				m_queuelist.Show(dockPanel1);
				m_buildinglist.Show(dockPanel1);
				m_transferstatus.Show(dockPanel1);
				m_researchstatus.Show(dockPanel1);
				m_villagelist.Show(dockPanel1);
			}
			ResumeLayout();
		}

		private IDockContent FindDocument(string text)
		{
			foreach(var x in new DockContent[] { m_buildinglist, m_inbuildinglist, m_queuelist, m_researchstatus, m_resourceshow, m_transferstatus, m_villagelist })
			{
				if(text == x.GetType().ToString())
					return x;
			}
			return null;
		}


		#region CMV
		private void CMV_Opening(object sender, CancelEventArgs e)
		{
			bool Enabled = CMVSnapshot.Enabled = CMVRefresh.Enabled = CMVRole.Enabled = m_villagelist.listViewVillage.SelectedItems.Count == 1;
			List<string> RoleList = new List<string>();
			if(Enabled)
			{
				if(!TravianData.Villages.ContainsKey(SelectVillage))
					return;
				foreach(var x in TravianData.Villages)
					if(!RoleList.Contains(x.Value.Role))
						RoleList.Add(x.Value.Role);
				CMVRoleText.Items.Clear();
				RoleList.ForEach(s => { CMVRoleText.Items.Add(s); });
				CMVRoleText.Text = TravianData.Villages[SelectVillage].Role;
			}
		}

		private void CMVRefresh_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			//int index = Convert.ToInt32(m_villagelist.listViewVillage.SelectedItems[0].SubItems[0].Text);
			TravianData.Villages[SelectVillage].InitializeBuilding();
			TravianData.Villages[SelectVillage].InitializeDestroy();
			TravianData.Villages[SelectVillage].InitializeUpgrade();
		}

		/// <summary>
		/// Set village resource low bound for outgoing transportation
		/// </summary>
		private void CMVLowerLimit_Click(object sender, EventArgs e)
		{
			if (!TravianData.Villages.ContainsKey(SelectVillage))
			{
				return;
			}

			TVillage village = this.TravianData.Villages[SelectVillage];
			if (village.isBuildingInitialized != 2)
			{
				return;
			}

			ResourceLimit limit = new ResourceLimit()
			{
				Village = village,
				Description = mui._("lowerlimit"),
				Limit = village.Market.LowerLimit == null ? new TResAmount(0, 0, 0, 0) : village.Market.LowerLimit,
				mui = this.mui
			};

			if (limit.ShowDialog() == DialogResult.OK && limit.Return != null)
			{
				village.Market.LowerLimit = limit.Return;
				village.SaveResourceLimits(this.tr.userdb);
			}
		}

		/// <summary>
		/// Set village resource upper bound for incoming transportation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CMVUpperLimit_Click(object sender, EventArgs e)
		{
			if (!TravianData.Villages.ContainsKey(SelectVillage))
			{
				return;
			}

			TVillage village = this.TravianData.Villages[SelectVillage];
			if (village.isBuildingInitialized != 2)
			{
				return;
			}

			ResourceLimit limit = new ResourceLimit()
			{
				Village = village,
				Description = mui._("upperlimit"),
				Limit = village.Market.UpperLimit == null ? village.ResourceCapacity : village.Market.UpperLimit,
				mui = this.mui
			};

			if (limit.ShowDialog() == DialogResult.OK && limit.Return != null)
			{
				village.Market.UpperLimit = limit.Return;
				village.SaveResourceLimits(this.tr.userdb);
			}
		}

		private void CMVSnapshot_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("这是一份村庄快照。如有需要，请您复制并稍后张贴到论坛，谢谢。");
			sb.AppendLine("This is a snapshot of your village. If necessary, please copy this and post to developers' forum later.");
			sb.AppendLine(TravianData.Villages[SelectVillage].Snapshot());
			MsgBox mb = new MsgBox() { message = sb.ToString() };
			mb.ShowDialog();
		}
		private void CMVSnapAll_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("这是一份村庄快照。如有需要，请您复制并稍后张贴到论坛，谢谢。");
			sb.AppendLine("This is a snapshot of your village. If necessary, please copy this and post to developers' forum later.");
			foreach(var v in TravianData.Villages)
				sb.AppendLine(v.Value.Snapshot());
			MsgBox mb = new MsgBox() { message = sb.ToString() };
			mb.ShowDialog();
		}
		private void CMVRoleText_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				if(!TravianData.Villages.ContainsKey(SelectVillage))
					return;
				tr.userdb["v" + SelectVillage + "role"] = TravianData.Villages[SelectVillage].Role = CMVRoleText.Text;
				foreach(ListViewItem x in m_villagelist.listViewVillage.Items)
				{
					if(x.SubItems[0].Text == SelectVillage.ToString())
					{
						x.SubItems[4].Text = CMVRoleText.Text;
						break;
					}
				}
			}
		}
		private void CMVRoleText_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			tr.userdb["v" + SelectVillage + "role"] = TravianData.Villages[SelectVillage].Role = CMVRoleText.SelectedItem as string;
			foreach(ListViewItem x in m_villagelist.listViewVillage.Items)
			{
				if(x.SubItems[0].Text == SelectVillage.ToString())
				{
					x.SubItems[4].Text = CMVRoleText.Text;
					break;
				}
			}
		}
		#endregion

		#region CMB
		private void CMBUp_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_buildinglist.listViewBuilding.SelectedItems.Count == 0)
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				for(int i = 0; i < m_buildinglist.listViewBuilding.SelectedItems.Count; i++)
				{
					int temp;
					if(!int.TryParse(m_buildinglist.listViewBuilding.SelectedItems[i].Text, out temp))
						continue;
					int Bid = Convert.ToInt32(m_buildinglist.listViewBuilding.SelectedItems[i].Text);
					if(Buildings.CheckLevelFull(CV.Buildings[Bid].Gid, CV.Buildings[Bid].Level, CV.isCapital))
						continue;
					CV.Queue.Add(new TQueue()
					{
						Bid = Bid,
						Gid = CV.Buildings[Bid].Gid,
						QueueType = TQueueType.Building
					});
					CV.SaveQueue(tr.userdb);
					int j = CV.Queue.Count - 1;
					ListViewItem lvi = m_queuelist.listViewQueue.Items.Add(CV.Queue[j].Bid.ToString());
					lvi.SubItems.Add(typelist[CV.Queue[j].Type]);
					lvi.SubItems.Add(dl.GetGidLang(CV.Queue[j].Gid));
					lvi.SubItems.Add("");
					lvi.SubItems.Add("");

					m_buildinglist.listViewBuilding.Items[m_buildinglist.listViewBuilding.SelectedIndices[i]].SubItems[1].Text += "*";
				}
			}
		}
		private void CMBUp2_Click(object sender, EventArgs e)
		{
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
		}
		private void CMBUp5_Click(object sender, EventArgs e)
		{
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
		}
		private void CMBUp9_Click(object sender, EventArgs e)
		{
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
			CMBUp_Click(sender, e);
		}
		private void CMBUpTo_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_buildinglist.listViewBuilding.SelectedItems.Count == 0)
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(TravianData.Villages[SelectVillage].isBuildingInitialized == 2)
			{
				for(int i = 0; i < m_buildinglist.listViewBuilding.SelectedItems.Count; i++)
				{
					int temp;
					if(!int.TryParse(m_buildinglist.listViewBuilding.SelectedItems[i].Text, out temp))
						continue;
					int Bid = Convert.ToInt32(m_buildinglist.listViewBuilding.SelectedItems[i].Text);
					int Gid = CV.Buildings[Bid].Gid;
					if(CV.Buildings[Bid].Level >= Buildings._cost[Gid].data.Length - 1)
						continue;
					BuildToLevel btl = new BuildToLevel()
					{
						BuildingName = tr.GetGidLang(Gid),
						DisplayName = dl.GetGidLang(Gid),
						CurrentLevel = CV.Buildings[Bid].Level,
						TargetLevel = Buildings._cost[Gid].data.Length - 1,
						mui = mui
					};
					if(btl.ShowDialog() == DialogResult.OK)
					{
						if(btl.Return < 0)
							continue;
						TQueue Q = new TQueue()
						{
							Bid = Bid,
							Gid = CV.Buildings[Bid].Gid,
							QueueType = TQueueType.Building,
							Status = " -> " + btl.Return.ToString(),
							TargetLevel = btl.Return
						};
						CV.Queue.Add(Q);
						CV.SaveQueue(tr.userdb);
						//int j = CV.Queue.Count - 1;
						ListViewItem lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
						lvi.SubItems.Add(typelist[Q.Type]);
						lvi.SubItems.Add(dl.GetGidLang(Q.Gid));
						lvi.SubItems.Add(Q.Status);
						lvi.SubItems.Add("");

						if(m_buildinglist.listViewBuilding.SelectedItems.Count > i)
							m_buildinglist.listViewBuilding.SelectedItems[i].SubItems[1].Text += "!";
					}
				}
			}
		}
		private void CMBDestroy_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_buildinglist.listViewBuilding.SelectedItems.Count == 0)
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(TravianData.Villages[SelectVillage].isBuildingInitialized == 2)
			{
				for(int i = 0; i < m_buildinglist.listViewBuilding.SelectedItems.Count; i++)
				{
					int temp;
					if(!int.TryParse(m_buildinglist.listViewBuilding.SelectedItems[i].Text, out temp))
						continue;
					int Bid = Convert.ToInt32(m_buildinglist.listViewBuilding.SelectedItems[i].Text);
					if(Bid < 19)
						continue;
					CV.Queue.Add(new TQueue()
					{
						Bid = Bid,
						Gid = CV.Buildings[Bid].Gid,
						QueueType = TQueueType.Destroy,
						Status = " -> 0",
						TargetLevel = 0
					});
					CV.SaveQueue(tr.userdb);
					int j = CV.Queue.Count - 1;
					ListViewItem lvi = m_queuelist.listViewQueue.Items.Add(CV.Queue[j].Bid.ToString());
					lvi.SubItems.Add(typelist[CV.Queue[j].Type]);
					lvi.SubItems.Add(dl.GetGidLang(CV.Queue[j].Gid));
					lvi.SubItems.Add(CV.Queue[j].Status);
					lvi.SubItems.Add("");

					m_buildinglist.listViewBuilding.Items[m_buildinglist.listViewBuilding.SelectedIndices[i]].SubItems[1].Text += "X";
				}
			}
		}
		private void CMBNew_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(TravianData.Villages[SelectVillage].isBuildingInitialized == 2)
			{
				NewBuilding nb = new NewBuilding(TravianData, SelectVillage, dl) { mui = mui };
				if(nb.ShowDialog() == DialogResult.OK)
				{
					CV.Queue.Add(new TQueue()
					{
						Bid = nb.OutBid,
						Gid = nb.OutGid,
						Status = "+",
						QueueType = TQueueType.Building
					});
					CV.SaveQueue(tr.userdb);
					int j = CV.Queue.Count - 1;
					ListViewItem lvi = m_queuelist.listViewQueue.Items.Add(CV.Queue[j].Bid.ToString());
					lvi.SubItems.Add(typelist[CV.Queue[j].Type]);
					lvi.SubItems.Add(dl.GetGidLang(CV.Queue[j].Gid));
					lvi.SubItems.Add(CV.Queue[j].Status);
					lvi.SubItems.Add("");

					CV.Buildings[nb.OutBid] = new TBuilding() { Gid = nb.OutGid };
				}
			}
			/*
			if(selectindex < 0)
				return;
			 */
		}
		private void CMBAI_C_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				CV.Queue.Add(new TQueue()
				{
					Bid = TQueue.AIBID,
					Gid = 0,
					Status = "N/A",
					QueueType = TQueueType.Building
				});
				CV.SaveQueue(tr.userdb);
				ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("*");
				lvi.SubItems.Add("AI");
				lvi.SubItems.Add("Amount");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
			}

		}
		private void CMBAI_L_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				CV.Queue.Add(new TQueue()
				{
					Bid = TQueue.AIBID,
					Gid = 1,
					Status = "N/A",
					QueueType = TQueueType.Building
				});
				CV.SaveQueue(tr.userdb);
				ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("*");
				lvi.SubItems.Add("AI");
				lvi.SubItems.Add("Level");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
			}

		}
		private void CMBRefresh_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			TravianData.Villages[SelectVillage].InitializeBuilding();
		}
		private void CMBRefreshDestroy_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			TravianData.Villages[SelectVillage].InitializeDestroy();
		}
		private void CMBParty500_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			CV.Queue.Add(new TQueue()
			{
				Bid = 1,
				Gid = 0,
				Status = "",
				QueueType = TQueueType.Party
			});
			CV.SaveQueue(tr.userdb);
			ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("1");
			lvi.SubItems.Add(typelist[6]);
			lvi.SubItems.Add("500");
			lvi.SubItems.Add("");
			lvi.SubItems.Add("");
		}
		private void CMBParty2000_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			CV.Queue.Add(new TQueue()
			{
				Bid = 2,
				Gid = 0,
				Status = "",
				QueueType = TQueueType.Party
			});
			CV.SaveQueue(tr.userdb);
			ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("2");
			lvi.SubItems.Add(typelist[6]);
			lvi.SubItems.Add("2000");
			lvi.SubItems.Add("");
			lvi.SubItems.Add("");
		}
		#endregion

		#region CMQ
		private void CMQDel_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_queuelist.listViewQueue.SelectedIndices.Count == 0)
				return;
			lock(QueueLock)
			{
				var CV = TravianData.Villages[SelectVillage];
				if(CV.isBuildingInitialized == 2)
				{
					for(int i = m_queuelist.listViewQueue.SelectedIndices.Count - 1; i >= 0; i--)
					{
						int QID = m_queuelist.listViewQueue.SelectedIndices[i];
						if(CV.Queue.Count > QID)
						{
							CV.Queue.RemoveAt(QID);
							CV.SaveQueue(tr.userdb);
						}
						if(m_queuelist.listViewQueue.Items.Count > QID)
							m_queuelist.listViewQueue.Items.RemoveAt(QID);
					}
				}
			}
		}
		private void CMQClear_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			lock(QueueLock)
			{
				var CV = TravianData.Villages[SelectVillage];
				if(CV.isBuildingInitialized == 2)
				{
					CV.Queue.Clear();
					CV.SaveQueue(tr.userdb);
					m_queuelist.listViewQueue.Items.Clear();
				}
			}
		}
		private void CMQUp_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_queuelist.listViewQueue.SelectedIndices.Count == 0 || m_queuelist.listViewQueue.SelectedIndices[0] == 0)
				return;
			lock(QueueLock)
			{
				var CV = TravianData.Villages[SelectVillage];
				if(CV.isBuildingInitialized == 2)
				{
					for(int i = 0; i < m_queuelist.listViewQueue.SelectedIndices.Count; i++)
					{
						int n = m_queuelist.listViewQueue.SelectedIndices[i];
						CV.Queue.Reverse(n - 1, 2);
						ListViewItem tlvi = m_queuelist.listViewQueue.Items[n - 1];
						m_queuelist.listViewQueue.Items.RemoveAt(n - 1);
						m_queuelist.listViewQueue.Items.Insert(n, tlvi);
					}
					CV.SaveQueue(tr.userdb);
				}
			}
		}
		private void CMQDown_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_queuelist.listViewQueue.SelectedIndices.Count == 0 || m_queuelist.listViewQueue.SelectedIndices[m_queuelist.listViewQueue.SelectedIndices.Count - 1] == m_queuelist.listViewQueue.Items.Count - 1)
				return;
			lock(QueueLock)
			{
				var CV = TravianData.Villages[SelectVillage];
				if(CV.isBuildingInitialized == 2)
				{
					for(int i = m_queuelist.listViewQueue.SelectedIndices.Count - 1; i >= 0; i--)
					//for(int i = 0; i < m_queuelist.listViewQueue.SelectedIndices.Count; i++)
					{
						int n = m_queuelist.listViewQueue.SelectedIndices[i];
						CV.Queue.Reverse(n, 2);
						ListViewItem tlvi = m_queuelist.listViewQueue.Items[n + 1];
						m_queuelist.listViewQueue.Items.RemoveAt(n + 1);
						m_queuelist.listViewQueue.Items.Insert(n, tlvi);
					}
					CV.SaveQueue(tr.userdb);
				}
			}
		}

		/// <summary>
		/// Pause/resume the selected task
		/// </summary>
		private void CMQPause_Click(object sender, EventArgs e)
		{
			if (!this.TravianData.Villages.ContainsKey(this.SelectVillage))
			{
				return;
			}

			if (m_queuelist.listViewQueue.SelectedIndices.Count == 0)
			{
				return;
			}

			TVillage village = this.TravianData.Villages[this.SelectVillage];
			if (village.isBuildingInitialized == 2)
			{
				lock (QueueLock)
				{
					foreach (int i in m_queuelist.listViewQueue.SelectedIndices)
					{
						TQueue task = village.Queue[i];
						task.Paused = !task.Paused;
					}

					village.SaveQueue(this.tr.userdb);
				}
			}

			this.DisplayQueue();
		}

		private void CMQTimer_Click(object sender, EventArgs e)
		{
			CMQTimer.Checked = !CMQTimer.Checked;
			timer1.Enabled = CMQTimer.Checked;
		}

		/// <summary>
		/// Export the villag task queue to a text file
		/// </summary>
		private void CMQImport_Click(object sender, EventArgs e)
		{
			if (!this.TravianData.Villages.ContainsKey(this.SelectVillage))
			{
				return;
			}

			TVillage village = this.TravianData.Villages[this.SelectVillage];
			if (village.isBuildingInitialized != 2)
			{
				return;
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Filter = "Stran task queue|*.stq";
			openFileDialog.Title = this.mui._("ImportTaskQueue");
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				lock (QueueLock)
				{
					village.RestoreQueue(openFileDialog.FileName);
				}
			}
		}

		/// <summary>
		/// Import the villag task queue from a previously saved text file
		/// </summary>
		private void CMQExport_Click(object sender, EventArgs e)
		{
			if (! this.TravianData.Villages.ContainsKey(this.SelectVillage))
			{
				return;
			}

			TVillage village = this.TravianData.Villages[this.SelectVillage];
			if (village.isBuildingInitialized != 2)
			{
				return;
			}

			lock (QueueLock)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.Filter = "Stran task queue|*.stq";
				saveFileDialog.Title = this.mui._("ExportTaskQueue");
				saveFileDialog.ShowDialog();
				if (saveFileDialog.FileName != "")
				{
					village.SaveQueue(saveFileDialog.FileName);
				}
			}
		}
		#endregion

		#region CMR
		private void CMR_Opening(object sender, CancelEventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			CMRResearch.Enabled = false;
			CMRUpgradeAtk.Enabled = false;
			CMRUpgradeAtkTo.Enabled = false;
			CMRUpgradeDef.Enabled = false;
			CMRUpgradeDefTo.Enabled = false;
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				CMRResearch.Enabled |= x.SubItems[1].BackColor != Color.White;
				CMRUpgradeAtkTo.Enabled = CMRUpgradeAtk.Enabled |= x.SubItems[2].BackColor != Color.White;
				CMRUpgradeDefTo.Enabled = CMRUpgradeDef.Enabled |= x.SubItems[3].BackColor != Color.White;
			}
		}
		private void CMRResearch_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				if(x.SubItems[1].BackColor != Color.White)
				{
					TQueue Q = new TQueue()
					{
						QueueType = TQueueType.Research,
						Bid = m_researchstatus.listViewUpgrade.Items.IndexOf(x) + 1
					};
					TravianData.Villages[SelectVillage].Queue.Add(Q);
					TravianData.Villages[SelectVillage].SaveQueue(tr.userdb);
					var lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
					lvi.SubItems.Add(typelist[Q.Type]);
					lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, Q.Bid));
					lvi.SubItems.Add(Q.Status);
					lvi.SubItems.Add("");
				}
			}
		}
		private void CMRUpgradeAtk_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				if(x.SubItems[2].BackColor != Color.White)
				{
					TQueue Q = new TQueue()
					{
						QueueType = TQueueType.UAttack,
						Bid = m_researchstatus.listViewUpgrade.Items.IndexOf(x) + 1
					};
					TravianData.Villages[SelectVillage].Queue.Add(Q);
					TravianData.Villages[SelectVillage].SaveQueue(tr.userdb);
					var lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
					lvi.SubItems.Add(typelist[Q.Type]);
					lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, Q.Bid));
					lvi.SubItems.Add(Q.Status);
					lvi.SubItems.Add("");
				}
			}
		}
		private void CMRUpgradeDef_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				if(x.SubItems[3].BackColor != Color.White)
				{
					TQueue Q = new TQueue()
					{
						QueueType = TQueueType.UDefense,
						Bid = m_researchstatus.listViewUpgrade.Items.IndexOf(x) + 1
					};
					TravianData.Villages[SelectVillage].Queue.Add(Q);
					TravianData.Villages[SelectVillage].SaveQueue(tr.userdb);
					var lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
					lvi.SubItems.Add(typelist[Q.Type]);
					lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, Q.Bid));
					lvi.SubItems.Add(Q.Status);
					lvi.SubItems.Add("");
				}
			}
		}
		private void CMRUpgradeAtkTo_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				if(x.SubItems[2].BackColor != Color.White)
				{
					int Bid = m_researchstatus.listViewUpgrade.Items.IndexOf(x) + 1;

					BuildToLevel btl = new BuildToLevel()
					{
						BuildingName = tr.GetAidLang(TravianData.Tribe, Bid),
						DisplayName = dl.GetAidLang(TravianData.Tribe, Bid),
						CurrentLevel = CV.Upgrades[Bid].AttackLevel,
						TargetLevel = CV.BlacksmithLevel,
						mui = mui
					};
					if(btl.ShowDialog() == DialogResult.OK)
					{
						if(btl.Return < 0)
							continue;

						TQueue Q = new TQueue()
						{
							QueueType = TQueueType.UAttack,
							Bid = Bid,
							Status = " -> " + btl.Return.ToString(),
							TargetLevel = btl.Return
						};
						TravianData.Villages[SelectVillage].Queue.Add(Q);
						CV.SaveQueue(tr.userdb);
						var lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
						lvi.SubItems.Add(typelist[Q.Type]);
						lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, Q.Bid));
						lvi.SubItems.Add(Q.Status);
						lvi.SubItems.Add("");
					}
				}
			}
		}
		private void CMRUpgradeDefTo_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			var CV = TravianData.Villages[SelectVillage];
			foreach(ListViewItem x in m_researchstatus.listViewUpgrade.SelectedItems)
			{
				if(x.SubItems[3].BackColor != Color.White)
				{
					int Bid = m_researchstatus.listViewUpgrade.Items.IndexOf(x) + 1;

					BuildToLevel btl = new BuildToLevel()
					{
						BuildingName = tr.GetAidLang(TravianData.Tribe, Bid),
						DisplayName = dl.GetAidLang(TravianData.Tribe, Bid),
						CurrentLevel = CV.Upgrades[Bid].DefenceLevel,
						TargetLevel = CV.ArmouryLevel,
						mui = mui
					};
					if(btl.ShowDialog() == DialogResult.OK)
					{
						if(btl.Return < 0)
							continue;

						TQueue Q = new TQueue()
						{
							QueueType = TQueueType.UDefense,
							Bid = Bid,
							Status = " -> " + btl.Return.ToString(),
							TargetLevel = btl.Return
						};
						TravianData.Villages[SelectVillage].Queue.Add(Q);
						CV.SaveQueue(tr.userdb);
						var lvi = m_queuelist.listViewQueue.Items.Add(Q.Bid.ToString());
						lvi.SubItems.Add(typelist[Q.Type]);
						lvi.SubItems.Add(dl.GetAidLang(TravianData.Tribe, Q.Bid));
						lvi.SubItems.Add(Q.Status);
						lvi.SubItems.Add("");
					}
				}
			}
		}
		private void CMRRefresh_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			TravianData.Villages[SelectVillage].InitializeUpgrade();
		}
		#endregion

		#region CMM
		private void CMMNew_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;

			TVillage CV = TravianData.Villages[SelectVillage];
			if(CV.isBuildingInitialized == 2)
			{
				TransferSetting ts = new TransferSetting()
				{
					FromVillageID = this.SelectVillage,
					TravianData = this.TravianData,
					UserDB = this.tr.userdb,
					mui = this.mui
				};

				if(ts.ShowDialog() == DialogResult.OK && ts.Return != null)
				{
					TQueue Q = new TQueue() {
						QueueType = TQueueType.Transfer,
						ExtraOptions = ts.Return.ToString(),
						Status = ts.Return.Status
					};
					CV.Queue.Add(Q);
					CV.SaveQueue(tr.userdb);

					ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("*");
					lvi.SubItems.Add(typelist[(int) TQueueType.Transfer]);
					lvi.SubItems.Add(ts.Return.GetTitle(TravianData));
					lvi.SubItems.Add(Q.Status);
					lvi.SubItems.Add("");
				}
			}
		}


		private void CMMNpcTrade_Click(object sender, EventArgs e)
		{
			if (!TravianData.Villages.ContainsKey(SelectVillage))
			{
				return;
			}

			TVillage CV = TravianData.Villages[SelectVillage];
			if (CV.isBuildingInitialized != 2)
			{
				return;
			}

			NpcTradeSetting setting = new NpcTradeSetting()
			{
				Village = CV,
				mui = this.mui
			};

			if (setting.ShowDialog() == DialogResult.OK && setting.Return != null)
			{
				NpcTradeOption option = setting.Return;
				TQueue Q = new TQueue()
				{
					QueueType = TQueueType.NpcTrade,
					ExtraOptions = option.ToString(),
					Status = option.Status
				};
				CV.Queue.Add(Q);
				CV.SaveQueue(tr.userdb);

				ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("*");
				lvi.SubItems.Add(typelist[(int)TQueueType.NpcTrade]);
				lvi.SubItems.Add(option.Title);
				lvi.SubItems.Add(Q.Status);
				lvi.SubItems.Add("");
			}
		}
		#endregion

		private void dockPanel1_Resize(object sender, EventArgs e)
		{
			if(dockPanel1.Contents.Count != 0)
			{
				string fn = "style\\" + LoginInfo.Username + "@" + LoginInfo.Server + "!style.xml";
				dockPanel1.SaveAsXml(fn);
			}
		}

		//public int timeoffset = 0;

		private void timersec1_Tick(object sender, EventArgs e)
		{
			LCLTime.Text = LCLTime.ToolTipText + " " + DateTime.Now.ToLongTimeString();
			SVRTime.Text = SVRTime.ToolTipText + " " + DateTime.Now.AddSeconds(TravianData.ServerTimeOffset).ToLongTimeString();
		}

		private void CMICancel_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_inbuildinglist.listViewInBuilding.SelectedIndices.Count == 1)
			{
				var CV = TravianData.Villages[SelectVillage];
				int key = m_inbuildinglist.listViewInBuilding.SelectedIndices[0];
				if(CV.InBuilding[key] != null && CV.InBuilding[key].Cancellable)
					tr.Cancel(SelectVillage, key);
			}
		}

		private void contextMenuInbuilding_Opening(object sender, CancelEventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;
			if(m_inbuildinglist.listViewInBuilding.SelectedIndices.Count == 1)
			{
				var CV = TravianData.Villages[SelectVillage];
				int key = m_inbuildinglist.listViewInBuilding.SelectedIndices[0];
				CMICancel.Enabled = CV.InBuilding[key] != null && CV.InBuilding[key].Cancellable && CV.InBuilding[key].FinishTime > DateTime.Now;

			}
		}

		private void CMBRaid_Click(object sender, EventArgs e)
		{
			if(!TravianData.Villages.ContainsKey(SelectVillage))
				return;

			TVillage CV = TravianData.Villages[SelectVillage];
			if(CV.isTroopInitialized == 2)
			{
				RaidOptForm rof = new RaidOptForm()
				{
					mui = this.mui,
					Troops = CV.Troops[0].Troops,
					dl = this.dl,
					Tribe = TravianData.Tribe
				};

				if(rof.ShowDialog() == DialogResult.OK && rof.Return != null)
				{
					/*
					TQueue Q = new TQueue()
					{
						QueueType = TQueueType.Transfer,
						ExtraOptions = ts.Return.ToString(),
						Status = ts.Return.Status
					};
					CV.Queue.Add(Q);
					CV.SaveQueue(tr.userdb);

					ListViewItem lvi = m_queuelist.listViewQueue.Items.Add("*");
					lvi.SubItems.Add(typelist[(int)TQueueType.Transfer]);
					lvi.SubItems.Add(ts.Return.GetTitle(TravianData));
					lvi.SubItems.Add(Q.Status);
					lvi.SubItems.Add("");
					 */
				}
			}
			else
				CV.InitializeTroop();
		}
	}
}
