/*
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * The Initial Developer of the Original Code is [MeteorRain <msg7086@gmail.com>].
 * Copyright (C) MeteorRain 2007, 2008. All Rights Reserved.
 * Contributor(s): [MeteorRain].
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace libTravian
{
	// for saving server related data into cache database
	static public class LocalDBCenter
	{
		static Dictionary<string, LocalDB> m_Cache = new Dictionary<string, LocalDB>();
		static public LocalDB getDB(string Server)
		{
			if(m_Cache.ContainsKey(Server))
				return m_Cache[Server];
			string dbPath = "db";
			if(File.Exists(dbPath))
				File.Delete(dbPath);
			if(!Directory.Exists(dbPath))
				Directory.CreateDirectory(dbPath);
			string Filename = dbPath + Path.DirectorySeparatorChar + Server.Replace(Path.DirectorySeparatorChar, '-');
			if(File.Exists(Filename))
			{
				try
				{
					Stream s = File.Open(Filename, FileMode.Open);
					BinaryFormatter b = new BinaryFormatter();
					m_Cache[Server] = (LocalDB)b.Deserialize(s);
					s.Close();
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			if(!m_Cache.ContainsKey(Server))
				m_Cache[Server] = new LocalDB();
			m_Cache[Server].Server = Server;
			return m_Cache[Server];
		}
		static public LocalDB getDB(string Username, string Server)
		{
			var db = getDB(Username + "@" + Server);
			db.Username = Username;
			db.Server = Server;
			return db;
		}
	}
	[Serializable]
	public class LocalDB : Dictionary<string, string>
	{
		public LocalDB()
			: base()
		{
		}
		public LocalDB(SerializationInfo si, StreamingContext context)
			: base(si, context)
		{
		}
		[NonSerialized]
		public string Server;
		[NonSerialized]
		public string Username;
		public new string this[string key]
		{
			get
			{
				return base[key];
			}
			set
			{
				if(ContainsKey(key) && base[key] == value)
					return;
				base[key] = value;
				string dbPath = "db";
				if(File.Exists(dbPath))
					File.Delete(dbPath);
				if(!Directory.Exists(dbPath))
					Directory.CreateDirectory(dbPath);
				string Filename;
				if(Username == null)
					Filename = dbPath + Path.DirectorySeparatorChar + Server.Replace(Path.DirectorySeparatorChar, '-');
				else
					Filename = dbPath + Path.DirectorySeparatorChar + Username + "@" + Server.Replace(Path.DirectorySeparatorChar, '-');
				try
				{
					Stream s = File.Open(Filename, FileMode.Create);
					BinaryFormatter b = new BinaryFormatter();
					b.Serialize(s, this);
					s.Close();
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}
