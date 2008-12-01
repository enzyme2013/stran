using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	class ProduceTroopQueue : IQueue
	{
		#region IQueue 成员


		public Travian UpCall { get; set; }

		[Json]
		public int VillageID { get; set; }

		[Json]
		public bool Paused { get; set; }

		public bool MarkDeleted { get; private set; }

		public string Title
		{
			get { return "";/* PartyType.ToString().Substring(1); */}
		}

		public string Status
		{
			get
			{
				//if(LastExec == DateTime.MinValue)
				//	return LastExec.ToShortTimeString();
				//else
					return "";
			}
		}


		public int CountDown
		{
			get { throw new NotImplementedException(); }
		}

		public void Action()
		{
			throw new NotImplementedException();
		}

		public int QueueGUID { get { return 10; } }

		#endregion
	}
}
