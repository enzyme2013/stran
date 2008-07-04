using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	public interface IDataType
	{
		Dictionary<string, string> StringProperties { get; set; }
		Dictionary<string, int> Int32Properties { get; set; }
	}
	/// <summary>
	/// Singleton Pattern, for travian overall data maintaince
	/// </summary>
	public class TravianDataCenter : IDataType
	{
		public static readonly TravianDataCenter Instance = new TravianDataCenter();

		#region IDataType 成员
		public Dictionary<string, string> StringProperties { get; set; }
		public Dictionary<string, int> Int32Properties { get; set; }
		#endregion
		public Dictionary<string, UserData> Users { get; set; }
		private TravianDataCenter()
		{
			Users = new Dictionary<string, UserData>();
		}
	}
	public class UserData : IDataType
	{
		#region IDataType 成员
		public Dictionary<string, string> StringProperties { get; set; }
		public Dictionary<string, int> Int32Properties { get; set; }
		#endregion
		public Dictionary<int, VillageData> Villages { get; set; }
		public UserData()
		{
			StringProperties = new Dictionary<string, string>();
			Int32Properties = new Dictionary<string, int>();
			Villages = new Dictionary<int, VillageData>();
		}
	}
	public class VillageData : IDataType
	{
		#region IDataType 成员
		public Dictionary<string, string> StringProperties { get; set; }
		public Dictionary<string, int> Int32Properties { get; set; }
		#endregion
		public VillageData()
		{
			StringProperties = new Dictionary<string, string>();
			Int32Properties = new Dictionary<string, int>();
		}
	}
}
