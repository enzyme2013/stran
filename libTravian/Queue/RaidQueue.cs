using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	public class RaidOption : IQueue
	{
		#region IQueue 成员

		public Travian UpCall { get; set; }

		public string Title
		{
			get
			{
				return Targets[TargetID].ToString();
			}
		}

		public string Status
		{
			get
			{
				if(Troops == null)
					return "N/A";
				var s = string.Format("({0}/{1})", TargetID + 1, Targets.Count);
				if(UpCall != null)
				{
					for(int i = 0; i < Troops.Length; i++)
						if(Troops[i] > 0)
							s += string.Format(" {0}-{1}", Troops[i], UpCall.GetAidLang(UpCall.TD.Tribe, i));
				}
				return s;
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

		public bool MarkDeleted
		{
			get { throw new NotImplementedException(); }
		}

		public int CountDown
		{
			get { throw new NotImplementedException(); }
		}

		public void Action()
		{
			throw new NotImplementedException();
		}

		[Json]
		public bool Paused
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Json]
		public int VillageID
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion


		public IList<TPoint> Targets { get; set; }
		public int TargetID { get; set; }
		public int[] Troops { get; set; }
		public void NextTarget()
		{
			if(TargetID == Targets.Count - 1)
				TargetID = 0;
			else
				TargetID++;
		}

		#region IQueue 成员
		//public string IO { get { return Export(); } set { Import(value); } }
		#endregion
	}
}
