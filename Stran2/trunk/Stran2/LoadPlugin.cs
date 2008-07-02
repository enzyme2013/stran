using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Stran2
{
	partial class MainModule
	{
		//Dictionary<
		MethodsCenter MC = new MethodsCenter();
		
		public void LoadPlugin()
		{
			var dllnames = Directory.GetFiles(".", "plugin.*.dll");
			foreach(var dllname in dllnames)
			{
				Assembly a = Assembly.LoadFrom(dllname);
				Type[] types = a.GetTypes();
				foreach(var type in types)
					if(type.GetInterface("IPlugin") == typeof(IPlugin))
					{
						IPlugin p = Activator.CreateInstance(type) as IPlugin;
						p.Initialize(MC);
						
						Console.WriteLine(type.Name);
					}
			}
		}
	}
	public class CrossPluginCall
	{
		public Object obj;
		public Type type;
	}
}
