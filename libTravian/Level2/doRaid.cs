using System;
using System.Collections.Generic;
using System.Text;

namespace libTravian
{
	partial class Travian
	{
		public void doRaid(int VillageID, TQueue Q)
		{
			TVillage CV = this.TD.Villages[VillageID];
			var opt = Q.NewOptions;
			if(!(opt is RaidOption))
			{
				DebugLog("Raid task w/o raid option. Task deleted.", DebugLevel.W);
				RemoveQueuedTask(VillageID, Q);
				return;
			}
			RaidOption Option = opt as RaidOption;

            // check enough troops

            // fetch and refresh data

            // post data to server, and refresh new data

			/*

			if(!Option.IsValid)
			{
				DebugLog("Invalid NPC trade task discarded: " + Option.ToString(), DebugLevel.W);
				this.RemoveQueuedTask(VillageID, Q);
				return;
			}

			if(Option.GetDelay(this.TD, VillageID) > 0)
			{
				return;
			}

			NpcTradeResult returnCode = doNpcTrade(VillageID, Option);
			switch(returnCode)
			{
				case NpcTradeResult.Failure:
					DebugLog("NPC trade task has failed: " + Option.ToString(), DebugLevel.W);
					this.RemoveQueuedTask(VillageID, Q);
					break;

				case NpcTradeResult.Delay:
					// Wait at least 10 minutes before retrying
					Option.MinimumDelay = 600;
					Q.ExtraOptions = Option.ToString();
					Q.Status = Option.Status;
					CV.SaveQueue(userdb);
					break;

				case NpcTradeResult.Success:
					this.RemoveQueuedTask(VillageID, Q);
					break;
			}
			 * */
		}

	}
}
