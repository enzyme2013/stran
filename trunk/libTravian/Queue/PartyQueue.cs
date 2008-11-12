using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	public class PartyQueue : IQueue
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
			get { return PartyType.ToString().Substring(1); }
		}

		public string Status
		{
			get
			{
				if(LastExec == DateTime.MinValue)
					return LastExec.ToShortTimeString();
				else
					return "";
			}
		}

		public void Import(string s)
		{
			throw new NotImplementedException();
		}

		public string Export()
		{
			throw new NotImplementedException();
		}

		public int CountDown
		{
			get
			{
				var CV = UpCall.TD.Villages[VillageID];
				int timecost = CV.TimeCost(Buildings.PartyCos[(int)PartyType]);
				if(CV.InBuilding[6] != null)
					return Math.Max(timecost, Convert.ToInt32(CV.InBuilding[6].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 30);
				else
					return timecost;
			}
		}

		public void Action()
		{
			var CV = UpCall.TD.Villages[VillageID];
			UpCall.PageQuery(VillageID, "build.php?gid=24&a=" + ((int)PartyType).ToString());
			LastExec = DateTime.Now;
			if(CV.InBuilding[6] == null || CV.InBuilding[6].FinishTime < DateTime.Now)
			{
				// error occurred!
				UpCall.DebugLog("Error on party! Delete the queue!", DebugLevel.W);
				MarkDeleted = true;
				UpCall.Dirty = true;
			}
			else
				UpCall.BuildCount();
		}

		#endregion

		private DateTime LastExec = DateTime.MinValue;

		[Json]
		public TPartyType PartyType { get; set; }

		public enum TPartyType
		{
			P500 = 1, P2000 = 2
		}

		public PartyQueue()
		{
		}
	}
}
