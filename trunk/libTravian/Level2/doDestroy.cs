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
using System.Net;

namespace libTravian
{
	partial class Travian
	{
		private void doDestroy(int VillageID, int QueueID)
		{
			//gid=15&a=185668&abriss=35
			var CV = TD.Villages[VillageID];
			var Q = CV.Queue[QueueID];
			if(Q.NextExec >= DateTime.Now)
				return;
			Q.NextExec = DateTime.Now.AddSeconds(50);
			Dictionary<string, string> Postdata = new Dictionary<string, string>();
			Postdata["gid"] = "15";
			Postdata["a"] = VillageID.ToString();
			Postdata["abriss"] = Q.Bid.ToString();
			Postdata["ok"] = "%E6%8B%86%E6%AF%81";
			//PageQuery(VillageID, "dorf1.php", TPageType.Dorf1);
			PageQuery(VillageID, "build.php", Postdata);
			/*
			wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
			string result = wc.UploadString("build.php",
				string.Format("gid=15&a={0}&abriss={1}&ok=%E6%8B%86%E6%AF%81",
					VillageID, Q.Bid));
			*/
			int lvl = CV.InBuilding[2] != null && CV.InBuilding[2].FinishTime > DateTime.Now ? CV.InBuilding[2].Level : -1;
			if(lvl < 0)
				DebugLog("Unknown state: Destroy to -1", DebugLevel.W);
			if(lvl <= 0)
			{
				CV.Buildings.Remove(Q.Bid);
				CV.Queue.Remove(Q);
				CV.SaveQueue(userdb);
				StatusUpdate(this, new StatusChanged()
				{
					ChangedData = ChangedType.Queue,
					VillageID = VillageID,
					Param = QueueID
				});
			}
			else
				Q.Status = lvl.ToString();
			BuildCount();
		}
	}
}
