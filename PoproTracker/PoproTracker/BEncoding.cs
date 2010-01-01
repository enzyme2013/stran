using System;
using System.Collections.Generic;
using System.Text;

namespace PoproTracker
{
	public interface IBE
	{
		string Dump();
	}

	public class BEString : IBE
	{
		string Value;
		public static implicit operator string(BEString s)
		{
			return s.Value;
		}
		public static implicit operator BEString(string s)
		{
			return new BEString { Value = s };
		}

		#region IBE 成员

		public string Dump()
		{
			return Encoding.UTF8.GetByteCount(Value) + ":" + Value;
		}

		#endregion
	}

	public class BENumber : IBE
	{
		long Value;
		public static implicit operator long(BENumber s)
		{
			return s.Value;
		}
		public static implicit operator BENumber(long s)
		{
			return new BENumber { Value = s };
		}

		#region IBE 成员

		public string Dump()
		{
			return "i" + Value.ToString() + "e";
		}

		#endregion
	}

	public class BEList : IBE
	{
		List<IBE> Value;
		public static implicit operator List<IBE>(BEList s)
		{
			return s.Value;
		}
		public static implicit operator BEList(List<IBE> s)
		{
			return new BEList { Value = s };
		}

		#region IBE 成员

		public string Dump()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("l");
			foreach (var V in Value)
				sb.Append(V.Dump());
			sb.Append("e");
			return sb.ToString();
		}

		#endregion
	}

	public class BEDict : IBE
	{
		Dictionary<BEString, IBE> Value;
		public static implicit operator Dictionary<BEString, IBE>(BEDict s)
		{
			return s.Value;
		}
		public static implicit operator BEDict(Dictionary<BEString, IBE> s)
		{
			return new BEDict { Value = s };
		}

		#region IBE 成员

		public string Dump()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("d");
			foreach (var V in Value)
			{
				sb.Append(V.Key.Dump());
				sb.Append(V.Value.Dump());
			}
			sb.Append("e");
			return sb.ToString();
		}

		#endregion
	}
}
