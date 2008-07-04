using System;
using System.Collections.Generic;
using System.Text;

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
		public void Run()
		{
			LoadPlugin();
			UserLoader.LoadUser(TDC, "Account");
			//while(true)
			//{

			//}
		}
	}
}
