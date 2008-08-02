using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;
using libTravian;
using Stran;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			/*
			string data = File.ReadAllText("build.htm");
			parseinmarket(data);
			 */
			//xmltest();

			/*
			int a = Z(-75, 147);
			Debug.Assert(X(a) == -75);
			Debug.Assert(Y(a) == 147);

			TVillage village = new TVillage() { Name = "我的村庄" };
			for (int i = 0; i < village.Resource.Length; i ++)
			{
				village.Resource[i] = new TResource(0, 0, 1000);
			}

			MUI mui = new MUI("en");

			ResourceLimit limit = new ResourceLimit()
			{
				Village = village,
				Description = mui._("lowerlimit"),
				Limit = village.Market.LowerLimit = new TResAmount(0, 0, 0, 0),
				mui = mui
			};

			limit.ShowDialog();
			*/
			parsertroop();
			Console.ReadLine();
		}
		public enum TMType
		{
			MyOut,
			MyBack,
			OtherCome
		};
		static void parseinmarket(string data)
		{
			string[] s = new string[2];
			int MCount = 0, MCarry = 0, MLevel = 0;
			TMType MType;
			var m = Regex.Match(data, "var haendler = (\\d+);");
			if(m.Success)
				MCount = Convert.ToInt32(m.Groups[1].Value);

			m = Regex.Match(data, "var carry = (\\d+);");
			if(m.Success)
				MCarry = Convert.ToInt32(m.Groups[1].Value);

			m = Regex.Match(data, "(\\d+)</b></h1>");
			if(m.Success)
				MLevel = Convert.ToInt32(m.Groups[1].Value);


			string t1 = "<p class=\"b\">";
			string[] sp = data.Split(new string[] { t1 }, StringSplitOptions.None);
			if(sp.Length == 3)
			{
				// Write out langfile
				if(s[0] == null)
					s[0] = sp[1].Split(new string[] { "</p>" }, 1, StringSplitOptions.None)[0];
				if(s[1] == null)
					s[1] = sp[2].Split(new string[] { "</p>" }, 1, StringSplitOptions.None)[0];
			}
			for(int i = 1; i < sp.Length; i++)
			{
				var mc = Regex.Matches(sp[i],
					"karte.php\\?d=(\\d+)&c=[^>]*\"><span class=\"c0\">([^<]+)</span>.*?<span id=timer\\d+>([0-9:]{6,})</span>.*?<span class=\"([c ]*?)f10\">(?:<img .*?>(\\d+)[^<]*){4,4}",
					 RegexOptions.Singleline);
				/// @@1 Target Pos
				/// @@2 Target VName
				/// @@3 TransferTime
				/// @@4 "" for MyOut, "c " for MyBack
				/// @@5 Amounts
				foreach(Match m1 in mc)
				{
					if(sp.Length == 3)
						MType = i == 1 ? TMType.OtherCome : TMType.MyOut;
					else if(s[0] != null && sp[i].Contains(s[0]))
						MType = TMType.OtherCome;
					else if(s[1] != null && sp[i].Contains(s[1]))
						MType = TMType.MyOut;
					else if(MCount == MLevel)
					{
						if(s[0] == null)
							s[0] = sp[1].Split(new string[] { "</p>" }, 1, StringSplitOptions.None)[0];
						MType = TMType.OtherCome;
					}
					else
						MType = TMType.MyOut;

					if(MType == TMType.MyOut && m1.Groups[4].Value.Length != 0)
						MType = TMType.MyBack;
					var am = new string[4];
					for(int j = 0; j < 4; j++)
						am[j] = m1.Groups[5].Captures[j].Value;
					Console.WriteLine("Pos:{0}, VName:{1}, Time:{2}, Type:{3}, Amount:{4}", m1.Groups[1], m1.Groups[2], m1.Groups[3], MType, string.Join("|", am));
				}
			}
		}

		static void xmltest()
		{
			var data = new Dictionary<string, string>();
			data["a"] = "vvv";
			data["helo"] = "12321";
			XmlSerializer s = new XmlSerializer(typeof(Dictionary<string, string>));
			TextWriter w = new StreamWriter("list.xml");
			s.Serialize(w, data);
			w.Close();
		}

		static public int Z(int X, int Y)
		{
			return 801 * (400 - Y) + X + 401;
		}
		static public int X(int value)
		{
			return value % 801 - 401;
		}
		static public int Y(int value)
		{
			return 400 - value / 801;
		}

		static void parsertroop()
		{
			var data = @"""1"" cellpadding=""2"" class=""tbg"">

<tr class=""cbg1"">
<td width=""21%""><a href=""karte.php?d=111909&c=09""><span class=""c0"">C1M9相坂さよ</span></a></td>
<td colspan=""11"" class=""b"">从0_0返回</td>
</tr>

<tr class=""unit"">
<td>&nbsp;</td><td><img src=""img/un/u/21.gif"" title=""方阵兵""></td><td><img src=""img/un/u/22.gif"" title=""剑士""></td><td><img src=""img/un/u/23.gif"" title=""探路者""></td><td><img src=""img/un/u/24.gif"" title=""雷法师""></td><td><img src=""img/un/u/25.gif"" title=""德鲁伊骑兵""></td><td><img src=""img/un/u/26.gif"" title=""海顿圣骑士""></td><td><img src=""img/un/u/27.gif"" title=""冲撞车""></td><td><img src=""img/un/u/28.gif"" title=""投石器""></td><td><img src=""img/un/u/29.gif"" title=""首领""></td><td><img src=""img/un/u/30.gif"" title=""拓荒者""></td><td><img src=""img/un/u/hero.gif"" title=""英雄""></td></tr><tr><td>军队</td><td class=""c"">0</td><td>9836</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td class=""c"">0</td><td>118</td><td class=""c"">0</td><td class=""c"">0</td><td>1</td></tr></tr><tr class=""cbg1""><td>目的地</td><td colspan=""11"">
<table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""f10"">
<tr align=""center"">
<td width=""50%"">&nbsp; 需要 <span id=timer1>18:18:46</span> 小时</td>
<td width=""50%"">于 19:02:08</span><span> 点</td>
</tr></table></td></table><p><b>村庄里的军队</b></p><p>
";
			var items = data.Split(new string[] { "<table cellspacing=" }, StringSplitOptions.None);
			foreach(var item in items)
			{
				var m = Regex.Match(item, "<td width=\"\\d+%\"><a href=\".*?\"><span class=\"c0\">(.*?)</span></a></td>.*<td colspan=.*?>(.*?)</td>.*?img/un/u/(\\d+)\\.gif.*?(?:<td[^>]*>(\\d+|\\?)</td>){10,11}.*?(?:>(\\d+)<img class=\"res|<span id=timer1>(.*?)</span>)", RegexOptions.Singleline);
				/*
				 * @@1 from vname
				 * @@2 to vname
				 * @@3 gif index for tribe
				 * @@4 troopcount
				 * @@5 cropcost
				 * @@6 time on way
				 */
				if(!m.Success)
					continue;
				Console.WriteLine("Success1");
				Debugger.Break();
			}
		}
	}
}
//801 * 400 - 801y + x
// 