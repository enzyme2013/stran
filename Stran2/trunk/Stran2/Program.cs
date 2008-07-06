using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Stran2.TCPInterface;

namespace Stran2
{
	class Program
	{
		static void Main(string[] args)
		{
			MainModule p = new MainModule();
			p.Run();
			Console.ReadKey();
		}
	}
	public partial class MainModule
	{
		TravianDataCenter TDC = TravianDataCenter.Instance;
		Thread maininboundthread;
		Thread mainoutboundthread;
		public void Run()
		{
			LoadPlugin();
			UserLoader.LoadUser(TDC, "Account");

			// background tcp server thread
			maininboundthread = new Thread(new ThreadStart(MainInBoundThread.Instance.ThreadEntry));
			maininboundthread.Start();

			mainoutboundthread = new Thread(new ThreadStart(MainOutBoundThread.Instance.ThreadEntry));
			mainoutboundthread.Start();

			while(true)
			{
				string x = Console.ReadLine();
				if(x == "exit")
				{
					MainInBoundThread.Instance.Terminate();
					maininboundthread.Abort();
					mainoutboundthread.Abort();
					
					break;
				}
			}
		}
	}
}
