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

namespace libScriptEngine
{
	partial class IScriptEngine
	{
		int getlevel(int vid, int bid)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(bid > 40 || bid <= 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			return TD.Villages[vid].Buildings[bid].Level;
		}
		int getgid(int vid, int bid)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(bid > 40 || bid <= 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			return TD.Villages[vid].Buildings[bid].Gid;
		}
	}
}
/*
int getlevel(int vid, int bid)
对于给定的bid，获取此建筑的等级。工地返回0。
int getgid(int vid, int bid)
对于给定的bid，获取此建筑对应的gid。工地返回0。
*/