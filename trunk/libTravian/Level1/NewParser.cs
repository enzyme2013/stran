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
 * Contributor(s): [MeteorRain], [jones125], [skycen].
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libTravian
{
	partial class Travian
	{
		//int LastVillageCount = 0;

		private void NewParseEntry(int VillageID, string data)
		{
			try
			{
				// also read igm message to see if user want to pause the timer:
				CheckPause(VillageID, data);
				if (string.IsNullOrEmpty(data))
					return;
				if (VillageID == 0)
				{
					NewParseVillages(data);
					return;
				}
				NewRefreshVillages(VillageID, data);
				NewParseResource(VillageID, data);
				NewParseDorf1Building(VillageID, data);
				NewParseDorf2Building(VillageID, data);
				NewParseGLanguage(VillageID, data);
				NewParseALanguage(VillageID, data);
				NewParseInbuilding(VillageID, data);
				NewParseInDestroying(VillageID, data);
				NewParseUpgrade(VillageID, data);
				NewParseTownHall(VillageID, data);
				NewParseMarket(VillageID, data);
				ParseTroops(VillageID, data);
			}
			catch (Exception ex)
			{
				DebugLog(ex, DebugLevel.E);
			}
			DB.Instance.Snapshot(TD);
			DB.Instance.Snapshot(this);
		}

		private void NewParseResource(int VillageID, string data)
		{
			if (VillageID == 0)
				return;
			MatchCollection m;
			m = Regex.Matches(data, "<td id=\"l\\d\" title=\"(-?\\d+)\">(\\d+)/(\\d+)</td>");
			if (m.Count == 4)
				for (int i = 0; i < 4; i++)
					TD.Villages[VillageID].Resource[i] = new TResource(
						Convert.ToInt32(m[i].Groups[1].Value),
						Convert.ToInt32(m[i].Groups[2].Value),
						Convert.ToInt32(m[i].Groups[3].Value)
						);

		}

		private int NewParseTribe()
		{
			string data = this.pageQuerier.PageQuery(0, "a2b.php", null, true, true);
			if (data == null)
				return 0;
			Match m = Regex.Match(data, "<img class=\"unit u(\\d*)\"");
			return Convert.ToInt32(m.Groups[1].Value) / 10 + 1;
		}

		private void NewRefreshVillages(int VillageID, string data)
		{
			int i;
			if (data == null)
				return;
			MatchCollection mc;
			mc = Regex.Matches(data, "&#x25CF;.*?newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
			if (mc.Count == 0)
				return;
			else
			{
				for (i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];
					int vid = Convert.ToInt32(m.Groups[1].Value);
					if (TD.Villages.ContainsKey(vid))
					{
						if (TD.Villages[vid].Name != m.Groups[2].Value)
						{
							TD.Dirty = true;
							TD.Villages[vid].Name = m.Groups[2].Value;
						}
					}
					else
					{
						TD.Villages[vid] = new TVillage()
						{
							ID = vid,
							Name = m.Groups[3].Value,
							X = Convert.ToInt32(m.Groups[3].Value),
							Y = Convert.ToInt32(m.Groups[4].Value),
							UpCall = this
						};
						TD.Dirty = true;
					}
				}
				StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
			}
			return;
		}

		private void NewParseVillages(string data)
		{
			//string data = TravianData.Villages[VillageID].PageCache[TPageType.Dorf1].PageContent;
			if (data == null)
				return;
			int i;
			int Currid = 0;
			MatchCollection mc;
			mc = Regex.Matches(data, "newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
			/*
			 * Groups:
			 * [1]: is_default
			 * [2]: village id
			 * [3]: village name
			 * [4&5]: position
			 */
			//int villagecount = mc.Count == 0 ? 1 : mc.Count;
			//if(villagecount <= LastVillageCount)
			//	return -1;
			//int cnt = 0;
			data = this.pageQuerier.PageQuery(0, "spieler.php?uid=" + TD.UserID, null, true, true);
			if (data == null)
				return;

			if (mc.Count == 0)
			{
				Match m = Regex.Match(data, "karte.php\\?d=(\\d+)&amp;c=.*?\">([^<]*)</a>.*?(</span>)?</td");
				if (TD.Villages.Count < 1)
				{
					TVillage tv = new TVillage()
					{
						Name = m.Groups[2].Value,
						Z = Convert.ToInt32(m.Groups[1].Value),
						isCapital = true,
						UpCall = this
					};
					string viddata = this.pageQuerier.PageQuery(0, "dorf3.php", null, true, true);
					if (viddata == null)
						return;
					m = Regex.Match(viddata, "newdid=(\\d+)");
					tv.ID = Convert.ToInt32(m.Groups[1].Value);
					TD.Villages[tv.ID] = tv;
					Currid = tv.ID;
				}
			}
			else
			{
				for (i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];
					int vid = Convert.ToInt32(m.Groups[1].Value);
					if (TD.Villages.ContainsKey(vid))
						continue;
					TD.Villages[vid] = new TVillage()
					{
						ID = vid,
						Name = m.Groups[2].Value,
						X = Convert.ToInt32(m.Groups[3].Value),
						Y = Convert.ToInt32(m.Groups[4].Value),
						UpCall = this
					};

					if (m.Groups[2].Value != "")
						Currid = vid;
				}

				mc = Regex.Matches(data, "karte.php\\?d=(\\d+)&amp;c=.*?\">([^<]*)</a>.*?(</span>)?</td");
				int CapZ = 0;
				foreach (Match m in mc)
				{
					if (m.Groups[3].Value.Length > 0)
						CapZ = Convert.ToInt32(m.Groups[1].Value);
				}
				foreach (KeyValuePair<int, TVillage> x in TD.Villages)
				{
					if (x.Value.Z == CapZ)
						x.Value.isCapital = true;
					else
						x.Value.isCapital = false;
				}
			}
			//LastVillageCount = villagecount;
			TD.Dirty = true;
			TD.ActiveDid = Currid;
		}

		public TimeSpan TimeSpanParse(string time)
		{
			string[] data = time.Split(':');
			int hours = 0, mins = 0, secs = 0;
			if (data.Length > 0)
				hours = int.Parse(data[0]);
			if (data.Length > 1)
				mins = int.Parse(data[1]);
			if (data.Length > 2)
				secs = int.Parse(data[2]);
			int days = hours / 24;
			hours %= 24;
			return new TimeSpan(days, hours, mins, secs);
		}

		private void NewParseInbuilding(int VillageID, string data)
		{
			var CV = TD.Villages[VillageID];
			if (!data.Contains("<div class=\"f10 b"))
				return;

			MatchCollection m;
			m = Regex.Matches(data, "<a\\shref=\"([^\"]*?)\"><img\\ssrc=\"[^\"]*?img/x.gif\"[^>]*?></a></td>.*?\r?\n?<td>([^<]*)\\s\\(\\S+\\s(\\d*)\\)</td>.*?\r?\n?<td><span\\sid=[\"]?timer\\d*[\"]?>(\\d+:\\d+:\\d+)</span>");
			/*
			 * [1]: cancel url
			 * [2]: build.name
			 * [3]: build.level
			 * [4]: build.lefttime
			 */
			CV.InBuilding[0] = null;
			CV.InBuilding[1] = null;
			for (int i = 0; i < m.Count; i++)
			{
				TInBuilding tinb;
				int gid = -1;
				foreach (var kvp in GidLang)
					if (kvp.Value == m[i].Groups[2].Value)
					{
						gid = kvp.Key;
						break;
					}

				if (gid != -1)
					tinb = new TInBuilding()
					{
						CancelURL = "dorf1.php" + m[i].Groups[1].Value.Replace("&amp;", "&"),
						Gid = gid,
						Level = Convert.ToInt32(m[i].Groups[3].Value),
						FinishTime = DateTime.Now.Add(TimeSpanParse(m[i].Groups[4].Value))
					};
				else
				{
					DebugLog("Cannot recognize Gid", DebugLevel.E);
					continue;
				}
				int tinbtype = TD.isRomans ? (tinb.Gid < 5 ? 0 : 1) : 0;
				CV.InBuilding[tinbtype] = tinb;
				if (CV.RB[tinbtype] != null &&
					CV.Buildings.ContainsKey(CV.RB[tinbtype].ABid) &&
					CV.RB[tinbtype].Gid == CV.Buildings[CV.RB[tinbtype].ABid].Gid &&
					CV.RB[tinbtype].Level == CV.Buildings[CV.RB[tinbtype].ABid].Level)
				{
					CV.Buildings[CV.RB[tinbtype].ABid].Level++;
					CV.Buildings[CV.RB[tinbtype].ABid].InBuilding = true;
					TD.Dirty = true;
				}
				else
				{
					int ibbid = 0, ibbcount = 0;
					foreach (var x in CV.Buildings)
						if (x.Value.Gid == tinb.Gid && x.Value.Level == tinb.Level - 1)
						{
							ibbid = x.Key;
							ibbcount++;
						}
					if (ibbid != 0 && ibbcount == 1)
					{
						CV.InBuilding[tinbtype].ABid = ibbid;
						CV.Buildings[ibbid].Level++;
						CV.Buildings[ibbid].InBuilding = true;
						TD.Dirty = true;
					}
				}
			}
		}

		private void NewParseInDestroying(int VillageID, string data)
		{
			if (!this.IsParsingBuildingPage(15, data))  // test if it can destroy
				return;
			var CV = TD.Villages[VillageID];

			Match m;
			m = Regex.Match(data, @"</a></td><td>([^<]*)\s\(\S+\s(\d*)\)</td><td><span\sid=timer\d*>(\d+:\d+:\d+)</span>");
			CV.InBuilding[2] = null;
			if (m.Success)
			{
				int gid = -1;
				foreach (KeyValuePair<int, string> kvp in GidLang)
					if (kvp.Value == m.Groups[1].Value)
					{
						gid = kvp.Key;
						break;
					}

				if (gid != -1)
					CV.InBuilding[2] = new TInBuilding()
					{
						Gid = gid,
						Level = Convert.ToInt32(m.Groups[2].Value),
						FinishTime = DateTime.Now.Add(TimeSpanParse(m.Groups[3].Value))
					};
				if (CV.RB[2] != null &&
					CV.Buildings.ContainsKey(CV.RB[2].ABid) &&
					CV.RB[2].Gid == CV.Buildings[CV.RB[2].ABid].Gid &&
					CV.RB[2].Level == CV.Buildings[CV.RB[2].ABid].Level)
				{
					CV.Buildings[CV.RB[2].ABid].InBuilding = true;
					TD.Dirty = true;
				}
				else
				{
					int ibbid = 0, ibbcount = 0;
					foreach (var x in CV.Buildings)
						if (x.Value.Gid == CV.InBuilding[2].Gid && x.Value.Level == CV.InBuilding[2].Level + 1)
						{
							ibbid = x.Key;
							ibbcount++;
						}
					if (ibbid != 0 && ibbcount == 1)
					{
						CV.InBuilding[2].ABid = ibbid;
						CV.Buildings[ibbid].InBuilding = true;
						TD.Dirty = true;
					}
				}
				m = Regex.Match(data, @"build\.php\?gid=15&amp;del=\d+");
				if (m.Success)
					CV.InBuilding[2].CancelURL = m.Groups[0].Value.Replace("&amp;", "&");
			}
			CV.isDestroyInitialized = 2;
		}

		private static int[][] NewDorf1Data = new int[][]{
			new int[]{4,4,1,4,4,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{3,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,2,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,1,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{4,4,1,3,4,4,4,4,4,4,4,4,4,4,4,2,4,4}
		};

		private void NewParseDorf1Building(int VillageID, string data)
		{
			Match m = Regex.Match(data, @"<div\sid=""f(\d+)"">");
			if (!m.Success)
				return;

			var CV = TD.Villages[VillageID];
			int dorfType = Convert.ToInt32(m.Groups[1].Value);
			for (int i = 0; i < NewDorf1Data[dorfType - 1].Length; i++)
				CV.Buildings[i + 1] = new TBuilding() { Gid = NewDorf1Data[dorfType - 1][i] };

			// should have patched the new server-side modification problem on [id="rf*"]
			MatchCollection mc = Regex.Matches(data, @"class="".*rf(\d+).level(\d+)""");
			if (mc.Count == 0)
				return;
			foreach (Match m1 in mc)
			{
				int bid = Convert.ToInt32(m1.Groups[1].Value);
				CV.Buildings[bid].Level = Convert.ToInt32(m1.Groups[2].Value);
			}
			TD.Dirty = true;
		}

		private void NewParseDorf2Building(int VillageID, string data)
		{
			Match mm = Regex.Match(data, @"d2_(\d+)");
			if (!mm.Success)
				return;

			var CV = TD.Villages[VillageID];
			MatchCollection mc = Regex.Matches(data, @"class=""building\sd(\d+)\sg(\d+)[b]?""");
			if (mc.Count == 0)
				return;
			foreach (Match m in mc)
				CV.Buildings[Convert.ToInt32(m.Groups[1].Value) + 18] = new TBuilding() { Gid = Convert.ToInt32(m.Groups[2].Value) };
			CV.Buildings[39] = new TBuilding() { Gid = 16 };
			CV.Buildings[40] = new TBuilding() { Gid = TD.Tribe + 30 };
			if (data.Contains("img/un/g/g40.gif"))
				CV.Buildings[26] = new TBuilding { Gid = 40 };

			mc = Regex.Matches(data, "<area href=\"build.php\\?id=(\\d+)\" title=\"[^0-9\"]*?(\\d+)[^0-9\"]*?\" coords");
			if (mc.Count == 0)
				return;
			foreach (Match m in mc)
			{
				int bid = Convert.ToInt32(m.Groups[1].Value);
				if (CV.Buildings.ContainsKey(bid))
					CV.Buildings[bid].Level = Convert.ToInt32(m.Groups[2].Value);
				else
					DebugLog("Unknown bid on parsing dorf2: " + bid, DebugLevel.W);
			}
			foreach (var x in CV.Queue)
				if (x is BuildingQueue)
				{
					var y = x as BuildingQueue;
					if (y.Bid != TQueue.AIBID && !CV.Buildings.ContainsKey(y.Bid))
						CV.Buildings[y.Bid] = new TBuilding() { Gid = y.Gid };
				}
			TD.Dirty = true;
		}

		private void NewParseGLanguage(int VillageID, string data)
		{
			MatchCollection mc = Regex.Matches(data, "build.php\\?id=(\\d+)\"[^>]*?\\stitle=\"([^\"]*?)\\s[^\\s]+\\s\\d+");
			//StringBuilder sb = new StringBuilder();
			int id;
			foreach (Match m in mc)
			{
				id = Convert.ToInt32(m.Groups[1].Value);
				//if (!TD.Villages[VillageID].Buildings.ContainsKey(id))
				//  continue;
				if (!GidLang.ContainsKey(TD.Villages[VillageID].Buildings[id].Gid))
					SetGidLang(TD.Villages[VillageID].Buildings[id].Gid, m.Groups[2].Value);
			}
		}

		private void NewParseALanguage(int VillageID, string data)
		{
			var mc = Regex.Matches(data, "Popup\\((\\d*),1\\);\">([^<]*)</a>");
			foreach (Match m in mc)
				SetAidLang(Convert.ToInt32(m.Groups[1].Value), m.Groups[2].Value);
		}

		private DateTime NewParseInDoing(string data, out int aid)
		{
			//var m2 = Regex.Match(data, "<img\\sclass=\"unit\\s\\w+([0-9]+)\".*?\\r?\\n.*?\\r?\\n.*?</td>.*?\\r?\\n.*?timer1>([0-9:]+)<");
			var m2 = Regex.Match(data, "(?:in_process.*?|%)\"><img class=\"unit u\\d?(\\d)\".*?timer1>([0-9:]+)<", RegexOptions.Singleline);
			if (m2.Success)
			{
				aid = Convert.ToInt32(m2.Groups[1].Value);
				return DateTime.Now.Add(TimeSpanParse(m2.Groups[2].Value));
			}
			else
			{
				aid = 0;
				return DateTime.MinValue;
			}
		}
		private DateTime NewParseInDoing(string data, out string text)
		{
			var m2 = Regex.Match(data, "class=\"s7\">(.*?)</td>.*?\r?\n.*?\r?\n?.*?timer1>([0-9:]+)<");
			if (m2.Success)
			{
				text = m2.Groups[1].Value;
				return DateTime.Now.Add(TimeSpanParse(m2.Groups[2].Value));
			}
			else
			{
				text = null;
				return DateTime.MinValue;
			}
		}

		private void NewParseTownHall(int VillageID, string data)
		{
			if (!data.Contains("gid=24"))// && !data.Contains(GetGidLang(24)))
				return;
			string type;
			DateTime FinishedTime = NewParseInDoing(data, out type);
			if (type == null)
				return;
			var CV = TD.Villages[VillageID];
			CV.InBuilding[6] = new TInBuilding()
			{
				FinishTime = FinishedTime
			};
		}
		private void NewParseUpgrade(int VillageID, string data)
		{
			var m01 = Regex.Match(data, "gid=(\\d+)");
			List<int> AllowGid = new List<int>() { 12, 13, 22 };
			int gid = -1;
			if (m01.Success)
			{
				// check gid match:
				gid = Convert.ToInt32(m01.Groups[1].Value);
				if (!AllowGid.Contains(gid))
					return;
			}
			else
			{
				foreach (var ngid in AllowGid)
					if (data.Contains("<h1>" + GetGidLang(ngid)))
					{
						gid = ngid;
						break;
					}
				if (gid == -1)
					return;
			}

			var CV = TD.Villages[VillageID];

			var m1 = Regex.Match(data, @"(\d+)</h1>");
			if (!m1.Success)
				return;
			if (gid == 22)
			{
				var mc = Regex.Matches(data, "Popup\\(\\d?(\\d),1", RegexOptions.Singleline);
				foreach (Match m in mc)
				{
					var TroopID = Convert.ToInt32(m.Groups[1].Value);
					CV.Upgrades[TroopID].CanResearch = true;
				}
				int AID = 0;
				DateTime FinishedTime = NewParseInDoing(data, out AID);
				if (AID > 0)
				{
					CV.InBuilding[5] = new TInBuilding() { ABid = AID, FinishTime = FinishedTime, Level = 0 };
					CV.Upgrades[AID].InUpgrading = true;
				}
			}
			else
			{
				if (gid == 12)
					CV.BlacksmithLevel = Convert.ToInt32(m1.Groups[1].Value);
				else
					CV.ArmouryLevel = Convert.ToInt32(m1.Groups[1].Value);
				var mc = Regex.Matches(data, "Popup\\(\\d?(\\d),1\\).*?\\(.*? (\\d+)\\)", RegexOptions.Singleline);
				foreach (Match m in mc)
				{
					/// @@1 TroopID
					/// @@2 Level
					var TroopID = Convert.ToInt32(m.Groups[1].Value);
					CV.Upgrades[TroopID].Researched = true;
					if (gid == 12)
						CV.Upgrades[TroopID].AttackLevel = Convert.ToInt32(m.Groups[2].Value);
					else
						CV.Upgrades[TroopID].DefenceLevel = Convert.ToInt32(m.Groups[2].Value);
				}
				int AID = 0;
				DateTime FinishedTime = NewParseInDoing(data, out AID);
				if (AID > 0)
				{
					if (gid == 12)
					{
						CV.Upgrades[AID].AttackLevel++;
						CV.InBuilding[3] = new TInBuilding() { ABid = AID, FinishTime = FinishedTime, Level = CV.Upgrades[AID].AttackLevel };
					}
					else
					{
						CV.Upgrades[AID].DefenceLevel++;
						CV.InBuilding[4] = new TInBuilding() { ABid = AID, FinishTime = FinishedTime, Level = CV.Upgrades[AID].DefenceLevel };
					}
					CV.Upgrades[AID].InUpgrading = true;
				}
			}
		}

		public TResAmount JustTransferredData = null;

		private void NewParseMarket(int VillageID, string data)
		{
			if (!this.IsParsingBuildingPage(17, data))
				return;
			//DebugLog("Transfer data being parsing", DebugLevel.I);
			var CV = TD.Villages[VillageID];
			//CV.isMarketInitialized = 2;
			if (Market[0] == Market[1])
				Market[0] = null;

			Match m = Regex.Match(data, "var carry = (\\d+);");
			if (!m.Success)
			{
				return;
			}

			int MCarry = Convert.ToInt32(m.Groups[1].Value);

			m = Regex.Match(data, "(\\d+)/(\\d+)<br>");
			if (!m.Success)
			{
				return;
			}

			int MCount = Convert.ToInt32(m.Groups[1].Value);
			int MLevel = Convert.ToInt32(m.Groups[2].Value);

			// Market: 0 as other, 1 as my
			string t1 = "<p class=\"b\">";
			string[] sp = data.Split(new string[] { t1 }, StringSplitOptions.None);
			if (sp.Length == 3)
			{
				// Write out langfile
				if (Market[0] == null)
					Market[0] = sp[1].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];
				if (Market[1] == null)
					Market[1] = sp[2].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];
			}

			CV.Market.ActiveMerchant = MCount;
			CV.Market.SingleCarry = MCarry;
			CV.Market.MaxMerchant = MLevel;
			CV.Market.MarketInfo.Clear();
			for (int i = 1; i < sp.Length; i++)
			{
				TMType MType;
				if (sp[i].Contains("c f10") && Market[1] == null)
					Market[1] = sp[i].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];
				var mc = Regex.Matches(sp[i],
					"<span class=\"c0\">(.*?)</span>.*?karte.php\\?d=(\\d+)&c=[^>]*\"><span class=\"c0\">([^<]+)</span>.*?<span id=timer\\d+>([0-9:]{6,})</span>.*?<span class=\"([c ]*?)f10\">(?:<img .*?>(\\d+)[^<]*){4,4}",
					 RegexOptions.Singleline);
				/// @@1 Username
				/// @@2 Target Pos
				/// @@3 Target VName
				/// @@4 TransferTime
				/// @@5 "" for MyOut, "c " for MyBack
				/// @@6 Amounts
				foreach (Match m1 in mc)
				{
					var am = new int[4];
					for (int j = 0; j < 4; j++)
						am[j] = Convert.ToInt32(m1.Groups[6].Captures[j].Value);

					if (JustTransferredData != null &&
						Market[1] == null &&
						am[0] == JustTransferredData.Resources[0] &&
						am[1] == JustTransferredData.Resources[1] &&
						am[2] == JustTransferredData.Resources[2] &&
						am[3] == JustTransferredData.Resources[3])
						Market[1] = sp[1].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];

					if (sp.Length == 3)
						MType = i == 1 ? TMType.OtherCome : TMType.MyOut;
					else if (Market[0] != null && sp[i].Contains(Market[0]) || Market[1] != null && !sp[i].Contains(Market[1]))
						MType = TMType.OtherCome;
					else if (Market[1] != null && sp[i].Contains(Market[1]))
						MType = TMType.MyOut;
					else if (MCount == MLevel || !m1.Groups[1].Value.Equals(TD.Username, StringComparison.OrdinalIgnoreCase))
					{
						if (Market[0] == null)
							Market[0] = sp[1].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];
						MType = TMType.OtherCome;
					}
					else
						MType = TMType.MyOut;

					if (m1.Groups[5].Value.Length != 0)
						MType = TMType.MyBack;
					//Console.WriteLine("Pos:{0}, VName:{1}, Time:{2}, Type:{3}, Amount:{4}", m1.Groups[1], m1.Groups[2], m1.Groups[3], MType, string.Join("|", am));
					CV.Market.MarketInfo.Add(new TMInfo()
					{
						Coord = Convert.ToInt32(m1.Groups[2].Value),
						VillageName = m1.Groups[3].Value,
						MType = MType,
						CarryAmount = new TResAmount(am),
						FinishTime = DateTime.Now.Add(TimeSpanParse(m1.Groups[4].Value)).AddSeconds(15)
					});
				}
			}
			//if (Market[0] != null)
			//TODO:DB.Instance.SetString(TD.Server, "market0", Market[0]);
			//if (Market[1] != null)
			//TODO:DB.Instance.SetString(TD.Server, "market1", Market[1]);
		}

		/// <summary>
		/// Test if we've enter the page of a specific build (i.e. build.php?gid=xxx)
		/// </summary>
		/// <param name="gid">Building type id</param>
		/// <param name="data">Page content</param>
		/// <returns>True if we've got a page for building corresponding to gid</returns>
		private bool IsParsingBuildingPage(int gid, string data)
		{
			Match m = Regex.Match(data, @"<h1>([^<]+)\s\w+\s\d+</h1>");
			if (m.Success && m.Groups[1].Value.Contains(GidLang[gid]))
			{
				return true;
			}

			return false;
		}

		private void ParseTroops(int VillageID, string data)
		{
			/*
			 * <table cellspacing="1" cellpadding="2" class="tbg">
			
			 * <tr class="cbg1">
			 * <td width="21%">
			 * <a href="karte.php?d=320810&c=1f"><span class="c0">0-9</span></a></td>
			 * <td colspan="10" class="b">从[1]风蓝返回</td>
			 * </tr>
			 * <tr class="unit">
			 * <td>&nbsp;</td>
			 * <td><img src="img/un/u/21.gif" title="方阵兵"></td>
			 * <td><img src="img/un/u/22.gif" title="剑士"></td>
			 * <td><img src="img/un/u/23.gif" title="探路者"></td>
			 * <td><img src="img/un/u/24.gif" title="雷法师"></td>
			 * <td><img src="img/un/u/25.gif" title="德鲁伊骑兵"></td>
			 * <td><img src="img/un/u/26.gif" title="海顿圣骑士"></td>
			 * <td><img src="img/un/u/27.gif" title="冲撞车"></td>
			 * <td><img src="img/un/u/28.gif" title="投石器"></td>
			 * <td><img src="img/un/u/29.gif" title="首领"></td>
			 * <td><img src="img/un/u/30.gif" title="拓荒者"></td>
			 * </tr>
			 * <tr>
			 * <td>军队</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td>559</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td>138</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * </tr>
			 * </tr>
			 * <tr class="cbg1"><td>目的地</td>
			 * <td colspan="10">
<table width="100%" cellspacing="0" cellpadding="0" class="f10">
<tr align="center">
<td width="50%">&nbsp; 需要 <span id=timer1>0:08:53</span> 小时</td>
<td width="50%">于 22:27:06</span><span> 点</td>
</tr></table>
			 * </td>
			 * </table>
			 */

			/*
			 * <table cellspacing="1" cellpadding="2" class="tbg">
			
			 * <tr class="cbg1">
			 * <td width="21%"><a href="karte.php?d=111909&c=09"><span class="c0">C1M9相坂さよ</span></a></td>
			 * <td colspan="11" class="b">自己的军队</td>
			 * </tr>
			 * <tr class="unit">
			 * <td>&nbsp;</td>
			 * <td><img src="img/un/u/21.gif" title="方阵兵"></td>
			 * <td><img src="img/un/u/22.gif" title="剑士"></td>
			 * <td><img src="img/un/u/23.gif" title="探路者"></td>
			 * <td><img src="img/un/u/24.gif" title="雷法师"></td>
			 * <td><img src="img/un/u/25.gif" title="德鲁伊骑兵"></td>
			 * <td><img src="img/un/u/26.gif" title="海顿圣骑士"></td>
			 * <td><img src="img/un/u/27.gif" title="冲撞车"></td>
			 * <td><img src="img/un/u/28.gif" title="投石器"></td>
			 * <td><img src="img/un/u/29.gif" title="首领"></td>
			 * <td><img src="img/un/u/30.gif" title="拓荒者"></td>
			 * <td><img src="img/un/u/hero.gif" title="英雄"></td>
			 * </tr>
			 * <tr>
			 * <td>军队</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td>24738</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td>3586</td>
			 * <td class="c">0</td>
			 * <td class="c">0</td>
			 * <td>1</td>
			 * </tr><tr class="cbg1"><td>粮食消耗</td>
			 * <td class="s7" colspan="11">70998<img class="res" src="img/un/r/4.gif">每小时</td></table>
			 * 
			 * 0 0 0 24751 0 0 0 3591 0 0
			 *
			data = @"<table cellspacing=""1"" cellpadding=""2"" class=""tbg"">

<tr class=""cbg1"">
<td width=""21%""><a href=""karte.php?d=320810&c=1f""><span class=""c0"">0-9</span></a></td>
<td colspan=""10"" class=""b"">从[1]风蓝返回</td>
</tr>

<tr class=""unit"">
<td>&nbsp;</td><td><img src=""img/un/u/21.gif"" title=""方阵兵""></td><td><img src=""img/un/u/22.gif"" title=""剑士""></td><td><img src=""img/un/u/23.gif"" title=""探路者""></td><td><img src=""img/un/u/24.gif"" title=""雷法师""></td><td><img src=""img/un/u/25.gif"" title=""德鲁伊骑兵""></td><td><img src=""img/un/u/26.gif"" title=""海顿圣骑士""></td><td><img src=""img/un/u/27.gif"" title=""冲撞车""></td><td><img src=""img/un/u/28.gif"" title=""投石器""></td><td><img src=""img/un/u/29.gif"" title=""首领""></td><td><img src=""img/un/u/30.gif"" title=""拓荒者""></td></tr><tr><td>军队</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>559</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>138</td><td class=""c"">0</td><td class=""c"">0</td></tr></tr><tr class=""cbg1""><td>目的地</td><td colspan=""10"">
<table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""f10"">
<tr align=""center"">
<td width=""50%"">&nbsp; 需要 <span id=timer1>0:08:53</span> 小时</td>
<td width=""50%"">于 22:27:06</span><span> 点</td>
</tr></table></td></table>"; // TODO: 初始化为适当的值

			/*
			data = @"<p><b>村庄里的军队</b></p><p>
<table cellspacing=""1"" cellpadding=""2"" class=""tbg"">

<tr class=""cbg1"">
<td width=""21%""><a href=""karte.php?d=320810&c=1f""><span class=""c0"">0-9</span></a></td>
<td colspan=""10"" class=""b"">自己的军队</td>
</tr>

<tr class=""unit"">
<td>&nbsp;</td><td><img src=""img/un/u/21.gif"" title=""方阵兵""></td><td><img src=""img/un/u/22.gif"" title=""剑士""></td><td><img src=""img/un/u/23.gif"" title=""探路者""></td><td><img src=""img/un/u/24.gif"" title=""雷法师""></td><td><img src=""img/un/u/25.gif"" title=""德鲁伊骑兵""></td><td><img src=""img/un/u/26.gif"" title=""海顿圣骑士""></td><td><img src=""img/un/u/27.gif"" title=""冲撞车""></td><td><img src=""img/un/u/28.gif"" title=""投石器""></td><td><img src=""img/un/u/29.gif"" title=""首领""></td><td><img src=""img/un/u/30.gif"" title=""拓荒者""></td></tr><tr><td>军队</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>571</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>138</td><td class=""c"">0</td><td class=""c"">0</td></tr>
<tr class=""cbg1""><td>粮食消耗</td><td class=""s7"" colspan=""10"">1970<img class=""res"" src=""img/un/r/4.gif"">每小时</td></table></p><p class=""c"">集结点建造完成";
			*/
			if (!IsParsingBuildingPage(16, data))
				return;
			/*
			data = @"""1"" cellpadding=""2"" class=""tbg"">

<tr class=""cbg1"">
<td width=""21%""><a href=""karte.php?d=111909&c=09""><span class=""c0"">C1M9相坂さよ</span></a></td>
<td colspan=""11"" class=""b"">从0_0返回</td>
</tr>

<tr class=""unit"">
<td>&nbsp;</td><td><img src=""img/un/u/21.gif"" title=""方阵兵""></td><td><img src=""img/un/u/22.gif"" title=""剑士""></td><td><img src=""img/un/u/23.gif"" title=""探路者""></td><td><img src=""img/un/u/24.gif"" title=""雷法师""></td><td><img src=""img/un/u/25.gif"" title=""德鲁伊骑兵""></td><td><img src=""img/un/u/26.gif"" title=""海顿圣骑士""></td><td><img src=""img/un/u/27.gif"" title=""冲撞车""></td><td><img src=""img/un/u/28.gif"" title=""投石器""></td><td><img src=""img/un/u/29.gif"" title=""首领""></td><td><img src=""img/un/u/30.gif"" title=""拓荒者""></td><td><img src=""img/un/u/hero.gif"" title=""英雄""></td></tr><tr><td>军队</td><td class=""c"">0</td><td>9836</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>118</td><td class=""c"">0</td><td class=""c"">0</td><td>1</td></tr></tr><tr class=""cbg1""><td>目的地</td><td colspan=""11"">
<table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""f10"">
<tr align=""center"">
<td width=""50%"">&nbsp; 需要 <span id=timer1>18:18:46</span> 小时</td>
<td width=""50%"">于 19:02:08</span><span> 点</td>
</tr></table></td></table><p><b>村庄里的军队</b></p><p>
";
			//*/
			var CV = TD.Villages[VillageID];
			CV.Troop.Troops.Clear();
			var items = data.Split(new string[] { "<table class=" }, StringSplitOptions.None);
			foreach (var item in items)
			{
				var m = Regex.Match(item, "<th\\sclass=\"village\"><a\\shref=\".*?\"><span\\sclass=\"c0\">(.*?)</span></a></th>.*<span\\sclass=\"c0\">(.*?)</span>.*?class=\"unit\\s\\w(\\d+)\".*?(?:<td[^>]*>(\\d+|\\?)</td>){10,11}.*?(?:>(\\d+)<img\\sclass=\"r4|.*?\\r?\\n.*?\\r?\\n.*?<span\\sid=timer\\d+>(.*?)</span>)", RegexOptions.Singleline);
				//var m = Regex.Match(item, "<td width=\"\\d+%\"><a href=\".*?\"><span class=\"c0\">(.*?)</span></a></td>.*<td colspan=.*?>(.*?)</td>.*?img/un/u/(\\d+)\\.gif.*?(?:<td[^>]*>(\\d+|\\?)</td>){10,11}.*?(?:>(\\d+)<img class=\"res|<span id=timer\\d+>(.*?)</span>)", RegexOptions.Singleline);
				/*
				 * @@1 from vname
				 * @@2 to vname
				 * @@3 gif index for tribe
				 * @@4 troopcount
				 * @@5 cropcost
				 * @@6 time on way
				 */
				if (!m.Success)
					continue;
				int[] tro = new int[m.Groups[4].Captures.Count];
				for (int i = 0; i < m.Groups[4].Captures.Count; i++)
					if (m.Groups[4].Captures[i].Value == "?")
						tro[i] = -1;
					else
						tro[i] = Convert.ToInt32(m.Groups[4].Captures[i].Value);
				/*
			 link  time  troopcount
				-     O       O      MyReturnWay
				O     O       O      MyAttackWay
				O     O       O      //MySupportWay
				O     -       O      MySupportOther
				-     O       -      BeAttackedWay
				-     O       -      //BeSupportedWay
				-     -       O      MySelf
				 */
				// <a href="karte.php?d=251174&c=33"><span class="c0">对世博家园06区--铁泥木进行攻击</span></a>
				bool hasLink = m.Groups[2].Value.Contains("<a href");
				bool hasTime = m.Groups[6].Success;
				bool hasCount = tro[0] != -1;
				TTroopType trooptype = hasLink ?
					(hasTime ? TTroopType.MyAttackWay : TTroopType.MySupportOther) :
					(hasTime ?
					(hasCount ? TTroopType.MyReturnWay : TTroopType.BeAttackedWay) :
					TTroopType.MySelf);
				string vname;
				if (trooptype == TTroopType.BeAttackedWay || trooptype == TTroopType.BeSupportedWay)
					vname = m.Groups[1].Value;
				else if (hasLink)
					vname = Regex.Replace(m.Groups[2].Value, "<[^>]+>", "");
				else
					vname = m.Groups[2].Value;
				DateTime finishTime = DateTime.MinValue;
				int tribe = Convert.ToInt32(m.Groups[3].Value) / 10 + 1;
				if (hasTime)
					finishTime = DateTime.Now.Add(TimeSpanParse(m.Groups[6].Value)).AddSeconds(20);
				TTInfo ttro = new TTInfo
				{
					Tribe = tribe,
					Troops = tro,
					TroopType = trooptype,
					FinishTime = finishTime,
					VillageName = vname
				};
				CV.Troop.Troops.Add(ttro);
				Console.WriteLine(ttro.VillageName);
			}
		}
	}
}
