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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace libScriptEngine
{
	public partial class SyntaxHighlighting : RichTextBox
	{
		[Category("Custom")]
		[DefaultValue(50)]
		public int MaxUndoRedoSteps { get; set; }
		[Category("Custom")]
		[DefaultValue(true)]
		public bool CaseSensitive { get; set; }
		//Internal use members
		private bool mParsing = false;

		public List<char> Seperators = new List<char>();
		public List<HighlightDescriptor> HighlightDescriptors = new List<HighlightDescriptor>();
		private AutoComplete ac;
		private bool isShown
		{
			get
			{
				return ac != null && ac.Visible;
			}
		}

		#region Undo/Redo Code
		private List<UndoRedoInfo> mUndoList = new List<UndoRedoInfo>();
		private Stack<UndoRedoInfo> mRedoStack = new Stack<UndoRedoInfo>();
		private bool mIsUndo = false;
		private UndoRedoInfo mLastInfo = new UndoRedoInfo("", new Win32.POINT(), 0);
		private int mMaxUndoRedoSteps = 50;
		public new bool CanUndo { get { return mUndoList.Count > 0; } }
		public new bool CanRedo { get { return mRedoStack.Count > 0; } }

		private void LimitUndo()
		{
			while(mUndoList.Count > mMaxUndoRedoSteps)
				mUndoList.RemoveAt(mMaxUndoRedoSteps);
		}

		public new void Undo()
		{
			if(!CanUndo)
				return;
			mIsUndo = true;
			mRedoStack.Push(new UndoRedoInfo(Text, GetScrollPos(), SelectionStart));
			UndoRedoInfo info = mUndoList[0];
			mUndoList.RemoveAt(0);
			Text = info.Text;
			SelectionStart = info.CursorLocation;
			SetScrollPos(info.ScrollPos);
			mLastInfo = info;
			mIsUndo = false;
		}
		public new void Redo()
		{
			if(!CanRedo)
				return;
			mIsUndo = true;
			mUndoList.Insert(0, new UndoRedoInfo(Text, GetScrollPos(), SelectionStart));
			LimitUndo();
			UndoRedoInfo info = mRedoStack.Pop();
			Text = info.Text;
			SelectionStart = info.CursorLocation;
			SetScrollPos(info.ScrollPos);
			mIsUndo = false;
		}
		private class UndoRedoInfo
		{
			public UndoRedoInfo(string text, Win32.POINT scrollPos, int cursorLoc)
			{
				Text = text;
				ScrollPos = scrollPos;
				CursorLocation = cursorLoc;
			}
			public readonly Win32.POINT ScrollPos;
			public readonly int CursorLocation;
			public readonly string Text;
		}
		#endregion

		#region Rtf building helper functions

		/// <summary>
		/// Set color and font to default control settings.
		/// </summary>
		/// <param name="sb">the string builder building the RTF</param>
		/// <param name="colors">colors hashtable</param>
		/// <param name="fonts">fonts hashtable</param>
		private void SetDefaultSettings(StringBuilder sb, Dictionary<Color, int> colors, Dictionary<string, int> fonts)
		{
			SetColor(sb, ForeColor, colors);
			SetFont(sb, Font, fonts);
			SetFontSize(sb, (int)Font.Size);
			EndTags(sb);
		}

		/// <summary>
		/// Set Color and font to a highlight descriptor settings.
		/// </summary>
		/// <param name="sb">the string builder building the RTF</param>
		/// <param name="hd">the HighlightDescriptor with the font and color settings to apply.</param>
		/// <param name="colors">colors hashtable</param>
		/// <param name="fonts">fonts hashtable</param>
		private void SetDescriptorSettings(StringBuilder sb, HighlightDescriptor hd, Dictionary<Color, int> colors, Dictionary<string, int> fonts)
		{
			SetColor(sb, WordStyles[hd.WT].Color, colors);
			if(WordStyles[hd.WT].Font != null)
			{
				SetFont(sb, WordStyles[hd.WT].Font, fonts);
				SetFontSize(sb, (int)WordStyles[hd.WT].Font.Size);
			}
			EndTags(sb);

		}
		/// <summary>
		/// Sets the color to the specified color
		/// </summary>
		private void SetColor(StringBuilder sb, Color color, Dictionary<Color, int> colors)
		{
			sb.Append(@"\cf").Append(colors[color]);
		}
		/// <summary>
		/// Sets the backgroung color to the specified color.
		/// </summary>
		private void SetBackColor(StringBuilder sb, Color color, Dictionary<Color, int> colors)
		{
			sb.Append(@"\cb").Append(colors[color]);
		}
		/// <summary>
		/// Sets the font to the specified font.
		/// </summary>
		private void SetFont(StringBuilder sb, Font font, Dictionary<string, int> fonts)
		{
			if(font == null)
				return;
			sb.Append(@"\f").Append(fonts[font.Name]);
		}
		/// <summary>
		/// Sets the font size to the specified font size.
		/// </summary>
		private void SetFontSize(StringBuilder sb, int size)
		{
			sb.Append(@"\fs").Append(size * 2);
		}
		/// <summary>
		/// Adds a newLine mark to the RTF.
		/// </summary>
		private void AddNewLine(StringBuilder sb)
		{
			sb.Append("\\par\n");
		}

		/// <summary>
		/// Ends a RTF tags section.
		/// </summary>
		private void EndTags(StringBuilder sb)
		{
			sb.Append(' ');
		}

		/// <summary>
		/// Adds a font to the RTF's font table and to the fonts hashtable.
		/// </summary>
		/// <param name="sb">The RTF's string builder</param>
		/// <param name="font">the Font to add</param>
		/// <param name="counter">a counter, containing the amount of fonts in the table</param>
		/// <param name="fonts">an hashtable. the key is the font's name. the value is it's index in the table</param>
		private void AddFontToTable(StringBuilder sb, Font font, ref int counter, Dictionary<string, int> fonts)
		{

			sb.Append(@"\f").Append(counter).Append(@"\fnil\fcharset0").Append(font.Name).Append(";}");
			fonts.Add(font.Name, counter++);
		}

		/// <summary>
		/// Adds a color to the RTF's color table and to the colors hashtable.
		/// </summary>
		/// <param name="sb">The RTF's string builder</param>
		/// <param name="color">the color to add</param>
		/// <param name="counter">a counter, containing the amount of colors in the table</param>
		/// <param name="colors">an hashtable. the key is the color. the value is it's index in the table</param>
		private void AddColorToTable(StringBuilder sb, Color color, ref int counter, Dictionary<Color, int> colors)
		{

			sb.Append(@"\red").Append(color.R).Append(@"\green").Append(color.G).Append(@"\blue")
				.Append(color.B).Append(";");
			colors.Add(color, counter++);
		}

		#endregion

		#region Overriden methods

		/// <summary>
		/// The on text changed overrided. Here we parse the text into RTF for the highlighting.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged(EventArgs e)
		{
			if(mParsing)
				return;
			mParsing = true;
			Win32.LockWindowUpdate(Handle);
			base.OnTextChanged(e);

			if(!mIsUndo)
			{
				mRedoStack.Clear();
				mUndoList.Insert(0, mLastInfo);
				this.LimitUndo();
				mLastInfo = new UndoRedoInfo(Text, GetScrollPos(), SelectionStart);
			}

			//Save scroll bar an cursor position, changeing the RTF moves the cursor and scrollbars to top positin
			Win32.POINT scrollPos = GetScrollPos();
			int cursorLoc = SelectionStart;

			//Created with an estimate of how big the stringbuilder has to be...
			StringBuilder sb = new
				StringBuilder((int)(Text.Length * 1.5 + 150));

			//Adding RTF header
			sb.Append(@"{\rtf1\fbidis\ansi\ansicpg1255\deff0\deflang1037{\fonttbl{");

			//Font table creation
			int fontCounter = 0;
			Dictionary<string, int> fonts = new Dictionary<string, int>();
			AddFontToTable(sb, Font, ref fontCounter, fonts);
			foreach(HighlightDescriptor hd in HighlightDescriptors)
			{
				if((WordStyles[hd.WT].Font != null) && !fonts.ContainsKey(WordStyles[hd.WT].Font.Name))
				{
					AddFontToTable(sb, WordStyles[hd.WT].Font, ref fontCounter, fonts);
				}
			}
			sb.Append("}\n");

			//ColorTable

			sb.Append(@"{\colortbl ;");
			Dictionary<Color, int> colors = new Dictionary<Color, int>();
			int colorCounter = 1;
			AddColorToTable(sb, ForeColor, ref colorCounter, colors);
			AddColorToTable(sb, BackColor, ref colorCounter, colors);

			foreach(HighlightDescriptor hd in HighlightDescriptors)
			{
				if(!colors.ContainsKey(WordStyles[hd.WT].Color))
				{
					AddColorToTable(sb, WordStyles[hd.WT].Color, ref colorCounter, colors);
				}
			}

			//Parsing text

			sb.Append("}\n").Append(@"\viewkind4\uc1\pard\ltrpar");
			SetDefaultSettings(sb, colors, fonts);

			char[] sperators = Seperators.ToArray();

			//Replacing "\" to "\\" for RTF...
			string[] lines = Text.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}").Split('\n');
			for(int lineCounter = 0; lineCounter < lines.Length; lineCounter++)
			{
				if(lineCounter != 0)
				{
					AddNewLine(sb);
				}
				string line = lines[lineCounter];
				string[] tokens = CaseSensitive ? line.Split(sperators) : line.ToUpper().Split(sperators);
				if(tokens.Length == 0)
				{
					sb.Append(line);
					AddNewLine(sb);
					continue;
				}

				int tokenCounter = 0;
				for(int i = 0; i < line.Length; )
				{
					char curChar = line[i];
					if(Seperators.Contains(curChar))
					{
						sb.Append(curChar);
						i++;
					}
					else
					{
						string curToken = tokens[tokenCounter++];
						bool bAddToken = true;
						foreach(HighlightDescriptor hd in HighlightDescriptors)
						{
							string compareStr = CaseSensitive ? hd.Token : hd.Token.ToUpper();
							bool match = false;

							//Check if the highlight descriptor matches the current toker according to the DescriptoRecognision property.
							//switch(hd.DescriptorRecognition)
							//{
							//	case DescriptorRecognition.WholeWord:
							//		if(curToken == compareStr)
							//		{
							//			match = true;
							//		}
							//		break;
							//	case DescriptorRecognition.StartsWith:
							//		if(curToken.StartsWith(compareStr))
							//		{
							//			match = true;
							//		}
							//		break;
							//	case DescriptorRecognition.Contains:
							//		if(curToken.IndexOf(compareStr) != -1)
							//		{
							//			match = true;
							//		}
							//		break;
							//}
							match = curToken == compareStr;
							if(!match)
							{
								//If this token doesn't match chech the next one.
								continue;
							}

							//printing this token will be handled by the inner code, don't apply default settings...
							bAddToken = false;

							//Set colors to current descriptor settings.
							SetDescriptorSettings(sb, hd, colors, fonts);

							//Print text affected by this descriptor.
							switch(hd.DT)
							{
								case DescriptorType.Word:
									sb.Append(line.Substring(i, curToken.Length));
									SetDefaultSettings(sb, colors, fonts);
									i += curToken.Length;
									break;
								case DescriptorType.ToEOL:
									sb.Append(line.Remove(0, i));
									i = line.Length;
									SetDefaultSettings(sb, colors, fonts);
									break;
								case DescriptorType.ToCloseToken:
									while((line.IndexOf(hd.CloseToken, i) == -1) && (lineCounter < lines.Length))
									{
										sb.Append(line.Remove(0, i));
										lineCounter++;
										if(lineCounter < lines.Length)
										{
											AddNewLine(sb);
											line = lines[lineCounter];
											i = 0;
										}
										else
										{
											i = line.Length;
										}
									}
									if(line.IndexOf(hd.CloseToken, i) != -1)
									{
										sb.Append(line.Substring(i, line.IndexOf(hd.CloseToken, i) + hd.CloseToken.Length - i));
										line = line.Remove(0, line.IndexOf(hd.CloseToken, i) + hd.CloseToken.Length);
										tokenCounter = 0;
										tokens = CaseSensitive ? line.Split(sperators) : line.ToUpper().Split(sperators);
										SetDefaultSettings(sb, colors, fonts);
										i = 0;
									}
									break;
							}
							break;
						}
						if(bAddToken)
						{
							//Print text with default settings...
							sb.Append(line.Substring(i, curToken.Length));
							i += curToken.Length;
						}
					}
				}
			}

			//			System.Diagnostics.Debug.WriteLine(sb.ToString());
			Rtf = sb.ToString();
			int[] tabstops = new int[32];
			tabstops[0] = 16;
			for(int i = 1; i < 32; i++)
				tabstops[i] = tabstops[0] * i;
			SelectAll();
			SelectionTabs = tabstops;
			SelectionLength = 0;


			//Restore cursor and scrollbars location.
			SelectionStart = cursorLoc;

			mParsing = false;

			SetScrollPos(scrollPos);
			Win32.LockWindowUpdate((IntPtr)0);
			Invalidate();

			if(isShown)
			{
				SetAutoCompleteLocation(false);
				SetBestSelectedAutoCompleteItem();
			}
		}


		protected override void OnVScroll(EventArgs e)
		{
			if(mParsing)
				return;
			base.OnVScroll(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(ac != null)
				ac.HideForm();
			base.OnMouseDown(e);
		}

		/// <summary>
		/// Taking care of Keyboard events
		/// </summary>
		/// <param name="m"></param>
		/// <remarks>
		/// Since even when overriding the OnKeyDown methoed and not calling the base function 
		/// you don't have full control of the input, I've decided to catch windows messages to handle them.
		/// </remarks>
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case Win32.WM_PAINT:
					//Don't draw the control while parsing to avoid flicker.
					if(mParsing)
						return;
					break;
				case Win32.WM_KEYDOWN:
					if((Keys)(int)m.WParam == Keys.Pause)
						ShowAutoComplete();

					if(isShown)
						switch((Keys)(int)m.WParam)
						{
							case Keys.Down:
								ac.MoveToNextWord();
								return;
							case Keys.Up:
								ac.MoveToPreviousWord();
								return;
							case Keys.Tab:
								int curTokenStartIndex = Text.LastIndexOfAny(Seperators.ToArray(), Math.Min(SelectionStart - 1, Text.Length - 1)) + 1;
								int curTokenEndIndex = Text.IndexOfAny(Seperators.ToArray(), SelectionStart);
								if(curTokenEndIndex == -1)
								{
									curTokenEndIndex = Text.Length;
								}
								string curTokenString = Text.Substring(curTokenStartIndex, Math.Max(curTokenEndIndex - curTokenStartIndex, 0)).ToUpper();
								ac.CompleteWord(curTokenString);
								return;
							case Keys.Escape:
								ac.HideForm();
								return;
						}
					if(((Keys)(int)m.WParam == Keys.Z) && ((Win32.GetKeyState(Win32.VK_CONTROL) & Win32.KS_KEYDOWN) != 0))
					{
						Undo();
						return;
					}
					else if(((Keys)(int)m.WParam == Keys.Y) && ((Win32.GetKeyState(Win32.VK_CONTROL) & Win32.KS_KEYDOWN) != 0))
					{
						Redo();
						return;
					}
					break;
				case Win32.WM_KEYUP:
					if((Keys)(int)m.WParam == Keys.Tab)
					{
						Console.WriteLine("***");
						return;
					}
					break;
			}
			Console.WriteLine(m.Msg);
			base.WndProc(ref m);
		}


		/// <summary>
		/// Hides the AutoComplete form when losing focus on textbox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			//if(isShown)
			//	if(!ac.Focused)
			//	ac.HideForm();
			base.OnLostFocus(e);
		}


		#endregion

		#region AutoComplete functions

		/// <summary>
		/// Entry point to autocomplete mechanism.
		/// Tries to complete the current word. if it fails it shows the AutoComplete form.
		/// </summary>
		private void CompleteWord()
		{
			ShowAutoComplete();
		}

		private void ReplaceWord(string text)
		{
			int curTokenStartIndex = Text.LastIndexOfAny(Seperators.ToArray(), Math.Min(SelectionStart - 1, Text.Length - 1)) + 1;
			int curTokenEndIndex = Text.IndexOfAny(Seperators.ToArray(), SelectionStart);
			if(curTokenEndIndex == -1)
			{
				curTokenEndIndex = Text.Length;
			}
			SelectionStart = Math.Max(curTokenStartIndex, 0);
			SelectionLength = Math.Max(0, curTokenEndIndex - curTokenStartIndex);
			SelectedText = text;
			SelectionStart = SelectionStart + SelectionLength;
			SelectionLength = 0;

			ac.HideForm();
		}

		/// <summary>
		/// Finds the and sets the best matching token as the selected item in the AutoCompleteForm.
		/// </summary>
		private void SetBestSelectedAutoCompleteItem()
		{
			int curTokenStartIndex = Text.LastIndexOfAny(Seperators.ToArray(), Math.Min(SelectionStart - 1, Text.Length - 1)) + 1;
			int curTokenEndIndex = Text.IndexOfAny(Seperators.ToArray(), SelectionStart);
			if(curTokenEndIndex == -1)
			{
				curTokenEndIndex = Text.Length;
			}
			string curTokenString = Text.Substring(curTokenStartIndex, Math.Max(curTokenEndIndex - curTokenStartIndex, 0)).ToUpper();

			ac.PositionToWord(curTokenString);

		}

		/// <summary>
		/// Sets the location of the AutoComplete form, making sure it's on the screen where the cursor is.
		/// </summary>
		/// <param name="moveHorizontly">determines wheather or not to move the form horizontly.</param>
		private void SetAutoCompleteLocation(bool moveHorizontly)
		{
			Point cursorLocation = GetPositionFromCharIndex(SelectionStart);
			Screen screen = Screen.FromPoint(cursorLocation);
			Point optimalLocation = new Point(PointToScreen(cursorLocation).X - 15, (int)(PointToScreen(cursorLocation).Y + Font.Size * 2 + 2));
			Rectangle desiredPlace = new Rectangle(optimalLocation, ac.Size);
			desiredPlace.Width = 152;
			if(desiredPlace.Left < screen.Bounds.Left)
			{
				desiredPlace.X = screen.Bounds.Left;
			}
			if(desiredPlace.Right > screen.Bounds.Right)
			{
				desiredPlace.X -= (desiredPlace.Right - screen.Bounds.Right);
			}
			if(desiredPlace.Bottom > screen.Bounds.Bottom)
			{
				desiredPlace.Y = cursorLocation.Y - 2 - desiredPlace.Height;
			}
			if(!moveHorizontly)
			{
				desiredPlace.X = ac.Left;
			}

			ac.Bounds = desiredPlace;
		}

		/// <summary>
		/// Shows the Autocomplete form.
		/// </summary>
		public void ShowAutoComplete()
		{
			if(ac == null)
			{
				ac = new AutoComplete();
				foreach(var x in HighlightDescriptors)
				{
					if(x.DT == DescriptorType.Word)
						ac.Words.Add(new AutoWord() { word = x.Token, type = x.WT });
				}
				ac.Words.Sort(new Comparison<AutoWord>((x, y) => string.Compare(x.word, y.word)));
				ac.InitList();
				ac.Show();
				ac.backcall = new AutoComplete.string_d(ReplaceWord);
			}
			SetAutoCompleteLocation(true);
			//ac.Show();
			ac.Visible = true;
			SetBestSelectedAutoCompleteItem();
			Focus();
		}



		#endregion 

		#region Scrollbar positions functions
		/// <summary>
		/// Sends a win32 message to get the scrollbars' position.
		/// </summary>
		/// <returns>a POINT structore containing horizontal and vertical scrollbar position.</returns>
		private unsafe Win32.POINT GetScrollPos()
		{
			Win32.POINT res = new Win32.POINT();
			IntPtr ptr = new IntPtr(&res);
			Win32.SendMessage(Handle, Win32.EM_GETSCROLLPOS, 0, ptr);
			return res;

		}

		/// <summary>
		/// Sends a win32 message to set scrollbars position.
		/// </summary>
		/// <param name="point">a POINT conatining H/Vscrollbar scrollpos.</param>
		private unsafe void SetScrollPos(Win32.POINT point)
		{
			IntPtr ptr = new IntPtr(&point);
			Win32.SendMessage(Handle, Win32.EM_SETSCROLLPOS, 0, ptr);

		}
		#endregion

		public SyntaxHighlighting()
		{
			InitializeComponent();
			MaxUndoRedoSteps = 50;
			CaseSensitive = true;
		}

		public Dictionary<WordType, WordStyle> WordStyles = new Dictionary<WordType, WordStyle>();
	}
}
