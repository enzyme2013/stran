using System;
using System.Drawing;
using System.Text;

namespace Stravian
{
	public class Village
	{
		public int id;
		public string name;
		public Point pos;
		public Building[] buildings;
		public Village(int id, string name, Point pos) : this(id, name, pos, false) { }
		public Village(int id, string name, Point pos, bool capital)
		{
			this.id = id;
			this.name = name;
			this.pos = pos;
			res = new resource();
			inb = new inbuild[5];
			buildings = new Building[41];
			this.capital = capital;
		}
		public Village(int id, string name, int z, bool capital)
		{
			this.id = id;
			this.name = name;
			this.z = z;
			res = new resource();
			inb = new inbuild[5];
			buildings = new Building[41];
			this.capital = capital;
		}
		public resource res;
		public inbuild[] inb;
		public libMerchant mer;
		public bool capital;
		public int z
		{
			get
			{
				return 801 * (400 - pos.Y) + pos.X + 401;
			}
			set
			{
				pos = new Point((value - 401) % 801, 400 - (value - 401) / 801);
			}
		}
	}

	public class inbuild
	{
		public int type, level, gid;
		public string name;
		public DateTime completetime;
		
		public inbuild(string name, int level, TimeSpan lefttime, string mine)
		{
			this.level = level;
			this.gid = -1;
			this.name = name;
			type = mine.IndexOf(name) >= 0 ? 0 : 1;
			completetime = DateTime.Now.Add(lefttime).AddSeconds(15);
		}
		public inbuild(int gid, int level, TimeSpan lefttime)
			: this(gid, level, lefttime, gid < 5 ? 0 : 1)
		{
		}
		public inbuild(int gid, int level, TimeSpan lefttime, int type)
		{
			this.level = level;
			this.gid = gid;
			this.type = type;
			completetime = DateTime.Now.Add(lefttime).AddSeconds(15);
		}
	}

	public class resource
	{
		public int[] amount, produce, capacity;
		public DateTime resuptime;
		public resource()
		{
			amount = new int[4];
			produce = new int[4];
			capacity = new int[4];
		}
		public void w(int index, int produce, int amount, int capacity)
		{
			this.produce[index] = produce;
			this.amount[index] = amount;
			this.capacity[index] = capacity;
			resuptime = DateTime.Now;
		}
		public TimeSpan lefttime(int index)
		{
			if(produce[index] == 0)
				return new TimeSpan(0);
			if(produce[index] < 0)
				return new TimeSpan(0, 0, (CurrAmount(index)) * 3600 / -produce[index]);
			return new TimeSpan(0, 0, (capacity[index] - CurrAmount(index)) * 3600 / produce[index]);
		}
		public int CurrAmount(int index)
		{
			return Math.Min((int)(amount[index] + DateTime.Now.Subtract(resuptime).TotalHours * produce[index]), capacity[index]);
		}
		static public resourceinfo operator -(resource op2, resourceinfo op1)
		{
			return new resourceinfo(
				op2.CurrAmount(0) - op1.resources[0],
				op2.CurrAmount(1) - op1.resources[1],
				op2.CurrAmount(2) - op1.resources[2],
				op2.CurrAmount(3) - op1.resources[3]);
		}
	}

	public class Building
	{
		public string status;
		public int level, gid;
		public Building(int gid, int level)
		{
			//this.name = name;
			this.gid = gid;
			this.level = level;
			status = "";
		}
		public Building(int gid) : this(gid, 0) { }
	}

	public class BQ
	{
		public enum TQueueType
		{
			None,
			Building,
			Destroy,
			UAttack,
			UDefense
		}
		public int Vid { get; set; }
		public int Bid { get; set; }
		public int Type
		{
			get
			{
				if(QueueType == TQueueType.Building)
					return Bid < 19 && Bid > 0 ? 0 : Bid != libTravian.AIBID ? 1 : 0;
				else 
					return (int)QueueType;
			}
		}
		public int Gid { get; set; }
		public int Delay { get; set; }
		public int TargetLevel { get; set; }
		public string Status { get; set; }
		public TQueueType QueueType;
		//public TUpgradeType UpgradeType;
		public BQ()
		{
			QueueType = TQueueType.None;
			Status = "";
		}
		/*
		public BQ(int vid, int bid, int gid)
		{
			//Type = bid < 19 && bid > 0 ? 0 : bid != libTravian.AIBID ? 1 : 0;
			this.Vid = vid;
			this.Bid = bid;
			this.Gid = gid;
			this.Delay = 0;
			if(bid == libTravian.AIBID)
				Status = "ÈË¹¤ÖÇÄÜ";
			QueueType = TQueueType.Building;
		}
		public BQ(int vid, int bid)
		{
			this.Vid = vid;
			this.Bid = bid;
			this.Delay = 0;
			QueueType = TQueueType.Destroy;
		}
		public BQ(int vid, int gid, TUpgradeType utype)
		{
			this.Vid = vid;
			this.Gid = gid;
			this.UpgradeType = utype;
		}
		*/


		//public BQ(int vid, int bid) : this(vid, bid, 0) { }
	}


	public class logininfo
	{
		public string Server { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Tribe { get; set; }
		public string Language { get; set; }
		public logininfo()
		{
			Language = "";
		}
		public logininfo(string[] accountdata)
		{
			Username = accountdata[0];
			Server = Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[1]));
			Password = Encoding.UTF8.GetString(Convert.FromBase64String(accountdata[2]));
			if(accountdata.Length > 3)
				Tribe = Convert.ToInt32(accountdata[3]);
			if(accountdata.Length > 4)
				Language = accountdata[4];
		}
	}
}
