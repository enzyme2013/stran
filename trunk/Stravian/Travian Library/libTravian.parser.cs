using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace Stravian
{
	// WebPage Ellement Parser
	partial class libTravian
	{
		int _villagecount = 0;
		public Dictionary<int, string> _lang = new Dictionary<int, string>(48);

		private int parseTribe(string result)
		{
			Match m = Regex.Match(result, "img/un/u/(\\d*)\\.gif");
			return Convert.ToInt32(m.Groups[1].Value) / 10 + 1;
		}

		private delegate void listaddf_d(int id, string name, Point pos, int did);
		private void listaddf(int id, string name, Point pos, int did)
		{
			ListViewItem lvi = ParentFrame.listViewVillage.Items.Add(id.ToString());
			lvi.SubItems.Add("0");
			lvi.SubItems.Add(name);
			lvi.SubItems.Add(string.Format("{0}|{1}", pos.X, pos.Y));
			lvi.SubItems.Add(did.ToString());
		}	//delegate
		private void parseVillages(string result)
		{
			int i;
			MatchCollection mc;
			mc = Regex.Matches(result, "<span(.*?)>&#8226;.*?newdid=(\\d*).*?>([^<]*?)</a>.*?\\((-?\\d*?)<.*?\">(-?\\d*?)\\)", RegexOptions.Singleline);
			//.*?\\((-?\\d*?)<.*\">(-?\\d*?)\\)
			/*
			 * Groups:
			 * [1]: is_default
			 * [2]: village id
			 * [3]: village name
			 * [4&5]: position
			 */
			int villagecount = mc.Count == 0 ? 1 : mc.Count;
			if(villagecount <= _villagecount)
				return;
			//int cnt = 0;
			result = fetchpage("spieler.php?uid=" + uid);

			if(mc.Count == 0)
			{
				Match m = Regex.Match(result, "karte.php\\?d=(\\d+)&c=.*?\">([^<]*)</a>.*?(</span>)?</td");
				if(villages.Count < 1)
					villages.Add(new Village(0, m.Groups[2].Value, Convert.ToInt32(m.Groups[1].Value), true));
				ParentFrame.Invoke(new listaddf_d(listaddf), new object[] { 0, m.Groups[2].Value, villages[0].pos, 0 });

			}
			else
				for(i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];
					int vid = Convert.ToInt32(m.Groups[2].Value);
					for(i = 0; i < villages.Count; i++)
						if(vid == villages[i].id)
							break;
					if(i != villages.Count)
						continue;
					villages.Add(new Village(vid, m.Groups[3].Value, new Point(Convert.ToInt32(m.Groups[4].Value), Convert.ToInt32(m.Groups[5].Value))));
					ParentFrame.Invoke(new listaddf_d(listaddf), new object[] { i, m.Groups[3].Value, new Point(Convert.ToInt32(m.Groups[4].Value), Convert.ToInt32(m.Groups[5].Value)), vid });

					if(m.Groups[1].Value != "")
						currid = i;
				}


			mc = Regex.Matches(result, "karte.php\\?d=(\\d+)&c=.*?\">([^<]*)</a>.*?(</span>)?</td");

			foreach(Village x in villages)
			{
				foreach(Match m in mc)
				{
					if(x.z == Convert.ToInt32(m.Groups[1].Value))
					{
						x.capital = (m.Groups[3].Value.Length > 0);
						break;
					}
				}

			}
			_villagecount = villagecount;
		}

		private void parseResource(string result)
		{
			MatchCollection m;
			m = Regex.Matches(result, "<td id=l\\d title=(-?\\d+)>(\\d+)/(\\d+)</td>");
			if(m.Count == 4)
			{
				for(int i = 0; i < 4; i++)
					villages[currid].res.w(i,
						Convert.ToInt32(m[i].Groups[1].Value),
						Convert.ToInt32(m[i].Groups[2].Value),
						Convert.ToInt32(m[i].Groups[3].Value)
						);
				//villages[currid].resuptime = DateTime.Now;
			}
		}

		private TimeSpan myTimeSpanParse(string time)
		{
			string[] data = time.Split(':');
			int hours = 0, mins = 0, secs = 0;
			if(data.Length > 0)
				hours = int.Parse(data[0]);
			if(data.Length > 1)
				mins = int.Parse(data[1]);
			if(data.Length > 2)
				secs = int.Parse(data[2]);
			int days = hours / 24;
			hours %= 24;
			return new TimeSpan(days, hours, mins, secs);
		}

		private void parseInbuilding(string result)
		{
			MatchCollection m;
			m = Regex.Matches(result, "\"></a></td><td>([^<]*) \\(\\S+ (\\d*)\\)</td><td><span id=timer\\d*>(\\d+:\\d+:\\d+)</span> ");
			/*
			 * [1]: build.name
			 * [2]: build.level
			 * [3]: build.lefttime
			 */
			villages[currid].inb[0] = null;
			villages[currid].inb[1] = null;
			for(int i = 0; i < m.Count; i++)
			{

				inbuild tinb;
				int gid = -1;
				foreach(KeyValuePair<int, string> kvp in _lang)
					if(kvp.Value == m[i].Groups[1].Value)
					{
						gid = kvp.Key;
						break;
					}

				if(gid != -1)
					tinb = new inbuild(
						gid,
						Convert.ToInt32(m[i].Groups[2].Value),
						myTimeSpanParse(m[i].Groups[3].Value));
				else
					continue;
					/*
				else
					tinb = new inbuild(
						m[i].Groups[1].Value,
						Convert.ToInt32(m[i].Groups[2].Value),
						myTimeSpanParse(m[i].Groups[3].Value), _mine);
					 */
				villages[currid].inb[_isRomans ? tinb.type : 0] = tinb;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <returns>Level after this destroying.</returns>
		private int parseIndestroying(string result)
		{
			Match m;
			m = Regex.Match(result, "\"></a></td><td>([^<]*) \\(\\S+ (\\d*)\\)</td><td><span id=timer\\d*>(\\d+:\\d+:\\d+)</span> ");
			villages[currid].inb[2] = null;
			if(m.Success)
			{
				int gid = -1;
				foreach(KeyValuePair<int, string> kvp in _lang)
					if(kvp.Value == m.Groups[1].Value)
					{
						gid = kvp.Key;
						break;
					}

				if(gid != -1)
					villages[currid].inb[2] = new inbuild(
						gid,
						Convert.ToInt32(m.Groups[2].Value),
						myTimeSpanParse(m.Groups[3].Value),
						2);
				return Convert.ToInt32(m.Groups[2].Value);
			}
			return -1;
			//" 19)</td><td><span id=timer1>0:43:40</span>"
		}

		private static int[][] dorf1List = new int[][]{
			new int[]{4,4,1,4,4,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{3,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,2,2,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{1,4,1,3,1,2,3,4,4,3,3,4,4,1,4,2,1,2},
			new int[]{4,4,1,3,4,4,4,4,4,4,4,4,4,4,4,2,4,4}
		};

		private bool dorf1Building(string data, ref Building[] buildings)
		{
			Match m = Regex.Match(data, "<div id=\"f(\\d+)\">");
			if(!m.Success)
				return false;
			int dorfType = Convert.ToInt32(m.Groups[1].Value);
			for(int i = 0; i < dorf1List[dorfType - 1].Length; i++)
			{
				buildings[i + 1] = new Building(dorf1List[dorfType - 1][i]);
			}
			return true;
		}
		private void parsedorf1Building(string result)
		{
			if(!dorf1Building(result, ref villages[currid].buildings))
				return;
			MatchCollection mc = Regex.Matches(result, "<img src=\"[^\"]*?img/un/g/s/s(\\d+).gif\" class=\"rf(\\d+)\">");
			if(mc.Count == 0)
				return;
			foreach(Match m in mc)
				villages[currid].buildings[Convert.ToInt32(m.Groups[2].Value)].level = Convert.ToInt32(m.Groups[1].Value);
		}

		private void parsedorf2Building(string result)
		{
			MatchCollection mc = Regex.Matches(result, "<img class=\"d(\\d+)\" src=\"[^\"]*?img/un/g/g(\\d+)[^.]*\\.gif\">");
			if(mc.Count == 0)
				return;

			foreach(Match m in mc)
				villages[currid].buildings[Convert.ToInt32(m.Groups[1].Value) + 18] = new Building(Convert.ToInt32(m.Groups[2].Value));

			Match mm = Regex.Match(result, "d2_(\\d+)");
			if(!mm.Success)
				return;
			/*
			 * Rm, Tt, Gl
			 * 11, 12, 1	+ 2
			 * 13, 14, 3	% 12
			 * 1,  2,  3	+ 30
			 * 31, 32, 33
			*/
			//if(mm.Groups[1].Value != "0")
			//	villages[currid].buildings[40] = new Building((Convert.ToInt32(mm.Groups[1].Value) + 2) % 12 + 30);
			villages[currid].buildings[40] = new Building(Tribe + 30);
			villages[currid].buildings[39] = new Building(16);

			mc = Regex.Matches(result, "<area href=\"build.php\\?id=(\\d+)\" title=\"[^0-9\"]*?(\\d+)[^0-9\"]*?\" coords");
			if(mc.Count == 0)
				return;
			foreach(Match m in mc)
				villages[currid].buildings[Convert.ToInt32(m.Groups[1].Value)].level = Convert.ToInt32(m.Groups[2].Value);

		}

		private void parseDorf1(string result)
		{
			//if(villages.Count == 0)
			//parseVillages(result);
			//parseMine(result);
			parseResource(result);
			parsedorf1Building(result);
			parseLanguage(result);
			parseInbuilding(result);
		}

		private void parseDorf2(string result)
		{
			//if(villages.Count == 0)
			//parseVillages(result);
			parseResource(result);
			parsedorf2Building(result);
			parseLanguage(result);
			parseInbuilding(result);
		}

		private void parseLanguage(string result)
		{
			MatchCollection mc = Regex.Matches(result, "build.php\\?id=(\\d+)\"[^>]*? title=\"([^\"]*?) [^ ]+ \\d");
			//StringBuilder sb = new StringBuilder();
			int id;
			foreach(Match m in mc)
			{
				id = Convert.ToInt32(m.Groups[1].Value);
				if(villages[currid].buildings[id] == null)
					continue;
				if(!_lang.ContainsKey(villages[currid].buildings[id].gid))
					_lang[villages[currid].buildings[id].gid] = m.Groups[2].Value;
			}
			if(_mine == null)
				_mine = _lang[1] + _lang[2] + _lang[3] + _lang[4];
		}
		/*
		private void parseMine(string result)
		{
			if(_mine != null)
				return;
			MatchCollection mc = Regex.Matches(result, "shape=\"circle\" title=\"(\\S*) ");
			StringBuilder sb = new StringBuilder();
			foreach(Match m in mc)
			{
				sb.Append(m.Groups[1].Value);
			}
			_mine = sb.ToString();
		}
		 * */
	}
}
