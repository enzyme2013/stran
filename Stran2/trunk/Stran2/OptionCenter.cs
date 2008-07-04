using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	/// <summary>
	/// Singleton Pattern, for options maintaince
	/// </summary>
	class OptionCenter
	{
		private Dictionary<string, string> options;
		private OptionCenter()
		{
			options = new Dictionary<string, string>();
		}
		public static readonly OptionCenter Instance = new OptionCenter();
		public void LoadOptions()
		{
		}
		public void SaveOptions()
		{
		}
		public string GetValue(string OptionName, string DefaultValue)
		{
			if(options.ContainsKey(OptionName))
				return options[OptionName];
			else
				return DefaultValue;
		}
		public int GetValue(string OptionName, int DefaultValue)
		{
			int o;
			if(options.ContainsKey(OptionName) && int.TryParse(options[OptionName], out o))
				return o;
			else
				return DefaultValue;
		}
	}
}
