﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;

namespace libTravian
{
	public class BuildingQueue : IQueue
	{
		#region IQueue 成员

		public Travian UpCall { get; set; }

		public string Title
		{
			get
			{
				if(TargetLevel == 0)
					return string.Format("{0} @ {1}", DisplayLang.Instance.GetGidLang(Gid), Bid);
				return string.Format("{0} @ {1} -> {2}", DisplayLang.Instance.GetGidLang(Gid), Bid, TargetLevel);
			}
		}

		public string Status
		{
			get
			{
				string level = TargetLevel == 0 ? "" : string.Format("{0} -> {1} ", CurrentLevel, TargetLevel);
				string status;
				if(!UpCall.TD.Villages.ContainsKey(VillageID))
				{
					UpCall.DebugLog("Unknown VillageID given in queue, cause to be deleted!", DebugLevel.E);
					MarkDeleted = true;
					return "UNKNOWN VID";
				}
				var CV = UpCall.TD.Villages[VillageID];
				int qtype = Bid < 19 && Bid > 0 ? 0 : 1;
				int timecost;
				if(CV.Buildings.ContainsKey(Bid))
					timecost = CV.TimeCost(Buildings.Cost(CV.Buildings[Bid].Gid, CV.Buildings[Bid].Level + 1));
				else
					timecost = CV.TimeCost(Buildings.Cost(Gid, 1));
				var x = CV.InBuilding[UpCall.TD.isRomans ? qtype : 0];
				if(timecost != 0)
					status = "Lacking of resource";
				else if((NextExec < DateTime.Now ) && (x == null || x.FinishTime.AddSeconds(15) < DateTime.Now))
					status = "Starting";
				else
					status = "Waiting";
				return level + status;
			}
		}

		public bool MarkDeleted { get; set; }

		[Json]
		public int VillageID { get; set; }

		[Json]
		public bool Paused { get; set; }

		public int CountDown
		{
			get
			{
				var CV = UpCall.TD.Villages[VillageID];
				int qtype = Bid < 19 && Bid > 0 ? 0 : 1;
				int timecost;
				if(CV.Buildings.ContainsKey(Bid))
					timecost = CV.TimeCost(Buildings.Cost(CV.Buildings[Bid].Gid, CV.Buildings[Bid].Level + 1));
				else
					timecost = CV.TimeCost(Buildings.Cost(Gid, 1));
				var x = CV.InBuilding[UpCall.TD.isRomans ? qtype : 0];
				if(timecost != 0)
					return timecost;
				else if ((NextExec < DateTime.Now) && (x == null || x.FinishTime.AddSeconds(15) < DateTime.Now))
					return 0;
				else if (NextExec >= DateTime.Now)
					return Convert.ToInt32(NextExec.Subtract(DateTime.Now).TotalSeconds) + 5;
				else
					return Convert.ToInt32(x.FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5;
			}
		}

		public void Action()
		{
			BuildingQueue Q = this;
			var CV = UpCall.TD.Villages[VillageID];
			if (Q.TargetLevel != 0 && CV.Buildings.ContainsKey(Q.Bid) && CV.Buildings[Q.Bid].Gid == Q.Gid && CV.Buildings[Q.Bid].Level >= Q.TargetLevel)
			{
				Q.MarkDeleted = true;
				UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
				return;
			}
			
			if(Q.NextExec >= DateTime.Now)
				return;
			Q.NextExec = DateTime.Now.AddSeconds(60);
			int Bid = UpCall.testPossibleNow(VillageID, Q);
			if(Bid == -1)
			{
				Q.MarkDeleted = true;
				UpCall.DebugLog("Delete Queue [" + Q.Title + "] because it's impossible to build it.", DebugLevel.W);
				UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
				return;
			}
			if(Bid != 0)
			{
				UpCall.DebugLog("Queue [" + Q.Title + "] needs Bid=" + Bid.ToString() + " to be extended.", DebugLevel.W);
				Q = new BuildingQueue() { UpCall = UpCall, Bid = Bid, Gid = CV.Buildings[Bid].Gid };
				UpCall.DebugLog("Create Queue [" + Q.Title + "] because it needs to be extended.", DebugLevel.W);
			}

			int bid = Q.Bid;
			int gid = Q.Gid;
			string result;
			result = UpCall.PageQuery(VillageID, "dorf1.php");
			if(result == null)
				return;
			result = UpCall.PageQuery(VillageID, "build.php?id=" + bid.ToString());
			if(result == null)
				return;
			Match m, n;
			m = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + gid + "&(?:amp;)?id=" + bid + "&(?:amp;)?c=[^\"]*");
			n = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + bid + "&(?:amp;)?c=[^\"&]*\"");
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
				if(gid != 10 && UpCall.GidLang.ContainsKey(10) && Regex.Match(result, "<span class=\"(c|none)\">[^<]*?" + UpCall.GetGidLang(10) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
					gid = 10;
					bid = findBuilding(VillageID, gid);
				}
                else if (gid != 11 && UpCall.GidLang.ContainsKey(11) && Regex.Match(result, "<span class=\"(c|none)\">[^<]*?" + UpCall.GetGidLang(11) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
					gid = 11;
					bid = findBuilding(VillageID, gid);
				}
                else if (gid != 4 && UpCall.GidLang.ContainsKey(4) && Regex.Match(result, "<span class=\"(c|none)\">[^<]*?" + UpCall.GetGidLang(4) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
				{
                	gid = 4;
                	bid = findBuilding(VillageID, gid);
                	if (UpCall.TD.isRomans && Q.Bid > 18)
                	{
                		UpCall.DebugLog("Romans need crop rule", DebugLevel.W);
                		if (CV.InBuilding[0] != null)
                		{
                			if (CV.InBuilding[0].Gid != 4)
                			{
	                			CV.Queue.Insert(0, new BuildingQueue()
				                {
								UpCall = UpCall,
								VillageID = VillageID,
								Bid = bid,
								Gid = gid
								});
                				UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
                				UpCall.DebugLog("Create Queue BID = [" + bid + "], due to Romans need crop.", DebugLevel.W);
                			}
                			Q.NextExec = CV.InBuilding[0].FinishTime.AddSeconds(CV.Queue[0].CountDown + 30);
                			return;
                		}
                		else if (CV.InBuilding[0] == null)
                		{
                			for (int i = 0; i < CV.Queue.Count; i ++)
                			{
                				if (CV.Queue[i] is BuildingQueue)
                				{
                					var L = CV.Queue[i] as BuildingQueue;
                					if (L.Gid < 4 && !L.Paused)
                						break;
                					else if (L.Gid == 4 && !L.Paused)
                					{
                						Q.NextExec = DateTime.Now.AddSeconds(L.CountDown + 30);
                						return;
                					}
                				}
                			}
                		}
                	}
                }
                else if (result.Contains("<p class=\"(c|none)\">"))
				{
					UpCall.DebugLog("Unexpected status! Report it on the forum! " + Q.Title, DebugLevel.E);
					Q.MarkDeleted = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
					return;
				}
                else if (UpCall.GidLang.ContainsKey(gid) && Regex.Match(result, "<span class=\"(c|none)\">[^<]*?" + UpCall.GetGidLang(gid) + "[^<]*?</span>", RegexOptions.IgnoreCase).Success)
					return;
                else if (result.Contains("<span class=\"(c|none)\">"))
				{
					//Q.Delay = rand.Next(500, 1000);
					// Delay shouldn't happen.
					Q.NextExec = DateTime.Now.AddSeconds(rand.Next(150, 300));
					UpCall.DebugLog("Data not refreshed? Add delay for " + Q.Title, DebugLevel.W);
					return;
				}
				else
				{
					if (RetryCount >= 2)
					{
						UpCall.DebugLog("Unknown status! And cause a queue been deleted! " + Q.Title, DebugLevel.E);
						Q.MarkDeleted = true;
						UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
					}
					UpCall.DebugLog("Unknown status!" + Q.Title, DebugLevel.E);
					UpCall.PageQuery(VillageID, "dorf1.php");
					UpCall.PageQuery(VillageID, "dorf2.php");
					RetryCount++;
					Q.NextExec = DateTime.Now.AddSeconds(rand.Next(150, 300));
					return;
				}
				RetryCount = 0;
				// test if resource enough
				int timecost;
				if(CV.Buildings.ContainsKey(bid))
					timecost = CV.TimeCost(Buildings.Cost(gid, CV.Buildings[bid].Level + 1));
				else
					timecost = CV.TimeCost(Buildings.Cost(gid, 1));
				if(CV.InBuilding[UpCall.TD.isRomans && bid > 18 ? 1 : 0] != null)
					timecost = Math.Max(timecost, Convert.ToInt32(DateTime.Now.Subtract(CV.InBuilding[UpCall.TD.isRomans && bid > 18 ? 1 : 0].FinishTime).TotalSeconds));
				if(timecost > 0)
				{
					UpCall.DebugLog("Need to upgrade (BID= " + bid.ToString() + ")but resource not enough so add into queue: " + Q.Title, DebugLevel.W);
					CV.Queue.Insert(0, new BuildingQueue()
					{
						UpCall = UpCall,
						VillageID = VillageID,
						Bid = bid,
						Gid = gid
					});
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });

					return;
				}
				result = UpCall.PageQuery(VillageID, "build.php?id=" + bid.ToString());
				if(result == null)
					return;
				m = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + gid + "&(?:amp;)?id=" + bid + "&(?:amp;)?c=[^\"]*");
				n = Regex.Match(result, "dorf(\\d)\\.php\\?a=" + bid + "&(?:amp;)?c=[^\"&]*\"");
				if(!m.Success && !n.Success)
				{
					UpCall.DebugLog("Unknown error on building " + Q.Title, DebugLevel.E);
					Q.MarkDeleted = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
					return;
				}
			}

			// New building
			int qtype = bid < 19 && bid > 0 ? 0 : 1;

			if(CV.Buildings.ContainsKey(bid))
				CV.RB[UpCall.TD.isRomans ? qtype : 0] = new TInBuilding() { ABid = bid, Gid = gid, Level = CV.Buildings[bid].Level };
			else
				CV.RB[UpCall.TD.isRomans ? qtype : 0] = new TInBuilding() { ABid = bid, Gid = gid, Level = 0 };
			if(m.Success)
				UpCall.PageQuery(VillageID, m.Groups[0].Value.Replace("&amp;", "&"));
			else
				UpCall.PageQuery(VillageID, n.Groups[0].Value.Replace("\"", "").Replace("&amp;", "&"));
			UpCall.BuildCount();
			//CV.Buildings[bid].Level++;
			if(Q.Bid == bid)
				UpCall.DebugLog("Build " + Q.Title, DebugLevel.I);
			else
				UpCall.DebugLog("Build (other) " + Q.Title, DebugLevel.I);
			if(Q.Bid == bid)
			{
				if(Q.TargetLevel == 0 || Q.TargetLevel <= CV.Buildings[bid].Level)
				{
					Q.MarkDeleted = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
				}
			}
			UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Buildings, VillageID = VillageID });
		}

		#endregion

		public BuildingQueue()
		{
			Bid = Gid = TargetLevel = 0;
			Paused = false;
		}

		private int findBuilding(int VillageID, int Gid)
		{
			int tlvl = 20, tid = 0;
			//var b = UpCall.TD.Villages[VillageID].Buildings;
			foreach(var x in UpCall.TD.Villages[VillageID].Buildings)
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
		/// Building slot ranges 1 - 2x
		/// </summary>
		[Json]
		public int Bid { get; set; }

		/// <summary>
		/// Building type (gid)
		/// </summary>
		[Json]
		public int Gid { get; set; }

		public int CurrentLevel
		{
			get
			{
				if(UpCall.TD.Villages[VillageID].Buildings.ContainsKey(Bid))
					return UpCall.TD.Villages[VillageID].Buildings[Bid].Level;
				return 99;
			}
		}

		/// <summary>
		/// Target building level
		/// </summary>
		[Json]
		public int TargetLevel { get; set; }
		
		[Json]
		public DateTime NextExec;
		
		private Random rand = new Random();
		private int RetryCount = 0;

		public int QueueGUID { get { return Bid < 19 && Bid > 0 ? 0 : 1; } }
	}
}
