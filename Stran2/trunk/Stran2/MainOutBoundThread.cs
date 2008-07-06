using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Stran2
{
	class MainOutBoundThread
	{
		private MainOutBoundThread()
		{
		}
		public static MainOutBoundThread Instance = new MainOutBoundThread();
		public void ThreadEntry()
		{
			try
			{
				while(true)
				{
					TaskCenter.Instance.Tick();
					Thread.Sleep(new TimeSpan(0, 0, 5));
				}
			}
			catch(ThreadAbortException e)
			{
				Debugger.Instance.DebugLog(e, DebugLevel.I);
			}
			catch(Exception e)
			{
				Debugger.Instance.DebugLog(e);
			}
		}
	}
}
