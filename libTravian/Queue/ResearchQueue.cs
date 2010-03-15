using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;

namespace libTravian
{
	public class ResearchQueue : IQueue
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
			get { return UpCall.GetAidLang(UpCall.TD.Tribe, Aid); }
		}

		public string Status
		{
			get
			{
				string level, status;
				int timecost;
				if(!UpCall.TD.Villages.ContainsKey(VillageID))
				{
					UpCall.DebugLog("Unknown VillageID given in queue, cause to be deleted!", DebugLevel.E);
					MarkDeleted = true;
					return "UNKNOWN VID";
				}
				var CV = UpCall.TD.Villages[VillageID];
				TInBuilding x;

				if(ResearchType == TResearchType.UpAttack)
				{
					if(TargetLevel == 0)
						level = "";
					else
						level = string.Format("{0}/{1}", CV.Upgrades[Aid].AttackLevel, TargetLevel);
					timecost = CV.TimeCost(Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Aid][CV.Upgrades[Aid].AttackLevel]);
					x = CV.InBuilding[3];
				}
				else if(ResearchType == TResearchType.UpDefence)
				{
					if(TargetLevel == 0)
						level = "";
					else
						level = string.Format("{0}/{1}", CV.Upgrades[Aid].DefenceLevel, TargetLevel);
					timecost = CV.TimeCost(Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Aid][CV.Upgrades[Aid].DefenceLevel]);
					x = CV.InBuilding[4];
				}
				else
				{
					level = "";
					timecost = CV.TimeCost(Buildings.ResearchCost[(UpCall.TD.Tribe - 1) * 10 + Aid]);
					x = CV.InBuilding[5];
				}
				if(timecost != 0)
					status = "Lacking of resource";
				else if(x == null || x.FinishTime.AddSeconds(15) < DateTime.Now)
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
				int timecost;
				var CV = UpCall.TD.Villages[VillageID];
				TInBuilding x;
				if(ResearchType == TResearchType.UpAttack)
				{
					timecost = CV.TimeCost(Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Aid][CV.Upgrades[Aid].AttackLevel]);
					x = CV.InBuilding[3];
				}
				else if(ResearchType == TResearchType.UpDefence)
				{
					timecost = CV.TimeCost(Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Aid][CV.Upgrades[Aid].DefenceLevel]);
					x = CV.InBuilding[4];
				}
				else
				{
					timecost = CV.TimeCost(Buildings.ResearchCost[(UpCall.TD.Tribe - 1) * 10 + Aid]);
					x = CV.InBuilding[5];
				}
				if(x != null && x.FinishTime.AddSeconds(15) > DateTime.Now)
					timecost = Math.Max(timecost, Convert.ToInt32(x.FinishTime.Subtract(DateTime.Now).TotalSeconds) + 15);
				return timecost;
			}
		}

		public void Action()
		{
			var CV = UpCall.TD.Villages[VillageID];
			int GID;
			var Q = this;
			switch(ResearchType)
			{
				case TResearchType.Research:
					if(!CV.Upgrades[Aid].CanResearch)
					{
						MarkDeleted = true;
						UpCall.Dirty = true;
						UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
						return;
					}
					GID = 22;
					break;
				case TResearchType.UpAttack:
					if(TargetLevel != 0 && CV.Upgrades[Aid].AttackLevel >= TargetLevel || CV.Upgrades[Aid].AttackLevel >= CV.BlacksmithLevel)
					{
						MarkDeleted = true;
						UpCall.Dirty = true;
						UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
						return;
					}
					GID = 12;
					break;
				case TResearchType.UpDefence:
					if(TargetLevel != 0 && CV.Upgrades[Aid].DefenceLevel >= TargetLevel || CV.Upgrades[Aid].DefenceLevel >= CV.ArmouryLevel)
					{
						MarkDeleted = true;
						UpCall.Dirty = true;
						UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
						return;
					}
					GID = 13;
					break;
				default:
					return;
			}
			
			string result = UpCall.PageQuery(VillageID, "build.php?gid=" + GID.ToString());
			Match m = Regex.Match(result, "&(?:amp;)a=" + Aid.ToString() + "&(?:amp;)c=(.*?)\">", RegexOptions.Singleline);
			if (m.Success)
			{
				string c = m.Groups[1].Value;
				UpCall.PageQuery(VillageID, "build.php?gid=" + GID.ToString() + "&a=" + Aid.ToString() + "&c=" + c);
				UpCall.BuildCount();
			}

			if(TargetLevel == 0 || ResearchType == TResearchType.Research)
			{
				MarkDeleted = true;
				UpCall.Dirty = true;
				UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
			}
			else if(ResearchType == TResearchType.UpAttack)
			{
				if(CV.Upgrades[Aid].AttackLevel >= TargetLevel || CV.Upgrades[Aid].AttackLevel >= CV.BlacksmithLevel)
				{
					MarkDeleted = true;
					UpCall.Dirty = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
				}
			}
			else if(ResearchType == TResearchType.UpDefence)
			{
				if(CV.Upgrades[Aid].DefenceLevel >= TargetLevel || CV.Upgrades[Aid].DefenceLevel >= CV.ArmouryLevel)
				{
					MarkDeleted = true;
					UpCall.Dirty = true;
					UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Queue, VillageID = VillageID });
				}
			}
			UpCall.CallStatusUpdate(this, new Travian.StatusChanged() { ChangedData = Travian.ChangedType.Research, VillageID = VillageID });
		}

		#endregion

		[Json]
		public TResearchType ResearchType { get; set; }

		public enum TResearchType
		{
			Research, UpAttack, UpDefence
		}

		[Json]
		public int Aid { get; set; }

		/// <summary>
		/// Target level
		/// </summary>
		[Json]
		public int TargetLevel { get; set; }

		public int QueueGUID
		{
			get
			{
				switch(ResearchType)
				{
					case TResearchType.UpAttack:
						return 3;
					case TResearchType.UpDefence:
						return 4;
					case TResearchType.Research:
						return 5;
					default: // will not happened
						return -1;
				}
			}
		}
	}
}
