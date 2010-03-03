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
                if (string.IsNullOrEmpty(data))
                    return;
                CheckPause(data);
                if (VillageID == 0)
                {
                    NewParseVillages(data);
                    return;
                }
                NewRefreshVillages(VillageID, data);
                if (!TD.Villages.ContainsKey(VillageID))
                	return;
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
                NewParseTroops(VillageID, data);
            }
            catch (Exception ex)
            {
                DebugLog(ex, DebugLevel.F);
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
            if (data == null || !TD.Villages.ContainsKey(VillageID))
                return;
            MatchCollection mc;
            mc = Regex.Matches(data, "&#x25CF;.*?newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
            if (mc.Count == 0)
                return;
            /*
            {
	            string viddata = this.pageQuerier.PageQuery(0, "dorf3.php", null, true, true);
	            if (viddata == null)
	            	return;
	            Match m = Regex.Match(viddata, "newdid=(\\d+)\">([^<]*?)</a></td>");
	            if (!m.Success)
	            	return;
	            int vid = Convert.ToInt32(m.Groups[1].Value);
	            string vname = m.Groups[2].Value;
	            if (TD.Villages.ContainsKey(vid))
	            {
	            	if (TD.Villages[vid].Name != vname)
	            	{
	            		TD.Villages[vid].Name = vname;
	            		TD.Dirty = true;
	            		StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
	            	}
	            }
	            else
	            {
	            	TD.Villages.Clear();
	            	this.PageQuery(0, "dorf1.php");
	            }
            }
            */
            else
            {
				Dictionary<int, TVillage> NEWTV = new Dictionary<int, TVillage>();
				bool newv = false;
                for (i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    int vid = Convert.ToInt32(m.Groups[1].Value);
                    if (TD.Villages.ContainsKey(vid))
                    {
                        if (TD.Villages[vid].Name != m.Groups[2].Value)
                        {
							TD.Villages[vid].Name = m.Groups[2].Value;
							TD.Dirty = true;
							newv = true;
                        }
                    }
                    else
                    {
                        TD.Villages[vid] = new TVillage()
                        {
                            ID = vid,
							Name = m.Groups[2].Value,
                            X = Convert.ToInt32(m.Groups[3].Value),
                            Y = Convert.ToInt32(m.Groups[4].Value),
							UpCall = this,
							Sort = i
                        };
                        TD.Dirty = true;
						newv = true;
                    }
					if (TD.Villages[vid].Sort != i)
					{
						TD.Villages[vid].Sort = i;
						TD.Dirty = true;
					}
					NEWTV.Add(vid, TD.Villages[vid]);
				}
				if (newv == true || TD.Villages.Count != NEWTV.Count)
				{
					TD.Villages.Clear();
					TD.Villages = NEWTV;
					TD.Dirty = true;
	                StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
				}
			}
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
                    TD.Dirty = true;
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
            //if (!data.Contains("<div class=\"f10 b"))
            //  return;

            MatchCollection m;
            m = Regex.Matches(data, "<a\\shref=\"([^\"]*?)\"><img\\ssrc=\"[^\"]*?img/x.gif\"[^>]*?></a></td>.*?\r?\n?<td>([^<]*)\\s\\(\\S+\\s(\\d*)\\)</td>.*?\r?\n?<td><span\\sid=\"?timer\\d+\"?>(\\d+:\\d+:\\d+)</span>");
            /*
             * [1]: cancel url
             * [2]: build.name
             * [3]: build.level
             * [4]: build.lefttime
             */
            Match m1 = Regex.Match(data, @"class="".*rf(\d+).level(\d+)""");
            Match m2 = Regex.Match(data, @"class=""building\sd(\d+)\sg(\d+)[b]?""");
            if (m1.Success || m2.Success)
            {
                CV.InBuilding[0] = null;
                CV.InBuilding[1] = null;
            }
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
                {
                    tinb = new TInBuilding()
                    {
                        CancelURL = "dorf1.php" + m[i].Groups[1].Value.Replace("&amp;", "&"),
                        Gid = gid,
                        Level = Convert.ToInt32(m[i].Groups[3].Value),
                        FinishTime = DateTime.Now.Add(TimeSpanParse(m[i].Groups[4].Value))
                    };
                }
                else
                {
                    DebugLog("Cannot recognize Gid", DebugLevel.W);
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
            if (this.GetBuildingLevel(15, data) < 0)  // test if it can destroy
                return;
            var CV = TD.Villages[VillageID];

            Match m;
            m = Regex.Match(data, @"demolish.*?<td>\s*([^<]*)\s\(\S+\s(\d+)\).*?timer\d+.*?>(\d+:\d+:\d+)</span>", RegexOptions.Singleline);
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
                	CV.Buildings[CV.RB[2].ABid].Level--;
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
                        CV.Buildings[ibbid].Level--;
                        CV.Buildings[ibbid].InBuilding = true;
                        TD.Dirty = true;
                    }
                }
                m = Regex.Match(data, @"build\.php\?gid=15&.*?del=\d+");
                if (m.Success)
                    CV.InBuilding[2].CancelURL = m.Groups[0].Value.Replace("&amp;", "&");
            }
            CV.isDestroyInitialized = 2;
        }

        private static int[][] NewDorf1Data = new int[][]{
			/*
			new int[]{4,4,1,4,4,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{3,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,2,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,1,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{4,4,1,3,4,4,4,4,4,4,4,4,4,4,4,2,4,4},
		*/
			new int[]{4, 4, 1, 4, 4, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{3, 4, 1, 3, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{1, 4, 1, 3, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{1, 4, 1, 2, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{1, 4, 1, 3, 1, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{4, 4, 1, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 4, 4},
			new int[]{1, 4, 4, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{3, 4, 4, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{3, 4, 4, 1, 1, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{3, 4, 1, 2, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2},
			new int[]{3, 1, 1, 3, 1, 4, 4, 3, 3, 2, 2, 3, 1, 4, 4, 2, 4, 4},
			new int[]{1, 4, 1, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2}
		};

        private void NewParseDorf1Building(int VillageID, string data)
        {
            Match m = Regex.Match(data, @"<div\sid=""f(\d+)"">");
            if (!m.Success)
            {
                m = Regex.Match(data, "id=\"village_map\" class=\"f(\\d+)\">");
                if (!m.Success)
                    return;
            }

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
            if (data.Contains("img/un/g/g40.gif") || data.Contains("class=\"ww g40\""))
                CV.Buildings[26] = new TBuilding { Gid = 40 };

            mc = Regex.Matches(data, "<area href=\"build.php\\?id=(\\d+)\" title=\"[^0-9\"]*?(\\d+)[^0-9\"]*?\"");
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
            var mc = Regex.Matches(data, "Popup\\((\\d*),1\\);\">\\s?([^<]*)</a>");
            foreach (Match m in mc)
                SetAidLang(Convert.ToInt32(m.Groups[1].Value), m.Groups[2].Value);
        }

        private DateTime NewParseInDoing(string data, out int aid)
        {
            //var m2 = Regex.Match(data, "<img\\sclass=\"unit\\s\\w+([0-9]+)\".*?\\r?\\n.*?\\r?\\n.*?</td>.*?\\r?\\n.*?timer1>([0-9:]+)<");
            //var m2 = Regex.Match(data, "(?:in_process.*?|%)\"><img class=\"unit u\\d?(\\d)\".*?timer1>([0-9:]+)<", RegexOptions.Singleline);
            var m2 = Regex.Match(data, "under_progress.*?<tbody>[^<]*?<tr>.*?<img class=\"unit u\\d?(\\d)\".*?timer\\d+\"?>([0-9:]+)<", RegexOptions.Singleline);
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
            //var m2 = Regex.Match(data, "class=\"s7\">(.*?)</td>.*?\r?\n.*?\r?\n?.*?timer1>([0-9:]+)<");
            var m2 = Regex.Match(data, "class=\"under_progress\">.*?<tbody><tr>[^<]*?<td[^<]*?>(.*?)</td>.*?timer\\d+\"?>([0-9:]+)<", RegexOptions.Singleline);
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
            List<int> AllowGid = new List<int>() { 12, 13, 22 };
            int level = 0, gid = -1;
            foreach (var ngid in AllowGid)
            {
                level = this.GetBuildingLevel(ngid, data);
                if (level >= 0)
                {
                    gid = ngid;
                    break;
                }
            }

            if (gid == -1)
                return;

            var CV = TD.Villages[VillageID];

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
                    CV.BlacksmithLevel = level;
                else
                    CV.ArmouryLevel = level;
                var mc = Regex.Matches(data, "Popup\\(\\d?(\\d),1\\).*?\\([^<]*?(\\d+).*?\\)", RegexOptions.Singleline);
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
            if (this.GetBuildingLevel(17, data) < 0)
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

            m = Regex.Match(data, "(\\d+)/(\\d+)</td>");
            if (!m.Success)
            {
                return;
            }

            int MCount = Convert.ToInt32(m.Groups[1].Value);
            int MLevel = Convert.ToInt32(m.Groups[2].Value);

            // Market: 0 as other, 1 as my
            string t1 = "<h4>";
            string[] sp = data.Split(new string[] { t1 }, StringSplitOptions.None);
            if (sp.Length == 3)
            {
                // Write out langfile
                if (Market[0] == null)
                    Market[0] = sp[1].Split(new string[] { "</h4>" }, StringSplitOptions.None)[0];
                if (Market[1] == null)
                    Market[1] = sp[2].Split(new string[] { "</h4>" }, StringSplitOptions.None)[0];
            }

            CV.Market.ActiveMerchant = MCount;
            CV.Market.SingleCarry = MCarry;
            CV.Market.MaxMerchant = MLevel;
            CV.Market.MarketInfo.Clear();
            for (int i = 1; i < sp.Length; i++)
            {
                TMType MType;
                if (sp[i].Contains("21%") && Market[1] == null)
                    Market[1] = sp[i].Split(new string[] { "</h4>" }, StringSplitOptions.None)[0];
                var mc = Regex.Matches(sp[i],
                    "spieler.php\\?uid=\\d+\">(.*?)</a>.*?karte.php\\?d=(\\d+)&c=[^>]*\">([^<]+)</a>.*?<span id=\"?timer\\d+\"?>([0-9:]{5,})</span>.*?(<span class=\"none\">|)(?:<img .*?>(\\d+)[^<]*){4,4}",
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
                        Market[1] = sp[1].Split(new string[] { "</h4>" }, StringSplitOptions.None)[0];

                    if (sp.Length == 3)
                        MType = i == 1 ? TMType.OtherCome : TMType.MyOut;
                    else if (Market[0] != null && sp[i].Contains(Market[0]) || Market[1] != null && !sp[i].Contains(Market[1]))
                        MType = TMType.OtherCome;
                    else if (Market[1] != null && sp[i].Contains(Market[1]))
                        MType = TMType.MyOut;
                    else if (MCount == MLevel || !m1.Groups[1].Value.Equals(TD.Username, StringComparison.OrdinalIgnoreCase))
                    {
                        if (Market[0] == null)
                            Market[0] = sp[1].Split(new string[] { "</h4>" }, StringSplitOptions.None)[0];
                        MType = TMType.OtherCome;
                    }
                    else
                        MType = TMType.MyOut;

                    if (m1.Groups[5].Value.Length != 0)
                        MType = TMType.MyBack;
					var vname = m1.Groups[3].Value;
                    //Console.WriteLine("Pos:{0}, VName:{1}, Time:{2}, Type:{3}, Amount:{4}", m1.Groups[1], m1.Groups[2], m1.Groups[3], MType, string.Join("|", am));
                    CV.Market.MarketInfo.Add(new TMInfo()
                    {
                        Coord = Convert.ToInt32(m1.Groups[2].Value),
						VillageName = vname,
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
        /// Parse the level of a matching building
        /// </summary>
        /// <param name="gid">Building type id</param>
        /// <param name="pageContent">HTML page returned by the server</param>
        /// <returns>Current building level or 0 if the current page isn't for the specific building</returns>
        public int GetBuildingLevel(int gid, string pageContent)
        {
            if (!GidLang.ContainsKey(gid))
            {
                return -1;
            }

            Match titleMatch = Regex.Match(pageContent, @"<h1>(.+)</h1>");
            if (!titleMatch.Success)
            {
                return -1;
            }

            string buildingTitle = titleMatch.Groups[1].Value;
            if (!buildingTitle.Contains(GidLang[gid]))
            {
            	if (gid == 16)
            	{
            		titleMatch = Regex.Match(pageContent, @"<h2>(.+)</h2>");
            		buildingTitle = titleMatch.Groups[1].Value;
            		if (!buildingTitle.Contains(GidLang[gid]))
            			return -1;
            		return 0;
            	}
                return -1;
            }

            Match levelMatch = Regex.Match(buildingTitle, @"(\d+)");
            if (!levelMatch.Success)
            {
                return -1;
            }

            return Convert.ToInt32(levelMatch.Groups[1].Value);
        }

        public void NewParseTroops(int VillageID, string data)
        {
        	Match m = Regex.Match(data, "<div class=\"mov\"><span class=\"a[13]\">[^<]*?</span></div>", RegexOptions.Singleline);
        	if(m.Success)
        		this.pageQuerier.PageQuery(VillageID, "build.php?gid=16", null, true, false);
        	else
        	{
	            TVillage village = this.TD.Villages[VillageID];
	 			if (this.GetBuildingLevel(16, data) < 0 && !data.Contains("<h1>Rally point"))
	            {
	                return;
	            }
	 			if (this.GetBuildingLevel(16, data) == 0 && !data.Contains("<h2>Rally point"))
	 			{
	 				village.Troop.Troops.Clear();
	 				return;
	 			}
	 			village.Troop.Troops.Clear();
	            string[] troopGroups = data.Split(new string[] { "<h4>" }, StringSplitOptions.None);
	            if (troopGroups.Length < 2)
	            {
	                return;
	            }
	
	            bool inVillageTroopsParsed = false;
	            for (int i = 1; i < troopGroups.Length; i++)
	            {
	                string[] troopDetails = HtmlUtility.GetElementsWithClass(
	                    troopGroups[i],
	                    "table",
	                    "troop_details");
	                bool postInVillageTroops = inVillageTroopsParsed;
	                foreach (string troopDetail in troopDetails)
	                {
	                    TTInfo troop = this.ParseTroopDetail(troopDetail, postInVillageTroops);
	                    if (troop != null)
	                    {
	                        village.Troop.Troops.Add(troop);
	                        if (troop.TroopType == TTroopType.InVillage)
	                        {
	                            inVillageTroopsParsed = true;
	                        }
	                    }
	                }
	            }
		        TD.Dirty = true;
	        	StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
        	}
        }

        private TTInfo ParseTroopDetail(string troopDetail, bool postInVillageTroops)
        {
            string header = HtmlUtility.GetElement(troopDetail, "thead");
            if (header == null)
            {
                return null;
            }

            string[] headerColumns = HtmlUtility.GetElements(header, "td");
            if (headerColumns.Length != 2)
            {
                return null;
            }

            Match ownerMatch = Regex.Match(headerColumns[0], @"<a href=""(.+?)"">(.+?)</a>");
            if (!ownerMatch.Success)
            {
                return null;
            }

            string owner = ownerMatch.Groups[2].Value;
            int ownerVillageZ = 0;
            string ownerVillageUrl = "";
            Match ownerVillageZMatch = Regex.Match(ownerMatch.Groups[1].Value, @"karte.php\?d=(\d+)&c=(\w+)");
            if (ownerVillageZMatch.Success)
            {
                ownerVillageZ = Convert.ToInt32(ownerVillageZMatch.Groups[1].Value);
                ownerVillageUrl = ownerVillageZMatch.Groups[0].Value;
            }

            Match nameMatch = Regex.Match(headerColumns[1], @"<a href=""(.+?)"">(.+?)</a>");
            if (!nameMatch.Success)
            {
                return null;
            }

            string name = nameMatch.Groups[2].Value;

            string unitsBody = HtmlUtility.GetElementWithClass(troopDetail, "tbody", "units");
            if (unitsBody == null)
            {
                return null;
            }

            string[] unitRows = HtmlUtility.GetElements(unitsBody, "tr");
            if (unitRows.Length < 2)
            {
                return null;
            }

            Match unitTypeMatch = Regex.Match(unitRows[0], @"class=""unit u(\d+)""");
            if (!unitTypeMatch.Success)
            {
                return null;
            }

            int unitType = Convert.ToInt32(unitTypeMatch.Groups[1].Value);
            int tribe = unitType / 10 + 1;

            string[] unitColumns = HtmlUtility.GetElements(unitRows[1], "td");
            if (unitColumns.Length < 10)
            {
                return null;
            }

            int[] units = new int[11];
            for (int i = 0; i < unitColumns.Length; i++)
            {
                Match match = Regex.Match(unitColumns[i], @"(\d+)");
                if (match.Success)
                {
                    units[i] = Convert.ToInt32(match.Groups[1].Value);
                }
                else if (unitColumns[i].Contains("?"))
                {
                    units[i] = -1;
                }
                else
                {
                    return null;
                }
            }

            string[] infoBodies = HtmlUtility.GetElementsWithClass(troopDetail, "tbody", "infos");
            if (infoBodies.Length == 0)
            {
                return null;
            }

            DateTime arrival = DateTime.MinValue;
            string callback = null;
            foreach (string infosBody in infoBodies)
            {
                string inDiv = HtmlUtility.GetElementWithClass(infosBody, "div", "in");
                if (inDiv == null)
                {
                    inDiv = HtmlUtility.GetElementWithClass(infosBody, "div", "in small");
                }

                if (inDiv != null)
                {
                    Match match = Regex.Match(inDiv, @"\d+:\d+:\d+");
                    if (match.Success)
                    {
                        arrival = DateTime.Now + TimeSpanParse(match.Groups[0].Value);
                        arrival.AddSeconds(20);
                    }
                }
                Match m = Regex.Match(infosBody, "<a href=\"([^>]*?)\">");
                if (m.Success)
                	callback = m.Groups[1].Value;
            }

            TTroopType type = postInVillageTroops ? TTroopType.InOtherVillages : TTroopType.InVillage;
            if (arrival != DateTime.MinValue)
            {
                type = postInVillageTroops ? TTroopType.Outgoing : TTroopType.Incoming;
            }

            return new TTInfo()
                {
                    Tribe = tribe,
                    Owner = owner,
                    OwnerVillageZ = ownerVillageZ,
                    OwnerVillageUrl = ownerVillageUrl,
                    VillageName = name,
                    Troops = units,
                    FinishTime = arrival,
                    TroopType = type,
                    CancelURL = callback
                };
        }

        private void ParseTroops(int VillageID, string data)
        {
            if (GetBuildingLevel(16, data) < 0 && !data.Contains("<h1>Rally point"))
                return;
            var CV = TD.Villages[VillageID];
            CV.Troop.Troops.Clear();
            var items = data.Split(new string[] { "<table class=" }, StringSplitOptions.None);
            foreach (var item in items)
            {
                var m = Regex.Match(item, "<td\\sclass=\"role\"><a\\shref=\".*?\">(.*?)</a></td><td colspan=\"1[01]\">(.*?)</a></td>.*?class=\"unit\\s\\w(\\d+)\".*?(?:<td[^>]*>(\\d+|\\?)</td>){10,11}.*?(?:>(\\d+)<img\\sclass=\"r4|.*?<span\\sid=\"?timer\\d+\"?>(.*?)</span>)", RegexOptions.Singleline);
                //                var m = Regex.Match(item, "<td\\sclass=\"role\"><a\\shref=\".*?\">(.*?)</a></td><td colspan=\"1[01]\"><a\\shref=\".*?\">(.*?)</a></td>.*?class=\"unit\\s\\w(\\d+)\".*?(?:<td[^>]*>(\\d+|\\?)</td>){10,11}.*?(?:>(\\d+)<img\\sclass=\"r4|.*?<span\\sid=\"?timer\\d+\"?>(.*?)</span>)", RegexOptions.Singleline);
                /*
                 * @@1 from vname
                 * @@2 to vname
                 * @@3 gif index for tribe
                 * @@4 troopcount
                 * @@5 cropcost
                 * @@6 time on way
                 */
                var r = Regex.Match(item, "(a2b.php\\?d=.*?&c=.*?)\"", RegexOptions.Singleline);
                string returnlink = r.Groups[1].Value;
                if (!m.Success)
                    continue;
                int[] tro = new int[11];
                for (int i = 0; i < m.Groups[4].Captures.Count; i++)
				{
                    if (m.Groups[4].Captures[i].Value == "?")
                        tro[i] = -1;
                    else
                        tro[i] = Convert.ToInt32(m.Groups[4].Captures[i].Value);
				}
                /*
               uid  time  	mark	troopcount  cropcost	returnlink
                -     O      R		 	O      	-			-		MyReturnWay
                -     O      			O      	-			-		MyAttackWay
                -     O      S			O      	-			-		MySupportWay // MyOtherSupportMeWay
                -     -      S			O      	O			O		MySupportOther
                -     O    			    -      	-			-		BeAttackedWay
                -     O    	 S		    -      	-			-		BeSupportedWay
                O     -    	 		    O      	O			-		MySelf // MyOtherSupportMe
                O     -    	 		    O       O       	O		BeSupportMe
                 */
                bool hasuid = m.Groups[2].Value.Contains("uid");
				bool hassupport = m.Groups[2].Value.StartsWith("增援到") || m.Groups[2].Value.EndsWith("支援") || m.Groups[2].Value.StartsWith("Reinforcement for");
				bool hasretrun = m.Groups[2].Value.EndsWith("回歸") || m.Groups[2].Value.EndsWith("返回") || m.Groups[2].Value.StartsWith("Return from");
				bool hasspy = m.Groups[2].Value.EndsWith("偵察") || m.Groups[2].Value.EndsWith("侦察") || m.Groups[2].Value.StartsWith("Scouting of");
				bool hasraid = m.Groups[2].Value.StartsWith("搶奪") || m.Groups[2].Value.EndsWith("抢夺") || m.Groups[2].Value.StartsWith("Raid against");
				bool hasattack = m.Groups[2].Value.StartsWith("攻擊") || m.Groups[2].Value.EndsWith("攻击") || m.Groups[2].Value.StartsWith("Attack against");
                bool hasTime = m.Groups[6].Success;
                bool hasCount = tro[0] != -1;
				bool hasCrop = m.Groups[5].Success;
                string vvname = CV.Name;
                string vname;
                TTroopType trooptype = hasuid ?
                    TTroopType.BeSupportMe :
                    (hasTime ?
                    (hasCount ?
                    (hasretrun ? TTroopType.MyReturnWay : (hassupport ? TTroopType.MySupportWay : TTroopType.MyAttackWay)) :
                    (hassupport ? TTroopType.BeSupportedWay : TTroopType.BeAttackedWay)) :
                     TTroopType.MySupportOther);
                if (trooptype == TTroopType.BeAttackedWay || trooptype == TTroopType.BeSupportedWay || trooptype == TTroopType.BeSupportMe)
                    vname = m.Groups[1].Value;
                else
                    vname = Regex.Replace(m.Groups[2].Value, "<[^>]+>", "");
                if (trooptype == TTroopType.MySupportWay && vname.Contains(vvname))
                {
                    trooptype = TTroopType.MyOtherSupportMeWay;
                    vname = m.Groups[1].Value;
                }
                if (trooptype == TTroopType.BeSupportMe)
                {
                    if (vvname == vname && !r.Success)
                        trooptype = TTroopType.MySelf;
                    else if (m.Groups[2].Value.Contains(TD.Username))
                        trooptype = TTroopType.MyOtherSupportMe;
                }

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
            StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
        }
    }
}
