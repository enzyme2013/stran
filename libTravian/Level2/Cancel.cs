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
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libTravian
{
	public class CancelOption
	{
		public int VillageID { get; set; }
		public int Key { get; set; }
	}
	public class RenameOption
	{
		public int VillageID {get; set;}
		public string VillageName {get; set;}
	}


	partial class Travian
	{
		public void doCancelWrapper(object o)
		{
			CancelOption to = o as CancelOption;
			int VillageID = to.VillageID;
			int Key = to.Key;
			doCancel(VillageID, Key);
		}
		private void doCancel(int VillageID, int Key)
		{
			lock(Level2Lock)
			{
				var CV = TD.Villages[VillageID];
				if(CV.InBuilding[Key] == null || !CV.InBuilding[Key].Cancellable)
					return;
				PageQuery(VillageID, CV.InBuilding[Key].CancelURL);
				CV.InBuilding[Key] = null;
			}
		}
		public void doRenameWrapper(object o)
		{
			RenameOption to = o as RenameOption;
			int VillageID = to.VillageID;
			string VillageName = to.VillageName;
			doRename(VillageID, VillageName);
		}
		private void doRename(int VillageID, string VillageName)
		{
			lock(Level2Lock)
			{
				var CV = TD.Villages[VillageID];
	            string OldVillageName = CV.Name;
	            // Get if possible Rename
	            string mainuser = PageQuery(VillageID, "spieler.php?uid=" + TD.UserID);
	            if (string.IsNullOrEmpty(mainuser))
	            	return;
	            Match mu = Regex.Match(mainuser, "<a href=\"spieler.php\\?s=1\">");
	            if (!mu.Success)
	            {
	                DebugLog("Not owner of this accounts.", DebugLevel.W);
	            }
	            else if (VillageName != OldVillageName && !string.IsNullOrEmpty(VillageName))
	            {
	                // Prepare data
	                Random rand = new Random();
	                string p_e = string.Empty;
					string p_uid = string.Empty;
					string p_did = string.Empty;
					string p_jahr = string.Empty;
					string p_monat = string.Empty;
					string p_tag = string.Empty;
					string p_be1 = string.Empty;
					string p_mw = string.Empty;
					string p_ort = string.Empty;
					string p_be2 = string.Empty;
	                string data = PageQuery(VillageID, "spieler.php?s=1");
	                if (string.IsNullOrEmpty(data))
	                	return;
	                Match m;
	                m = Regex.Match(data, "type=\"hidden\" name=\"e\" value=\"(\\d+?)\"");
	                if (m.Success)
	                	p_e = m.Groups[1].Value;
	                m = Regex.Match(data, "type=\"hidden\" name=\"uid\" value=\"(\\d+?)\"");
	                if (m.Success)
	                    p_uid = m.Groups[1].Value;
	                m = Regex.Match(data, "type=\"hidden\" name=\"did\" value=\"(\\d+?)\"");
	                if (m.Success)
	                    p_did = m.Groups[1].Value;
                    m = Regex.Match(data, "tabindex=\"3\" type=\"text\" name=\"jahr\" value=\"(.*?)\" maxlength=\"4\"");
	                if (m.Success)
	                    p_jahr = m.Groups[1].Value;
	                m = Regex.Match(data, "<option value=\"(\\d+?)\" selected=\"selected\">");
	                if (m.Success)
	                    p_monat = m.Groups[1].Value;
	                else
	                    p_monat = "0";
	                m = Regex.Match(data, "tabindex=\"1\" class=\"text day\" type=\"text\" name=\"tag\" value=\"(.*?)\" maxlength=\"2\"");
	                if (m.Success)
	                    p_tag = m.Groups[1].Value;
	                m = Regex.Match(data, "type=\"radio\" name=\"mw\" value=\"(\\d+?)\" checked tabindex=\"4\"");
 	                if (m.Success)
	                   p_mw = m.Groups[1].Value;
	                m = Regex.Match(data, "tabindex=\"5\" type=\"text\" name=\"ort\" value=\"(.*?)\" maxlength=\"30\"");
	                if (m.Success)
	                    p_ort = m.Groups[1].Value;
	                m = Regex.Match(data, "tabindex=\"7\" name=\"be1\">([^<]*?)</textarea>", RegexOptions.Singleline);
	                if (m.Success)
	                    p_be1 = m.Groups[1].Value;
                    m = Regex.Match(data, "tabindex=\"8\" name=\"be2\">([^<]*?)</textarea>", RegexOptions.Singleline);
	                if (m.Success)
	                    p_be2 = m.Groups[1].Value;
                    
                    Dictionary<string, string> PostData = new Dictionary<string, string>();
	                PostData["e"] = p_e;
	                PostData["uid"] = p_uid;
	                PostData["did"] = p_did;
	                PostData["jahr"] = p_jahr;
	                PostData["monat"] = p_monat;
	                PostData["tag"] = p_tag;
	                PostData["be1"] = p_be1.Replace("\n", "\r\n");
	                PostData["mw"] = p_mw;
	                PostData["ort"] = p_ort;
	                PostData["dname"] = VillageName;
	                PostData["be2"] = p_be2.Replace("\n", "\r\n");
	                PostData["s1.x"] = rand.Next(10, 70).ToString();
	                PostData["s1.y"] = rand.Next(3, 17).ToString();
	                string result = PageQuery(VillageID, "spieler.php", PostData);
	                if (string.IsNullOrEmpty(result))
	                	return;
	                if (TD.Villages.Count == 1)
	                {
	                	m = Regex.Match(result, "<td class=\"nam\"><a href=\"karte.php\\?d=(\\d+)&amp;c=[^>]*?\">([^<]*?)</a>");
	                	if (m.Success && CV.Z == Convert.ToInt32(m.Groups[1].Value) && CV.Name != m.Groups[2].Value)
	                	{
	                		CV.Name = m.Groups[2].Value;
	                		StatusUpdate(this, new StatusChanged() { ChangedData = ChangedType.Villages });
	                	}
	                }
	            }
			}
		}
	}
}
