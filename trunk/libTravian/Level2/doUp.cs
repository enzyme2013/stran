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
		private void doUp(int VillageID, int QueueID, TQueueType QueueType)
		{
			var CV = TD.Villages[VillageID];
			int GID;
			var Q = CV.Queue[QueueID];
			switch(QueueType)
			{
				case TQueueType.Research:
					if(!CV.Upgrades[Q.Bid].CanResearch)
					{
						if(CV.Queue.Contains(Q))
						{
							CV.Queue.Remove(Q);
							CV.SaveQueue(userdb);
							StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
						}
						return;
					}
					GID = 22;
					break;
				case TQueueType.UAttack:
					if(Q.TargetLevel != 0 && CV.Upgrades[Q.Bid].AttackLevel >= Q.TargetLevel || CV.Upgrades[Q.Bid].AttackLevel >= CV.BlacksmithLevel)
					{
						if(CV.Queue.Contains(Q))
						{
							CV.Queue.Remove(Q);
							CV.SaveQueue(userdb);
							StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
						}
						return;
					}
					GID = 12;
					break;
				case TQueueType.UDefense:
					if(Q.TargetLevel != 0 && CV.Upgrades[Q.Bid].DefenceLevel >= Q.TargetLevel || CV.Upgrades[Q.Bid].DefenceLevel >= CV.ArmouryLevel)
					{
						if(CV.Queue.Contains(Q))
						{
							CV.Queue.Remove(Q);
							CV.SaveQueue(userdb);
							StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
						}
						return;
					}
					GID = 13;
					break;
				default:
					return;
			}
			string result = PageQuery(VillageID, "build.php?gid=" + GID.ToString() + "&a=" + Q.Bid.ToString());

			if(CV.Queue.Contains(Q))
			{
				if(Q.TargetLevel == 0)
				{
					CV.Queue.Remove(Q);
					CV.SaveQueue(userdb);
					StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
				}
				else if(QueueType == TQueueType.UAttack)
				{
					if(CV.Upgrades[Q.Bid].AttackLevel >= Q.TargetLevel || CV.Upgrades[Q.Bid].AttackLevel >= CV.BlacksmithLevel)
					{
						CV.Queue.Remove(Q);
						CV.SaveQueue(userdb);
						StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
					}
					Q.Status = string.Format("{0}/{1}", CV.Upgrades[Q.Bid].AttackLevel, Q.TargetLevel);
				}
				else
				{
					if(CV.Upgrades[Q.Bid].DefenceLevel >= Q.TargetLevel || CV.Upgrades[Q.Bid].DefenceLevel >= CV.ArmouryLevel)
					{
						CV.Queue.Remove(Q);
						CV.SaveQueue(userdb);
						StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = VillageID, Param = QueueID });
					}
					Q.Status = string.Format("{0}/{1}", CV.Upgrades[Q.Bid].DefenceLevel, Q.TargetLevel);
				}
			}
			StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Research, VillageID = VillageID });
			//build.php?id=23&a=3
		}
	}
}
