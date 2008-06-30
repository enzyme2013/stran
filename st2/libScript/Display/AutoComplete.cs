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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace libScriptEngine
{
	public partial class AutoComplete : Form
	{
		public List<AutoWord> Words = new List<AutoWord>();
		public delegate void string_d(string data);
		public string_d backcall { get; set; }
		public AutoComplete()
		{
			InitializeComponent();
		}

		private void AutoComplete_Load(object sender, EventArgs e)
		{

		}
		public void PositionToWord(string Word)
		{
			int i = LookforWord(Word);
			if(i < listView1.Items.Count)
				listView1.Items[i].Selected = true;
			label1.Text = Word;
		}
		public void CompleteWord(string Word)
		{
			int i = LookforWord(Word);
			if(i < 0 && listView1.SelectedIndices.Count > 0)
				i = listView1.SelectedIndices[0];
			if(i < 0)
				return;
			if(i < Words.Count)
				backcall(Words[i].word);
		}
		private int LookforWord(string Word)
		{
			int index = Words.BinarySearch(new AutoWord() { word = Word }, new DC());
			if(index < 0)
				index = ~index;
			return index;
		}
		public void MoveToNextWord()
		{
			if(listView1.SelectedIndices.Count > 0 && listView1.SelectedIndices[0] < listView1.Items.Count - 1)
				listView1.Items[listView1.SelectedIndices[0] + 1].Selected = true;
		}
		public void MoveToPreviousWord()
		{
			if(listView1.SelectedIndices.Count > 0 && listView1.SelectedIndices[0] > 0)
				listView1.Items[listView1.SelectedIndices[0] - 1].Selected = true;
		}
		public void HideForm()
		{
			if(Focused || listView1.Focused)
				return;
			Visible = false;
		}
		public void InitList()
		{
			foreach(var x in Words)
			{
				var lvi = listView1.Items.Add(x.word);
				lvi.ImageIndex = (int)x.type;
			}
		}
	}
	public class DC : IComparer<AutoWord>
	{
		public int Compare(AutoWord x, AutoWord y)
		{
			return string.Compare(x.word, y.word, true);
		}
	}

}
