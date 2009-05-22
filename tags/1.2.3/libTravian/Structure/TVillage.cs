using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.IO;
using System.Reflection;

namespace libTravian
{
	public class TVillage
	{
		[Json]
		public int ID { get; set; }
		[Json]
		public int X { get; set; }
		[Json]
		public int Y { get; set; }
		[Json]
		public string Name { get; set; }
		[Json]
		public bool isCapital { get; set; }
		[Json]
		public SortedDictionary<int, TBuilding> Buildings { get; set; }
		[Json]
		public Dictionary<int, TRU> Upgrades { get; set; }
		[Json]
		public int BlacksmithLevel { get; set; }
		[Json]
		public int ArmouryLevel { get; set; }
		[Json]
		public TResource[] Resource { get; set; }
		[Json]
		public TInBuilding[] InBuilding { get; set; }
		[Json]
		public List<IQueue> Queue { get; set; }
		[Json]
		public int isBuildingInitialized { get; set; }
		[Json]
		public int isUpgradeInitialized { get; set; }
		[Json]
		public int isDestroyInitialized { get; set; }
		[Json]
		public int isTroopInitialized { get; set; }
		public Travian UpCall { get; set; }
		public TPoint Coord
		{
			get
			{
				return new TPoint(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}
		public int Z
		{
			get
			{
				return 801 * (400 - Y) + (X + 401);
			}
			set
			{
				X = value % 801 - 401;
				Y = 400 - value / 801;
			}
		}

		public TResAmount ResourceCapacity
		{
			get
			{
				int[] capacity = new int[this.Resource.Length];
				for(int i = 0; i < capacity.Length; i++)
				{
					capacity[i] = this.Resource[i].Capacity;
				}

				return new TResAmount(capacity);
			}
		}

		public void InitializeBuilding()
		{
			isBuildingInitialized = 1;
			UpCall.FetchVillageBuilding(ID);
			// TODO: Fixme
			InitializeTroop();
		}
		public void InitializeUpgrade()
		{
			isUpgradeInitialized = 1;
			UpCall.FetchVillageUpgrade(ID);
		}
		public void InitializeDestroy()
		{
			isDestroyInitialized = 1;
			UpCall.FetchVillageDestroy(ID);
		}
		public void InitializeTroop()
		{
			isTroopInitialized = 1;
			UpCall.FetchVillageTroop(ID);
		}
		public int TimeCost(TResAmount ResCost)
		{
			int time = 0;
			for(int i = 0; i < 4; i++)
				if(ResCost.Resources[i] > Resource[i].CurrAmount)
				{
					int costtime = -1;
					if(Resource[i].Produce > 0)
						costtime = (ResCost.Resources[i] - Resource[i].CurrAmount) * 3600 / Resource[i].Produce;
					if(costtime < 0)
						costtime = 32767;
					if(costtime > time)
						time = costtime;
				}
			return time;
		}
		[Json]
		public TInBuilding[] RB = new TInBuilding[5];
		[Json]
		public TMarket Market;
		[Json]
		public TTroop Troop;
		public TVillage()
		{
			Resource = new TResource[4];
			InBuilding = new TInBuilding[7];
			Queue = new List<IQueue>();
			Upgrades = new Dictionary<int, TRU>();
			Market = new TMarket();
			Troop = new TTroop();
			for(int i = 1; i <= 10; i++)
				Upgrades[i] = new TRU();
		}

		public override string ToString()
		{
			return TypeViewer.ToString(this);
		}

		/// <summary>
		/// Show the number of queued tasks and upper/lower limit tags
		/// </summary>
		/// <param name="db">User DB storing the queue/limit info</param>
		/// <returns>Status string</returns>
		public string GetStatus()
		{
			string status = this.Queue.Count.ToString();

			if(this.Market.LowerLimit != null)
			{
				status = status + "v";
			}

			if(this.Market.UpperLimit != null)
			{
				status = status + "^";
			}

			return status;
		}

		public string Snapshot()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Basic data:");
			sb.AppendLine(TypeViewer.Snapshot(this));
			sb.AppendLine(TypeViewer.Snapshot(this.Market));
			if(isBuildingInitialized == 2)
			{
				sb.AppendLine("Market:");
				foreach(TMInfo info in this.Market.MarketInfo)
				{
					sb.Append("\t");
					sb.AppendLine(info.ToString());
				}
				sb.AppendLine("Building:");
				foreach(var b in Buildings)
				{
					sb.Append("\t");
					sb.Append(b.Key);
					sb.Append(": ");
					sb.AppendLine(b.Value.ToString());
				}
				sb.AppendLine("Upgrades:");
				foreach(var b in Upgrades)
				{
					sb.Append("\t");
					sb.Append(b.Key);
					sb.Append(": ");
					sb.AppendLine(b.Value.ToString());
				}
				sb.AppendLine("Resource:");
				foreach(var b in Resource)
				{
					sb.Append("\t");
					sb.AppendLine(b.ToString());
				}
				sb.AppendLine("InBuilding:");
				for(var i = 0; i < InBuilding.Length; i++)
				{
					sb.Append("\t");
					sb.Append(i);
					sb.Append(": ");
					if(InBuilding[i] == null)
						sb.AppendLine("NULL");
					else
						sb.AppendLine(InBuilding[i].ToString());
				}
				sb.AppendLine("RecentBuilt:");
				for(var i = 0; i < RB.Length; i++)
				{
					sb.Append("\t");
					sb.Append(i);
					sb.Append(": ");
					if(RB[i] == null)
						sb.AppendLine("NULL");
					else
						sb.AppendLine(RB[i].ToString());
				}
				sb.AppendLine("Queue:");
				foreach(var b in Queue)
				{
					sb.Append("\t");
					sb.AppendLine(b.ToString());
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Export task queue to a text file
		/// </summary>
		/// <param name="filename">Text file name</param>
		//[Obsolete]
		public void SaveQueue(string filename)
		{
			try
			{
				var json = JsonMapper.ToJson(new Dictionary<int, List<IQueue>>{{ 0, Queue }});
				File.WriteAllText(filename, json);
			}
			catch { }
		}

		/// <summary>
		/// Import task queue from a text file
		/// </summary>
		/// <param name="filename">Text file name</param>
		//[Obsolete]
		public void RestoreQueue(string filename)
		{
			try
			{
				var Qs = JsonMapper.ToObject<Dictionary<int, List<IQueue>>>(File.ReadAllText(filename));
				foreach(var q in Qs[0])
				{
					q.UpCall = UpCall;
					q.VillageID = ID;
					if(q is BuildingQueue)
					{
						var Q = q as BuildingQueue;
						if(Buildings.ContainsKey(Q.Bid))
						{
							if(Buildings[Q.Bid].Gid != Q.Gid)
								Q.MarkDeleted = true;
						}
						else
							Buildings[Q.Bid] = new TBuilding { Gid = Q.Gid, Level = 0 };
					}
				}
				for(int i = Qs[0].Count - 1; i >= 0; i--)
					if(Qs[0][i].MarkDeleted)
						Qs[0].RemoveAt(i);
				UpCall.Dirty = true;
				
				Queue.AddRange(Qs[0]);
			}
			catch { }
		}
	}

	public class TResource
	{
		[Json]
		public int Capacity { set; get; }
		[Json]
		public int Amount;// { private set; get; }
		[Json]
		public int Produce { set; get; }
		[Json]
		public long Time { get { return UpdateTime.Ticks; } set { UpdateTime = new DateTime(value); } }
		private DateTime UpdateTime;
		public TResource(int Produce, int Amount, int Capacity)
		{
			this.Produce = Produce;
			this.Amount = Amount;
			this.Capacity = Capacity;
			UpdateTime = DateTime.Now;
		}
		public TResource()
		{
		}
		public void Write(int Amount)
		{
			this.Amount = Math.Min(Amount, Capacity);
			UpdateTime = DateTime.Now;
		}
		public TimeSpan LeftTime
		{
			get
			{
				if(Produce < 0)
					return new TimeSpan(0, 0, Convert.ToInt32(Convert.ToInt64(CurrAmount) * 3600 / -Produce));
				else if(Produce > 0)
					return new TimeSpan(0, 0, Convert.ToInt32(Convert.ToInt64(Capacity - CurrAmount) * 3600 / Produce));
				else
					return new TimeSpan(1, 0, 0, 0);
			}
		}
		public int CurrAmount
		{
			get
			{
				return Math.Max(0, Math.Min(Amount + (int)(DateTime.Now.Subtract(UpdateTime).TotalHours * Produce), Capacity));
			}
		}
		public override string ToString()
		{
			return TypeViewer.ToString(this);
		}

	}

	/// <summary>
	/// Index as Bid.
	/// </summary>
	public class TBuilding
	{
		[Json]
		public int Gid { get; set; }
		[Json]
		public int Level { get; set; }
		[Json]
		public bool InBuilding { get; set; }
		public TBuilding()
		{
			//Console.WriteLine("TBuilding init");
		}
		public override string ToString()
		{
			return TypeViewer.ToString(this);
			//return string.Format("GID:{0}, Level:{1}, InBuilding:{2}", Gid, Level, InBuilding);
		}
	}

	/// <summary>
	/// Index as in-building type.
	/// </summary>
	public class TInBuilding
	{
		/// <summary>
		/// Bid for building, Aid for upgrade.
		/// </summary>
		[Json]
		public int ABid { get; set; }
		[Json]
		public int Gid { get; set; }
		[Json]
		public int Level { get; set; }
		[Json]
		public DateTime FinishTime { get; set; }
		public TInBuilding() { }
		public override string ToString()
		{
			return TypeViewer.ToString(this);
			//return string.Format("GID:{0}, BID:{1}, Level:{2}, FinishTime:{3}", Gid, ABid, Level, FinishTime.ToString());
		}
		[Json]
		public string CancelURL { get; set; }
		public bool Cancellable
		{
			get
			{
				return !string.IsNullOrEmpty(CancelURL);
			}
		}
	}

	public enum TQueueType
	{
		None,
		Building,
		Destroy,
		UAttack,
		UDefense,
		Research,
		Party,
		Transfer,
		NpcTrade,
		Raid
	}

	/// <summary>
	/// A queued task
	/// </summary>
	public class TQueue
	{
		/// <summary>
		/// Represents AI build strategy
		/// </summary>
		public static readonly int AIBID = -1024;

		/// <summary>
		/// Building slot ranges 1 - 50(?)
		/// </summary>
		public int Bid { get; set; }

		/// <summary>
		/// Building type
		/// </summary>
		public int Gid { get; set; }

		/// <summary>
		/// Target building level
		/// </summary>
		public int TargetLevel { get; set; }

		/// <summary>
		/// For display only
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Need to distinguish out-skirt (0) and within-village (1) building tasks 
		/// </summary>
		public int Type
		{
			get
			{
				if(QueueType == TQueueType.Building)
					return Bid < 19 && Bid > 0 ? 0 : Bid != AIBID ? 1 : 0;
				else
					return (int)QueueType;
			}
		}

		/// <summary>
		/// Task type
		/// </summary>
		public TQueueType QueueType { get; set; }

		/// <summary>
		/// Encoded extra task options (used by transfer only)
		/// </summary>
		public string ExtraOptions { get; set; }

		/// <summary>
		/// New Options for taking the place of the old option string
		/// </summary>
		public IQueue NewOptions { get; set; }

		/// <summary>
		/// When the queued task is ready to go (for display only)
		/// </summary>
		public DateTime NextExec { get; set; }

		/// <summary>
		/// When set to true, the corresponding task will be temporarily suspended
		/// </summary>
		public bool Paused { get; set; }

		/// <summary>
		/// Default constructor (will not create a valid task object though)
		/// </summary>
		public TQueue()
		{
			this.QueueType = TQueueType.None;
			this.Bid = 0;
			this.Gid = 0;
			this.Status = "";
			this.ExtraOptions = "";
			this.NextExec = DateTime.MinValue;
			this.Paused = false;
		}

		/// <summary>
		/// Decode a queued task from an encoded string
		/// </summary>
		/// <param name="data">Encoded string</param>
		/// <returns>Decoded task</returns>
		public static TQueue FromString(string data)
		{
			TQueue task = new TQueue();

			foreach(string attribute in data.Split(','))
			{
				string[] kvpair = attribute.Trim().Split(':');
				if(kvpair.Length != 2)
				{
					continue;
				}

				switch(kvpair[0])
				{
					case "Bid":
						task.Bid = Convert.ToInt32(kvpair[1]);
						break;
					case "Gid":
						task.Gid = Convert.ToInt32(kvpair[1]);
						break;
					case "TargetLevel":
						task.TargetLevel = Convert.ToInt32(kvpair[1]);
						break;
					case "Status":
						task.Status = kvpair[1];
						break;
					case "QueueType":
						task.QueueType = (TQueueType)Enum.Parse(typeof(TQueueType), kvpair[1]);
						break;
					case "ExtraOptions":
						task.ExtraOptions = kvpair[1];
						break;
					case "Paused":
						task.Paused = Boolean.Parse(kvpair[1]);
						break;
				}
			}

			return task;
		}

		/// <summary>
		/// Encode all properties in format "name:value,name:value,..."
		/// </summary>
		/// <returns>Encoded string</returns>
		public override string ToString()
		{
			return TypeViewer.ToString(this);
		}
	}

	public class TRU
	{
		[Json]
		public bool CanResearch { get; set; }
		[Json]
		public bool Researched { get; set; }
		[Json]
		public int AttackLevel { get; set; }
		[Json]
		public int DefenceLevel { get; set; }
		[Json]
		public bool InUpgrading { get; set; }
		public TRU()
		{
			AttackLevel = -1;
			DefenceLevel = -1;
		}
		public override string ToString()
		{
			return TypeViewer.ToString(this);
		}
	}

	public class TMarket
	{
		[Json]
		public int SingleCarry { get; set; }
		[Json]
		public int ActiveMerchant { get; set; }
		[Json]
		public int MaxMerchant { get; set; }
		[Json]
		public List<TMInfo> MarketInfo { get; set; }

		/// <summary>
		/// When transfer outward, don't let remaining resource below this 
		/// </summary>
		[Json]
		public TResAmount UpperLimit { get; set; }

		/// <summary>
		/// Stop receiving transporations when current resource amount is higher than this
		/// </summary>
		[Json]
		public TResAmount LowerLimit { get; set; }

		/// <summary>
		/// How long will the next market event (e.g., merchan returns) happen
		/// </summary>
		public int MinimumDelay
		{
			get
			{
				DateTime nextEventAt = DateTime.MaxValue;
				foreach(TMInfo transfer in this.MarketInfo)
				{
					if(transfer.FinishTime < nextEventAt)
					{
						nextEventAt = transfer.FinishTime;
					}
				}

				if(nextEventAt < DateTime.MaxValue)
				{
					return (int)Math.Round((nextEventAt - DateTime.Now).TotalSeconds);
				}
				else
				{
					return 0;
				}
			}
		}

		public void tick(TVillage CV, int MarketSpeed)
		{
			for(int i = MarketInfo.Count - 1; i >= 0; i--)
			{
				TMInfo x = MarketInfo[i];
				if(x.FinishTime > DateTime.Now)
					continue;
				if(x.MType == TMType.MyBack)
				{
					MarketInfo.Remove(x);
					if(SingleCarry == 0)
						continue;
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ActiveMerchant.ToString());
					ActiveMerchant += Convert.ToInt32(Math.Ceiling((double)(x.CarryAmount.Resources[0] + x.CarryAmount.Resources[1] + x.CarryAmount.Resources[2] + x.CarryAmount.Resources[3]) / SingleCarry));
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ActiveMerchant.ToString());
				}
				else if(x.MType == TMType.MyOut)
				{
					x.MType = TMType.MyBack;
					var distance = CV.Coord * x.Coord;
					var time = distance * 3600 / MarketSpeed;
					try
					{
						x.FinishTime = x.FinishTime.AddSeconds(time);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(
							string.Format("{0}\r\nMarketSpeed:{1}\r\nMyCoord:{2}\r\nTargetCoord:{3}",
							ex.Message, MarketSpeed, CV.Coord, x.Coord));
					}

				}
				else
				{
					for(int j = 0; j < 4; j++)
						CV.Resource[j].Write(CV.Resource[j].CurrAmount + x.CarryAmount.Resources[j]);
					MarketInfo.Remove(x);
				}
			}
		}
		public TMarket()
		{
			SingleCarry = 0;
			ActiveMerchant = 0;
			MaxMerchant = 0;
			MarketInfo = new List<TMInfo>();
		}
		[Obsolete]
		public string Export()
		{
			return string.Format("{0}&{1}&{2}", SingleCarry, ActiveMerchant, MaxMerchant);
		}
		[Obsolete]
		public void Import(string s)
		{
			string[] p = s.Split('&');
			if(p.Length == 3)
			{
				SingleCarry = Convert.ToInt32(p[0]);
				ActiveMerchant = Convert.ToInt32(p[1]);
				MaxMerchant = Convert.ToInt32(p[2]);
			}
		}
	}
	public enum TMType
	{
		MyOut,
		MyBack,
		OtherCome
	};

	/// <summary>
	/// Stores the info of an ongoing transportation
	/// </summary>
	public class TMInfo
	{
		[Json]
		public TMType MType { get; set; }
		[Json]
		public TResAmount CarryAmount { get; set; }
		[Json]
		public TPoint Coord { get; set; }
		[Json]
		public string VillageName { get; set; }
		[Json]
		public DateTime FinishTime { get; set; }

		public override string ToString()
		{
			return String.Format(
				"{0},{1},{2},{3},{4}",
				this.FinishTime,
				this.CarryAmount,
				this.VillageName,
				this.Coord,
				this.MType);
		}
	}

	public class TTroop
	{
		[Json]
		public int TournamentLevel { get; set; }
		[Json]
		public List<TTInfo> Troops { get; set; }
		public bool ShouldRefresh { get; set; }
		public void tick(TVillage CV)
		{
			for(int i = Troops.Count - 1; i >= 0; i--)
			{
				var T = Troops[i];
				if(T.FinishTime == DateTime.MinValue)
					continue;
				if(T.FinishTime < DateTime.Now)
				{
					ShouldRefresh = true;
					break;
					// could be written better in the future
				}
			}
		}
		public TTroop()
		{
			TournamentLevel = 0;
			Troops = new List<TTInfo>();
		}
	}

	public class TTInfo
	{
		[Json]
		public int Tribe;
		[Json]
		public int[] Troops;
		[Json]
		public TTroopType TroopType;
		[Json]
		public DateTime FinishTime;
		[Json]
		public string VillageName;
		public override string ToString()
		{
			return String.Format(
				"{0},{1},{2},{3}",
				this.Tribe,
				this.TroopType,
				this.FinishTime,
				this.VillageName);
		}
		public string FriendlyName
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < Troops.Length; i++)
				{
					if(Troops[i] != 0)
						sb.AppendFormat("{0} {1}, ", DisplayLang.Instance.GetAidLang(Tribe, i + 1), Troops[i]);
				}
				if(sb.Length > 2)
					sb.Remove(sb.Length - 2, 2);
				else
					return "None";
				return sb.ToString();
			}
		}
	}
	/*
 link  time  troopcount
  -     O       O      MyReturnWay
  O     O       O      MyAttackWay
  O     O       O      MySupportWay
  O     -       O      MySupportOther
  -     O       -      BeAttackedWay
  -     O       -      BeSupportedWay
  -     -       O      MySelf
	 */
	public enum TTroopType
	{
		MyReturnWay, MyAttackWay, MySupportWay, MySupportOther, BeAttackedWay, BeSupportedWay, MySelf
	}

	public class DummyQueueContainer
	{
		[Json]
		public List<IQueue> Data;
	}
}
