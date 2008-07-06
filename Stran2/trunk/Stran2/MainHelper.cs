using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	class MainHelper
	{
		private MainHelper()
		{
		}
		public static MainHelper Instance = new MainHelper();
		public void InitVillage(string username, int villageID)
		{
			PageQuerier.Instance.GetEx(TravianDataCenter.Instance.Users[username], villageID, "dorf1.php", null, true, false);
			PageQuerier.Instance.GetEx(TravianDataCenter.Instance.Users[username], villageID, "dorf2.php", null, true, false);
		}
	}
}
