using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Stran2
{
	public delegate void ParserPluginCall(string pageData, UserData users, int villageId);
	public delegate void ActionPluginCall(ITask task, UserData users, int villageId);
	public interface IPlugin
	{
		void Initialize();
		bool CheckDepend();
	}
}
