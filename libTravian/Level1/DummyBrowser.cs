using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.JScript;
using HtmlAgilityPack;

namespace libTravian
{
	static public class DummyBrowser
	{
		static public bool DummyCheck(string PageData, IPageQuerier PQ)
		{
			// Check stats.php
			var mc = Regex.Matches(PageData, @"""(stats\.php\?p=[^""]+)""(\+.*)*");
			if (mc.Count == 0)
				return false;
			// File.WriteAllText("PAGEDUMP_" + DateTime.Now.Ticks + ".htm", PageData);
			foreach (Match mm in mc)
			{
				var uri = mm.Groups[1].Value;
				if (mm.Groups[2].Success)
					uri += mm.Groups[2].Value;
				uri = FilterDummyParams(PageData, uri, PQ);
				PQ.PageQuery(0, uri, null, false, true);
			}
			return true;
		}

		static private string FilterDummyParams(string PageData, string str, IPageQuerier PQ)
		{
			return str
				.Replace("'+escape(Browser.Engine.name)+'", "trident")
				.Replace("'+escape(Browser.Platform.name)+'", "win")
				.Replace("'+escape(screen.width)+'", ScreenWidth.ToString())
				.Replace("'+escape(screen.height)+'", ScreenHeight.ToString())
				.Replace("'+escape(document.referrer)+'", GlobalObject.escape(PQ.Referer))
				.Replace("'+escape(navigator.userAgent)+'", GlobalObject.escape(UA))
				.Replace("+CountRec1(mtop),", CountRec1(PageData, "mtop").ToString());
		}

		static private int CountRec1(string PageData, string ElementID)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(PageData);
			return doc.DocumentNode.SelectNodes("//div[@id='mtop']//*").Count + 1;
		}

		static public int ScreenWidth = 1440;
		static public int ScreenHeight = 900;
		static public string UA = Travian.UA;
	}
}
