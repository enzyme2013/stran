using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Stran2.TCPInterface
{
	class MainInBoundThread
	{
		private MainInBoundThread()
		{
		}
		public static MainInBoundThread Instance = new MainInBoundThread();
		private List<Thread> InnerThreadList = new List<Thread>();
		public void ThreadEntry()
		{
			try
			{
				Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				soc.Bind(new IPEndPoint(IPAddress.Any, 23));
				soc.Listen(2);
				while(true)
				{
					var s = soc.Accept();
					Thread t = new Thread(new ParameterizedThreadStart(SocketThread));
					t.Start(s);
					InnerThreadList.Add(t);
					foreach(var th in InnerThreadList)
						if(!th.IsAlive)
						{
							InnerThreadList.Remove(th);
							break;
						}
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
		public void SocketThread(object socket)
		{
			try
			{
				Socket soc = socket as Socket;
				byte[] buffer = new byte[255];
				int actualread = 0;
				while((actualread = soc.Receive(buffer)) > 0)
				{
					string command = Encoding.Default.GetString(buffer, 0, actualread);
					soc.Send(Encoding.Default.GetBytes(command));
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
		public void Terminate()
		{
			foreach(var th in InnerThreadList)
				if(th.IsAlive)
					th.Abort();
		}
	}
}
