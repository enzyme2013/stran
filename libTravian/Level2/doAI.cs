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
		private int AIDelay(int VillageID, TQueue Q)
		{
			var CV = TD.Villages[VillageID];
			var Cinb = CV.InBuilding;
			int delay = 0;
			if(Cinb[0] != null)
				delay = Math.Max(delay, Convert.ToInt32(Cinb[0].FinishTime.Subtract(DateTime.Now).TotalSeconds));
			//if(Cinb[1] != null)
			//	delay = Math.Max(delay, Convert.ToInt32(Cinb[1].FinishTime.Subtract(DateTime.Now).TotalSeconds));
			if(Q.NextExec != DateTime.MinValue)
				delay = Math.Max(delay, Convert.ToInt32(Q.NextExec.Subtract(DateTime.Now).TotalSeconds));
			return delay;
		}
		static double[] resrate = new double[4] { 10, 10, 9, 7 };
		private void doAI(int VillageID, int QueueID)
		{
			var CV = TD.Villages[VillageID];
			var Q = CV.Queue[QueueID];
			var Param = Q.Gid;

			if(Q.NextExec >= DateTime.Now)
				return;
			Q.NextExec = DateTime.Now.AddSeconds(50);

			int bid = -1, gid = 0;
			double extrarate = CV.Resource[0].Capacity / CV.Resource[3].Capacity;
			if(Param == 0)
			{
				// by current warehouse amount

				int i;
				if(CV.isBuildingInitialized != 2)
					return;
				//int[] prior = currv.res.CurrAmount;
				int min = 1;
				for(i = 0; i < 4; i++)
					if(CV.Resource[min].CurrAmount / resrate[min] > CV.Resource[i].CurrAmount * (i == 3 ? extrarate : 1) / resrate[i])
						min = i;
				for(i = 1; i <= 18; i++)
					if(CV.Buildings[i].Gid == min + 1)
						if(bid == -1)
							bid = i;
						else if(CV.Buildings[i].Level < CV.Buildings[bid].Level)
							bid = i;
				gid = min + 1;
			}
			else
			{
				// by resource field level
				int i;
				int minlevel = 10;
				int[] buildpriority = new int[5];
				for(i = 1; i <= 18; i++)
				{
					if(!CV.Buildings.ContainsKey(i))
						continue;
					var tlevel = CV.Buildings[i].Gid != 4 ? CV.Buildings[i].Level : CV.Buildings[i].Level + 1;
					if(tlevel < minlevel)
						minlevel = tlevel;
				}
				int min = 1;
				for(i = 1; i <= 18; i++)
				{
					if(!CV.Buildings.ContainsKey(i))
						continue;
					var tlevel = CV.Buildings[i].Gid != 4 ? CV.Buildings[i].Level : CV.Buildings[i].Level + 1;
					if(tlevel == minlevel && buildpriority[CV.Buildings[i].Gid] == 0)
					{
						min = CV.Buildings[i].Gid - 1;
						buildpriority[CV.Buildings[i].Gid] = i;
					}
				}
				for(i = 0; i < 4; i++)
					if(CV.Resource[min].CurrAmount / resrate[min] > CV.Resource[i].CurrAmount / resrate[i] && buildpriority[i + 1] != 0)
						min = i;
				gid = min + 1;
				bid = buildpriority[gid];

				if(minlevel == 10 && CV.Queue.Contains(Q))
				{
					CV.Queue.Remove(Q);
					CV.SaveQueue(userdb);
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
					return;
				}
			}
			// balance on warehouse and resource field:
			int[] rate2;
			if(TD.Villages.Count > 20)
				rate2 = new int[3] { 7, 8, 1000 };
			else if(TD.Villages.Count > 5)
				rate2 = new int[3] { 3, 4, 3000 };
			else
				rate2 = new int[3] { 2, 3, 3000 };

			// check warehouse/granary
			int tgid, tbid;

			// romans double-way build
			var Cinb = CV.InBuilding;
			if(Cinb[1] == null || Convert.ToInt32(Cinb[1].FinishTime.Subtract(DateTime.Now).TotalSeconds) <= 0)
			{
				if(CV.Resource[0].Capacity < Buildings.Cost(gid, CV.Buildings[bid].Level + 1).Resources[0] * 3 ||
					CV.Resource[1].Capacity < Buildings.Cost(gid, CV.Buildings[bid].Level + 1).Resources[1] * 3 ||
					CV.Resource[2].Capacity < Buildings.Cost(gid, CV.Buildings[bid].Level + 1).Resources[2] * 3
					)
				{
					tgid = 10;
					tbid = findDorf2Building(CV.Buildings, tgid);
					if(tbid != -1)
					{
						gid = tgid;
						bid = tbid;
					}
				}
				else if(CV.Resource[3].Capacity < Buildings.Cost(gid, CV.Buildings[bid].Level + 1).Resources[3] * 4)
				{
					tgid = 11;
					tbid = findDorf2Building(CV.Buildings, tgid);
					if(tbid != -1)
					{
						gid = tgid;
						bid = tbid;
					}
				}
				else if(!NoMB) // check main building
				{
					tgid = 15;
					tbid = findDorf2Building(CV.Buildings, tgid);
					if(tbid != -1 && CV.Buildings[tbid].Level < 20 && CV.Buildings[tbid].Level * 1000 < CV.Resource[0].Capacity)
					{
						gid = tgid;
						bid = tbid;
					}
					// if nothing match, build the resource field
				}
			}
			var BQ = new TQueue()
			{
				Bid = bid,
				Gid = gid,
				QueueType = TQueueType.Building
			};
			Q.Status = GetGidLang(gid);
			int timecost;
			if(CV.Buildings.ContainsKey(bid))
				timecost = CV.TimeCost(Buildings.Cost(gid, CV.Buildings[bid].Level + 1));
			else
				timecost = CV.TimeCost(Buildings.Cost(gid, 1));
			if(timecost > 0)
				Q.NextExec = DateTime.Now.AddSeconds(Math.Min(timecost, rand.Next(500, 1000)));
			else
				doBuild(VillageID, BQ);
		}
		public int findDorf2Building(SortedDictionary<int, TBuilding> b, int gid)
		{
			int k;
			int tlvl = 20, tid = 0;
			for(k = 19; k <= 40; k++)
				if(b.ContainsKey(k) && b[k].Gid == gid)
					if(tlvl > b[k].Level)
					{
						tlvl = b[k].Level;
						tid = k;
					}
			if(tid != 0)
				return tid;
			for(k = 0; k < Buildings.PreferPos[gid].Length; k++)
				if(!b.ContainsKey(Buildings.PreferPos[gid][k]))
					return Buildings.PreferPos[gid][k];
			//for(k = 19; k < b.Length; k++)

			return -1;
		}

	}
}
