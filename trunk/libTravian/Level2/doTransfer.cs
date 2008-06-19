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
using System.Text.RegularExpressions;

namespace libTravian
{
	/// <summary>
	/// Automatic resource balance flavors in resource transportations
	/// </summary>
	public enum ResourceDistributionType
	{
		/// <summary>
		/// Always transport fixed amounts of resources
		/// </summary>
		None = 0,

		/// <summary>
		/// Distribute the same amount of resource among all categories
		/// </summary>
		Uniform,

		/// <summary>
		/// Evenly distribute the source village's remaining resources
		/// </summary>
		BalanceSource,

		/// <summary>
		/// Evenly distribute destination village's resources wrt storage capacity
		/// </summary>
		BalanceTarget
	}

	/// <summary>
	/// Define what's in a transfer
	/// </summary>
	public class TransferOption
	{
		/// <summary>
		/// Short names for distribution type None, Source and Destination
		/// </summary>
		private static readonly string[] DistributionShortName = new string[] { "=>", "=>", "=S=>", "=T=>" };

		/// <summary>
		/// When the mechant (occupied by the previous transfer) will return
		/// </summary>
		private DateTime resumeTime;

		/// <summary>
		/// The destination village (where the resource is going)
		/// </summary>
		public int TargetVillageID { get; set; }

		/// <summary>
		/// How many times left
		/// </summary>
		public int MaxCount { get; set; }

		/// <summary>
		/// How many transfers been done so far
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Resource distribution options
		/// </summary>
		public ResourceDistributionType Distribution { get; set; }

		/// <summary>
		/// Do not transport
		/// </summary>
		public bool NoCrop { get; set; }

		/// <summary>
		/// Destination village
		/// </summary>
		public TPoint TargetPos { get; set; }

		/// <summary>
		/// Resource amount
		/// </summary>
		public TResAmount ResourceAmount { get; set; }

		/// <summary>
		/// Minimum interval between two consequential transfers in seconds
		/// </summary>
		public int MinimumInterval { get; set; }

		/// <summary>
		/// Return false if the total resource amount is 0
		/// </summary>
		public bool IsValid
		{
			get { return !this.TargetPos.IsEmpty && this.ResourceAmount.TotalAmount() > 0; }
		}

		/// <summary>
		/// Minimum seconds to wait until the mechant resturns
		/// </summary>
		public int MinimumDelay
		{
			get
			{
				int value = 0;
				if (this.resumeTime > DateTime.Now)
				{
					try
					{
						value = Convert.ToInt32((this.resumeTime - DateTime.Now).TotalSeconds);
					}
					catch (OverflowException)
					{
					}
				}

				return value;
			}
			set
			{
				this.resumeTime = DateTime.Now.AddSeconds(value);
			}
		}

		/// <summary>
		/// Display transfer amound ant repeat counts left
		/// </summary>
		public string Status
		{
			get
			{
				string count = this.Count.ToString() + "/";
				count += this.MaxCount == 0 ? "Inf" : this.MaxCount.ToString();
				return count + DistributionShortName[(int)this.Distribution] + this.ResourceAmount.ToString();
			}
		}

		/// <summary>
		/// For the first time, a transfer can start immediately (of course, if mechants/resources are available)
		/// </summary>
		public TransferOption()
		{
			this.resumeTime = DateTime.MinValue;
			this.ResourceAmount = new TResAmount(0, 0, 0, 0);
		}

		/// <summary>
		/// Display target village info
		/// </summary>
		/// <param name="travianData">Game info including target village info</param>
		/// <returns>Target village name and coordination</returns>
		public string GetTitle(Data travianData)
		{
			string pos = this.TargetPos.ToString();
			if (travianData != null && travianData.Villages.ContainsKey(this.TargetVillageID))
			{
				pos = pos + " " + travianData.Villages[this.TargetVillageID].Name;
			}

			return pos;
		}

		/// <summary>
		/// Return the minimum delay before the next transfer could start
		/// </summary>
		/// <param name="travianData">User data, including src/dst village info</param>
		/// <param name="sourceVillageID">Src village ID</param>
		/// <returns>Minimum delay in seconds</returns>
		public int GetDelay(Data travianData, int sourceVillageID)
		{
			if (!travianData.Villages.ContainsKey(sourceVillageID))
			{
				return 86400;
			}

			TVillage village = travianData.Villages[sourceVillageID];
			if (village.isBuildingInitialized != 2)
			{
				return 86400;
			}

			int []adjustedResources = new int[this.ResourceAmount.Resources.Length];
			for (int i = 0; i < adjustedResources.Length; i++)
			{
				adjustedResources[i] = this.ResourceAmount.Resources[i];
				if (village.Market.LowerLimit != null && adjustedResources[i] > 0)
				{
					adjustedResources[i] += village.Market.LowerLimit.Resources[i];
				}
			}

			int timecost = Math.Max(this.MinimumDelay, village.TimeCost(new TResAmount(adjustedResources)));
			if (this.ExceedTargetCapacity(travianData, sourceVillageID))
			{
				timecost = Math.Max(timecost, 86400);
			}

			if (this.ResourceAmount.TotalAmount() > village.Market.SingleCarry * village.Market.ActiveMerchant)
			{
				timecost = Math.Max(timecost, village.Market.MinimumDelay + 5);
			}

			return timecost;
		}

		/// <summary>
		/// Test if the target village have enought storage capacity for the to-be-transfered resources
		/// </summary>
		/// <param name="travianData">User game info, including target village distance and storage capacity</param>
		/// <returns>True if the transportation will overflow the target village</returns>
		public bool ExceedTargetCapacity(Data travianData, int sourceVillageID)
		{
			TResAmount targetCapacity = this.GetTargetCapacity(travianData, sourceVillageID);
			if (targetCapacity == null)
			{
				if (this.Distribution == ResourceDistributionType.BalanceTarget)
				{
					return true;
				}
			}
			else
			{
				for (int i = 0; i < targetCapacity.Resources.Length; i++)
				{
					if (this.ResourceAmount.Resources[i] > targetCapacity.Resources[i])
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Distribute transported resource amount 
		/// </summary>
		/// <param name="travianData">Game info of the current user</param>
		public void CalculateResourceAmount(Data travianData, int sourceVillageID)
		{
			switch (this.Distribution)
			{
				case ResourceDistributionType.None:
					break;
				case ResourceDistributionType.Uniform:
					this.EvenlyDistibuteResource();
					break;
				case ResourceDistributionType.BalanceSource:
					this.BalanceSourceResource(travianData, sourceVillageID);
					break;
				case ResourceDistributionType.BalanceTarget:
					this.BalanceDestinationResource(travianData, sourceVillageID);
					break;
			}
		}

		private void EvenlyDistibuteResource()
		{
			int total = this.ResourceAmount.TotalAmount();
			int slots = this.NoCrop ? 3 : 4;

			this.ResourceAmount = new TResAmount(0, 0, 0, 0);
			for (int i = 0; i < slots; i++)
			{
				this.ResourceAmount.Resources[i] = total / slots;
			}

			this.ResourceAmount.Resources[0] += total - this.ResourceAmount.TotalAmount();
		}

		private void BalanceSourceResource(Data travianData, int sourceVillageID)
		{
			TResAmount targetAmount = new TResAmount(0, 0, 0, 0);
			if (travianData != null &&
				travianData.Villages.ContainsKey(sourceVillageID) &&
				travianData.Villages[sourceVillageID].isBuildingInitialized == 2)
			{
				TVillage village = travianData.Villages[sourceVillageID];
				for (int i = 0; i < targetAmount.Resources.Length; i++)
				{
					targetAmount.Resources[i] = village.Resource[i].CurrAmount;
					if (village.Market.LowerLimit != null)
					{
						targetAmount.Resources[i] -= village.Market.LowerLimit.Resources[i];
					}
				}

				targetAmount.Normalize();
			}

			this.DoBalance(targetAmount);
		}

		private void BalanceDestinationResource(Data travianData, int sourceVillageID)
		{
			TResAmount targetAmount = this.GetTargetCapacity(travianData, sourceVillageID);
			if (targetAmount == null)
			{
				targetAmount = new TResAmount(0, 0, 0, 0);
			}

			this.DoBalance(targetAmount);
		}

		/// <summary>
		/// Estimate the target village capacity when transportantion arrives, based on 
		/// its current resource amount, production rate, distance, and merchant speed.
		/// </summary>
		/// <param name="travianData">Contains game info</param>
		/// <param name="sourceVillageID">Where the merchant starts, for computing distance</param>
		/// <returns>Estimated capacity</returns>
		private TResAmount GetTargetCapacity(Data travianData, int sourceVillageID)
		{
			if (travianData != null &&
				travianData.Villages.ContainsKey(this.TargetVillageID) &&
				travianData.Villages.ContainsKey(sourceVillageID))
			{
				TVillage source = travianData.Villages[sourceVillageID];
				TVillage destination = travianData.Villages[this.TargetVillageID];
				if (destination.isBuildingInitialized == 2)
				{
					TResource[] VR = destination.Resource;
					int[] resources = new int[VR.Length];
					int speed = travianData.MarketSpeed == 0 ? 24 : travianData.MarketSpeed;
					double timecost = source.Coord * destination.Coord / speed;
					for (int i = 0; i < resources.Length; i++)
					{
						resources[i] = VR[i].Capacity;
						if (destination.Market.UpperLimit != null)
						{
							resources[i] = destination.Market.UpperLimit.Resources[i];
						}

						resources[i] -= VR[i].CurrAmount + (int)(VR[i].Produce * timecost);
					}

					TResAmount capacity = new TResAmount(resources);
					foreach (TMInfo transfer in destination.Market.MarketInfo)
					{
						if (transfer.MType == TMType.OtherCome)
						{
							capacity -= transfer.CarryAmount;
						}
					}

					capacity.Normalize();
					return capacity;
				}
			}

			return null;
		}

		private void DoBalance(TResAmount targetAmount)
		{
			int total = this.ResourceAmount.TotalAmount();
			int slots = this.NoCrop ? 3 : 4;

			// Sort targetAmount by desc order
			int[] ranks = new int[] { 0, 1, 2, 3 };
			for (int i = 0; i < slots - 1; i++)
			{
				for (int j = i + 1; j < slots; j++)
				{
					if (targetAmount.Resources[ranks[i]] < targetAmount.Resources[ranks[j]])
					{
						int temp = ranks[i];
						ranks[i] = ranks[j];
						ranks[j] = temp;
					}
				}
			}

			// Allocate by rank
			this.ResourceAmount.Clear();
			for (int i = 1; i < slots; i++)
			{
				int inc = targetAmount.Resources[ranks[i - 1]] - targetAmount.Resources[ranks[i]];
				if (total < this.ResourceAmount.TotalAmount() + inc * i)
				{
					inc = (total - this.ResourceAmount.TotalAmount()) / i;
				}

				for (int j = 0; j < i; j++)
				{
					this.ResourceAmount.Resources[ranks[j]] += inc;
				}
			}

			// Allocate remaining resources and round up with unit of 50
			int bonus = (total - this.ResourceAmount.TotalAmount()) / slots;
			for (int i = 0; i < slots; i++)
			{
				this.ResourceAmount.Resources[i] += bonus;
				this.ResourceAmount.Resources[i] = (this.ResourceAmount.Resources[i] / 50) * 50;
			}

			// If we have anything left, give it to a lucky guy
			int luckyOne = ranks[0];
			this.ResourceAmount.Resources[luckyOne] += total - this.ResourceAmount.TotalAmount();
		}

		/// <summary>
		/// Encode option in a readable string that doesn't contain '|', ',', or ':'
		/// </summary>
		/// <returns>Encoded string</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0}&{1}&{2}", this.TargetVillageID, this.MaxCount, this.resumeTime.Ticks);
			sb.AppendFormat("&{0}&{1}", this.TargetPos.X, this.TargetPos.Y);
			for (int i = 0; i < 4; i++)
			{
				sb.AppendFormat("&{0}", this.ResourceAmount.Resources[i]);
			}

			sb.AppendFormat("&{0}&{1}&{2}", this.Distribution, this.NoCrop, this.Count);
			sb.AppendFormat("&{0}", this.MinimumInterval);
			return sb.ToString();
		}

		/// <summary>
		/// Decode the transfer option from an encoded string
		/// </summary>
		/// <param name="s">Encode data</param>
		/// <returns>Decoded data</returns>
		public static TransferOption FromString(string s)
		{
			string[] data = s.Split(new char[] { '&' });

			int index = 0;
			TransferOption result = new TransferOption();

			try
			{
				result.TargetVillageID = Int32.Parse(data[index++]);
				result.MaxCount = Int32.Parse(data[index++]);
				result.resumeTime = new DateTime(Int64.Parse(data[index++]));

				int x = Int32.Parse(data[index++]);
				int y = Int32.Parse(data[index++]);
				result.TargetPos = new TPoint(x, y);

				int[] amount = new int[4];
				for (int i = 0; i < 4; i++)
				{
					amount[i] = Int32.Parse(data[index++]);
				}

				result.ResourceAmount = new TResAmount(amount);


				result.Distribution = (ResourceDistributionType)Enum.Parse(typeof(ResourceDistributionType), data[index++]);
				result.NoCrop = Boolean.Parse(data[index++]);
				result.Count = Int32.Parse(data[index++]);
				result.MinimumInterval = Int32.Parse(data[index++]);
			}
			catch
			{
				// Parse failure happens after we add more options
			}

			return result;
		}
	}

	partial class Travian
	{
		private TResAmount JustTransferredData = null;


		/// <summary>
		/// Wrapper of the real doTransfer function, which
		/// 1) Verify that the tranfer amount is valid (non-zero)
		/// 2) Verify that merchant dispatched for the same mission has returned.
		/// 3) Recalculate the transfer amount for dynamic resource balance
		/// 4) Verify that the transfer won't overflow the arrival village's warehouse/granary
		/// 5) Update task status after a successful merchan dispatch
		/// 6) Remove the task from the village queue when it's no longer valid/needed
		/// </summary>
		/// <param name="VillageID">Departure village ID</param>
		/// <param name="Q">Transportation task</param>
		public void doTransfer(int VillageID, TQueue Q)
		{
			TransferOption option = TransferOption.FromString(Q.ExtraOptions);
			if (!option.IsValid)
			{
				DebugLog("Invalid transfer task discarded: " + option.ToString(), DebugLevel.W);
				this.RemoveQueuedTask(VillageID, Q);
				return;
			}

			if (option.MinimumDelay > 0)
			{
				return;
			}

			if (option.TargetVillageID != 0 &&
				TD.Villages.ContainsKey(option.TargetVillageID) &&
				TD.Villages[option.TargetVillageID].isBuildingInitialized == 2)
			{
				PageQuery(option.TargetVillageID, "dorf1.php");
				option.CalculateResourceAmount(TD, VillageID);
				if (option.ExceedTargetCapacity(TD, VillageID))
				{
					return;
				}
			}
			else
			{
				option.CalculateResourceAmount(TD, VillageID);
			}

			int timeCost = doTransfer(VillageID, option.ResourceAmount, option.TargetPos);
			if (timeCost >= 0)
			{
				var CV = TD.Villages[VillageID];
				option.MinimumDelay = Math.Max(option.MinimumInterval, timeCost * 2 + 10);
				option.Count++;
				if (option.MaxCount == 0 || option.Count < option.MaxCount)
				{
					Q.ExtraOptions = option.ToString();
					Q.Status = option.Status;
					CV.SaveQueue(userdb);
				}
				else
				{
					this.RemoveQueuedTask(VillageID, Q);
				}
			}
		}

		/// <summary>
		/// Dispatch a transportation of a given amount of resource from one village to a given destiantion
		/// </summary>
		/// <param name="VillageID">Unique ID of the departure village</param>
		/// <param name="Amount">Amounts of resources to transport</param>
		/// <param name="TargetPos">Position of the arrival village</param>
		/// <returns>Error return minus number. Succeed return single way transfer time cost.</returns>
		public int doTransfer(int VillageID, TResAmount Amount, TPoint TargetPos)
		{
			string result = PageQuery(VillageID, "build.php?gid=17");
			if (result == null)
				return -1;
			var CV = TD.Villages[VillageID];
			Dictionary<string, string> PostData = new Dictionary<string, string>();
			var m = Regex.Match(result, "name=\"id\" value=\"(\\d+)\"");
			if (!m.Success)
				return -1; // Parse error!
			PostData["id"] = m.Groups[1].Value;
			m = Regex.Match(result, "var haendler = (\\d+);");
			if (!m.Success)
				return -1;
			var MCount = Convert.ToInt32(m.Groups[1].Value);
			m = Regex.Match(result, "var carry = (\\d+);");
			if (!m.Success)
				return -1;
			var MCarry = Convert.ToInt32(m.Groups[1].Value);

			if (Amount.TotalAmount() > MCarry * MCount)
			{
				return -2; // Beyond transfer ability
			}

			for (int i = 0; i < 4; i++)
			{
				PostData["r" + (i + 1).ToString()] = Amount.Resources[i].ToString();
			}

			PostData["dname"] = "";
			PostData["x"] = TargetPos.X.ToString();
			PostData["y"] = TargetPos.Y.ToString();
			PostData["s1"] = "ok";

			result = PageQuery(VillageID, "build.php", PostData);

			if (result == null)
				return -1;
			m = Regex.Match(result, "name=\"sz\" value=\"(\\d+)\"");
			if (!m.Success)
				return -1; // Parse error!
			PostData["sz"] = m.Groups[1].Value;
			PostData["kid"] = TargetPos.Z.ToString();
			PostData["a"] = VillageID.ToString();
			m = Regex.Match(result, "<td>([0-9:]{6,})</td>");
			if (!m.Success)
				return -1; // Parse error!
			int TimeCost = Convert.ToInt32(TimeSpanParse(m.Groups[1].Value).TotalSeconds);
			if (TD.MarketSpeed != 0)
			{
				// calc market speed
				var distance = CV.Coord * TargetPos;
				TD.MarketSpeed = Convert.ToInt32(Math.Round(distance * 3600 / TimeCost));
				if (!svrdb.ContainsKey("MarketSpeedX"))
				{
					int StdSpeed;
					if (TD.Tribe == 1)
						StdSpeed = 16;
					else if (TD.Tribe == 2)
						StdSpeed = 12;
					else
						StdSpeed = 24;
					svrdb["MarketSpeedX"] = (TD.MarketSpeed / StdSpeed).ToString();
				}
			}
			JustTransferredData = Amount;
			result = PageQuery(VillageID, "build.php", PostData);

			// write data into target village if it's my village.
			foreach (var x in TD.Villages)
			{
				if (x.Value == CV)
					continue;
				if (x.Value.Coord == TargetPos)
				{
					if (x.Value.isBuildingInitialized == 2)
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
			DebugLog(string.Format("Transfer {0}({1}) => {2} {3}", CV.Coord.ToString(), VillageID, TargetPos.ToString(),
				Amount.ToString()), DebugLevel.I);
			return TimeCost;
		}

		/// <summary>
		/// Remove a item from the village task queue
		/// </summary>
		/// <param name="villageID">Which village the task queue belongs to</param>
		/// <param name="task">The item to delete from the task queue</param>
		private void RemoveQueuedTask (int villageID, TQueue task)
		{
			TVillage village = this.TD.Villages[villageID];
			int queueID = village.Queue.IndexOf(task);
			village.Queue.Remove(task);
			village.SaveQueue(userdb);
			StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Queue, VillageID = villageID, Param = queueID });
		}
	}
}
