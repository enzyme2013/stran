using System;
using System.Collections.Generic;
using System.Text;

namespace libTravian
{
	class RaidOption : IOption
	{
		#region IOption 成员

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

		#endregion

		public Travian UpCall { get; set; }

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
	}
}
