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
			Popro pop = new Popro();
			pop.StartServer();
			Console.WriteLine("Stop?");
			Console.ReadLine();
			pop.EndServer();
			Console.WriteLine("Stopped");
		}
	}
}
