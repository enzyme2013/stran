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
		int resamount(int vid, int x)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(x > 4 || x < 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			return TD.Villages[vid].Resource[x].CurrAmount;
		}
		int rescapacity(int vid, int x)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(x > 4 || x < 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			return TD.Villages[vid].Resource[x].Capacity;
		}
		int resproduce(int vid, int x)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(x > 4 || x < 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			return TD.Villages[vid].Resource[x].Produce;
		}
	}
}
/*
int resamount(int vid, int x)
对于给定的x(0~4代表资源种类)，获得当前的资源储备量。
int rescapacity(int vid, int x)
对于给定的x(0~4代表资源种类)，获得当前的仓库容量。
int resproduce(int vid, int x)
对于给定的x(0~4代表资源种类)，获得当前的资源生产量。粮食可以是负数。
*/