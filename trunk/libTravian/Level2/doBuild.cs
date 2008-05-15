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
using System.Text;
using System.Text.RegularExpressions;

namespace libTravian
{
	partial class Travian
	{
		private Random rand = new Random();
		private void doBuild(int VillageID, int QueueID)
		{
			var CV = TD.Villages[VillageID];
			TQueue Q = CV.Queue[QueueID];
			doBuild(VillageID, Q);
		}
		private void doBuild(int VillageID, TQueue Q)
		{
			var CV = TD.Villages[VillageID];

			if(Q.NextExec >= DateTime.Now)
				return;
			Q.NextExec = DateTime.Now.AddSeconds(60);
			int Bid = testPossibleNow(VillageID, Q);
			if(Bid == -1)
			{
				if(CV.Queue.Contains(Q))
				{
					CV.Queue.Remove(Q);
					CV.SaveQueue(userdb);
				}
				DebugLog("Delete Queue [" + Q.ToString() + "] because it's impossible to build it.", DebugLevel.W);
				return;
			}
			if(Bid != 0)
			{
				DebugLog("Queue [" + Q.ToString() + "] needs Bid=" + Bid.ToString() + " to be extended.", DebugLevel.I);
				Q = new TQueue() { Bid = Bid, Gid = CV.Buildings[Bid].Gid, QueueType = TQueueType.Building };
				DebugLog("Create Queue [" + Q.ToString() + "] because it needs to be extended.", DebugLevel.I);
			}

			int bid = Q.Bid;
			int gid = Q.Gid;
			string result;
			result = PageQuery(VillageID, "dorf1.php");
			if(result == null)
				return;
			result = PageQuery(VillageID, "build.php?id=" + bid.ToString());
			if(result == null)
				return;
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

				if(gid != 10 && GidLang.ContainsKey(10) && Regex.Match(result, "<span class=\"c\">[^<]*?" + GetGidLang(10) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
					gid = 10;
					bid = findBuilding(VillageID, gid);
				}
				else if(gid != 11 && GidLang.ContainsKey(11) && Regex.Match(result, "<span class=\"c\">[^<]*?" + GetGidLang(11) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
					gid = 11;
					bid = findBuilding(VillageID, gid);
				}
				else if(gid != 4 && GidLang.ContainsKey(4) && Regex.Match(result, "<span class=\"c\">[^<]*?" + GetGidLang(4) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
					gid = 4;
					bid = findBuilding(VillageID, gid);
				}
				else if(result.Contains("<p class=\"c\">"))
				{
					DebugLog("Unexpected status! Report it on the forum! " + Q.ToString(), DebugLevel.W);
					if(CV.Queue.Contains(Q))
					{
						CV.Queue.Remove(Q); // shouldn't occur.
						CV.SaveQueue(userdb);
					}
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID });
					return;
				}
				else if(GidLang.ContainsKey(gid) && Regex.Match(result, "<span class=\"c\">[^<]*?" + GetGidLang(gid) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
					return;
				else if(result.Contains("<span class=\"c\">"))
				{
					//Q.Delay = rand.Next(500, 1000);
					// Delay shouldn't happen.
					Q.NextExec = DateTime.Now.AddSeconds(rand.Next(150, 300));
					DebugLog("Data not refreshed? Add delay for " + Q.ToString(), DebugLevel.I);
					return;
				}
				else
				{
					PageQuery(VillageID, "dorf1.php");
					PageQuery(VillageID, "dorf2.php");
					DebugLog("Unknown status! And cause a queue been deleted! " + Q.ToString(), DebugLevel.W);
					if(CV.Queue.Contains(Q))
					{
						int QueueID = CV.Queue.IndexOf(Q);
						CV.Queue.Remove(Q); // shouldn't occur.
						CV.SaveQueue(userdb);
						StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
					}
					return;
				}
				// test if resource enough
				int timecost;
				if(CV.Buildings.ContainsKey(bid))
					timecost = CV.TimeCost(Buildings.Cost(gid, CV.Buildings[bid].Level + 1));
				else
					timecost = CV.TimeCost(Buildings.Cost(gid, 1));
				if(CV.InBuilding[TD.isRomans && bid > 18 ? 1 : 0] != null)
					timecost = Math.Max(timecost, Convert.ToInt32(DateTime.Now.Subtract(CV.InBuilding[TD.isRomans && bid > 18 ? 1 : 0].FinishTime).TotalSeconds));
				if(timecost > 0)
				{
					DebugLog("Need to build but resource not enough so add into queue: " + Q.ToString(), DebugLevel.I);
					int QueueID;
					if(CV.Queue.Contains(Q))
						QueueID = CV.Queue.IndexOf(Q);
					else
						QueueID = 0;
					CV.Queue.Insert(QueueID, new TQueue() { Bid = bid, Gid = gid, QueueType = TQueueType.Building });
					CV.SaveQueue(userdb);
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });

					return;
				}
				result = PageQuery(VillageID, "build.php?id=" + bid.ToString());
				if(result == null)
					return;
				m = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + gid + "&id=" + bid + "&c=[^\"]*");
				n = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + bid + "&c=[^\"]*");
				if(!m.Success && !n.Success)
				{
					DebugLog("Unknown error on building " + Q.ToString(), DebugLevel.E);
					if(CV.Queue.Contains(Q))
					{
						int QueueID = CV.Queue.IndexOf(Q);
						CV.Queue.Remove(Q);
						CV.SaveQueue(userdb);
						StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
					}
					return;
				}
			}

			// New building
			if(CV.Buildings.ContainsKey(bid))
				CV.RB[TD.isRomans ? Q.Type : 0] = new TInBuilding() { ABid = bid, Gid = gid, Level = CV.Buildings[bid].Level };
			else
				CV.RB[TD.isRomans ? Q.Type : 0] = new TInBuilding() { ABid = bid, Gid = gid, Level = 1 };
			if(m.Success)
				PageQuery(VillageID, m.Groups[0].Value);
			else
				PageQuery(VillageID, n.Groups[0].Value);
			BuildCount();
			//CV.Buildings[bid].Level++;
			if(Q.Bid == bid)
				DebugLog("Build " + Q.ToString(), DebugLevel.I);
			else
				DebugLog("Build BID=" + bid.ToString(), DebugLevel.I);
			if(Q.Bid == bid)
			{
				if(CV.Queue.Contains(Q) && (Q.TargetLevel == 0 || Q.TargetLevel <= CV.Buildings[bid].Level))
				{
					int QueueID = CV.Queue.IndexOf(Q);
					CV.Queue.Remove(Q);
					CV.SaveQueue(userdb);
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
				}
				else
					Q.Status = string.Format("{0}/{1}", CV.Buildings[bid].Level, Q.TargetLevel);
			}
			else
				Q.Status = Q.ToString();
			StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Buildings, VillageID = VillageID });
		}

		private int findBuilding(int VillageID, int Gid)
		{
			int tlvl = 20, tid = 0;
			var b = TD.Villages[VillageID].Buildings;
			foreach(var x in TD.Villages[VillageID].Buildings)
				if(x.Value.Gid == Gid)
					if(tlvl > x.Value.Level)
					{
						tlvl = x.Value.Level;
						tid = x.Key;
					}
			if(tid != 0)
				// build a new building
				//for(int k = (gid <= 4 ? 1 : 19); k < (gid <= 4 ? 20 : b.Length); k++)
				//	if(b[k] != null && b[k].name == svrlang.Building[gid])
				return tid;
			return -1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="VillageID"></param>
		/// <param name="Q"></param>
		/// <returns>0 -> Directly buildable. positive -> pre-build gid. negative -> impossible.</returns>
		public int testPossibleNow(int VillageID, TQueue Q)
		{
			var CV = TD.Villages[VillageID];
			if(CV.Buildings.ContainsKey(Q.Bid) && CV.Buildings[Q.Bid].Gid == Q.Gid && CV.Buildings[Q.Bid].Level != 0)
				return Buildings.CheckLevelFull(Q.Gid, CV.Buildings[Q.Bid].Level, CV.isCapital) ? -1 : 0;
			return testPossibleNewNow(TD.Villages, CV, Q.Gid, Q.Bid);
		}

		static public int testPossibleNewNow(Dictionary<int, TVillage> Villages, TVillage CV, int Gid, int Bid)
		{
			List<int> CapitalNo = new List<int> { 27, 29, 30 };
			List<int> NotCapitalNo = new List<int> { 34 };
			List<int> Repeatable = new List<int> { 10, 11, 23 };
			//TQueue Q = CV.Queue[QueueID];
			// Extend

			if(Gid < 5)
				return 0;

			// Below are building new one
			if(CV.isCapital && CapitalNo.Contains(Gid))
				return -1;
			if(!CV.isCapital && NotCapitalNo.Contains(Gid))
				return -1;
			// Residence/Palace problem
			if(Gid == 26)
			{
				int PCount = 0;
				foreach(var x in Villages)
					if(x.Value.isBuildingInitialized == 2)
						foreach(var y in x.Value.Buildings)
							if(y.Value.Gid == 26)
							{
								PCount++;
								break;
							}
							else if(y.Value.Gid == 25)
								break;
				return PCount == 0 ? 0 : -1;
			}

			// Check duplicate
			int toBuild = 0;
			if(Repeatable.Contains(Gid))
			{
				foreach(var x in CV.Buildings)
				{
					if(x.Key == Bid)
						continue;
					if(x.Value.Gid == Gid)
						if(x.Value.Level == 20)
						{
							toBuild = 0;
							break;
						}
						else
							toBuild = x.Key;
				}
				if(toBuild != 0)
					return toBuild;
				else
					return 0;
			}
			// Check duplicate for non-repeatable
			foreach(var x in CV.Buildings)
				if(x.Value.Gid == Gid && x.Key != Bid)
					return -1;

			// Check depend
			if(!Buildings.Depends.ContainsKey(Gid))
				return 0;
			bool gNotFound = false;
			foreach(var x in Buildings.Depends[Gid])
			{
				bool NotFound = true;
				int canUp = 0;
				foreach(var y in CV.Buildings)
					if(x.Gid == y.Value.Gid)
						if(x.Level > y.Value.Level)
							canUp = y.Key;
						else
						{
							NotFound = false;
							break;
						}
				if(NotFound && canUp != 0)
					return canUp;
				gNotFound = gNotFound || NotFound;
				if(gNotFound)
					break;
			}
			return gNotFound ? -1 : 0;
		}
	}
}
