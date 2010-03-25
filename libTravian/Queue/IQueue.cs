using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	public interface IQueue
	{
		/// <summary>
		/// UpCall for callback
		/// </summary>
		Travian UpCall { get; set; }

		/// <summary>
		/// Village ID
		/// </summary>
		[Json]
		int VillageID { get; set; }

		/// <summary>
		/// If this queue should be delete
		/// </summary>
		bool MarkDeleted { get; }

		/// <summary>
		/// When set to true, the corresponding task will be suspended
		/// </summary>
		[Json]
		bool Paused { get; set; }

		/// <summary>
		/// Brief information of the task
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Detail information of the task
		/// </summary>
		string Status { get; }

		/// <summary>
		/// Seconds countdown to do the action
		/// </summary>
		int CountDown { get; }

		/// <summary>
		/// Do the action
		/// </summary>
		/// <returns></returns>
		void Action();
		
		/// <summary>
		/// 0 : AIQueue, BuildingQueue
		/// 1 : BuildingQueue
		/// 2 : DestroyQueue
		/// 10 : UpAttack
		/// 11 : UpDefence
		/// 12 : Research
		/// 13 : AutoFSQueue
		/// 14 : AlarmQueue
		/// 20 : PartyQueue
		/// 21 : BalancerQueue
		/// 22 : AutoBalanceQueue
		/// 30 : RefreshVillageQueue
		/// 31 : AutoRefreshVillageQueue
		/// 
		/// 50 : TransferQUeue
		/// 51 : NPCTradeQueue
		/// 52 : TradeQueue - selfqueue
		/// 53 : TradeQueue - AutoSell
		/// 54 : TradeQueue - AutoBuy
		/// 60 : AttackQueue
		/// 61 : RaidQueue
		/// 62 : ProduceQueue
		/// </summary>
		int QueueGUID { get; }
	}
}
