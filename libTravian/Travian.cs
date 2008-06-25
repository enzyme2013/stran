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
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Drawing;

namespace libTravian
{
	public partial class Travian: IPageQuerier
	{
		public event EventHandler<StatusChanged> StatusUpdate;

		public enum ChangedType
		{
			None,
			Villages,
			Buildings,
			Queue,
			Research,
			Stop,
			Market
		}
		public class StatusChanged : EventArgs
		{
			public ChangedType ChangedData { get; set; }
			public int VillageID { get; set; }
			public int Param { get; set; }
		}

		private WebClient wc;
		public LocalDB svrdb;
		public LocalDB userdb;
		public IPageQuerier pageQuerier;

		private bool NoMB = false;

		public Data TD { get; set; }
		public Travian(Data TravianData, Dictionary<string, string> Options)
		{
			GidLang = new Dictionary<int, string>(40);
			AidLang = new Dictionary<int, string>(30);
			TD = TravianData;
			svrdb = LocalDBCenter.getDB(TravianData.Server);
			userdb = LocalDBCenter.getDB(TravianData.Username, TravianData.Server);
			this.pageQuerier = this;

			// loading cached data into Language class
			for(int i = 1; i <= 40; i++)
			{
				if(svrdb.ContainsKey("gid" + i.ToString()))
					GidLang[i] = svrdb["gid" + i.ToString()];
			}
			for(int i = 1; i <= 30; i++)
			{
				if(svrdb.ContainsKey("aid" + i.ToString()))
					AidLang[i] = svrdb["aid" + i.ToString()];
			}
			int StdSpeed;
			if(TD.Tribe == 1)
				StdSpeed = 16;
			else if(TD.Tribe == 2)
				StdSpeed = 12;
			else
				StdSpeed = 24;
			int MarketSpeedX = 1;
			if(svrdb.ContainsKey("MarketSpeedX"))
				MarketSpeedX = Convert.ToInt32(svrdb["MarketSpeedX"]);
			TD.MarketSpeed = StdSpeed * MarketSpeedX;

			if(Options.ContainsKey("NoMB"))
			{
				//DebugLog("Get option: NoMB", DebugLevel.I);
				NoMB = Convert.ToBoolean(Options["NoMB"]);
			}
			if(Options.ContainsKey("resrate"))
			{
				//DebugLog("Get option: ResRate", DebugLevel.I);
				var r = Options["resrate"].Split(':');
				if(r.Length == 4)
				{
					resrate = new double[4];
					for(int i = 0; i < 4; i++)
						resrate[i] = Convert.ToDouble(r[i]);
				}
			}
			if(Options.ContainsKey("remotestop"))
				RemoteStopWord = Options["remotestop"];

			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			//Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		}

		static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		}
	}
}
