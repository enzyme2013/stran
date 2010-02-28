using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace PoproTracker
{
	public class Popro
	{
		Mongoose web_server;
		public PoproMod Mod = new PoproMod();
		public void StartServer()
		{
			web_server = new Mongoose();

			web_server.set_option("ports", "8000");
			web_server.set_uri_callback("/*", new MongooseCallback(Mod.MangooseProcess));

		}

		void _server_ExceptionThrown(object source, Exception exception)
		{
			Console.WriteLine(exception.Message);
		}

		public void EndServer()
		{
			Mod.Shutdown();
		}
	}
}
