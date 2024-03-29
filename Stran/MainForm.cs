﻿/*
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using libTravian;
using Stran.Properties;

namespace Stran
{
	public partial class MainForm : Form
	{
		static public MainForm Instance;
		static public List<TLoginInfo> accounts = new List<TLoginInfo>();
		static public Dictionary<string, string> Options = new Dictionary<string, string>();
		static int Pagecount = 0, Buildcount = 0;
		static object writelock = new object();
		// svn propset svn:keywords "Rev" MainForm.cs
		//string svnid = "$Rev$";
		static public string VERSION;
		private MUI mui { get; set; }

		//public static readonly string[] typelist = new string[] { "资源田", "建筑", "拆除", "攻击力", "防御力" };
		public static string[] tribelist = new string[] { "自动探测", "罗马", "日尔曼", "高卢" };
		public bool CloseMainform = false;
		public bool StartSmall = false;
		public MainForm()
		{
			InitializeComponent();
			Instance = this;
			AssemblyName aName = this.GetType().Assembly.GetName();
			Version v = aName.Version;
			//int rev = Convert.ToInt32(svnid.Split(' ')[1]);
			DateTime CompileTime = GetPe32Time(this.GetType().Assembly.Location);
			if(CompileTime == DateTime.MinValue)
				CompileTime = File.GetLastWriteTime(this.GetType().Assembly.Location);
			VERSION = Text = string.Format("{0} {1} [{2} 测试版本]", aName.Name, v, CompileTime.ToString("yyyy-MM-dd"));
			notifyIcon1.Text = Text;
			Buildings.Init();
			DummyBrowser.ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
			DummyBrowser.ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

			//Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
		}

		public void listView1_Refresh()
		{
			listView1.Items.Clear();
			for (int i = 0; i < accounts.Count; i++)
			{
				ListViewItem lvi = listView1.Items.Add(accounts[i].Username);
				lvi.SubItems.Add(accounts[i].Server);
				lvi.SubItems.Add(tribelist[accounts[i].Tribe]);
				lvi.SubItems.Add(accounts[i].Proxy);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (File.Exists("Option"))
			{
				FileStream fs = new FileStream("Option", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				while (!sr.EndOfStream)
				{
					string[] opt = sr.ReadLine().Split('=');
					if (opt.Length == 2)
						Options.Add(opt[0], opt[1]);
				}
				sr.Close();
			}
			if (Options.ContainsKey("lang"))
				mui = new MUI(Options["lang"]);
			else
				mui = new MUI("cn");
			mui.RefreshLanguage(this);
			if (File.Exists("Account"))
			{
				FileStream fs = new FileStream("Account", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				while (!sr.EndOfStream)
				{
					accounts.Add(new TLoginInfo(sr.ReadLine().Split(':')));
				}
				listView1_Refresh();
				sr.Close();
			}
			ReadStatistics();

			if (File.Exists("MOTD"))
			{
				FileStream fs = new FileStream("MOTD", FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				textLog.AppendText(sr.ReadToEnd());
				sr.Close();
			}
			/*
			int scrWidth = Screen.PrimaryScreen.WorkingArea.Width;
			if(scrWidth > 1278)
			{
					Width = 1278;
					Left = (scrWidth - Width) / 2;
			}
			*/
			if (!Directory.Exists("style"))
				Directory.CreateDirectory("style");
			if (File.Exists("style\\form!style"))
			{
				var x = File.ReadAllText("style\\form!style").Split(',');
				//, string.Format("{0},{1}", Width, Height));
				Width = Convert.ToInt32(x[0]);
				Height = Convert.ToInt32(x[1]);
				Left = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
				Top = (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2;

			}
			if (Options.ContainsKey("autorun"))  //自动登录
			{
				char opt = Options["autorun"].Trim()[0];
				switch (opt)
				{
					case 'y':
					case 'Y':
					case 't':
					case 'T':
					case '1':
						for (int i = 0; i < accounts.Count; i++)
							newtab(accounts[i]);
						break;
				}
			}
			if (Options.ContainsKey("close")) //取消关闭提醒
			{
				char clo = Options["close"].Trim()[0];
				switch (clo)
				{
					case 'y':
					case 'Y':
					case 't':
					case 'T':
					case '1':
						CloseMainform = true;
						break;
				}
			}

			if (Options.ContainsKey("AutoPlay")) //攻擊警報音樂
			{
				char pop = Options["AutoPlay"].Trim()[0];
				switch (pop)
				{
					case 'y':
					case 'Y':
					case 't':
					case 'T':
					case '1':
						MainFrame.AutoPlay = true;
						break;
				}
			}
			if (Options.ContainsKey("StartSmall")) //啟動時最小化視窗
			{
				char pop = Options["StartSmall"].Trim()[0];
				switch (pop)
				{
					case 'y':
					case 'Y':
					case 't':
					case 'T':
					case '1':
						StartSmall = true;
						break;
				}
			}
			if (!Options.ContainsKey("NoRaid2to7"))
			{
				RaidQueue.NoRaid2To7 = false;
			}
			if (StartSmall)
			{
				this.WindowState = FormWindowState.Minimized;
				this.notifyIcon1.Visible = true;
				this.ShowInTaskbar = false;
				StartSmall = false;
			}
		}
		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				if (WindowState == FormWindowState.Minimized)
				{
					Visible = true;
					WindowState = FormWindowState.Normal;
					ShowInTaskbar = true;
				}
				else
				{
					WindowState = FormWindowState.Minimized;
					Visible = false;
				}
		}
		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
				Visible = false;
			else
				File.WriteAllText("style\\form!style", string.Format("{0},{1}", Width, Height));
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!CloseMainform)
			{
				if (MessageBox.Show(mui._("reallyexittext"), mui._("reallyclosecap"),
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
					e.Cancel = true;
			}
		}

		private void saveAccountInfo()
		{
			FileStream fs = new FileStream("Account", FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			for (int i = 0; i < accounts.Count; i++)
			{
				sw.WriteLine("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
						accounts[i].Username,
						Convert.ToBase64String(Encoding.UTF8.GetBytes(accounts[i].Server)),
						Convert.ToBase64String(Encoding.UTF8.GetBytes(accounts[i].Password)),
						accounts[i].Tribe,
						accounts[i].Language,
						accounts[i].ServerLang,
						accounts[i].Proxy);
				accounts[i].Changed = false;
			}
			sw.Close();
		}

		private void newtab(TLoginInfo LoginInfo)
		{
			// check if repeat login
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				if (!(tabControl1.TabPages[i].Controls[0] is MainFrame))
					continue;
				MainFrame mf = tabControl1.TabPages[i].Controls[0] as MainFrame;
				if (mf.LoginInfo.Username == LoginInfo.Username && mf.LoginInfo.Server == LoginInfo.Server)
				{
					textLog.AppendText(string.Format("[{0}][{1}] {2}{3}", DateTime.Now.ToString(), "Info", mui._("repeatlogin") + LoginInfo.Username + " @ " + LoginInfo.Server, Environment.NewLine));
					return;
				}
			}

			TabPage tp = new TabPage();
			MainFrame uc1 = new MainFrame() { LoginInfo = LoginInfo, mui = mui };
			//uc1.tb = textLog;
			uc1.UpTP = tp;
			uc1.Dock = DockStyle.Fill;
			tp.Controls.Add(uc1);
			tabControl1.TabPages.Add(tp);
			tabControl1.SelectTab(tp);
			uc1.Login();
			timer1.Enabled = true;
		}

		private static void ReadStatistics()
		{
			try
			{
				if (File.Exists("Statistics"))
				{
					FileStream fs = new FileStream("Statistics", FileMode.Open, FileAccess.Read);
					StreamReader sr = new StreamReader(fs, Encoding.UTF8);
					Pagecount = Convert.ToInt32(sr.ReadLine());
					Buildcount = Convert.ToInt32(sr.ReadLine());
					sr.Close();
				}
			}
			catch (Exception)
			{ }
		}
		private static void WriteStatistics()
		{
			int pcount = Travian.Pagecount;
			Travian.Pagecount -= pcount;
			Pagecount += pcount;

			int bcount = Travian.Buildcount;
			Travian.Buildcount -= bcount;
			Buildcount += bcount;

			lock (writelock)
			{
				FileStream fs = new FileStream("Statistics", FileMode.Create, FileAccess.Write);
				StreamWriter sr = new StreamWriter(fs, Encoding.UTF8);
				sr.WriteLine(Pagecount);
				sr.WriteLine(Buildcount);
				sr.Close();
			}
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
			MsgBox fe = new MsgBox() { message = sb.ToString() };
			fe.ShowDialog();
		}

		#region ContextMenu
		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			loginToolStripMenuItem.Enabled = listView1.SelectedIndices.Count != 0;
			if (listView1.SelectedIndices.Count == 0)
				loginToolStripMenuItem.Text = mui._("loginnoacc");
			else if (listView1.SelectedIndices.Count == 1)
				loginToolStripMenuItem.Text = mui._("loginacc") + listView1.SelectedItems[0].Text;
			else
				loginToolStripMenuItem.Text = mui._("loginaccs");
		}

		private void CMenuLogin_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < listView1.SelectedIndices.Count; i++)
				newtab(accounts[listView1.SelectedIndices[i]]);
		}
		private void CMenuLoginAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < accounts.Count; i++)
				newtab(accounts[i]);
		}

		private void CMenuAddAccount_Click(object sender, EventArgs e)
		{
			NewAccount na = new NewAccount(false) { mui = mui };
			if (na.ShowDialog() == DialogResult.OK && na.accountresult != null)
			{
				accounts.Add(na.accountresult);
				listView1_Refresh();
				saveAccountInfo();
			}
		}
		private void CMenuEditAccount_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count < 1)
				return;
			NewAccount na = new NewAccount(true) { mui = mui, logininfo = accounts[listView1.SelectedIndices[0]] };
			if (na.ShowDialog() == DialogResult.OK && na.accountresult != null)
			{
				//accounts.Add(na.accountresult);
				accounts[listView1.SelectedIndices[0]].Username = na.accountresult.Username;
				accounts[listView1.SelectedIndices[0]].Server = na.accountresult.Server;
				accounts[listView1.SelectedIndices[0]].Tribe = na.accountresult.Tribe;
				accounts[listView1.SelectedIndices[0]].Language = na.accountresult.Language;
				accounts[listView1.SelectedIndices[0]].Proxy = na.accountresult.Proxy;
				if (na.accountresult.Password != "")
					accounts[listView1.SelectedIndices[0]].Password = na.accountresult.Password;
				listView1_Refresh();
				saveAccountInfo();
			}

		}
		private void CMenuDeleteAccount_Click(object sender, EventArgs e)
		{
			for (int i = listView1.SelectedIndices.Count - 1; i >= 0; i--)
				accounts.RemoveAt(listView1.SelectedIndices[i]);
			listView1_Refresh();
			saveAccountInfo();
		}

		#endregion

		private void timer1_Tick(object sender, EventArgs e)
		{
			WriteStatistics();
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				if (!(tabControl1.TabPages[i].Controls[0] is MainFrame))
					continue;
				MainFrame mf = tabControl1.TabPages[i].Controls[0] as MainFrame;
				if (mf.LoginInfo.Tribe == 0)
					return;
				else if (mf.LoginInfo.Changed)
				{
					listView1_Refresh();
					saveAccountInfo();
					break;
				}
			}

			//timer1.Enabled = false;
		}

		private void timerIcon_Tick(object sender, EventArgs e)
		{
			notifyIcon1.Icon = Resources.free;
			timerIcon.Enabled = false;
		}
		private void ACUpClick(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == 0)
				return;
			for (int i = 0; i < listView1.SelectedIndices.Count; i++)
			{
				int n = listView1.SelectedIndices[i];
				accounts.Reverse(n - 1, 2);
				ListViewItem tlvi = listView1.Items[n - 1];
				listView1.Items.RemoveAt(n - 1);
				listView1.Items.Insert(n, tlvi);
			}
			saveAccountInfo();
		}

		private void ACDownClick(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[listView1.SelectedIndices.Count - 1] == listView1.Items.Count - 1)
				return;
			for (int i = listView1.SelectedIndices.Count - 1; i >= 0; i--)
			{
				int n = listView1.SelectedIndices[i];
				accounts.Reverse(n, 2);
				ListViewItem tlvi = listView1.Items[n + 1];
				listView1.Items.RemoveAt(n + 1);
				listView1.Items.Insert(n, tlvi);
			}
			saveAccountInfo();
		}

		DateTime GetPe32Time(string fileName)
		{
			int seconds;
			using (var br = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
			{
				var bs = br.ReadBytes(2);
				if (bs.Length != 2) return DateTime.MinValue;
				if (bs[0] != 'M' || bs[1] != 'Z') return DateTime.MinValue;
				br.BaseStream.Seek(0x3c, SeekOrigin.Begin);
				var offset = br.ReadByte();
				br.BaseStream.Seek(offset, SeekOrigin.Begin);
				bs = br.ReadBytes(4);
				if (bs.Length != 4) return DateTime.MinValue;
				if (bs[0] != 'P' || bs[1] != 'E' || bs[2] != 0 || bs[3] != 0) return DateTime.MinValue;
				bs = br.ReadBytes(4);
				if (bs.Length != 4) return DateTime.MinValue;
				seconds = br.ReadInt32();
			}
			return DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc).
					AddSeconds(seconds).ToLocalTime();
		}
	}

	public class TLoginInfo
	{
		int m_tribe;
		public bool Changed { get; set; }
		public string Server { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Proxy { get; set; }
		public int Tribe
		{
			get
			{
				return m_tribe;
			}
			set
			{
				m_tribe = value;
				Changed = true;
			}
		}
		public string Language { get; set; }
		public string ServerLang { get; set; }
		public TLoginInfo()
		{
		}
		public TLoginInfo(string[] accountdata)
		{
			Username = accountdata[0];
			Server = Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[1]));
			Password = Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[2]));
			if (accountdata.Length > 3)
				Tribe = Convert.ToInt32(accountdata[3]);
			if (accountdata.Length > 4)
				Language = accountdata[4];
			if (accountdata.Length > 5)
				ServerLang = accountdata[5];
			if (accountdata.Length > 6)
				Proxy = accountdata[6];
			if (accountdata.Length > 7) Proxy += ":" + accountdata[7];
		}
		string invalidchar = "\\/:*?\"<>|";
		public string GetKey()
		{
			string str = string.Format("{0}@{1}", Username, Server);
			foreach (var x in invalidchar)
			{
				str = str.Replace(x, '-');
			}
			return str;
		}
	}
}
