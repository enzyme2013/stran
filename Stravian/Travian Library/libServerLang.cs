using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stravian
{
	// MultiLanguage Interface
	public class libServerLang
	{
		public Dictionary<int, string> Building { get; set; }
		public string Auther { get; private set; }
		public libServerLang(string language)
		{
			try
			{
				Building = new Dictionary<int,string>(40);
				string lang_file = string.Format("lang\\svr_{0}.txt", language);
				if(!File.Exists(lang_file))
					lang_file = "lang\\svr_cn.txt";
				if(!File.Exists(lang_file))
				{
					return;
				}
				string[] s = File.ReadAllLines(lang_file, Encoding.UTF8);
				foreach(var s1 in s)
				{
					var pairs = s1.Split('=');
					if(pairs.Length != 2)
						continue;
					if(pairs[0].StartsWith("gid"))
						try
						{
							int gid = Convert.ToInt32(pairs[0].Substring(3));
							Building[gid] = pairs[1];
						}
						catch(Exception)
						{
							continue;
						}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}
	}
}
