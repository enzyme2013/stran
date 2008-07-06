using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Stran2
{
	public class TDebugInfo
	{
		public DateTime Time { get; set; }
		public string MethodName { get; set; }
		public string Filename { get; set; }
		public int Line { get; set; }
		public DebugLevel Level { get; set; }
		public string Text { get; set; }
	}
	public enum DebugLevel
	{
		I,
		W,
		E,
		F
	}
	public class LogArgs : EventArgs
	{
		public TDebugInfo DebugInfo { get; set; }
	}

	public class Debugger
	{
		public static readonly Debugger Instance = new Debugger();
		private Debugger() { }

		//public event EventHandler<LogArgs> OnError;
		//public const int DebugCount = 16;
		//public List<TDebugInfo> DebugList = new List<TDebugInfo>(DebugCount);
		public void DebugLog(string Text, DebugLevel Level)
		{
			StackFrame x = new StackTrace(true).GetFrame(1);
			string MethodName = x.GetMethod().Name;
			string Filename = x.GetFileName();
			Filename = string.IsNullOrEmpty(Filename) ? "null" : Filename.Substring(16);
			int Line = x.GetFileLineNumber();
			TDebugInfo db = new TDebugInfo()
			{
				Filename = Filename,
				Level = Level,
				Line = Line,
				MethodName = MethodName,
				Text = Text,
				Time = DateTime.Now
			};
			//if(DebugList.Count > DebugCount)
			//	DebugList.RemoveAt(0);
			//DebugList.Add(db);

			OnError(db);
		}
		public void DebugLog(Exception e)
		{
			DebugLog(e, DebugLevel.F);
		}
		public void DebugLog(Exception e, DebugLevel Level)
		{
			StackFrame x = new StackTrace(e).GetFrame(0);
			string MethodName = x.GetMethod().Name;
			string Filename = x.GetFileName();
			Filename = string.IsNullOrEmpty(Filename) ? "null" : Filename.Substring(16);
			int Line = x.GetFileLineNumber();
			TDebugInfo db = new TDebugInfo()
			{
				Filename = Filename,
				Level = DebugLevel.F,
				Line = Line,
				MethodName = MethodName,
				Text = e.Message + Environment.NewLine + e.StackTrace,
				Time = DateTime.Now
			};
			//if(DebugList.Count > DebugCount)
			//	DebugList.RemoveAt(0);
			//DebugList.Add(db);
			OnError(db);
		}

		public void OnError(TDebugInfo DB)
		{
			string str = string.Format("[{0} {1}][{2}]{3,18}@{4,-35}:{5,-3} {6}",
				DB.Time.Day,
				DB.Time.ToLongTimeString(),
				DB.Level.ToString(),
				DB.MethodName,
				DB.Filename,
				DB.Line,
				DB.Text);
			//textBox1.AppendText(str + "\r\n");
			//LastDebug.Text = str;
			Console.WriteLine(str);
		}
	}
}
