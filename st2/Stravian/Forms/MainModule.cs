using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Stravian
{
	public partial class UserControl1 : UserControl
	{
		Label[] reslabel;
		libTravian tr = null;
		public TabPage tp;
		public TextBox tb;
		int queuecount = 0;
		public string _server, _username, _password;
		int selectindex = 0;

		public UserControl1()
		{
			InitializeComponent();
			reslabel = new Label[] { label1, label2, label3, label4 };
			//_tribe = 0;
		}

		public void login(logininfo l)
		{
			_server = l.Server;
			_username = l.Username;
			_password = l.Password;
			if(tr != null)
				tr = null;
			listViewVillage.Items.Clear();
			listViewBuilding.Items.Clear();
			tr = new libTravian(l) { ParentFrame = this };
			//tr.login(_username, _password, tribe);
			tr.login();
			tp.Text = string.Format("{0} @ {1}", _username, _server.Replace("travian.", ""));
		}

		private string TimeToString(long timecost)
		{
			if(timecost >= 86400)
				return "∞";
			TimeSpan ts = new TimeSpan(timecost * 10000000);
			return ts.ToString();
		}

		private void listViewVillage_Changed(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			//tr.WriteLog("ListViewMethod", string.Format("called: {0} {1}", listViewVillage.SelectedIndices.Count, selectindex));
			if(listViewVillage.SelectedIndices.Count == 1)
				selectindex = listViewVillage.SelectedIndices[0];
			if(tr.villages == null)
				return;
			if(selectindex >= tr.villages.Count)
				return;
			Village cv = tr.villages[selectindex];
			if(cv == null)
				return;
			listViewBuilding.Items.Clear();
			foreach(Label l in reslabel)
				l.Text = "";
			if(cv.res.resuptime != DateTime.MinValue)
			{
				for(int i = 0; i < 4; i++)
				//if(tr.villages[listBox1.SelectedIndices[0]].res[i] != null)
				{
					//ListViewItem lvi = listViewBuilding.Items.Add(tr.svrlang.Resource[i]);
					//lvi.SubItems.Add(
					reslabel[i].Text = string.Format("{0}/{1}\n({2:0}, {3}:{4:00}:{5:00})\n({6}, {7:F2}%)",
						cv.res.CurrAmount(i),
						cv.res.capacity[i],
						cv.res.produce[i],
						Math.Floor(cv.res.lefttime(i).TotalHours),
						cv.res.lefttime(i).Minutes,
						cv.res.lefttime(i).Seconds,
						cv.res.capacity[i] - cv.res.CurrAmount(i),
						cv.res.CurrAmount(i) * 100.0 / cv.res.capacity[i]
						);//);
					//lvi.SubItems.Add("");
				}
				for(int i = 0; i < cv.inb.Length; i++)
				{
					if(cv.inb[i] == null)
						continue;
					if(cv.inb[i].completetime < DateTime.Now)
						continue;
					ListViewItem lvi = listViewBuilding.Items.Add(libTravian.typelist[cv.inb[i].type]);
					TimeSpan ts = cv.inb[i].completetime.Subtract(DateTime.Now);

					lvi.SubItems.Add(string.Format("{0}  {1}  {2:0}:{3:00}:{4:00} -> {5}",
						tr.displaylang.Building[cv.inb[i].gid],
						cv.inb[i].level,
						Math.Floor(ts.TotalHours), ts.Minutes, ts.Seconds,
						cv.inb[i].completetime.ToLongTimeString()));
					lvi.SubItems.Add("");
				}

				for(int i = 1; i <= 40; i++)
				{
					if(cv.buildings[i] == null)
						continue;
					ListViewItem lvi = listViewBuilding.Items.Add(i.ToString());
					lvi.SubItems.Add(string.Format("{0}  {1}", tr.displaylang.Building[cv.buildings[i].gid], cv.buildings[i].level));
					lvi.SubItems.Add(cv.buildings[i].status);
					if(!Buildings.checklevelfull(cv.buildings[i].gid, cv.buildings[i].level, cv.capital))
					{
						var timecost = Buildings.cost(cv.buildings[i].gid, cv.buildings[i].level + 1) ^ cv.res;
						if(timecost > 0)
						{
							lvi.BackColor = Color.FromArgb(255, 192, 192);
							lvi.SubItems[1].Text += " <- " + TimeToString(timecost);
						}
						else
							lvi.BackColor = Color.FromArgb(255, 255, 192);
					}
					//tr.villages[listBox1.SelectedIndices[0]].buildings
				}
			}
			else
			{
				if(listViewVillage.Items[selectindex].Selected)
				{
					int index = selectindex;
					//listViewVillage.Items[selectindex].Selected = false;
					//if(tr.currid == index)
					//	tr.loginstate(1);
					//else
					//tr.WriteLog("ListViewMethod", "Calling fetchvillage");
					tr.fetchvillage(index);
				}
			}
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			if(listViewVillage.SelectedItems.Count == 1)
				refreshToolStripMenuItem.Text = string.Format("&R. 刷新 '{0}'", listViewVillage.SelectedItems[0].SubItems[2].Text);
			else
				e.Cancel = true;
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int index = listViewVillage.SelectedIndices[0];
			listViewVillage.Items[listViewVillage.SelectedIndices[0]].Selected = false;
			//if(tr.currid == index)
			//	tr.loginstate(1);
			//else
			tr.fetchvillage(index);
		}

		private void addToQueueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(listViewVillage.SelectedIndices.Count == 0)
				return;
			for(int i = 0; i < listViewBuilding.SelectedItems.Count; i++)
			{
				int temp;
				if(!int.TryParse(listViewBuilding.SelectedItems[i].Text, out temp))
					continue;
				/*int delay = 10;
				if(tr._isRomans)
					foreach(inbuild ib in tr.villages[listBox1.SelectedIndices[0]].inb)
					{
						if(ib.type == (Convert.ToInt32(listView1.SelectedItems[i].Text) < 19 ? 1 : 2))
							delay = Convert.ToInt32(ib.completetime.Subtract(DateTime.Now).TotalSeconds);
					}
				else if(tr.villages[listBox1.SelectedIndices[0]].inb.Count == 1)
					delay = Convert.ToInt32(tr.villages[listBox1.SelectedIndices[0]].inb[0].completetime.Subtract(DateTime.Now).TotalSeconds);
				 */
				//tr.bq.Add(new BQ(listBoxVillage.SelectedIndices[0], Convert.ToInt32(listViewBuilding.SelectedItems[i].Text)));
				tr.bq.Add(new BQ()
				{
					Vid = listViewVillage.SelectedIndices[0],
					Bid = Convert.ToInt32(listViewBuilding.SelectedItems[i].Text),
					Gid = tr.villages[listViewVillage.SelectedIndices[0]].buildings[Convert.ToInt32(listViewBuilding.SelectedItems[i].Text)].gid,
					QueueType = BQ.TQueueType.Building
				});
				ListViewItem lvi = listViewQueue.Items.Add(tr.bq[tr.bq.Count - 1].Vid.ToString());
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Bid.ToString());
				lvi.SubItems.Add(libTravian.typelist[tr.bq[tr.bq.Count - 1].Type]);
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Status);
				lvi.SubItems.Add("-");
				lvi.SubItems.Add(tr.displaylang.Building[tr.villages[listViewVillage.SelectedIndices[0]].buildings[tr.bq[tr.bq.Count - 1].Bid].gid]);
				/*
				tr.WriteLog(".Queue", string.Format("{0} @ {1} @ {2}({3}, {4})",
					tr.svrlang.Building[tr.villages[listBoxVillage.SelectedIndices[0]].buildings[tr.bq[tr.bq.Count - 1].bid].gid],
					tr.bq[tr.bq.Count - 1].bid,
					tr.villages[listBoxVillage.SelectedIndices[0]].name,
					tr.bq[tr.bq.Count - 1].vid,
					tr.villages[listBoxVillage.SelectedIndices[0]].id));
				*/
				listViewBuilding.Items[listViewBuilding.SelectedIndices[i]].SubItems[1].Text += "*";
			}
		}
		/*
		private void fetchStatusToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listBoxVillage.SelectedIndex < 0)
				return;
			tr.temp = listBoxVillage.SelectedIndex;
			for(int i = 0; i < listViewBuilding.SelectedItems.Count; i++)
			{
				int temp;
				if(!int.TryParse(listViewBuilding.SelectedItems[i].Text, out temp))
					continue;
				tr.fetchstatus(Convert.ToInt32(listViewBuilding.SelectedItems[i].Text));
			}
			//tr.WriteError("Unsupportted function: fetchStatus");
		}
		*/
		private void enableTimerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			enableTimerToolStripMenuItem.Checked = !enableTimerToolStripMenuItem.Checked;
			timer1.Enabled = enableTimerToolStripMenuItem.Checked;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if(tr != null)
			{
				if(queuecount != tr.bq.Count)
				{
					queuecount = tr.bq.Count;
					tp.Text = string.Format("{0} @ {1} ({2})", _username, _server.Replace("travian.", ""), queuecount);
				}
				if(selectindex >= 0 && tr.villages.Count > selectindex && tr.villages[selectindex].res.produce[0] != 0)
					for(int i = 0; i < 4; i++)
					{
						reslabel[i].Text = string.Format("{0}/{1}\n({2:0}, {3}:{4:00}:{5:00})\n({6}, {7:F2}%)",
							tr.villages[selectindex].res.CurrAmount(i),
							tr.villages[selectindex].res.capacity[i],
							tr.villages[selectindex].res.produce[i],
							Math.Floor(tr.villages[selectindex].res.lefttime(i).TotalHours),
							tr.villages[selectindex].res.lefttime(i).Minutes,
							tr.villages[selectindex].res.lefttime(i).Seconds,
							tr.villages[selectindex].res.capacity[i] - tr.villages[selectindex].res.CurrAmount(i),
							tr.villages[selectindex].res.CurrAmount(i) * 100.0 / tr.villages[selectindex].res.capacity[i]
							);
					}
				int[] vqcnt = new int[tr.villages.Count];
				foreach(BQ bq in tr.bq)
				{
					vqcnt[bq.Vid]++;
				}
				if(listViewVillage.Items.Count == tr.villages.Count)
				{
					for(int i = 0; i < tr.villages.Count; i++)
					{
						if(listViewVillage.Items[i].SubItems[1].Text != vqcnt[i].ToString())
							listViewVillage.Items[i].SubItems[1].Text = vqcnt[i].ToString();
						/*
						string oldstring = listViewVillage.Items[i].Text;
						string ov = oldstring.Substring(oldstring.IndexOf('[') + 1, oldstring.IndexOf(']') - oldstring.IndexOf('[') - 1);
						if(ov != vqcnt[i].ToString())
						{
							string l = oldstring.Substring(0, oldstring.IndexOf('[') + 1);
							string r = oldstring.Substring(oldstring.IndexOf(']'));
							listViewVillage.Items[i].Text = l + vqcnt[i].ToString() + r;
						}
						//*/
					}
				}
				//*/
				tr.tick();
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tr.bq.Clear();
			listViewQueue.Items.Clear();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewQueue.SelectedIndices.Count == 0)
				return;
			for(int i = listViewQueue.SelectedIndices.Count - 1; i >= 0; i--)
			{
				tr.bq.RemoveAt(listViewQueue.SelectedIndices[i]);
				listViewQueue.Items.RemoveAt(listViewQueue.SelectedIndices[i]);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(tr == null)
			{
				((TabControl)tp.Parent).TabPages.Remove(tp);
				timer1.Enabled = false;
				tp.Dispose();
				return;
			}
			if(tr.bq.Count == 0 ||
				MessageBox.Show("队列中仍有项目。真的要关闭吗？", "警告!",
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				((TabControl)tp.Parent).TabPages.Remove(tp);
				timer1.Enabled = false;
				tp.Dispose();
			}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(selectindex < 0)
				return;
			NewBuilding nb = new NewBuilding(tr.villages[selectindex].buildings, tr.displaylang);
			if(nb.ShowDialog() == DialogResult.OK)
			{
				tr.bq.Add(new BQ()
				{
					Vid = selectindex,
					Bid = nb.OutBid,
					Gid = nb.OutGid,
					QueueType = BQ.TQueueType.Building
				});
				ListViewItem lvi = listViewQueue.Items.Add(tr.bq[tr.bq.Count - 1].Vid.ToString());
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Bid.ToString());
				lvi.SubItems.Add(libTravian.typelist[tr.bq[tr.bq.Count - 1].Type]);
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Status);
				lvi.SubItems.Add("-");
				lvi.SubItems.Add(tr.displaylang.Building[tr.bq[tr.bq.Count - 1].Gid]);
				/*
				tr.WriteLog(".Queue", string.Format("{0} @ {1} @ {2}({3}, {4})",
					tr.svrlang.Building[tr.bq[tr.bq.Count - 1].gid],
					tr.bq[tr.bq.Count - 1].bid,
					tr.villages[listViewVillage.SelectedIndices[0]].name,
					tr.bq[tr.bq.Count - 1].vid,
					tr.villages[listViewVillage.SelectedIndices[0]].id));
				 */
				if(nb.OutBid > 0)
					tr.villages[selectindex].buildings[nb.OutBid] = new Building(nb.OutGid);
			}
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewQueue.SelectedIndices.Count == 0 || listViewQueue.SelectedIndices[0] == 0)
				return;
			for(int i = 0; i < listViewQueue.SelectedIndices.Count; i++)
			{
				int n = listViewQueue.SelectedIndices[i];
				tr.bq.Reverse(n - 1, 2);
				ListViewItem tlvi = listViewQueue.Items[n - 1];
				listViewQueue.Items.RemoveAt(n - 1);
				listViewQueue.Items.Insert(n, tlvi);
			}
		}

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewQueue.SelectedIndices.Count == 0 || listViewQueue.SelectedIndices[listViewQueue.SelectedIndices.Count - 1] == listViewQueue.Items.Count - 1)
				return;
			for(int i = listViewQueue.SelectedIndices.Count - 1; i >= 0; i--)
			{
				int n = listViewQueue.SelectedIndices[i];
				tr.bq.Reverse(n, 2);
				ListViewItem tlvi = listViewQueue.Items[n + 1];
				listViewQueue.Items.RemoveAt(n + 1);
				listViewQueue.Items.Insert(n, tlvi);
			}
		}

		private void AIModuleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BQ b = new BQ()
			{
				Vid = selectindex,
				Bid = libTravian.AIBID,
				Gid = 0,
				QueueType = BQ.TQueueType.Building
			};
			tr.bq.Add(b);
			ListViewItem lvi = listViewQueue.Items.Add(b.Vid.ToString());
			lvi.SubItems.Add(b.Bid.ToString());
			lvi.SubItems.Add(libTravian.typelist[b.Type]);
			lvi.SubItems.Add(b.Status);
			lvi.SubItems.Add("-");
			/*
			tr.WriteLog(".AI Queued", string.Format("{0}({1}, {2})",
				tr.villages[listViewVillage.SelectedIndices[0]].name,
				b.vid,
				tr.villages[listViewVillage.SelectedIndices[0]].id));
			 */
			lvi.SubItems.Add("AI");
		}

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
		}

		private void toolStripMenuItem6_Click(object sender, EventArgs e)
		{
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
			addToQueueToolStripMenuItem_Click(sender, e);
		}

		private void toolStripMenuItem7_Click(object sender, EventArgs e)
		{
			//listViewQueue.Items.Clear();
			//for(int i = 0; i < 
		}

		private void toolStripMenuItem8_Click(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(listViewVillage.SelectedIndices.Count == 0)
				return;
			for(int i = 0; i < listViewBuilding.SelectedItems.Count; i++)
			{
				int temp;
				if(!int.TryParse(listViewBuilding.SelectedItems[i].Text, out temp))
					continue;
				tr.bq.Add(new BQ()
				{
					Vid = selectindex,
					Bid = Convert.ToInt32(listViewBuilding.SelectedItems[i].Text),
					TargetLevel = 0,
					QueueType = BQ.TQueueType.Destroy
				});
				ListViewItem lvi = listViewQueue.Items.Add(tr.bq[tr.bq.Count - 1].Vid.ToString());
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Bid.ToString());
				lvi.SubItems.Add(libTravian.typelist[tr.bq[tr.bq.Count - 1].Type]);
				lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Status);
				lvi.SubItems.Add("-");
				lvi.SubItems.Add(tr.displaylang.Building[tr.villages[selectindex].buildings[tr.bq[tr.bq.Count - 1].Bid].gid]);
				/*
				tr.WriteLog(".Queue", string.Format("{0} @ {1} @ {2}({3}, {4})",
					tr.svrlang.Building[tr.villages[listBoxVillage.SelectedIndices[0]].buildings[tr.bq[tr.bq.Count - 1].bid].gid],
					tr.bq[tr.bq.Count - 1].bid,
					tr.villages[listBoxVillage.SelectedIndices[0]].name,
					tr.bq[tr.bq.Count - 1].vid,
					tr.villages[listBoxVillage.SelectedIndices[0]].id));
				*/
				listViewBuilding.Items[listViewBuilding.SelectedIndices[i]].SubItems[1].Text += "*";
			}
		}

		private void toolStripMenuItem9_Click(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(listViewVillage.SelectedIndices.Count == 0)
				return;
			for(int i = 0; i < listViewBuilding.SelectedItems.Count; i++)
			{
				int temp;
				if(!int.TryParse(listViewBuilding.SelectedItems[i].Text, out temp))
					continue;
				int Bid = Convert.ToInt32(listViewBuilding.SelectedItems[i].Text);
				int Gid = tr.villages[selectindex].buildings[Bid].gid;
				BuildToLevel btl = new BuildToLevel()
				{
					BuildingName = tr.displaylang.Building[Gid],
					CurrentLevel = tr.villages[selectindex].buildings[Bid].level,
					TargetLevel = Buildings._cost[Gid].data.Length - 1
				};
				if(btl.ShowDialog() == DialogResult.OK)
				{
					if(btl.Return < 0)
						continue;

					tr.bq.Add(new BQ()
					{
						Vid = selectindex,
						Bid = Bid,
						Gid = Gid,
						TargetLevel = btl.Return,
						QueueType = BQ.TQueueType.Building
					});
					ListViewItem lvi = listViewQueue.Items.Add(tr.bq[tr.bq.Count - 1].Vid.ToString());
					lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Bid.ToString());
					lvi.SubItems.Add(libTravian.typelist[tr.bq[tr.bq.Count - 1].Type]);
					lvi.SubItems.Add(tr.bq[tr.bq.Count - 1].Status);
					lvi.SubItems.Add("-");
					lvi.SubItems.Add(tr.displaylang.Building[tr.villages[selectindex].buildings[tr.bq[tr.bq.Count - 1].Bid].gid] + " -> " + tr.bq[tr.bq.Count - 1].TargetLevel);

					listViewBuilding.Items[listViewBuilding.SelectedIndices[i]].SubItems[1].Text += "!";
				}
			}
		}

		private void listViewVillage_Click(object sender, EventArgs e)
		{
			if(tr == null)
				return;
			if(tr.villages == null)
				return;
			if(selectindex < 0)
				return;
			if(selectindex >= tr.villages.Count)
				return;
			Village cv = tr.villages[selectindex];
			if(cv == null)
				return;
			if(cv.res.resuptime != DateTime.MinValue)
				listViewVillage_Changed(sender, e);
		}

	}
}
