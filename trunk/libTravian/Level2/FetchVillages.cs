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
using System.Threading;

namespace libTravian
{
	partial class Travian
	{
		private object Level2Lock = new object();
		private void doFetchVillages()
		{
			lock(Level2Lock)
			{
				if(TD.Tribe == 0)
					TD.Tribe = NewParseTribe();
				if(TD.Tribe != 0)
					PageQuery(0, "dorf1.php");
				StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages, VillageID = TD.ActiveDid });
			}
		}

		private void doFetchVBuilding(object o)
		{
			lock(Level2Lock)
			{
				int VillageID = (int)o;
				TD.Villages[VillageID].isBuildingInitialized = 1;
				TD.Villages[VillageID].Buildings = new SortedDictionary<int, TBuilding>();
				PageQuery(VillageID, "dorf1.php");
				PageQuery(VillageID, "dorf2.php");
				PageQuery(VillageID, "build.php?gid=17");
				TD.Villages[VillageID].RestoreResourceLimits(userdb);
				TD.Villages[VillageID].isBuildingInitialized = 2;
				StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Buildings, VillageID = VillageID });
				string key = "v" + VillageID.ToString() + "Queue";
				if(TD.Villages[VillageID].Queue.Count == 0 && userdb.ContainsKey(key) && userdb[key] != "")
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = -1 });
			}
		}

		private void doFetchVUpgrade(object o)
		{
			lock(Level2Lock)
			{
				int VillageID = (int)o;
				TD.Villages[VillageID].isUpgradeInitialized = 1;
				PageQuery(VillageID, "build.php?gid=12");
				PageQuery(VillageID, "build.php?gid=13");
				PageQuery(VillageID, "build.php?gid=22");
				TD.Villages[VillageID].isUpgradeInitialized = 2;
				StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Research, VillageID = VillageID });
			}
		}
		private void doFetchVDestroy(object o)
		{
			lock(Level2Lock)
			{
				int VillageID = (int)o;
				TD.Villages[VillageID].isDestroyInitialized = 1;
				PageQuery(VillageID, "build.php?gid=15");
				TD.Villages[VillageID].isDestroyInitialized = 2;
			}
		}
	}
}
