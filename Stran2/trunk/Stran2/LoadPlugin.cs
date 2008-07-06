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
		MethodsCenter MC = MethodsCenter.Instance;
		
		public void LoadPlugin()
		{
			var dllnames = Directory.GetFiles(".", "plugin.*.dll");
			foreach(var dllname in dllnames)
			{
				try
				{
					Assembly a = Assembly.LoadFrom(dllname);
					Type[] types = a.GetTypes();
					foreach(var type in types)
						if(type.GetInterface("IPlugin") == typeof(IPlugin))
						{
							Console.WriteLine("Plugin class {0} found.", type.Name);
							IPlugin p = Activator.CreateInstance(type) as IPlugin;
							p.Initialize();
						}
						else if(type.GetInterface("ITaskOption") == typeof(ITaskOption))
						{
							Console.WriteLine("TaskOptioner class {0} found.", type.Name);
							ITaskOption to = Activator.CreateInstance(type) as ITaskOption;
						}
				}
				catch(Exception e)
				{
					Debugger.Instance.DebugLog(e);
				}
			}
		}
	}
}
