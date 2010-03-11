﻿using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	public class DestroyQueue : IQueue
	{
		#region IQueue 成员

		public Travian UpCall { get; set; }

		[Json]
		public int VillageID { get; set; }

		[Json]
		public bool Paused { get; set; }

		public bool MarkDeleted { get; private set; }

		public string Title
		{
			get
			{
				return string.Format("{0} {1} -> 0", DisplayLang.Instance.GetGidLang(Gid), CurrentLevel);
			}
		}

		public string Status
		{
			get
			{
				string level = string.Format("{0} -> 0 ", CurrentLevel);
				string status;
				if(!UpCall.TD.Villages.ContainsKey(VillageID))
				{
					UpCall.DebugLog("Unknown VillageID given in queue, cause to be deleted!", DebugLevel.E);
					MarkDeleted = true;
					return "UNKNOWN VID";
				}
				var CV = UpCall.TD.Villages[VillageID];
				var x = CV.InBuilding[2];
				if(x == null || x.FinishTime.AddSeconds(15) < DateTime.Now)
					status = "Starting";
				else
					status = "Waiting";
				return level + status;
			}
		}

		public int CountDown
		{
			get
			{
				var CV = UpCall.TD.Villages[VillageID];
				var x = CV.InBuilding[2];
				int timecost = 0;
				if(NextExec >= DateTime.Now)
					timecost = Convert.ToInt32(NextExec.Subtract(DateTime.Now).TotalSeconds) + 30;
				if(x != null && x.FinishTime.AddSeconds(30) > DateTime.Now) 
					timecost = Math.Max(timecost, Convert.ToInt32(x.FinishTime.Subtract(DateTime.Now).TotalSeconds) + 30);
				return timecost;
			}
		}

		public void Action()
		{
			var CV = UpCall.TD.Villages[VillageID];
			if(NextExec >= DateTime.Now)
				return;
			foreach (var x in CV.Buildings)
			{
				if (x.Value.Gid == 15 && CV.Buildings[x.Key].Level < 10)
				{
					UpCall.DebugLog("Please upgrade Main Building", DebugLevel.W);
					MarkDeleted = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
					return;
				}
			}
			NextExec = DateTime.Now.AddSeconds(rand.Next(150, 300));
			Dictionary<string, string> Postdata = new Dictionary<string, string>(){
				{"gid", "15"},
				{"a", VillageID.ToString()},
				{"abriss", Bid.ToString()},
				{"ok", "%E6%8B%86%E6%AF%81"}
			};

			UpCall.PageQuery(VillageID, "build.php", Postdata);
			UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Buildings, VillageID = VillageID });
			
			int lvl = CV.InBuilding[2] != null && CV.InBuilding[2].FinishTime > DateTime.Now ? CV.InBuilding[2].Level : -1;
			if(lvl < 0)
				UpCall.DebugLog("Unknown state", DebugLevel.E);
			if(lvl <= 0)
			{
				MarkDeleted = true;
				UpCall.FetchVillageBuilding(VillageID);
				UpCall.Dirty = true;
				UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
			}
			UpCall.BuildCount();
		}

		#endregion

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

		[Json]
		public DateTime NextExec;

		private Random rand = new Random();

		public int CurrentLevel
		{
			get
			{
				if(UpCall.TD.Villages[VillageID].Buildings.ContainsKey(Bid))
					return UpCall.TD.Villages[VillageID].Buildings[Bid].Level;
				return 0;
			}
		}

		public DestroyQueue()
		{
		}
		public int QueueGUID { get { return 2; } }
	}
}