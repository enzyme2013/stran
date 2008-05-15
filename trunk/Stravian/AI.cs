using System;
using System.Collections.Generic;
using System.Text;

namespace Stravian
{
	class AI
	{
		static double[] resrate;
		static public Dictionary<int, int[]> preferpos = new Dictionary<int, int[]>();
		static public void Init()
		{
			preferpos[5] = new int[] { 28 };
			preferpos[6] = new int[] { 24 };
			preferpos[7] = new int[] { 19 };
			preferpos[8] = new int[] { 32 };
			preferpos[9] = new int[] { 20 };
			preferpos[10] = new int[] { 35, 37, 32, 33, 28, 24, 19 };
			preferpos[11] = new int[] { 36, 38, 20, 29 };
			preferpos[12] = new int[] { 30 };
			preferpos[13] = new int[] { 25 };
			preferpos[14] = new int[] { 22 };
			preferpos[15] = new int[] { 26 };
			preferpos[16] = new int[] { 39 };
			preferpos[17] = new int[] { 27 };
			preferpos[18] = new int[] { 33 };
			preferpos[19] = new int[] { 34 };
			preferpos[20] = new int[] { 31 };
			preferpos[21] = new int[] { 31 };
			preferpos[22] = new int[] { 23 };
			preferpos[24] = new int[] { 33 };
			preferpos[25] = new int[] { 21 };
			preferpos[26] = new int[] { 21 };
			preferpos[28] = new int[] { 22 };
		}
		static public BQ doAI(libTravian tr, int vid)
		{
			if(resrate == null)
			{
				if(MainForm.options.ContainsKey("resrate"))
				{
					string[] t = MainForm.options["resrate"].Split(':');
					if(t.Length == 4)
					{
						resrate = new double[4];
						for(int j = 0; j < 4; j++)
							resrate[j] = Convert.ToDouble(t[j]);
					}
				}
				if(resrate == null)
					resrate = new double[4] { 10, 10, 9, 7 };
			}

			// now it works really simple
			// only farming
			int i;
			Village currv = tr.villages[vid];
			if(currv.res == null)
				return null;
			//int[] prior = currv.res.CurrAmount;
			int min = 1;
			for(i = 0; i < 4; i++)
				if(currv.res.CurrAmount(min) / resrate[min] > currv.res.CurrAmount(i) / resrate[i])
					min = i;
			int bid = -1, gid = 0;
			for(i = 1; i < 19; i++)
				if(currv.buildings[i].gid == min + 1)
					if(bid == -1)
						bid = i;
					else if(currv.buildings[i].level < currv.buildings[bid].level)
						bid = i;
			gid = min + 1;
			// check warehouse/granary
			int tgid, tbid;
			if(currv.res.capacity[0] < Buildings.cost(gid, currv.buildings[bid].level + 1).resources[0] * 3 ||
				currv.res.capacity[1] < Buildings.cost(gid, currv.buildings[bid].level + 1).resources[1] * 3 ||
				currv.res.capacity[2] < Buildings.cost(gid, currv.buildings[bid].level + 1).resources[2] * 3
				)
			{
				tgid = 10;
				tbid = findDorf2Building(currv.buildings, tgid);
				if(tbid != -1)
				{
					gid = tgid;
					bid = tbid;
				}
			}
			else if(currv.res.capacity[3] < Buildings.cost(gid, currv.buildings[bid].level + 1).resources[3] * 4)
			{
				tgid = 11;
				tbid = findDorf2Building(currv.buildings, tgid);
				if(tbid != -1)
				{
					gid = tgid;
					bid = tbid;
				}
			}
			else // check main building
			{
				tgid = 15;
				tbid = findDorf2Building(currv.buildings, tgid);
				if(tbid != -1 && currv.buildings[tbid].level < 20 && currv.buildings[tbid].level < currv.res.capacity[0] / 4000)
				{
					gid = tgid;
					bid = tbid;
				}
			}

			return new BQ()
			{
				Vid = vid,
				Bid = bid,
				Gid = gid,
				QueueType = BQ.TQueueType.Building
			};
		}
		static public int findDorf2Building(Building[] b, int gid)
		{
			int k;
			int tlvl = 20, tid = 0;
			for(k = 19; k < b.Length; k++)
				if(b[k] != null && b[k].gid == gid)
					if(tlvl > b[k].level)
					{
						tlvl = b[k].level;
						tid = k;
					}
			if(tid != 0)
				return tid;
			for(k = 0; k < preferpos[gid].Length; k++)
				if(b[preferpos[gid][k]] == null)
					return preferpos[gid][k];
			//for(k = 19; k < b.Length; k++)

			return -1;
		}
	}
}
