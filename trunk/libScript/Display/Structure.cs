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
 * 
 * This code is modified from [SyntaxHighlightingArtice] from codeproject.
 * http://www.codeproject.com/KB/edit/SyntaxHighlighting.aspx By [Uri Guy]
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace libScriptEngine
{
	public class HighlightDescriptor
	{
		public HighlightDescriptor(string token, DescriptorType dt, WordType wt)
		{
			if(dt == DescriptorType.ToCloseToken)
			{
				throw new ArgumentException("You may not choose ToCloseToken DescriptorType without specifing an end token.");
			}
			CloseToken = null;
			Token = token;
			DT = dt;
			WT = wt;
		}

		public HighlightDescriptor(string token, string closeToken, DescriptorType dt, WordType wt)
		{
			CloseToken = closeToken;
			Token = token;
			DT = dt;
			WT = wt;
		}

		public readonly string Token;
		public readonly string CloseToken;
		public readonly DescriptorType DT;
		public readonly WordType WT;
	}


	public enum DescriptorType
	{
		/// <summary>
		/// Causes the highlighting of a single word
		/// </summary>
		Word,
		/// <summary>
		/// Causes the entire line from this point on the be highlighted, regardless of other tokens
		/// </summary>
		ToEOL,
		/// <summary>
		/// Highlights all text until the end token;
		/// </summary>
		ToCloseToken
	}
	/*
	public enum DescriptorRecognition
	{
		/// <summary>
		/// Only if the whole token is equal to the word
		/// </summary>
		WholeWord,
		/// <summary>
		/// If the word starts with the token
		/// </summary>
		StartsWith,
		/// <summary>
		/// If the word contains the Token
		/// </summary>
		Contains
	}
	*/
	public enum WordType
	{
		KeyWords, BuildInFunctions, Type, Comment, String
	}

	public class WordStyle
	{
		public Color Color;
		public Font Font;
	}

	public struct AutoWord
	{
		public string word { get; set; }
		public WordType type { get; set; }
		public override string ToString()
		{
			return word;
		}
	}
	
}
