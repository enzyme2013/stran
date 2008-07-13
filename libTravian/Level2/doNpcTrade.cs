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
	/// Stores settings for an NPC trade task
	/// </summary>
	public class NpcTradeOption
	{
		/// <summary>
		/// When can we retry the NPC trade task
		/// </summary>
		private DateTime resumeTime;

		/// <summary>
		/// Start NPC trading after resources has exceeded all thresholds, e.g.,
		/// 100|100|100|1800 exceeds 0|0|0|1000, but not 10|10|10|1000
		/// </summary>
		public TResAmount Threshold { get; set; }

		/// <summary>
		/// Target resource distribution ratio
		/// </summary>
		public TResAmount Distribution { get; set; }

		/// <summary>
		/// Maximum number of repeats (0 = inf)
		/// </summary>
		public int MaxCount { get; set; }

		/// <summary>
		/// Current number of repeats
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// A percentage p: an NPC trade won't happen unless at least p%
		/// of resource has been transfered from one type to other type(s)
		/// </summary>
		public int MinTradeRatio { get; set; }

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
		/// Task title for queue display (threashold->distribution)
		/// </summary>
		public string Title
		{
			get 
			{ 
				return String.Format("{0}=>{1}%", this.Threshold, 	this.MinTradeRatio); 
			}
		}

		/// <summary>
		/// Task status for queue display (counts?)
		/// </summary>
		public string Status
		{
			get 
			{
				string maxCount = this.MaxCount == 0 ? "inf" : this.MaxCount.ToString();
				return String.Format("{0}/{1}=>{2}", this.Count, maxCount, this.Distribution); 
			}
		}

		/// <summary>
		/// A valid task should have non-zero thresholds and non-zero distribution
		/// </summary>
		public bool IsValid
		{
			get { return this.Threshold.TotalAmount > 0 && this.Distribution.TotalAmount > 0; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public NpcTradeOption()
		{
			this.Threshold = new TResAmount();
			this.Distribution = new TResAmount();
			this.MaxCount = 1;
			this.MinTradeRatio = 50;
		}

		/// <summary>
		/// Decode the NPC trade option from an encoded string
		/// </summary>
		/// <param name="s">Encode data</param>
		/// <returns>Decoded data</returns>
		public static NpcTradeOption FromString(string s)
		{
			string[] data = s.Split(new char[] { '&' });

			int index = 0;
			NpcTradeOption result = new NpcTradeOption();

			try
			{
				result.resumeTime = new DateTime(Int64.Parse(data[index++]));

				int[] amount = new int[4];
				for (int i = 0; i < 4; i++)
				{
					amount[i] = Int32.Parse(data[index++]);
				}

				result.Threshold = new TResAmount(amount);

				amount = new int[4];
				for (int i = 0; i < 4; i++)
				{
					amount[i] = Int32.Parse(data[index++]);
				}

				result.Distribution = new TResAmount(amount);

				result.Count = Int32.Parse(data[index++]);
				result.MaxCount = Int32.Parse(data[index++]);
				result.MinTradeRatio = Int32.Parse(data[index++]);
			}
			catch
			{
				// Parse failure happens after we add more options
			}

			return result;
		}

		/// <summary>
		/// Encode option in a readable string that doesn't contain '|', ',', or ':'
		/// </summary>
		/// <returns>Encoded string</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}", this.resumeTime.Ticks);
			sb.AppendFormat("&{0}", this.Threshold.ToString().Replace('|', '&'));
			sb.AppendFormat("&{0}", this.Distribution.ToString().Replace('|', '&'));
			sb.AppendFormat("&{0}&{1}&{2}", this.Count, this.MaxCount, this.MinTradeRatio);

			return sb.ToString();
		}

		/// <summary>
		/// Return the minimum delay before the NPC trade could start
		/// </summary>
		/// <param name="travianData">User data, including the village info</param>
		/// <param name="villageID">village ID</param>
		/// <returns>Minimum delay in seconds</returns>
		public int GetDelay(Data travianData, int villageID)
		{
			if (!travianData.Villages.ContainsKey(villageID))
			{
				return 86400;
			}

			TVillage village = travianData.Villages[villageID];
			if (village.isBuildingInitialized != 2)
			{
				return 86400;
			}

			int sum = 0;
			for (int i = 0; i < village.Resource.Length; i++)
			{
				sum += village.Resource[i].CurrAmount;
			}

			if (this.RedistributeResources(travianData, villageID, sum) == null)
			{
				return 86400;
			}

			int timecost = Math.Max(this.MinimumDelay, village.TimeCost(this.Threshold));

			return timecost;
		}

		/// <summary>
		/// Distribute a total amount based on the distribution ratio
		/// </summary>
		public TResAmount RedistributeResources(Data travianData, int villageID, int sum)
		{
			if (!travianData.Villages.ContainsKey(villageID))
			{
				return null;
			}

			TVillage village = travianData.Villages[villageID];
			if (village.isBuildingInitialized != 2)
			{
				return null;
			}

			TResAmount target = new TResAmount(this.Distribution);
			TResAmount distribution = new TResAmount();

			// Allocate by proportion
			int residual = sum - distribution.TotalAmount;
			while (target.TotalAmount > 0 && residual > 10)
			{
				double[] proportions = target.Proportions;
				for (int i = 0; i < proportions.Length; i++)
				{
					distribution.Resources[i] += (int)(residual * proportions[i]);
					if (distribution.Resources[i] > village.Resource[i].Capacity)
					{
						distribution.Resources[i] = village.Resource[i].Capacity;
						target.Resources[i] = 0;
					}
				}

				residual = sum - distribution.TotalAmount;
			}

			// Don't trade if residual exceeds 50% threshold
			bool tooManyResidual = true;
			for (int i = 0; i < this.Threshold.Resources.Length; i++)
			{
				if (residual < this.Threshold.Resources[i] * (100 - this.MinTradeRatio) / 100)
				{
					tooManyResidual = false;
					break;
				}
			}

			if (tooManyResidual)
			{
				return null;
			}

			// Allocate by capacity
			for (int i = 0; i < distribution.Resources.Length; i++)
			{
				distribution.Resources[i] = Math.Min(
					distribution.Resources[i] + residual,
					village.Resource[i].Capacity);
				residual = sum - distribution.TotalAmount;
			}

			return distribution;
		}
	}

	partial class Travian
	{
		/// <summary>
		/// Return value of doNpcTrade(int, TResAmount, TResAmount)
		/// </summary>
		public enum NpcTradeResult
		{
			/// <summary>
			/// The trade is complete
			/// </summary>
			Success,

			/// <summary>
			/// A parsing error has happened
			/// </summary>
			Failure,

			/// <summary>
			/// Some trading condition doesn't apply
			/// </summary>
			Delay
		}


		/// <summary>
		/// doNcpTrade wrapper
		/// </summary>
		public void doNpcTrade(int villageID, TQueue Q)
		{
			TVillage village = this.TD.Villages[villageID];
			NpcTradeOption option = NpcTradeOption.FromString(Q.ExtraOptions);

			if (!option.IsValid)
			{
				DebugLog("Invalid NPC trade task discarded: " + option.ToString(), DebugLevel.W);
				this.RemoveQueuedTask(villageID, Q);
				return;
			}

			if (option.GetDelay(this.TD, villageID) > 0)
			{
				return;
			}

			NpcTradeResult returnCode = doNpcTrade(villageID, option);
			switch (returnCode)
			{
				case NpcTradeResult.Failure:
					DebugLog("NPC trade task has failed: " + option.ToString(), DebugLevel.W);
					this.RemoveQueuedTask(villageID, Q);
					return;

				case NpcTradeResult.Delay:
					// Wait at least 10 minutes before retrying
					option.MinimumDelay = 600;
					break;

				case NpcTradeResult.Success:
					option.Count++;
					if (option.MaxCount != 0 & option.Count >= option.MaxCount)
					{
						this.RemoveQueuedTask(villageID, Q);
						return;
					}

					// This is an unfinished multiple NPC trade task, wait at least 1 hr
					option.MinimumDelay = 3600;
					break;
			}

			Q.ExtraOptions = option.ToString();
			Q.Status = option.Status;
			village.SaveQueue(userdb);
		}

		/// <summary>
		/// Trade resource with NPC with 1:1 rate (after paying 3 gold)
		/// </summary>
		public NpcTradeResult doNpcTrade(int villageID, NpcTradeOption option)
		{
			// Get NPC trade form
			string result = PageQuery(villageID, "build.php?gid=17&t=3");
			if (result == null)
			{
				return NpcTradeResult.Failure;
			}

			// Parse capacity and sum
			Match match = Regex.Match(result, "var summe=(?<summe>\\d+);var max123=(?<max123>\\d+);var max4=(?<max4>\\d+);");
			if (!match.Success)
			{
				return NpcTradeResult.Failure;
			}

			int sum = Int32.Parse(match.Groups["summe"].Value);

			// Parse id
			match = Regex.Match(result, "\\<input type=\"hidden\" name=\"id\" value=\"(?<id>\\d+)\"\\>");
			if (!match.Success)
			{
				return NpcTradeResult.Failure;
			}

			string id = match.Groups["id"].Value;

			// Parse m1[] and m2[]
			MatchCollection matches = Regex.Matches(result, "\\<input type=\"hidden\" name=\"m1\\[\\]\" value=(?<m1>\\d+)\\>");
			if (matches.Count != 4)
			{
				return NpcTradeResult.Failure;
			}

			TResAmount m1 = new TResAmount(new int[matches.Count]);
			for (int i = 0; i < matches.Count; i ++)
			{
				m1.Resources[i] = Int32.Parse(matches[i].Groups["m1"].Value);
			}

			// Does m1 exceeds threshold?
			for (int i = 0; i < m1.Resources.Length; i ++)
			{
				if (m1.Resources[i] < option.Threshold.Resources[i])
				{
					return NpcTradeResult.Delay;
				}
			}

			// Compute m2
			TResAmount m2 = option.RedistributeResources(this.TD, villageID, sum);
			if (m2 == null)
			{
				return NpcTradeResult.Delay;
			}

			// Prepare data
			Dictionary<string, string> postData = new Dictionary<string, string>();
			postData["id"] = id;
			postData["t"] = "3";
			postData["a"] = "6";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < m2.Resources.Length; i++)
			{
				if (i > 0)
				{
					sb.Append("&");
				}

				sb.AppendFormat("m2[]={0}&m1[]={1}", m2.Resources[i], m1.Resources[i]);
			}

			postData["!!!RawData!!!"] = sb.ToString();

			// Post form
			result = this.PageQuery(villageID, "build.php", postData);
			if (result == null)
			{
				return NpcTradeResult.Failure;
			}

			match = Regex.Match(result, "\\<b\\>3\\</b\\>[^\\<\\>]*\\</p\\>\\<script language=\"JavaScript\">var summe=");
			if (!match.Success)
			{
				return NpcTradeResult.Failure;
			}

			DebugLog(string.Format("NPC trade {0} -> {1} ({2}) ", m1, m2, villageID), DebugLevel.I);
			return NpcTradeResult.Success;
		}
	}
}
