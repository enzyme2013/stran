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
		/// Same as the key of the travian data dictionary
		/// {$Username}@{$Server}
		/// </summary>
		string UserKey { get; set; }

		/// <summary>
		/// Village ID related to this task
		/// </summary>
		int VillageId { get; set; }

		/// <summary>
		/// Indicate if this task should be executed
		/// </summary>
		bool IsEnabled { get; set; }

		/// <summary>
		/// Options related to the task
		/// </summary>
		ITaskOption TaskOption { get; set; }
	}

	public interface ITaskOption
	{
		/// <summary>
		/// Convert to human-readable description
		/// </summary>
		/// <returns></returns>
		string ToDescription();

		/// <summary>
		/// Convert to machine-readable format to save into file
		/// </summary>
		/// <returns></returns>
		string Serialization();

		/// <summary>
		/// Try to parse serialized task options and
		/// return a re-constructed option class
		/// </summary>
		/// <returns></returns>
		bool TryParse(string SerializedOptionString, ref ITaskOption Option);
	}

	public class TaskOptionCenter
	{
		private TaskOptionCenter()
		{
			TaskOptioners = new List<ITaskOption>();
		}
		public static readonly TaskOptionCenter Instance = new TaskOptionCenter();
		public List<ITaskOption> TaskOptioners { get; private set; }
	}
}
