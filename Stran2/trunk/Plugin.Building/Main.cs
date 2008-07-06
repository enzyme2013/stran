using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	public class BuildingPlugin : IPlugin
	{
		#region IPlugin 成员

		public void Initialize()
		{
			MethodsCenter.Instance.RegisterParser(new ParserPluginCall(Dorf1));
			MethodsCenter.Instance.RegisterParser(new ParserPluginCall(Dorf2));
			MethodsCenter.Instance.RegisterMethod(GetType().GetMethod("GetBidLevel"));
		}

		public bool CheckDepend()
		{
			return true;
		}

		#endregion

		public void Dorf1(string pageData, UserData users, int villageId)
		{
		}

		public void Dorf2(string pageData, UserData users, int villageId)
		{
		}

		public int GetBidLevel(int Bid)
		{
			return 0;
		}

	}

	public class BuildTaskOption : ITaskOption
	{
		#region ITaskOption 成员

		public string ToDescription()
		{
			return string.Format("BId: {0}, GId: {1}, Level: {2}",
				BId, GId, TargetLevel);
		}

		public string Serialization()
		{
			throw new NotImplementedException();
		}

		public bool TryParse(string SerializedOptionString, ref ITaskOption Option)
		{
			if(!SerializedOptionString.StartsWith("-build-"))
				return false;
			throw new NotImplementedException();
		}

		#endregion

		public int BId { get; set; }
		public int GId { get; set; }

		/// <summary>
		/// Delete me if TargetLevel >= CurrentLevel after executing the task
		/// </summary>
		public int TargetLevel { get; set; }


	}

}
