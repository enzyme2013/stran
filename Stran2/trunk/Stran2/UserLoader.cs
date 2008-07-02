using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Stran2
{
	static class UserLoader
	{
		static public void LoadUser(TravianDataCenter TDC, string AccountName)
		{
			if(File.Exists(AccountName))
			{
				FileStream fs = new FileStream(AccountName, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);
				while(!sr.EndOfStream)
				{
					var accountdata = sr.ReadLine().Split(':');
					var Username = accountdata[0];
					var Server = Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[1]));
					var userkey = Username + "@" + Server;
					UserData ud;
					if(TDC.Users.ContainsKey(userkey))
						ud = TDC.Users[userkey];
					else
						TDC.Users.Add(userkey, ud = new UserData());
					ud.StringProperties.Add("Username", Username);
					ud.StringProperties.Add("Server", Server);
					ud.StringProperties.Add("Password", Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[2])));
					if(accountdata.Length > 3)
						ud.Int32Properties.Add("Tribe", Convert.ToInt32(accountdata[3]));
					if(accountdata.Length > 4)
						ud.StringProperties.Add("Language", accountdata[4]);
				}
				sr.Close();
			}
		}
	}
}
