using System;
using System.Collections.Generic;
using System.Text;
using HttpServer;
using HttpServer.HttpModules;
using System.Net;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Timers;

namespace PoproTracker
{
	public class BEException : Exception
	{
		public BEException(string message)
			: base(message)
		{
		}
		public override string ToString()
		{
			BEDict dict = new Dictionary<BEString, IBE> { { "failure reason", (BEString)Message } };
			return dict.Dump();
		}
	}
	public class PoproMod : HttpModule
	{
		bool AllowUnregisteredTorrent = false;
		Dictionary<string, Dictionary<string, PeerInfo>> PeerList = new Dictionary<string, Dictionary<string, PeerInfo>>();
		Dictionary<string, TFileInfo> RegisteredTorrents = new Dictionary<string, TFileInfo>();
		MySqlConnection DB;
		Timer Tick;

		public PoproMod()
		{
			DB = new MySqlConnection("server=ftp.xdmhy.net;uid=root;pwd=1;database=bt;charset=utf8;");
			DB.Open();
			Tick = new Timer(20000);
			Tick.Elapsed += new ElapsedEventHandler((sender, e) => LoadRegisteredTorrent());
			Tick.Enabled = true;
			LoadRegisteredTorrent();
		}

		public void LoadRegisteredTorrent()
		{
			lock (RegisteredTorrents)
			{
				// save to db
				if (RegisteredTorrents.Count > 0)
				{
					var scmd = DB.CreateCommand();
					scmd.CommandText = "UPDATE fileinfo SET completed = completed + @c WHERE fid = @f";
					scmd.Prepare();
					scmd.Parameters.Add(new MySqlParameter("@c", MySqlDbType.Int32));
					scmd.Parameters.Add(new MySqlParameter("@f", MySqlDbType.Int32));
					foreach (var fi in RegisteredTorrents)
					{
						if (fi.Value.fid > 0 && fi.Value.newcompleted > 0)
						{
							scmd.Parameters[0].Value = fi.Value.newcompleted;
							scmd.Parameters[1].Value = fi.Value.fid;
							scmd.ExecuteNonQuery();
							fi.Value.newcompleted = 0;
						}
					}
				}

				var cmd = DB.CreateCommand();
				cmd.CommandText = "SELECT fid, hash FROM fileinfo";
				var reader = cmd.ExecuteReader();
				RegisteredTorrents.Clear();
				while (reader.Read())
				{
					var tfi = new TFileInfo
					{
						fid = reader.GetInt32("fid"),
						hash = reader.GetString("hash")
					};
					RegisteredTorrents[tfi.hash] = tfi;
				}
				reader.Close();
			}
		}

		BEDict TrackerRouter(string Action, HttpInput Get, string IP, string info_hash)
		{
			if (Action == "announce")
			{
				var Passkey = Get["passkey"].Value;
				if (info_hash == null)
					throw new BEException("No info_hash provided.");
				info_hash = BitConverter.ToString(HttpUtility.UrlDecodeToBytes(Encoding.ASCII.GetBytes(info_hash))).Replace("-", "").ToLower();
				if(info_hash.Length != 40)
					throw new BEException("Invalid info_hash length.");
				var peer_id = Get["peer_id"].Value;
				if (peer_id == null)
					throw new BEException("No peer_id provided.");
				if (!RegisteredTorrents.ContainsKey(info_hash))
				{
					if (AllowUnregisteredTorrent)
						throw new BEException("Unregistered torrent.");
					else
						RegisteredTorrents[info_hash] = new TFileInfo { fid = 0, hash = info_hash };
				}
				var _xfi = RegisteredTorrents[info_hash];
				lock (PeerList)
				{
					if (!PeerList.ContainsKey(info_hash))
						PeerList[info_hash] = new Dictionary<string, PeerInfo>();
				}

				var ThisPeerList = PeerList[info_hash];
				var xevent = Get["event"].Value;
				var peers = new List<IBE>();
				int numwant, Port;
				if (!int.TryParse(Get["numwant"].Value, out numwant))
					numwant = 50;
				if (!int.TryParse(Get["port"].Value, out Port))
					Port = 0;
				var nopeerid = Get["no_peer_id"].Value == "1";
				long left;
				if (!long.TryParse(Get["left"].Value, out left))
					left = 0;
				lock (ThisPeerList)
				{
					if (xevent == "stopped")
					{
						if (ThisPeerList.ContainsKey(peer_id))
						{
							if (ThisPeerList[peer_id].Left == 0)
								_xfi.Seeding--;
							else
								_xfi.Leeching--;
							if (ThisPeerList[peer_id].Left != 0 && left == 0)
								_xfi.newcompleted++;
							ThisPeerList.Remove(peer_id);
						}
					}
					else
					{
						if (ThisPeerList.ContainsKey(peer_id))
						{
							ThisPeerList[peer_id].IP = IP;
							ThisPeerList[peer_id].Port = Port;
							ThisPeerList[peer_id].Left = left;
						}
						else
						{
							ThisPeerList[peer_id] = new PeerInfo
							{
								PeerId = peer_id,
								Left = left,
								IP = IP,
								Port = Port
							};
						}
						if (xevent == "completed")
							_xfi.newcompleted++;
						if (numwant > 0)
						{
							IEnumerable<KeyValuePair<String, PeerInfo>> rawpeers;
							if (xevent == "completed" || left == 0)
								rawpeers = ThisPeerList.OrderBy(x => x.Value.Left).Reverse().Take(numwant);
							else
								rawpeers = ThisPeerList.Where(x => x.Value.Left != 0).OrderBy(x => x.Value.Left).Reverse().Take(numwant);
							foreach (var rawpeer in rawpeers)
							{
								Dictionary<BEString, IBE> Peer = new Dictionary<BEString, IBE>();
								if (nopeerid)
									Peer["peer id"] = (BEString)rawpeer.Key;
								Peer["ip"] = (BEString)rawpeer.Value.IP;
								Peer["port"] = (BENumber)rawpeer.Value.Port;
								peers.Add((BEDict)Peer);
							}
						}
					}
				}


				var response = new Dictionary<BEString, IBE> {
					{"complete", (BENumber)_xfi.Seeding},
					{"incomplete", (BENumber)_xfi.Leeching},
					{"interval", (BENumber)600},
					{"min interval", (BENumber)10},
					{"peers", (BEList)peers}
				};

				Console.WriteLine("{0} peers returned.", peers.Count);
				return response;

				//if(PeerList.ContainsKey(
				throw new BEException("Not implemented.");
			}
			throw new Exception();
		}



		public override bool Process(IHttpRequest request, IHttpResponse response, HttpServer.Sessions.IHttpSession session)
		{
			string info_hash = null;
			var m = Regex.Match(request.UriPath, "info_hash=([^&]+)&");
			if (m.Success)
				info_hash = m.Groups[1].Value;
			//request.
			try
			{
				var Action = request.UriParts[0];
				var IP = request.RemoteEndPoint.Address.ToString();
				var Result = TrackerRouter(Action, request.QueryString, IP, info_hash);
				response.Body = new MemoryStream(Encoding.UTF8.GetBytes(Result.Dump()));
			}
			catch (Exception)
			{
				response.Status = HttpStatusCode.NotFound;
			}
			response.Send();
			return true;
		}
	}
}

/*
QueryString[info_hash] = 
QueryString[peer_id] = -UT1850
QueryString[port] = 64848
QueryString[uploaded] = 0
QueryString[downloaded] = 0
QueryString[left] = 0
QueryString[corrupt] = 0
QueryString[key] = 13A4CAE0
QueryString[event] = started
QueryString[numwant] = 200
QueryString[compact] = 1
QueryString[no_peer_id] = 1
QueryString[ipv6] = 2001:0:cf2
*/