using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace libTravian
{
	public interface IQueue
	{
		/// <summary>
		/// UpCall for callback
		/// </summary>
		Travian UpCall { get; set; }

		/// <summary>
		/// Village ID
		/// </summary>
		[Json]
		int VillageID { get; set; }

		/// <summary>
		/// If this queue should be delete
		/// </summary>
		bool MarkDeleted { get; }

		/// <summary>
		/// When set to true, the corresponding task will be suspended
		/// </summary>
		[Json]
		bool Paused { get; set; }

		/// <summary>
		/// Brief information of the task
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Detail information of the task
		/// </summary>
		string Status { get; }

		/// <summary>
		/// Decode options from an encoded string
		/// </summary>
		/// <param name="s">Encode data</param>
		/// <returns>Decoded data</returns>
		void Import(string s);

		/// <summary>
		/// Encode option in a readable string that doesn't contain ',' or ':'
		/// </summary>
		/// <returns>Encoded string</returns>
		string Export();

		/// <summary>
		/// Seconds countdown to do the action
		/// </summary>
		int CountDown { get; }

		/// <summary>
		/// Do the action
		/// </summary>
		/// <returns></returns>
		void Action();

		/// <summary>
		/// Warp the Import/Export function
		/// </summary>
		//[Json]
		//string IO { get; set; }
	}
}
