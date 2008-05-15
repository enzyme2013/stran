using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;

namespace Stravian
{
	public partial class MainForm : Form
	{
		static public List<logininfo> accounts = new List<logininfo>();
		static public Dictionary<string, string> options = new Dictionary<string, string>();
		MD5 md5 = MD5.Create();
		static int Pagecount = 0, Buildcount = 0, Eventcount = 0;

		static object writelock = new object();
		public MainForm()
		{
			InitializeComponent();
			Assembly myAsm = Assembly.Load("Stravian");
			AssemblyName aName = myAsm.GetName();
			Version v = aName.Version;
			Text = string.Format("Stravian {0}.{1}{2}{3} ´ºÐÝ¤ß¤ÎÑaÁ• ÈýßLÄ¿", v.Major, v.Minor, v.Build, v.Revision);
			notifyIcon1.Text = Text;
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			loginToolStripMenuItem.Enabled = listView1.SelectedIndices.Count != 0;
			if(listView1.SelectedIndices.Count == 0)
				loginToolStripMenuItem.Text = "&L. µÇÂ¼";
			else if(listView1.SelectedIndices.Count == 1)
				loginToolStripMenuItem.Text = "&L. µÇÂ¼: " + listView1.SelectedItems[0].Text;
			else
				loginToolStripMenuItem.Text = "&L. µÇÂ¼ÕâÐ©ÕÊºÅ";
		}

		private UserControl1 donewtab()
		{
			TabPage tp = new TabPage("ÐÂ±êÇ©");
			UserControl1 uc1 = new UserControl1();
			uc1.tb = textLog;
			uc1.tp = tp;
			uc1.Dock = DockStyle.Fill;
			tp.Controls.Add(uc1);
			tabControl1.TabPages.Add(tp);
			tabControl1.SelectTab(tp);
			return uc1;
		}

		private delegate void void_d();
		public void RefreshListView1()
		{
			Invoke(new void_d(listView1_Refresh));
			Invoke(new void_d(saveAccountInfo));
		}
		public void listView1_Refresh()
		{
			listView1.Items.Clear();
			for(int i = 0; i < accounts.Count; i++)
			{
				ListViewItem lvi = listView1.Items.Add(accounts[i].Username);
				lvi.SubItems.Add(accounts[i].Server);
				lvi.SubItems.Add(libTravian.tribelist[accounts[i].Tribe]);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//Text = v.ToString();
			if(File.Exists("Account"))
			{
				FileStream fs = new FileStream("Account", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				while(!sr.EndOfStream)
				{
					accounts.Add(new logininfo(sr.ReadLine().Split(':')));
				}
				listView1_Refresh();
				sr.Close();
			}
			ReadStatistics();

			if(File.Exists("Option"))
			{
				FileStream fs = new FileStream("Option", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				while(!sr.EndOfStream)
				{
					string[] opt = sr.ReadLine().Split('=');
					if(opt.Length == 2)
						options.Add(opt[0], opt[1]);
				}
				sr.Close();
			}
			Buildings.Init();
			if(File.Exists("MOTD"))
			{
				FileStream fs = new FileStream("MOTD", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				textLog.AppendText(sr.ReadToEnd());
				sr.Close();
			}

		}
		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
				if(WindowState == FormWindowState.Minimized)
				{
					Visible = true;
					WindowState = FormWindowState.Normal;
				}
				else
				{
					WindowState = FormWindowState.Minimized;
					Visible = false;
				}
		}
		private void MainForm_Resize(object sender, EventArgs e)
		{
			if(WindowState == FormWindowState.Minimized)
				Visible = false;
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Application.Exit();
		}
		
		private void saveAccountInfo()
		{
			FileStream fs = new FileStream("Account", FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			for(int i = 0; i < accounts.Count; i++)
				sw.WriteLine("{0}:{1}:{2}:{3}",
					accounts[i].Username,
					Convert.ToBase64String(Encoding.UTF8.GetBytes(accounts[i].Server)),
					Convert.ToBase64String(Encoding.UTF8.GetBytes(accounts[i].Password)),
					accounts[i].Tribe,
					accounts[i].Language);
			sw.Close();
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for(int i = 0; i < listView1.SelectedIndices.Count; i++)
				newtab(accounts[listView1.SelectedIndices[i]]);
		}
		private void loginAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for(int i = 0; i < accounts.Count; i++)
				newtab(accounts[i]);
		}

		/*private void newtab()
		{
			donewtab();
		}*/
		private void newtab(logininfo l)
		{
			// check if repeat login
			for(int i = 1; i < tabControl1.TabCount; i++)
			{
				if(!(tabControl1.TabPages[i].Controls[0] is UserControl1))
					continue;
				if((tabControl1.TabPages[i].Controls[0] as UserControl1)._username == l.Username && (tabControl1.TabPages[i].Controls[0] as UserControl1)._server == l.Server)
				{
					textLog.AppendText(string.Format("[{0}][{1}] {2}{3}", DateTime.Now.ToString(), "Info", "ÖØ¸´µÇÂ¼µÄÕÊºÅ " + l.Username + " @ " + l.Server, Environment.NewLine));
					return;
				}
			}
			UserControl1 uc1 = donewtab();
			uc1.login(l);
		}

		private void addAccountToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewAccount na = new NewAccount(false);
			if(na.ShowDialog() == DialogResult.OK && na.accountresult != null)
			{
				accounts.Add(na.accountresult);
				listView1_Refresh();
				saveAccountInfo();
			}
		}
		private void editAccountToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listView1.SelectedIndices.Count < 1)
				return;
			NewAccount na = new NewAccount(true);
			na.init(accounts[listView1.SelectedIndices[0]]);
			if(na.ShowDialog() == DialogResult.OK && na.accountresult != null)
			{
				//accounts.Add(na.accountresult);
				accounts[listView1.SelectedIndices[0]].Username = na.accountresult.Username;
				accounts[listView1.SelectedIndices[0]].Server = na.accountresult.Server;
				accounts[listView1.SelectedIndices[0]].Tribe = na.accountresult.Tribe;
				if(na.accountresult.Password != "")
					accounts[listView1.SelectedIndices[0]].Password = na.accountresult.Password;
				listView1_Refresh();
				saveAccountInfo();
			}

		}
		private void deleteAccountToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for(int i = listView1.SelectedIndices.Count - 1; i >= 0 ; i--)
				accounts.RemoveAt(listView1.SelectedIndices[i]);
			listView1_Refresh();
		}

		//private delegate void string_d(string text);

		public static void FetchPageCount()
		{
			Pagecount++;
			WriteStatistics();
		}
		public static void BuildCount()
		{
			Buildcount++;
			WriteStatistics();
		}
		public static void EventCount()
		{
			Eventcount++;
			WriteStatistics();
		}
		private static void ReadStatistics()
		{
			try
			{
				if(File.Exists("Statistics"))
				{
					FileStream fs = new FileStream("Statistics", FileMode.Open, FileAccess.Read);
					StreamReader sr = new StreamReader(fs, Encoding.UTF8);
					Pagecount = Convert.ToInt32(sr.ReadLine());
					Buildcount = Convert.ToInt32(sr.ReadLine());
					Eventcount = Convert.ToInt32(sr.ReadLine());
					sr.Close();
				}
			}
			catch(Exception)
			{ }
		}
		private static void WriteStatistics()
		{
			lock(writelock)
			{
				FileStream fs = new FileStream("Statistics", FileMode.Create, FileAccess.Write);
				StreamWriter sr = new StreamWriter(fs, Encoding.UTF8);
				sr.WriteLine(Pagecount);
				sr.WriteLine(Buildcount);
				sr.WriteLine(Eventcount);
				sr.Close();
			}
		}

	}
}
