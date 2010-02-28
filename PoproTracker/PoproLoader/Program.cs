using System;
using System.Collections.Generic;
using System.Text;
using PoproTracker;

namespace PoproLoader
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Popro pop = new Popro();
			pop.StartServer();
			Console.WriteLine("Stop?");
			Console.ReadLine();
			pop.EndServer();
			Console.WriteLine("Stopped");
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception) e.ExceptionObject;
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
			Console.WriteLine("Terminating? {0}", e.IsTerminating);
		}
	}
}
