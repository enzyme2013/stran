using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Stravian
{
	partial class libTravian
	{
		public static readonly int AIBID = -1024;

		public string Server { set; private get; }
		public string Username { set; private get; }
		public string Password { set; private get; }
		public int Tribe { set; private get; }
		public string Language { set; private get; }
		public UserControl1 ParentFrame { set; private get; }

		WebClient wc = new WebClient();
		private object wclock = new object(), idlock = new object();
		Random rand = new Random();
		public List<Village> villages = new List<Village>(4);
		public int currid = 0;
		string cookie = "";
		public List<BQ> bq = new List<BQ>(32);
		public bool _isRomans;
		//public int temp; // for temp use;
		public libServerLang displaylang;
		string _mine;
		public int uid;
		//string baseurl;

		//public static readonly string[] statuslist = new string[] { "Successful or Link mismatch", "已经有建筑在建造中", "资源不足", "建造完成", "需要先建造一个农场", "请先升级你的仓库" };
		//public static readonly string[] resourcelist = new string[] { "木材", "泥土", "铁块", "粮食" };
		//public static readonly string minelist = "黏土矿伐木场铁矿场农场";
		public static readonly string[] typelist = new string[] { "资源田", "建筑", "拆除", "攻击力", "防御力" };
		public static readonly string[] tribelist = new string[] { "自动探测", "罗马", "日尔曼", "高卢" };

		public libTravian(logininfo l)
		{
			Server = l.Server;
			Username = l.Username;
			Password = l.Password;
			Tribe = l.Tribe;
			Language = l.Language;
			//Server s = libServer.find(baseurl);
			//if(s == null)
			//	WriteError("No appropriate language found. Use 'cn' for default.");
			//svrlang = new libServerLang(s.svrLang);
			//_mine = string.Join("|", svrlang.Building, 1, 4);
			if(MainForm.options.ContainsKey("proxy"))
			{
				try
				{
					string p = MainForm.options["proxy"];
					wc.Proxy = new WebProxy(p);
				}
				catch(Exception)
				{ }
			}

			wc.BaseAddress = string.Format("http://{0}/", l.Server);
			wc.Encoding = Encoding.UTF8;
			//ParentFrame = form;
			//this.baseurl = baseurl;
			wc.Headers.Add(HttpRequestHeader.Referer, wc.BaseAddress);
			displaylang = new libServerLang(l.Language);
			AI.Init();
		}
		/*
		public void login(string username, string password, int tribe)
		{
			Username = username;
			Password = password;
			Tribe = tribe;
			login();
		}
		*/
		public void login()
		{
			loginstate(1);
			loginstep(1);
			Thread t = new Thread(new ThreadStart(dologin));
			t.Name = "dologin";
			t.Start();
		}
		public void dologin()
		{
			//wait();
			lock(wclock)
			{
				try
				{
					if(dorelogin())
					{
						if(Tribe == 0)
						{
							ParentFrame.Invoke(new int_d(loginstep), new object[] { 2 });
							docheckRomans();
						}
						else
						{
							_isRomans = Tribe == 1;
							ParentFrame.Invoke(new int_d(loginstep), new object[] { 3 });
							fetchdorf1();
						}
					}
					else
					{
						ParentFrame.Invoke(new int_d(loginstep), new object[] { -1 });
						ParentFrame.Invoke(new int_d(loginstate), new object[] { -1 });
						//signal();
					}
				}
				catch(Exception)
				{ }
			}
		}
		public bool dorelogin()
		{
			try
			{
				WriteInfo("Logging in as '" + Username + "', may take a few seconds...");
				string data = wc.DownloadString("/");
				string userkey, passkey, alkey;
				Match m;
				m = Regex.Match(data, "type=\"text\" name=\"(\\S+?)\"");
				if(m.Success)
					userkey = m.Groups[1].Value;
				else
				{
					WriteError("Parse userkey error!");
					return false;
				}
				m = Regex.Match(data, "type=\"password\" name=\"(\\S+?)\"");
				if(m.Success)
					passkey = m.Groups[1].Value;
				else
				{
					WriteError("Parse passkey error!");
					return false;
				}
				m = Regex.Match(data, "><input type=\"hidden\" name=\"(\\S+?)\" value");
				if(m.Success)
					alkey = m.Groups[1].Value;
				else
				{
					WriteError("Parse alkey error!");
					return false;
				}
				//wc.BaseAddress = "http://127.0.0.1/";
				string q = string.Format("w=1024%3A768&login={0}&{3}={1}&{4}={2}&{5}=&s1.x=0&s1.y=0",
					Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds),
					HttpUtility.UrlEncode(Username), HttpUtility.UrlEncode(Password), userkey, passkey, alkey);
				HttpUtility httputil = new HttpUtility();
				wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
				wc.Headers.Add(HttpRequestHeader.Referer, wc.BaseAddress);
				string result = wc.UploadString("dorf1.php?ok", "POST", q);
				if(result.IndexOf("login") >= 0)
				{
					WriteError("Not logged in. Error occurred");
					//MessageBox.Show("Login failed.");
					return false;
				}
				string[] t = wc.ResponseHeaders.GetValues("Set-Cookie");
				foreach(string t1 in t)
					if(t1.IndexOf("T3E") >= 0)
						cookie = t1;
				if(cookie == null)
				{
					WriteError("Cookie is NULL!");
					return false;
				}
				//cookie = wc.ResponseHeaders.Get("Set-Cookie");
				wc.Headers[HttpRequestHeader.Cookie] = cookie;
				wc.Headers[HttpRequestHeader.Referer] = wc.BaseAddress;

				m = Regex.Match(result, "spieler.php\\?uid=(\\d*)");
				if(m.Success)
					uid = Convert.ToInt32(m.Groups[1].Value);

				return true;
			}
			catch(Exception e)
			{
				WriteError(e.Message);
				WriteError(e.StackTrace);
			}
			return false;
		}
		public void checkRomans()
		{
			Thread t = new Thread(new ThreadStart(docheckRomans));
			t.Name = "docheckRomans";
			t.Start();
		}
		public void docheckRomans()
		{
			//wait();
			//			WriteInfo("Checking if you are Romans...");
			string result = fetchpage("a2b.php");
			Tribe = parseTribe(result);
			_isRomans = Tribe == 1;
			for(int i = 0; i < MainForm.accounts.Count; i++)
				if(MainForm.accounts[i].Username == Username && MainForm.accounts[i].Server == Server)
				{
					MainForm.accounts[i].Tribe = Tribe;
					if(ParentFrame.Parent.Parent.Parent.GetType() == typeof(MainForm))
						((MainForm)ParentFrame.Parent.Parent.Parent).RefreshListView1();
					break;
				}
			if(_isRomans)
				WriteInfo("You are Romans.");
			dofetchdorf1();
		}
		public void fetchdorf1()
		{
			Thread t = new Thread(new ThreadStart(dofetchdorf1));
			t.Name = "dofetchdorf1";
			t.Start();
		}
		public void dofetchdorf1()
		{
			dofetchdorf1(0);
		}
		public void dofetchdorf1(int newdid)
		{
			//			WriteInfo("Checking your village status...");
			lock(wclock)
			{
				ParentFrame.Invoke(new int_d(loginstep), new object[] { 3 });
				string result = fetchpage("dorf1.php?ok&newdid=" + newdid);
				parseVillages(result);
				parseDorf1(result);
				dofetchdorf2(newdid);
				//_form.Invoke(new int_d(listselectf), new object[] { currvid });
				//_form.Invoke(new int_d(loginstate), new object[] { 2 });
			}
		}
		public void fetchdorf2()
		{
			Thread t = new Thread(new ThreadStart(dofetchdorf2));
			t.Name = "dofetchdorf2";
			t.Start();
		}
		public void dofetchdorf2()
		{
			dofetchdorf2(0);
		}
		public void dofetchdorf2(int newdid)
		{
			//			WriteInfo("Checking your building status...");
			ParentFrame.Invoke(new int_d(loginstep), new object[] { 4 });
			string result = fetchpage("dorf2.php?newdid=" + newdid);
			parseDorf2(result);
			dofetchMainBuilding(newdid);
			//signal();
			//Console.WriteLine(findBuilding(villages[currid].buildings, 4));
		}
		public void dofetchMainBuilding(int newdid)
		{
			string result = fetchpage("build.php?gid=15&newdid=" + newdid);
			parseIndestroying(result);
			ParentFrame.Invoke(new int_d(listselectf), new object[] { currid });
			ParentFrame.Invoke(new int_d(loginstate), new object[] { 2 });
		}

		public void WriteLog(string source, string text)
		{
			ParentFrame.Invoke(new string_d(tf), new object[] { string.Format("[{0}][{1}@{2}{3}] {4}{5}", DateTime.Now.ToString(), Username, Server.Replace("travian.", ""), source, text, Environment.NewLine) });
			//_logbox.AppendText();
		}
		public void WriteError(string text)
		{
			WriteLog(".Error", text);
		}
		public void WriteInfo(string text)
		{
			WriteLog("", text);
		}
		private delegate void string_d(string text);
		private delegate void int_d(int i);
		private delegate void void_d();
		private void tf(string text)
		{
			ParentFrame.tb.AppendText(text);
		}			//delegate
		public void loginstate(int i)
		{
			switch(i)
			{
				case 1:
					//_form.tabControl1.SelectTab(_form.tabLog);
					//_form.buttonLogin.Enabled = false;
					//_form.buttonLogin.Text = "登录中";
					ParentFrame.progressBarLogin.Visible = true;
					ParentFrame.progressBarLogin.Value = 0;
					break;
				case 2:
					//_frame.tabControl1.SelectTab(_frame.tabStatus);
					//_form.buttonLogin.Enabled = true;
					//_form.buttonLogin.Text = "登录";
					ParentFrame.progressBarLogin.Visible = false;
					break;
				case -1:
					//_form.buttonLogin.Enabled = true;
					//_form.buttonLogin.Text = "登录失败";
					ParentFrame.progressBarLogin.Visible = false;
					break;
			}
		}		//delegate
		private void loginstep(int i)
		{
			if(i != -1)
				ParentFrame.progressBarLogin.Value = i;
			else
				ParentFrame.progressBarLogin.Visible = false;
		}		//delegate
		private void listselectf(int i)
		{
			//if (_form.listBox1.SelectedIndex != i)
			if(ParentFrame.listViewVillage.SelectedIndices.Count == 1)
				ParentFrame.listViewVillage.SelectedItems[0].Selected = false;
			ParentFrame.listViewVillage.Items[i].Selected = true;
		}		//delegate
		public void lv2delf(int i)
		{
			ParentFrame.listViewQueue.Items.RemoveAt(i);
			bq.RemoveAt(i);
		}			//delegate
		public void lvupdatef(int i)
		{
			if(ParentFrame.listViewVillage.SelectedIndices.Count == 1)
			{
				int x = ParentFrame.listViewVillage.SelectedIndices[0];
				ParentFrame.listViewVillage.SelectedItems[x].Selected = false;
				ParentFrame.listViewVillage.SelectedItems[x].Selected = true;
			}

		}			//delegate
		public void lv2ins()
		{
			ListViewItem lvi = ParentFrame.listViewQueue.Items.Insert(0, bq[0].Vid.ToString());
			lvi.SubItems.Add(bq[0].Bid.ToString());
			lvi.SubItems.Add(typelist[bq[0].Type]);
			lvi.SubItems.Add(bq[0].Status);
			lvi.SubItems.Add("");
			lvi.SubItems.Add(displaylang.Building[villages[currid].buildings[bq[0].Bid].gid]);
		}					//delegate

		public void fetchvillage(int newid)
		{
			Thread t = new Thread(new ParameterizedThreadStart(dofetchvillage));
			t.Name = "dofetchvillage";
			t.Start(newid.ToString());
		}
		public void dofetchvillage(object _i)
		{
			/*
			wait();
			if(villages[currid].id != 0)
				fetchpage("nachrichten.php?newdid=" + villages[currid].id);
			*/
			lock(idlock)
			{
				int newid = Convert.ToInt32(_i as string);
				currid = newid;
				dofetchdorf1(villages[currid].id);
			}
		}
		public void tick()
		{
			try
			{
				if(!Monitor.TryEnter(wclock))
					return;
				Monitor.Exit(wclock);
				List<string> status = new List<string>();
				for(int i = 0; i < bq.Count; i++)
					switch(bq[i].QueueType)
					{
						case BQ.TQueueType.Building:
							if(!status.Contains(bq[i].Vid.ToString() + (_isRomans ? bq[i].Type.ToString() : "")))
							{
								status.Add(bq[i].Vid.ToString() + (_isRomans ? bq[i].Type.ToString() : ""));
								int timecost;
								if(bq[i].Delay > 0)
								{
									bq[i].Delay -= 5;
									ParentFrame.listViewQueue.Items[i].SubItems[4].Text = bq[i].Delay.ToString();
								}
								else
								{
									if(bq[i].Bid == libTravian.AIBID)
									{
										if(villages[bq[i].Vid].inb[0] == null || villages[bq[i].Vid].inb[0].completetime < DateTime.Now.AddSeconds(15))
											trybuild(i);
										ParentFrame.listViewQueue.Items[i].SubItems[4].Text =
											(villages[bq[i].Vid].inb[0] != null ?
											Convert.ToInt32(villages[bq[i].Vid].inb[0].completetime.Subtract(DateTime.Now).TotalSeconds + 15) : 0).ToString();
									}
									else
									{
										if(villages[bq[i].Vid].buildings[bq[i].Bid] != null)
											timecost = Buildings.cost(villages[bq[i].Vid].buildings[bq[i].Bid].gid, villages[bq[i].Vid].buildings[bq[i].Bid].level + 1) ^ villages[bq[i].Vid].res;
										else
											timecost = Buildings.cost(bq[i].Gid, 1) ^ villages[bq[i].Vid].res;
										if((villages[bq[i].Vid].inb[_isRomans ? bq[i].Type : 0] == null ||
											villages[bq[i].Vid].inb[_isRomans ? bq[i].Type : 0].completetime < DateTime.Now.AddSeconds(-15)) &&
											timecost <= 0)
										{
											//_form.timer1.Enabled = false;
											trybuild(i);
											return;
										}
										// -1 inside listview
										//TimeSpan t;
										ParentFrame.listViewQueue.Items[i].SubItems[4].Text = Math.Max(villages[bq[i].Vid].inb[_isRomans ? bq[i].Type : 0] != null ? Convert.ToInt32(villages[bq[i].Vid].inb[_isRomans ? bq[i].Type : 0].completetime.Subtract(DateTime.Now).TotalSeconds + 15) : 0, timecost).ToString();
									}
									ParentFrame.listViewQueue.Items[i].SubItems[3].Text = bq[i].Status;
								}
							}
							break;
						case BQ.TQueueType.Destroy:
							if(!status.Contains(bq[i].Vid.ToString() + "2"))
							{
								status.Add(bq[i].Vid.ToString() + "2");
								if(villages[bq[i].Vid].inb[2] == null || villages[bq[i].Vid].inb[2].completetime < DateTime.Now.AddSeconds(-15))
								{
									distroy(i);
									return;
								}
								ParentFrame.listViewQueue.Items[i].SubItems[4].Text = (villages[bq[i].Vid].inb[2] != null ? Convert.ToInt32(villages[bq[i].Vid].inb[2].completetime.Subtract(DateTime.Now).TotalSeconds + 15) : 0).ToString();
							}
							break;
						case BQ.TQueueType.UAttack:
						case BQ.TQueueType.UDefense:
							break;
					}
			}
			catch(Exception e)
			{
				WriteError(e.Message + Environment.NewLine + e.StackTrace);
			}
		}
		public void trybuild(int i)
		{
			Thread t = new Thread(new ParameterizedThreadStart(dotrybuild));
			t.Name = "dotrybuild";
			t.Start(i.ToString());
		}
		/*public void dotrybuild(object _i)
		{
			lock(wclock)
			{
				try
				{
					int bqcount = bq.Count;
					if(bqcount != bq.Count)
						return;
					int i = Convert.ToInt32(_i as string);
					int j = bq[i].vid;
					string result;
					currid = j;
					// refresh dorf1
					string newdid = "newdid=" + villages[j].id.ToString();
					result = fetchpage("dorf1.php?" + newdid);
					parseDorf1(result);
					int gid = 0;
					int bid = 0;
					if(bq[i].gid == 0)
					{
						if(bq[i].bid == libTravian.AI)
						{
							BQ b = Stravian.AI.doAI(this, j);
							bid = b.bid;
						}
						else
							bid = bq[i].bid;
						result = fetchpage("build.php?id=" + bid.ToString() + "&" + newdid);
					}
					else
					{
						// new building
						//dorf2.php?a=17&id=27&c=e2f
						gid = Buildings.checkdepend(bq[i].gid, villages[j].buildings);//, svrlang);
						if(villages[j].inb[_isRomans ? 1 : 0] != null && villages[j].inb[_isRomans ? 1 : 0].name == svrlang.Building[gid])
						{
							fetchdorf1();
							return;
						}
						bid = bq[i].bid;
						if(gid == 16)
							bid = 39;
						else if(gid >= 31 && gid <= 33)
							bid = 40;
						if(gid == bq[i].gid)
						{
							// build directly
							if(bid == 0 || (villages[j].buildings[bq[i].bid] != null && villages[j].buildings[bq[i].bid].gid != gid))
								for(int k = 19; k < 39; k++)
									if(villages[j].buildings[k] == null)
									{
										bid = k;
										break;
									}
							if(bid == 0)
							{
								// Village is full, delete myself
								WriteError("Village is full");
								_frame.Invoke(new int_d(lv2delf), new object[] { i });
								return;
							}
						}
						else
						{
							// look for another build-site for a base building
							bid = 0;

							// upgrade
							for(int k = 19; k < 39; k++)
								if(villages[j].buildings[k] != null && villages[j].buildings[k].gid == gid)
								{
									bid = k;
									break;
								}
							if(bid == 0)
							{
								// if not found, build new building
								for(int k = 19; k < 39; k++)
									if(villages[j].buildings[k] == null && k != bq[i].bid)
									{
										bid = k;
										break;
									}
								if(bid == 0)
								{
									WriteError("Village would be full after base building being built");
									_frame.Invoke(new int_d(lv2delf), new object[] { i });
									return;
								}
							}
						}
						// nothing wrong, build it
						result = fetchpage("build.php?id=" + bid.ToString() + "&" + newdid);
						Match m = Regex.Match(result, svrlang.Building[gid] + ".*?></p>", RegexOptions.Singleline);
						if(!m.Success)
						{
							WriteError("Unknown reason caused new building failed on " + svrlang.Building[gid]);
							_frame.Invoke(new int_d(lv2delf), new object[] { i });
							return;
						}
						result = m.Groups[0].Value;
					}
					int status = status_check(result);
					bq[i].status = svrlang.Status[status];
					//WriteLog("trybuild", svrlang.Status[status]);
					int tid;
					if(bq[i].bid == libTravian.AI)
					{
						bq[i].delay = 600;
					}
					parseResource(result);

					switch(status)
					{
						case 3:
							_frame.Invoke(new int_d(lv2delf), new object[] { i });
							break;
						case 4:
							//look for a cropland
							tid = findBuilding(villages[j].buildings, 4);
							bq.Insert(0, new BQ(j, tid));
							_frame.Invoke(new void_d(lv2ins), null);
							break;
						case 5:
							tid = findBuilding(villages[j].buildings, 10);
							if(tid == -1)
								bq.Insert(0, new BQ(j, 0, 10));
							else
								bq.Insert(0, new BQ(j, tid));
							_frame.Invoke(new void_d(lv2ins), null);
							break;
						case 0:
							Match m = Regex.Match(result, "dorf(\\d)\\.php\\?a=\\d*?(&id=\\d*)?&c=[^\"]*");
							if(m.Success)
							{
								string t = fetchpage(m.Groups[0].Value);
								if(bid < 19)
									parseDorf1(t);
								else
									parseDorf2(t);
								villages[currid].buildings[bid].level++;
								if(bq[i].bid == libTravian.AI)
								{
									WriteLog(".Building(AI)", string.Format("{0} {1} @ {2} @ {3}", svrlang.Building[villages[currid].buildings[bid].gid], villages[currid].buildings[bid].level, bid, villages[currid].name));
									bq[i].status = svrlang.Building[villages[currid].buildings[bid].gid];
									bq[i].delay = 0;
								}
								else
								{
									WriteLog(".Building", string.Format("{0} {1} @ {2} @ {3}", svrlang.Building[villages[currid].buildings[bq[i].bid].gid], villages[currid].buildings[bq[i].bid].level, bq[i].bid, villages[currid].name));
									if(gid == bq[i].gid)
										_frame.Invoke(new int_d(lv2delf), new object[] { i });
								}
								if(_frame.Parent.Parent.Parent is MainForm)
									(_frame.Parent.Parent.Parent as MainForm).BuildCount();
							}
							break;
					}
				}
				catch(Exception e)
				{
					WriteError(e.Message);
					WriteError(e.StackTrace);
				}
				finally
				{
					
				}
			}
		}*/
		public void dotrybuild(object _i)
		{
			int i = Convert.ToInt32(_i as string);
			lock(wclock)
			{
				try
				{
					int bqcount = bq.Count;
					if(bqcount != bq.Count)
						return;
					int j = bq[i].Vid;
					string result;
					currid = j;
					// refresh dorf1
					string newdid = "newdid=" + villages[j].id.ToString();
					result = fetchpage("dorf1.php?" + newdid);
					parseDorf1(result);
					int gid = 0;
					int bid = 0;
					if(bq[i].Bid == libTravian.AIBID)
					{
						BQ b = AI.doAI(this, j);
						bid = b.Bid;
						gid = b.Gid;
					}
					else
					{
						bid = bq[i].Bid;
						gid = bq[i].Gid;
					}
					if(bid == 0 && gid == 0)
					{
						bq[i].Delay = 1200;
						return;
					}
					if(bid == 0)
						bid = findBuilding(villages[j].buildings, gid);
					else if(gid == 0)
						gid = villages[j].buildings[bid].gid;

					if(bid == -1)
					{
						WriteLog(".TryBuild", "Unknown BID on GID: " + gid);
						ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
						return;
					}
					result = fetchpage("build.php?id=" + bid.ToString() + "&" + newdid);
					Match m, n;
					m = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + gid + "&id=" + bid + "&c=[^\"]*");
					n = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + bid + "&c=[^\"]*");
					if(!m.Success && !n.Success)
					{
						// check reason
						/*
						 * <span class="c">已经有建筑在建造中</span>
						 * <div class="c">资源不足</div>
						 * <p class="c">伐木场建造完成</p>
						 * <span class="c">建造所需资源超过仓库容量上限,请先升级你的仓库</span>
						 * <span class="c">粮食产量不足: 需要先建造一个农场</span>
						 * 
						 */

						if(_lang.ContainsKey(10) && Regex.Match(result, "<span class=\"c\">[^<]*?:[^<]*?" + _lang[10] + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
						{
							gid = 10;
							bid = findBuilding(villages[j].buildings, gid);
						}
						else if(_lang.ContainsKey(4) && Regex.Match(result, "<span class=\"c\">[^<]*?" + _lang[4] + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
						{
							gid = 4;
							bid = findBuilding(villages[j].buildings, gid);
						}
						else if(result.IndexOf("<p class=\"c\">") >= 0)
						{
							ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
							return;
						}
						else if(_lang.ContainsKey(gid) && Regex.Match(result, "<span class=\"c\">[^<]*?" + _lang[gid] + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
							return;
						else if(result.IndexOf("<span class=\"c\">") >= 0)
						{
							bq[i].Delay = rand.Next(500, 1000);
							return;
						}
						else
						{
							parseDorf1(fetchpage("dorf1.php"));
							parseDorf2(fetchpage("dorf2.php"));
							ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
							return;
						}

						result = fetchpage("build.php?id=" + bid.ToString() + "&" + newdid);
						m = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + gid + "&id=" + bid + "&c=[^\"]*");
						n = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + bid + "&c=[^\"]*");
						if(!m.Success && !n.Success)
						{
							WriteLog(".TryBuild", "Error on building " + _lang[gid] + displaylang.Building[gid]);
							ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
							return;
						}
					}

					// New building
					string t;
					if(m.Success)
						t = fetchpage(m.Groups[0].Value + "&" + newdid);
					else
						t = fetchpage(n.Groups[0].Value + "&" + newdid);
					if(bid < 19)
						parseDorf1(t);
					else
						parseDorf2(t);
					villages[currid].buildings[bid].level++;
					if(bq[i].Bid == libTravian.AIBID)
					{
						WriteLog(".Building(AI)", string.Format("{0} {1} @ {2} @ {3}", displaylang.Building[villages[currid].buildings[bid].gid], villages[currid].buildings[bid].level, bid, villages[currid].name));
						bq[i].Status = _lang[villages[currid].buildings[bid].gid];
						bq[i].Delay = 0;
					}
					else
					{
						WriteLog(".Building", string.Format("{0} {1} @ {2} @ {3}", displaylang.Building[villages[currid].buildings[bq[i].Bid].gid], villages[currid].buildings[bq[i].Bid].level, bq[i].Bid, villages[currid].name));
						if(bq[i].TargetLevel != 0)
						{
							if(villages[currid].buildings[bid].level >= bq[i].TargetLevel)
								ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
						}
						else
							if(gid == bq[i].Gid)
								ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
					}
					MainForm.BuildCount();

				}
				catch(Exception e)
				{
					WriteError(e.Message + Environment.NewLine + e.StackTrace);
					bq[i].Delay = 600;
				}
				finally
				{

				}
			}
		}

		public void distroy(int i)
		{
			Thread t = new Thread(new ParameterizedThreadStart(dodistroy));
			t.Name = "dodistroy";
			t.Start(i.ToString());
		}
		public void dodistroy(object _i)
		{
			//gid=15&a=185668&abriss=35
			int bqcount = bq.Count;
			lock(wclock)
			{
				if(bqcount != bq.Count)
					return;
				int i = Convert.ToInt32(_i as string);
				int j = bq[i].Vid;
				currid = j;
				fetchpage("nachrichten.php?newdid=" + villages[j].id.ToString());
				wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
				string result = wc.UploadString("build.php", string.Format("gid=15&a={0}&abriss={1}&ok=%E6%8B%86%E6%AF%81", villages[j].id, bq[i].Bid));
				int lvl = parseIndestroying(result);
				if(lvl == 0)
				{
					villages[j].buildings[bq[i].Bid] = null;
					ParentFrame.Invoke(new int_d(lv2delf), new object[] { i });
				}
				else
					bq[i].Status = lvl.ToString();
				MainForm.BuildCount();
			}
			//throw new Exception("AAA");
		}
		private int findBuilding(Building[] b, int gid)
		{
			int tlvl = 20, tid = 0;
			for(int k = (gid <= 4 ? 1 : 19); k < (gid <= 4 ? 20 : b.Length); k++)
				if(b[k] != null && b[k].gid == gid)
					if(tlvl > b[k].level)
					{
						tlvl = b[k].level;
						tid = k;
					}
			if(tid != 0)
				// build a new building
				//for(int k = (gid <= 4 ? 1 : 19); k < (gid <= 4 ? 20 : b.Length); k++)
				//	if(b[k] != null && b[k].name == svrlang.Building[gid])
				return tid;
			return -1;
		}
		private string fetchpage(string uri)
		{
			try
			{
				string result = wc.DownloadString(uri);
				string[] t = wc.ResponseHeaders.GetValues("Set-Cookie");
				if(t != null)
				{
					foreach(string t1 in t)
						if(t1.IndexOf("T3E") >= 0)
							cookie = t1;
					wc.Headers[HttpRequestHeader.Cookie] = cookie;
				}
				
				if(result.IndexOf("login") >= 0)
				{
					if(!dorelogin())
					{
						ParentFrame.timer1.Stop();
						return "";
					}
					result = wc.DownloadString(uri);
				}
				if(result.IndexOf(".php?ok") >= 0)
				{
					wc.DownloadString("dorf1.php?ok");
					wc.DownloadString(uri);
				}
				MainForm.FetchPageCount();

				//wclock = false;
				return result;
			}
			catch(Exception e)
			{
				WriteError(e.Message);
			}
			return "";
		}
	}
}
