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
using System.Drawing;
using System.Text.RegularExpressions;

namespace libTravian
{
	public class TransferOption
	{
		public int VillageID { get; set; }
		public TResAmount ResourceAmount { get; set; }
		public TPoint TargetPos { get; set; }
	}
	partial class Travian
	{
		private TResAmount JustTransferredData = null;
		public void doTransferWrapper(object o)
		{
			TransferOption to = o as TransferOption;
			TResAmount Amount = to.ResourceAmount;
			TPoint TargetPos = to.TargetPos;
			int VillageID = to.VillageID;
			doTransfer(VillageID, Amount, TargetPos);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="VillageID"></param>
		/// <param name="Amount"></param>
		/// <param name="TargetPos"></param>
		/// <returns>Error return minus number. Succeed return single way transfer time cost.</returns>
		public int doTransfer(int VillageID, TResAmount Amount, TPoint TargetPos)
		{
			string result = PageQuery(VillageID, "build.php?gid=17");
			if(result == null)
				return -1;
			var CV = TD.Villages[VillageID];
			Dictionary<string, string> PostData = new Dictionary<string, string>();
			var m = Regex.Match(result, "name=\"id\" value=\"(\\d+)\"");
			if(!m.Success)
				return -1; // Parse error!
			PostData["id"] = m.Groups[1].Value;
			m = Regex.Match(result, "var haendler = (\\d+);");
			if(!m.Success)
				return -1;
			var MCount = Convert.ToInt32(m.Groups[1].Value);
			m = Regex.Match(result, "var carry = (\\d+);");
			if(!m.Success)
				return -1;
			var MCarry = Convert.ToInt32(m.Groups[1].Value);

			int TAmount = 0;
			for(int i = 0; i < 4; i++)
			{
				PostData["r" + (i + 1).ToString()] = Amount.Resources[i].ToString();
				TAmount += Amount.Resources[i];
			}
			if(TAmount > MCarry * MCount)
				return -2; // Beyond transfer ability

			PostData["dname"] = "";
			PostData["x"] = TargetPos.X.ToString();
			PostData["y"] = TargetPos.Y.ToString();
			PostData["s1"] = "ok";

			result = PageQuery(VillageID, "build.php", PostData);

			if(result == null)
				return -1;
			m = Regex.Match(result, "name=\"sz\" value=\"(\\d+)\"");
			if(!m.Success)
				return -1; // Parse error!
			PostData["sz"] = m.Groups[1].Value;
			PostData["kid"] = TargetPos.Z.ToString();
			PostData["a"] = VillageID.ToString();
			m = Regex.Match(result, "<td>([0-9:]{6,})</td>");
			if(!m.Success)
				return -1; // Parse error!
			int TimeCost = Convert.ToInt32(TimeSpanParse(m.Groups[1].Value).TotalSeconds);
			if(TD.MarketSpeed != 0)
			{
				// calc market speed
				var distance = CV.Coord * TargetPos;
				TD.MarketSpeed = Convert.ToInt32(Math.Round(distance * 3600 / TimeCost));
				if(!svrdb.ContainsKey("MarketSpeedX"))
				{
					int StdSpeed;
					if(TD.Tribe == 1)
						StdSpeed = 16;
					else if(TD.Tribe == 2)
						StdSpeed = 12;
					else
						StdSpeed = 24;
					svrdb["MarketSpeedX"] = (TD.MarketSpeed / StdSpeed).ToString();
				}
			}
			JustTransferredData = Amount;
			result = PageQuery(VillageID, "build.php", PostData);


			// write data into target village if it's my village.
			foreach(var x in TD.Villages)
			{
				if(x.Value == CV)
					continue;
				if(x.Value.Coord == TargetPos)
				{
					if(x.Value.isBuildingInitialized == 2)
						x.Value.Market.MarketInfo.Add(new TMInfo()
						{
							CarryAmount = Amount,
							Coord = CV.Coord,
							FinishTime = DateTime.Now.AddSeconds(TimeCost),
							MType = TMType.OtherCome,
							VillageName = CV.Name
						});
					break;
				}
			}
			return TimeCost;
		}
	}
}
