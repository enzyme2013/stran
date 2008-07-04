using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	public class Plugin_Building : IPlugin
	{
		#region IPlugin 成员

		public void Initialize(MethodsCenter MC)
		{
			MC.RegisterParser(new ParserPluginCall(Dorf1));
			MC.RegisterParser(new ParserPluginCall(Dorf2));
			MC.RegisterMethod(GetType().GetMethod("GetBidLevel"));
		}

		#endregion

		public void Dorf1(string pageData, UserData users, int villageId)
		{
		}

		public void Dorf2(string pageData, UserData users, int villageId)
		{
		}

		public int GetBidLevel(int Bid)
		{
			return 0;
		}

	}
}
