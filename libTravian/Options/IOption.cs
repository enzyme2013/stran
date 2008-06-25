using System;
using System.Collections.Generic;
using System.Text;

namespace libTravian
{
	public interface IOption
	{
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
	}
}
