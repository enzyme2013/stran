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
		int gettaskdelay(int vid, int type)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(type > 5 || type < 0)
				return int.MinValue;
			if(TD.Villages[vid].isBuildingInitialized != 2)
				return -1;
			int tinbtype = (type == 1 && !TD.isRomans) ? 0 : type;
			int delay = 0;
			if(TD.Villages[vid].InBuilding[tinbtype] != null)
				delay = Convert.ToInt32(TD.Villages[vid].InBuilding[tinbtype].FinishTime.Subtract(DateTime.Now).TotalSeconds);
			return delay;
		}
	}
}
/*
int gettaskdelay(int vid, int type)
对于给定的type，获取此类任务还剩余的计时时间。如果没有此类任务正在进行，返回0。
typelist:
0.资源田
1.建筑
2.拆除
3.攻击等级
4.防御等级
5.研究
非罗马状态下，0和1返回相同结果。
*/