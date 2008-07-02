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
	public class TravianDataCenter : IDataType
	{
		#region IDataType 成员
		public Dictionary<string, string> StringProperties { get; set; }
		public Dictionary<string, int> Int32Properties { get; set; }
		#endregion
		public Dictionary<string, UserData> Users { get; set; }
		public TravianDataCenter()
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
		public Dictionary<string, VillageData> Villages { get; set; }
		public UserData()
		{
			StringProperties = new Dictionary<string, string>();
			Int32Properties = new Dictionary<string, int>();
			Villages = new Dictionary<string, VillageData>();
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
