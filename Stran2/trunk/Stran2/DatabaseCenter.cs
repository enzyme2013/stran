using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	class DatabaseCenter
	{
		private DatabaseCenter()
		{
		}
		public static DatabaseCenter Instance = new DatabaseCenter();
		string GetDBData(string username)
		{
			return GetDBData(username, 0);
		}
		string GetDBData(string username, int villageID)
		{
			return "";
		}
		public void SaveDBData(string username, string data)
		{
			SaveDBData(username, 0, data);
		}
		public void SaveDBData(string username, int villageID, string data)
		{
		}
	}
}
