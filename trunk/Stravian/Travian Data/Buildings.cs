using System;

namespace Stravian
{
	public class resourceinfo
	{
		public int[] resources;
		public resourceinfo(int r1, int r2, int r3, int r4)
		{
			resources = new int[4] { r1, r2, r3, r4 };
		}
		static public int operator ^(resourceinfo op1, resource op2)
		{
			int time = 0;
			for(int i = 0; i < 4; i++)
				if(op1.resources[i] > op2.CurrAmount(i))
				{
					int costtime = (op1.resources[i] - op2.CurrAmount(i)) * 3600 / op2.produce[i];
					if(costtime < 0)
						costtime = 32767;
					if(costtime > time)
						time = costtime;
				}
			return time;
		}
	}
	public class rinfo_array
	{
		resourceinfo[] _data;
		double ratio;
		public int length { get; private set; }
		resourceinfo resbase;
		private int mytrunc(double x)
		{
			int t1 = (int)(x / 10) * 10;
			int t2 = (int)x - t1;
			if(t2 >= 7.5)
				t1 += 10;
			else if(t2 >= 2.5)
				t1 += 5;
			return t1;
		}
		public rinfo_array(int Length, int type, resourceinfo Resbase)
		{
			resbase = Resbase;
			length = Length;
			ratio = type == 0 ? 1.67 : 1.28;
		}
		public resourceinfo[] data
		{
			get
			{
				if(_data == null)
				{
					double[] r = new double[4];
					int i, j;
					_data = new resourceinfo[length + 1];
					_data[1] = resbase;
					for(i = 0; i < 4; i++)
						r[i] = resbase.resources[i];
					for(i = 2; i <= length; i++)
					{
						_data[i] = new resourceinfo(0, 0, 0, 0);
						for(j = 0; j < 4; j++)
						{
							r[j] *= ratio;
							_data[i].resources[j] = mytrunc(r[j]);
						}
					}
				}
				return _data;
			}
		}
	}

	class Buildings
	{
		static public rinfo_array[] _cost;
		static T_BuildLevel[,] _depends;
		public static void Init()
		{
			if(_cost == null)
				InitCost();
			if(_depends == null)
				InitDepend();
		}
		public static void InitCost()
		{
			_cost = new rinfo_array[40];
			_cost[1] = new rinfo_array(20, 0, new resourceinfo(40, 100, 50, 60));
			_cost[2] = new rinfo_array(20, 0, new resourceinfo(80, 40, 80, 50));
			_cost[3] = new rinfo_array(20, 0, new resourceinfo(100, 80, 30, 60));
			_cost[4] = new rinfo_array(20, 0, new resourceinfo(70, 90, 70, 20));
			_cost[5] = new rinfo_array(5, 1, new resourceinfo(520, 380, 290, 90));
			_cost[6] = new rinfo_array(5, 1, new resourceinfo(440, 480, 320, 50));
			_cost[7] = new rinfo_array(5, 1, new resourceinfo(200, 450, 510, 120));
			_cost[8] = new rinfo_array(5, 1, new resourceinfo(500, 440, 380, 1240));
			_cost[9] = new rinfo_array(5, 1, new resourceinfo(1200, 1480, 870, 1600));
			_cost[10] = new rinfo_array(20, 1, new resourceinfo(130, 160, 90, 40));
			_cost[11] = new rinfo_array(20, 1, new resourceinfo(80, 100, 70, 20));
			_cost[12] = new rinfo_array(20, 1, new resourceinfo(170, 200, 380, 130));
			_cost[13] = new rinfo_array(20, 1, new resourceinfo(130, 210, 410, 130));
			_cost[14] = new rinfo_array(20, 1, new resourceinfo(1750, 2250, 1530, 240));
			_cost[15] = new rinfo_array(20, 1, new resourceinfo(70, 40, 60, 20));
			_cost[16] = new rinfo_array(20, 1, new resourceinfo(110, 160, 90, 70));
			_cost[17] = new rinfo_array(20, 1, new resourceinfo(80, 70, 120, 70));
			_cost[18] = new rinfo_array(20, 1, new resourceinfo(180, 130, 150, 80));
			_cost[19] = new rinfo_array(20, 1, new resourceinfo(210, 140, 260, 120));
			_cost[20] = new rinfo_array(20, 1, new resourceinfo(260, 140, 220, 100));
			_cost[21] = new rinfo_array(20, 1, new resourceinfo(460, 510, 600, 320));
			_cost[22] = new rinfo_array(20, 1, new resourceinfo(220, 160, 90, 40));
			_cost[23] = new rinfo_array(10, 1, new resourceinfo(40, 50, 30, 10));
			_cost[24] = new rinfo_array(20, 1, new resourceinfo(1250, 1110, 1260, 600));
			_cost[25] = new rinfo_array(20, 1, new resourceinfo(580, 460, 350, 180));
			_cost[26] = new rinfo_array(20, 1, new resourceinfo(550, 800, 750, 250));
			_cost[27] = new rinfo_array(10, 1, new resourceinfo(2880, 2740, 2580, 990));
			_cost[28] = new rinfo_array(20, 1, new resourceinfo(1400, 1330, 1200, 400));
			_cost[29] = new rinfo_array(20, 1, new resourceinfo(630, 420, 780, 360));
			_cost[30] = new rinfo_array(20, 1, new resourceinfo(780, 420, 660, 300));
			_cost[31] = new rinfo_array(20, 1, new resourceinfo(70, 90, 170, 70));
			_cost[32] = new rinfo_array(20, 1, new resourceinfo(120, 200, 0, 80));
			_cost[33] = new rinfo_array(20, 1, new resourceinfo(160, 100, 80, 60));
			_cost[34] = new rinfo_array(20, 1, new resourceinfo(155, 130, 125, 70));
			_cost[35] = new rinfo_array(20, 1, new resourceinfo(1200, 1400, 1050, 2200));
			_cost[36] = new rinfo_array(20, 1, new resourceinfo(100, 100, 100, 100));
			_cost[37] = new rinfo_array(20, 1, new resourceinfo(700, 670, 700, 240));
		}
		public static void InitDepend()
		{
			_depends = new T_BuildLevel[40, 4];
			_depends[5, 0] = new T_BuildLevel(1, 10);
			_depends[5, 1] = new T_BuildLevel(15, 5);
			_depends[6, 0] = new T_BuildLevel(2, 10);
			_depends[6, 1] = new T_BuildLevel(15, 5);
			_depends[7, 0] = new T_BuildLevel(3, 10);
			_depends[7, 1] = new T_BuildLevel(15, 5);
			_depends[8, 0] = new T_BuildLevel(4, 5);
			_depends[9, 0] = new T_BuildLevel(4, 10);
			_depends[9, 1] = new T_BuildLevel(15, 5);
			_depends[9, 2] = new T_BuildLevel(8, 5);
			_depends[12, 0] = new T_BuildLevel(22, 3);
			_depends[13, 0] = new T_BuildLevel(22, 1);
			_depends[14, 0] = new T_BuildLevel(16, 15);
			_depends[17, 0] = new T_BuildLevel(10, 1);
			_depends[17, 1] = new T_BuildLevel(11, 1);
			_depends[17, 2] = new T_BuildLevel(15, 3);
			_depends[19, 0] = new T_BuildLevel(16, 1);
			_depends[20, 0] = new T_BuildLevel(12, 3);
			_depends[20, 1] = new T_BuildLevel(22, 5);
			_depends[21, 0] = new T_BuildLevel(22, 10);
			_depends[21, 1] = new T_BuildLevel(15, 5);
			_depends[22, 0] = new T_BuildLevel(19, 3);
			_depends[22, 1] = new T_BuildLevel(16, 1);
			_depends[24, 0] = new T_BuildLevel(22, 10);
			_depends[24, 1] = new T_BuildLevel(15, 10);
			_depends[25, 0] = new T_BuildLevel(15, 5);
			//_depends[25, 1] = new T_BuildLevel(26, -1);
			_depends[26, 0] = new T_BuildLevel(18, 1);
			//_depends[26, 1] = new T_BuildLevel(25, -1);
			//_depends[27, 0] = new T_BuildLevel(-1, -1);
			_depends[28, 0] = new T_BuildLevel(17, 20);
			_depends[28, 1] = new T_BuildLevel(20, 10);
			//_depends[29, 1] = new T_BuildLevel(26, -1);
			_depends[29, 0] = new T_BuildLevel(19, 20);
			//_depends[30, 1] = new T_BuildLevel(26, -1);
			_depends[30, 0] = new T_BuildLevel(20, 20);
			_depends[34, 0] = new T_BuildLevel(15, 5);
			_depends[34, 1] = new T_BuildLevel(26, 3);
			//_depends[35, 0] = new T_BuildLevel(-1, -1);
			_depends[37, 0] = new T_BuildLevel(15, 3);
			_depends[37, 1] = new T_BuildLevel(16, 1);

		}
		public static resourceinfo cost(int gid, int level)
		{
			try
			{
				if(gid < 0)
					return new resourceinfo(0, 0, 0, 0);
				if(level >= _cost[gid].data.Length)
					return new resourceinfo(0, 0, 0, 0);
				else
					return _cost[gid].data[level];
			}
			catch(Exception)
			{
				//throw (new Exception("_cost[" + gid.ToString() + ", " + level.ToString() + "]"));
				return new resourceinfo(0, 0, 0, 0);
			}

		}
		public static int checkdepend(int gid, Building[] b)//, libServerLang svrlang)
		{
			int newid = gid;
			for(int i = 0; i < _depends.GetLength(1); i++)
				if(_depends[gid, i] != null)
				{
					//Check dependence
					bool manzoku = false, found = false;
					for(int j = 1; j < b.Length; j++)
						if(b[j] != null && b[j].gid == _depends[gid, i].gid)
						{
							found = true;
							if(b[j].level >= _depends[gid, i].level)
								manzoku = true;
						}
					if(!manzoku)
					{
						newid = _depends[gid, i].gid;
						return newid;
					}
					else if(!found)
					{
						newid = checkdepend(_depends[gid, i].gid, b);//, svrlang);
						return newid;
					}
				}
			return newid;
		}
		public static bool checklevelfull(int gid, int level, bool capital)
		{
			return (!capital && gid < 5 && level >= 10) || level >= _cost[gid].length;
		}
	}
	class T_BuildLevel
	{
		public int gid, level;
		public T_BuildLevel(int gid, int level)
		{
			this.gid = gid;
			this.level = level;
		}
	}
}
