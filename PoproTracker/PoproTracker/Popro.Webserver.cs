using System;
using System.Collections.Generic;
using System.Text;
using HttpServer;
using System.Net;
using System.IO;

namespace PoproTracker
{
	public class Popro
	{
		private HttpServer.HttpServer _server;
		public PoproMod Mod = new PoproMod();
		public void StartServer()
		{
			_server = new HttpServer.HttpServer();
			_server.Add(Mod);
			//_server.ExceptionThrown += new ExceptionHandler(_server_ExceptionThrown);
			try
			{
				_server.Start(IPAddress.Any, 8000);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		void _server_ExceptionThrown(object source, Exception exception)
		{
			Console.WriteLine(exception.Message);
		}
		private void OnRequest(object source, RequestEventArgs args)
		{
			IHttpClientContext context = (IHttpClientContext)source;
			IHttpRequest request = args.Request;
			
		}

		public void EndServer()
		{
			Mod.Shutdown();
			_server.Stop();
		}
	}
}
