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
		public void Tick()
		{
			try
			{
				foreach (var vid in TD.Villages.Keys)
				//for(int i = 0; i < TD.Villages.Count; i++)
				{
					var CV = TD.Villages[vid];
					try
					{
						CV.Market.tick(ref CV, TD.MarketSpeed);
					}
					catch (Exception ex)
					{
						DebugLog(ex, DebugLevel.W);
					}
				}
			}
			catch (InvalidOperationException e)
			{
				// Good bye! Collection was modified; enumeration operation may not execute.
				DebugLog(e, DebugLevel.I);
			}
			if (NextExec > DateTime.Now)
				return;
			Thread t = new Thread(new ThreadStart(doTick));
			t.Start();
		}

		public void doTick()
		{
			if (!Monitor.TryEnter(Level2Lock))
				return;
			//foreach(var CV in TD.Villages)
			try
			{
				foreach (var vid in TD.Villages.Keys)
				//for(int iii = 0; iii < TD.Villages.Count; iii++)
				{
					var CV = TD.Villages[vid];
					var CVQ = CV.Queue;
					List<int> status = new List<int>();
					for (int i = 0; i < CVQ.Count; i++)
						switch (CVQ[i].QueueType)
						{
							case TQueueType.Building:
								if (CV.isBuildingInitialized == 0)
								{
									CV.InitializeBuilding();
									continue;
								}
								else if (CV.isBuildingInitialized == 1)
									continue;
								if (!status.Contains(TD.isRomans ? CVQ[i].Type : 0))
								{
									status.Add(TD.isRomans ? CVQ[i].Type : 0);

									if (CVQ[i].Bid == TQueue.AIBID && AIDelay(CV.ID, CVQ[i]) <= 0)
									{
										doAI(CV.ID, i);
										break;
									}
									if (GetDelay(CV.ID, CVQ[i]) <= 0)
									{
										doBuild(CV.ID, i);
										break;
									}
								}
								break;
							case TQueueType.Destroy:
								if (CV.isDestroyInitialized == 0)
								{
									CV.InitializeDestroy();
									continue;
								}
								else if (CV.isDestroyInitialized == 1)
									continue;
								if (!status.Contains(2))
								{
									status.Add(2);
									if (GetDelay(CV.ID, CVQ[i]) <= 0)
									{
										doDestroy(CV.ID, i);
										break;
									}
								}
								break;
							case TQueueType.UAttack:
							case TQueueType.UDefense:
							case TQueueType.Research:
								if (CV.isUpgradeInitialized == 0)
								{
									CV.InitializeUpgrade();
									continue;
								}
								else if (CV.isUpgradeInitialized == 1)
									continue;
								if (!status.Contains(CVQ[i].Type))
								{
									status.Add(CVQ[i].Type);
									if (GetDelay(CV.ID, CVQ[i]) <= 0)
									{
										doUp(CV.ID, i, CVQ[i].QueueType);
										break;
									}
								}
								break;
							case TQueueType.Party:
								if (!status.Contains(CVQ[i].Type))
								{
									status.Add(CVQ[i].Type);
									if (GetDelay(CV.ID, CVQ[i]) <= 0)
									{
										doParty(CV.ID, i);
										break;
									}
								}
								break;
							case TQueueType.Transfer:
								if (GetDelay(CV.ID, CVQ[i]) <= 0)
								{
									doTransfer(CV.ID, CVQ[i]);
								}
								break;
							case TQueueType.NpcTrade:
								if (GetDelay(CV.ID, CVQ[i]) <= 0)
								{
									doNpcTrade(CV.ID, CVQ[i]);
								}
								break;
						}
				}
			}
			catch (InvalidOperationException e)
			{
				// Good bye! Collection was modified; enumeration operation may not execute.
				DebugLog(e, DebugLevel.I);
			}
			Monitor.Exit(Level2Lock);
		}

		/// <summary>
		/// Returns the minmum delay before a queued task can start running
		/// </summary>
		/// <param name="VillageID">which village the queued task belongs to</param>
		/// <param name="Q">A queued task</param>
		/// <returns>Delay in seconds</returns>
		public int GetDelay(int VillageID, TQueue Q)
		{
			var CV = TD.Villages[VillageID];
			int timecost;
			switch (Q.Type)
			{
				case 0:
				case 1:
					if (Q.Bid == TQueue.AIBID)
						return AIDelay(VillageID, Q);
					if (CV.Buildings.ContainsKey(Q.Bid))
						timecost = CV.TimeCost(Buildings.Cost(CV.Buildings[Q.Bid].Gid, CV.Buildings[Q.Bid].Level + 1));
					else
						timecost = CV.TimeCost(Buildings.Cost(Q.Gid, 1));
					var x = CV.InBuilding[TD.isRomans ? Q.Type : 0];
					if (timecost != 0)
						return timecost;
					else if (x == null || x.FinishTime.AddSeconds(15) < DateTime.Now)
						return 0;
					else
						return Convert.ToInt32(x.FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5;
				//break;
				case 2:
					if (CV.InBuilding[Q.Type] != null)
						return Convert.ToInt32(CV.InBuilding[Q.Type].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5;
					else
						return 0;
				case 3:
					if (CV.isUpgradeInitialized == 0)
						doFetchVUpgrade(VillageID);
					if (CV.isUpgradeInitialized != 2)
						break;
					timecost = CV.TimeCost(Buildings.UpCost[(TD.Tribe - 1) * 10 + Q.Bid][CV.Upgrades[Q.Bid].AttackLevel]);
					if (CV.InBuilding[Q.Type] != null)
						return Math.Max(timecost, Convert.ToInt32(CV.InBuilding[Q.Type].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5);
					else
						return timecost;
				case 4:
					if (CV.isUpgradeInitialized == 0)
						doFetchVUpgrade(VillageID);
					if (CV.isUpgradeInitialized != 2)
						break;
					timecost = CV.TimeCost(Buildings.UpCost[(TD.Tribe - 1) * 10 + Q.Bid][CV.Upgrades[Q.Bid].DefenceLevel]);
					if (CV.InBuilding[Q.Type] != null)
						return Math.Max(timecost, Convert.ToInt32(CV.InBuilding[Q.Type].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5);
					else
						return timecost;
				case 5:
					if (CV.isUpgradeInitialized == 0)
						doFetchVUpgrade(VillageID);
					if (CV.isUpgradeInitialized != 2)
						break;
					timecost = CV.TimeCost(Buildings.ResearchCost[(TD.Tribe - 1) * 10 + Q.Bid]);
					if (CV.InBuilding[Q.Type] != null)
						return Math.Max(timecost, Convert.ToInt32(CV.InBuilding[Q.Type].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5);
					else
						return timecost;
				case 6:
					timecost = CV.TimeCost(Buildings.PartyCos[Q.Bid - 1]);
					if (CV.InBuilding[Q.Type] != null)
						return Math.Max(timecost, Convert.ToInt32(CV.InBuilding[Q.Type].FinishTime.Subtract(DateTime.Now).TotalSeconds) + 5);
					else
						return timecost;

				case (int)TQueueType.Transfer:
					TransferOption transferOption = TransferOption.FromString(Q.ExtraOptions);
					transferOption.CalculateResourceAmount(TD, VillageID);
					return transferOption.GetDelay(this.TD, VillageID);

				case (int) TQueueType.NpcTrade:
					NpcTradeOption npcTradeOption = NpcTradeOption.FromString(Q.ExtraOptions);
					return npcTradeOption.GetDelay(this.TD, VillageID);

				default:
					return 1;
			}
			return 1;
		}
	}
}
