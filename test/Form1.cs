using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libScriptEngine;

namespace test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Font MainFont = syntaxHighlighting1.Font;
			string[] KeyWords = new string[] { "int", "void", "new", "string", "private", "public", "if", "for", "do", "while", "return" };
			string[] Functions = new string[] {
				"resamount", "rescapacity", "resproduce", "getlevel", "getgid", "gettaskdelay", "getmerchants",
				"getsinglecarry", "gettroop", "gettroopinbuild", "getuplevel", "getcoord", "hasnewigm", "getnewigm",
				"getigmsubject", "getigmtext" };
			syntaxHighlighting1.WordStyles.Add(WordType.KeyWords, new WordStyle() { Color = Color.Blue, Font = MainFont });
			syntaxHighlighting1.WordStyles.Add(WordType.BuildInFunctions, new WordStyle() { Color = Color.Purple, Font = MainFont });
			//syntaxHighlighting1.HighlightDescriptors.Add(new HighlightDescriptor("int", DescriptorType.Word, WordType.KeyWords));
			AddKeyWord(KeyWords);
			AddFunction(Functions);
			AddSep(" \r\n,.+-*/<>()[]{}%&'\"\t");
			syntaxHighlighting1.WordStyles.Add(WordType.String, new WordStyle() { Color = Color.Chocolate, Font = MainFont });
			syntaxHighlighting1.HighlightDescriptors.Add(new HighlightDescriptor("\"", "\"", DescriptorType.ToCloseToken, WordType.String));
			/*
			 * int[] tabstops = new int[32];
			tabstops[0] = 2;
			for(int i = 1; i < 32; i++)
				tabstops[i] = tabstops[0] * i;
			syntaxHighlighting1.SelectAll();
			syntaxHighlighting1.SelectionTabs = tabstops;
			syntaxHighlighting1.SelectionLength = 0;
			 * */
		}
		private void AddKeyWord(string[] strs)
		{
			foreach(var str in strs)
				syntaxHighlighting1.HighlightDescriptors.Add(new HighlightDescriptor(str, DescriptorType.Word, WordType.KeyWords));
		}
		private void AddFunction(string[] strs)
		{
			foreach(var str in strs)
				syntaxHighlighting1.HighlightDescriptors.Add(new HighlightDescriptor(str, DescriptorType.Word, WordType.BuildInFunctions));
		}
		private void AddSep(string str)
		{
			foreach(var ch in str)
				syntaxHighlighting1.Seperators.Add(ch);
		}
	}
}
