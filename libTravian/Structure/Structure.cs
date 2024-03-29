﻿/*
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
using System.IO;
using System.Text;
using System.Net;
using System.Collections;
using System.Reflection;
using LitJson;


namespace libTravian
{
	public class Data
	{
		[Json]
		public string Cookie { get; set; }
		[Json]
		public int UserID { get; set; }
		[Json]
		public int MarketSpeed { get; set; }
		[Json]
		public int ServerTimeOffset { get; set; }
		[Json]
		public int ActiveDid { get; set; }
		[Json]
		public Dictionary<int, TVillage> Villages { get; set; }

		public bool Dirty;

		public int Tribe { get; set; }
		public WebProxy Proxy { get; set; }
		public string Server { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool isRomans
		{
			get
			{
				return Tribe == 1;
			}
		}
		public int NewIGMCount
		{
			get
			{
				return IGMData.Count;
			}
		}
		public Dictionary<int, TIGM> IGMData { get; set; }
		//public Travian UpCall { get; set; }
		public string key
		{
			get
			{
				if(_key == null)
					_key = DB.Instance.GetKey(Username, Server);
				return _key;
			}
		}
		private string _key;

		public Data()
		{
			Villages = new Dictionary<int, TVillage>();
			ActiveDid = -1;
			IGMData = new Dictionary<int, TIGM>();
		}
	}

	public class TIGM
	{
		public int ID {get; set;}
		public bool NEW {get; set;}
		public string Title {get; set;}
		public string SentAccount {get; set;}
		public string Date {get; set;}
		public string Content {get;set;}
		public TIGM()
		{
			ID = 0;
			NEW = false;
		}
	}
	/*
	public class TStatus
	{
		public string Status { get; set; }
		public string Details { get; set; }
	}
	*/


	public static class TypeViewer
	{
		public static string ToString(object sender)
		{
			StringBuilder sb = new StringBuilder();
			Type t = sender.GetType();
			var p = t.GetProperties();//BindingFlags.Public);
			foreach (var x in p)
			{
				if (x.GetIndexParameters().Length == 0)
				{
					if (sb.Length != 0)
						sb.Append(", ");
					sb.Append(x.Name);
					sb.Append(":");
					sb.Append(x.GetValue(sender, null));
				}
			}
			return sb.ToString();
		}
		public static string Snapshot(object sender)
		{
			StringBuilder sb = new StringBuilder();
			Type t = sender.GetType();
			var p = t.GetProperties();//BindingFlags.Public);
			foreach (var x in p)
			{
				if (x.PropertyType == typeof(int) ||
					x.PropertyType == typeof(string) ||
					x.PropertyType == typeof(bool) ||
					x.PropertyType == typeof(DateTime)
					)
				{
					if (sb.Length != 0)
						sb.Append(Environment.NewLine);
					sb.Append(x.Name);
					sb.Append(":");
					sb.Append(x.GetValue(sender, null));
				}
			}
			return sb.ToString();
		}
	}
}
