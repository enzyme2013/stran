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
		public void StartServer()
		{
			_server = new HttpServer.HttpServer();
			_server.Add(new PoproMod());
			_server.Start(IPAddress.Any, 8081);
		}
		private void OnRequest(object source, RequestEventArgs args)
		{
			IHttpClientContext context = (IHttpClientContext)source;
			IHttpRequest request = args.Request;
			
		}

		public void EndServer()
		{
			_server.Stop();
		}
	}
}
