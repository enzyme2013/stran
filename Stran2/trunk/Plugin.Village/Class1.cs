using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Stran2
{
	public class VillagePlugin : IPlugin
	{
		public IPageQuerier PQ = PageQuerier.Instance;

		#region IPlugin 成员

		public void Initialize()
		{
			MethodsCenter.Instance.RegisterParser(new ParserPluginCall(VillagesParser));
		}

		public bool CheckDepend()
		{
			return true;
		}

		#endregion

		public void VillagesParser(string pageData, UserData users, int villageId)
		{
			if(villageId == 0)
				ParseVillages(pageData, users);
			else
				RefreshVillages(pageData, users, villageId);
		}

		private void RefreshVillages(string data, UserData TD, int VillageID)
		{
			int i;
			if(data == null)
				return;
			MatchCollection mc;
			mc = Regex.Matches(data, "<span([^>]*?)>&#8226;.*?newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
			if(mc.Count == 0)
				return;
			else
			{
				for(i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];
					int vid = Convert.ToInt32(m.Groups[2].Value);
					if(TD.Villages.ContainsKey(vid))
						TD.Villages[vid].StringProperties["Name"] = m.Groups[3].Value;
					else
					{
						var CV = TD.Villages[vid] = new VillageData();
						CV.StringProperties["Name"] = m.Groups[3].Value;
						CV.Int32Properties["vid"] = vid;
						CV.Int32Properties["X"] = Convert.ToInt32(m.Groups[4].Value);
						CV.Int32Properties["Y"] = Convert.ToInt32(m.Groups[5].Value);
						/*
						if(userdb.ContainsKey("v" + TD.Villages[vid].ID + "role"))
							TD.Villages[vid].Role = userdb["v" + TD.Villages[vid].ID + "role"];
						else
							TD.Villages[vid].Role = "None";
						*/
					}
				}
				//StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
			}
			return;
		}

		private void ParseVillages(string data, UserData TD)
		{
			//string data = TravianData.Villages[VillageID].PageCache[TPageType.Dorf1].PageContent;
			if(data == null)
				return;
			int i;
			int Currid = 0;
			MatchCollection mc;
			mc = Regex.Matches(data, "<span([^>]*?)>&#8226;.*?newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
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
			if(!TD.Int32Properties.ContainsKey("UserID"))
			{
				var m = Regex.Match(data, "spieler.php\\?uid=(\\d*)");
				if(m.Success)
					TD.Int32Properties["UserID"] = Convert.ToInt32(m.Groups[1].Value);
			}
			data = PQ.GetEx(TD, 0, "spieler.php?uid=" + TD.Int32Properties["UserID"].ToString(), null, true, true);
			
			if(data == null)
				return;

			if(mc.Count == 0)
			{
				Match m = Regex.Match(data, "karte.php\\?d=(\\d+)&c=.*?\">([^<]*)</a>.*?(</span>)?</td");
				if(TD.Villages.Count < 1)
				{
					var CV = new VillageData();
					CV.StringProperties["Name"] = m.Groups[2].Value;
					CV.Int32Properties["X"] = TPoint.ZToX(Convert.ToInt32(m.Groups[1].Value));
					CV.Int32Properties["Y"] = TPoint.ZToY(Convert.ToInt32(m.Groups[1].Value));
					CV.Int32Properties["Z"] = Convert.ToInt32(m.Groups[1].Value);
					CV.Int32Properties["isCapital"] = 1;
					string viddata = PQ.GetEx(TD, 0, "dorf3.php", null, true, true);
					if(viddata == null)
						return;
					m = Regex.Match(viddata, "newdid=(\\d+)");
					CV.Int32Properties["vid"] = Convert.ToInt32(m.Groups[1].Value);
					/*
					tv.ID = Convert.ToInt32(m.Groups[1].Value);
					if(userdb.ContainsKey("v" + tv.ID + "role"))
						tv.Role = userdb["v" + tv.ID + "role"];
					else
						tv.Role = "None";
					TD.Villages[tv.ID] = tv;
					 */
					Currid = CV.Int32Properties["vid"];
					TD.Villages.Add(Currid, CV);
				}
			}
			else
			{
				for(i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];
					int vid = Convert.ToInt32(m.Groups[2].Value);
					if(TD.Villages.ContainsKey(vid))
						continue;
					var CV = TD.Villages[vid] = new VillageData();
					CV.Int32Properties["vid"] = vid;
					CV.StringProperties["Name"] = m.Groups[3].Value;
					CV.Int32Properties["X"] = Convert.ToInt32(m.Groups[4].Value);
					CV.Int32Properties["Y"] = Convert.ToInt32(m.Groups[5].Value);
					CV.Int32Properties["Z"] = TPoint.XYToZ(Convert.ToInt32(m.Groups[4].Value), Convert.ToInt32(m.Groups[5].Value));
					/*
					if(userdb.ContainsKey("v" + TD.Villages[vid].ID + "role"))
						TD.Villages[vid].Role = userdb["v" + TD.Villages[vid].ID + "role"];
					else
						TD.Villages[vid].Role = "None";
					 */
					if(m.Groups[1].Value != "")
						Currid = vid;
				}

				mc = Regex.Matches(data, "karte.php\\?d=(\\d+)&c=.*?\">([^<]*)</a>.*?(</span>)?</td");
				int CapZ = 0;
				foreach(Match m in mc)
				{
					if(m.Groups[3].Value.Length > 0)
						CapZ = Convert.ToInt32(m.Groups[1].Value);
				}
				foreach(var x in TD.Villages)
				{
					if(x.Value.Int32Properties["Z"] == CapZ)
						x.Value.Int32Properties["isCapital"] = 1;
					else
						x.Value.Int32Properties["isCapital"] = 0;
				}
			}
			//LastVillageCount = villagecount;
			TD.Int32Properties["ActiveDid"] = Currid;
		}

	}
}
