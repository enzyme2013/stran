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

namespace libTravian
{
	partial class Travian
	{
		private void doParty(int VillageID, int QueueID)
		{
			var CV = TD.Villages[VillageID];
			TQueue Q = CV.Queue[QueueID];
			if(Q.NextExec >= DateTime.Now)
				return;
			Q.NextExec = DateTime.Now.AddSeconds(60);
			PageQuery(VillageID, "build.php?gid=24&a=" + Q.Bid.ToString());
			if(CV.InBuilding[6] == null || CV.InBuilding[6].FinishTime < DateTime.Now)
			{
				// error occurred!
				DebugLog("Error on party! Delete the queue!", DebugLevel.W);
				if(CV.Queue.Contains(Q))
				{
					CV.Queue.Remove(Q);
					CV.SaveQueue(userdb);
				}
			}
			else
				Q.Status = DateTime.Now.ToShortTimeString();
		}
	}
}
