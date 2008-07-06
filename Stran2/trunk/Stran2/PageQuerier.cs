using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace Stran2
{
	/// <summary>
	/// For test propose
	/// </summary>
	public interface IPageQuerier
	{
		string Get(UserData UD, int VillageID, string Uri);

		string Post(UserData UD, int VillageID, string Uri, Dictionary<string, string> Data);

		string GetEx(UserData UD, int VillageID, string Uri, Dictionary<string, string> Data, bool CheckLogin, bool NoParser);
	}

	/// <summary>
	/// Wrap all network execution
	/// </summary>
	public class PageQuerier : IPageQuerier
	{
		private PageQuerier() { }
		public static readonly PageQuerier Instance = new PageQuerier();

		public int PageCount { get; private set; }

		WebClient wc;

		public string Get(UserData UD, int VillageID, string Uri)
		{
			return GetEx(UD, VillageID, Uri, null, true, false);
		}
		public string Post(UserData UD, int VillageID, string Uri, Dictionary<string, string> Data)
		{
			return GetEx(UD, VillageID, Uri, Data, true, false);
		}
		private string AddNewdid(int VillageID, string Uri)
		{
			if(VillageID == 0)
				return Uri;
			if(Uri.Contains("?"))
				return Uri + "&newdid=" + VillageID;
			else
				return Uri + "?newdid=" + VillageID;
		}

		private void PageQueryDebugLog(UserData UD, int VillageID, string Uri)
		{
			var st = new StackTrace(true);
			StackFrame x = null;
			string MethodName = null;
			int i;
			for(i = 2; i < st.FrameCount; i++)
			{
				x = st.GetFrame(i);
				MethodName = x.GetMethod().Name;
				if(MethodName != "PageQuery")
					break;
			}
			if(i == st.FrameCount)
			{
				x = st.GetFrame(3);
				MethodName = x.GetMethod().Name;
			}
			string Filename = x.GetFileName();
			int Line = x.GetFileLineNumber();
			/*
			TDebugInfo db = new TDebugInfo()
			{
				Filename = Filename,
				Level = DebugLevel.I,
				Line = Line,
				MethodName = MethodName,
				Text = "Page: " + Uri + " (" + VillageID.ToString() + ")",
				Time = DateTime.Now
			};
			OnError(this, new LogArgs() { DebugInfo = db });
			 */
		}

		public string GetEx(UserData UD, int VillageID, string Uri, Dictionary<string, string> Data, bool CheckLogin, bool NoParser)
		{
			try
			{
				PageQueryDebugLog(UD, VillageID, Uri);
				if(wc == null)
				{
					wc = new WebClient();
				}
					wc.BaseAddress = string.Format("http://{0}/", UD.StringProperties["Server"]);
					wc.Encoding = Encoding.UTF8;
					wc.Headers[HttpRequestHeader.Referer] = wc.BaseAddress;
				string proxy;
				if((proxy = OptionCenter.Instance.GetValue("Proxy", null)) != null)
					wc.Proxy = new WebProxy(proxy);
				if(!UD.StringProperties.ContainsKey("Cookie"))
					if(CheckLogin && !Login(UD))
						return null;
				Uri = AddNewdid(VillageID, Uri);
				string QueryString = null;
				string result;
				if(Data != null)
				{
					StringBuilder sb = new StringBuilder();
					foreach(var x in Data)
					{
						if(sb.Length != 0)
							sb.Append("&");

						// Got to support some weired form data, like arrays
						if(x.Key == "!!!RawData!!!")
						{
							sb.Append(x.Value);
							continue;
						}

						sb.Append(HttpUtility.UrlEncode(x.Key));
						sb.Append("=");
						sb.Append(HttpUtility.UrlEncode(x.Value));
					}
					QueryString = sb.ToString();
					wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
					result = wc.UploadString(Uri, QueryString);
				}
				else
					result = wc.DownloadString(Uri);
				string[] t = wc.ResponseHeaders.GetValues("Set-Cookie");
				if(t != null)
				{
					foreach(string t1 in t)
						if(t1.Contains("T3E"))
							UD.StringProperties["Cookie"] = t1;
					wc.Headers[HttpRequestHeader.Cookie] = UD.StringProperties["Cookie"];
				}

				if(!CheckLogin)
					return result;

				if(result.Contains("login"))
				{
					if(!Login(UD))
						return null;
					if(Data != null)
					{
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						result = wc.UploadString(Uri, QueryString);
					}
					else
						result = wc.DownloadString(Uri);
				}
				if(result.Contains(".php?ok"))
				{
					wc.DownloadString("dorf1.php?ok");
					if(Data != null)
					{
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						result = wc.UploadString(Uri, QueryString);
					}
					else
						result = wc.DownloadString(Uri);
				}
				FetchPageCount();

				var m = Regex.Match(result, "<span id=\"tp1\" class=\"b\">([0-9:]+)</span>");
				if(m.Success)
				{
					var time = DateTime.Parse(m.Groups[1].Value);
					var timeoff = time.Subtract(DateTime.Now);
					if(timeoff < new TimeSpan(-12, 0, 0))
						timeoff.Add(new TimeSpan(24, 0, 0));
					else if(timeoff > new TimeSpan(12, 0, 0))
						timeoff.Subtract(new TimeSpan(-24, 0, 0));
					UD.Int32Properties["ServerTimeOffset"] = Convert.ToInt32(timeoff.TotalSeconds);
				}
				if(!NoParser)
					MethodsCenter.Instance.CallParser(result, UD, VillageID);
				return result;
			}
			catch(Exception e)
			{
				Debugger.Instance.DebugLog(e);
			}
			return null;
		}

		void FetchPageCount()
		{
			PageCount++;
		}

		private int UnixTime(DateTime time)
		{
			return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
		}
		public bool Login(UserData UD)
		{
			string Username = UD.StringProperties["Username"];
			string Password = UD.StringProperties["Password"];
			if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
				return false;
			try
			{
				//WriteInfo("Logging in as '" + Username + "', may take a few seconds...");
				string data = wc.DownloadString("/");
				if(!data.Contains("Travian"))
				{
					Debugger.Instance.DebugLog("Cannot visit travian website!", DebugLevel.F);
					return false;
				}
				string userkey, passkey, alkey;
				Match m;
				m = Regex.Match(data, "type=\"text\" name=\"(\\S+?)\"");
				if(m.Success)
					userkey = m.Groups[1].Value;
				else
				{
					Debugger.Instance.DebugLog("Parse userkey error!", DebugLevel.F);
					return false;
				}
				m = Regex.Match(data, "type=\"password\" name=\"(\\S+?)\"");
				if(m.Success)
					passkey = m.Groups[1].Value;
				else
				{
					Debugger.Instance.DebugLog("Parse passkey error!", DebugLevel.F);
					return false;
				}
				m = Regex.Match(data, "><input type=\"hidden\" name=\"(\\S+?)\" value");
				if(m.Success)
					alkey = m.Groups[1].Value;
				else
				{
					Debugger.Instance.DebugLog("Parse alkey error!", DebugLevel.F);
					return false;
				}
				Dictionary<string, string> PostData = new Dictionary<string, string>();
				PostData["w"] = "1024:768";
				PostData["login"] = (UnixTime(DateTime.Now) - 10).ToString();
				PostData[userkey] = Username;
				PostData[passkey] = Password;
				PostData[alkey] = "";
				PostData["s1.x"] = "0";
				PostData["s1.y"] = "0";

				string result = PageQuerier.Instance.GetEx(UD, 0, "dorf1.php?ok", PostData, false, true);

				if(result.Contains("login"))
				{
					Debugger.Instance.DebugLog("Username or Password error!", DebugLevel.F);
					//MessageBox.Show("Login failed.");
					return false;
				}

				m = Regex.Match(result, "spieler.php\\?uid=(\\d*)");
				if(m.Success)
					UD.Int32Properties["UserID"] = Convert.ToInt32(m.Groups[1].Value);

				return true;
			}
			catch(Exception e)
			{
				Debugger.Instance.DebugLog(e);
				return false;
			}
		}
	}
}
