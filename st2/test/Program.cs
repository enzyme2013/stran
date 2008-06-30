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

			Console.ReadLine();
			*/
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
	}
}
//801 * 400 - 801y + x