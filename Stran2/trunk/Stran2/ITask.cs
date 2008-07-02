using System;
using System.Collections.Generic;
using System.Text;

namespace Stran2
{
	public interface ITask
	{
		/// <summary>
		/// Never execute this task before NextExec
		/// </summary>
		DateTime NextExec { get; set; }

		/// <summary>
		/// Village ID related to this task
		/// </summary>
		int VillageId { get; set; }

		/// <summary>
		/// Indicate if this task should be executed
		/// </summary>
		bool IsEnabled { get; set; }
	}

	public interface IOption
	{
		/// <summary>
		/// Convert to description
		/// </summary>
		/// <returns></returns>
		string ToString();
	}
}
