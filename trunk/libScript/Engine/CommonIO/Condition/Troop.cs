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
		[Obsolete("Not-Implemented")]
		int gettroop(int vid, int aid)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(aid > 11 || aid < 1)
				return int.MinValue;
			return -2;
		}
		[Obsolete("Not-Implemented")]
		int gettroopinbuild(int vid, int aid, bool big)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(aid > 10 || aid < 1)
				return int.MinValue;
			return -2;
		}
		int getuplevel(int vid, int aid, int actiontype)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			if(aid > 10 || aid < 0)
				return int.MinValue;
			if(actiontype > 3 || actiontype < 1)
				return int.MinValue;
			if(actiontype != 3 && !TD.Villages[vid].Upgrades[aid].Researched)
				return -1;
			if(actiontype == 1)
				return TD.Villages[vid].Upgrades[aid].AttackLevel;
			if(actiontype == 2)
				return TD.Villages[vid].Upgrades[aid].DefenceLevel;
			if(actiontype == 3)
			{
				if(TD.Villages[vid].Upgrades[aid].Researched)
					return 1;
				if(TD.Villages[vid].Upgrades[aid].CanResearch)
					return 0;
				return -1;
			}
			return int.MinValue;
		}
	}
}
/*
int gettroop(int vid, int aid)
根据给定的aid，返回其拥有的军队数量。
成功返回军队数量，失败返回-1。
int gettroopinbuild(int vid, int aid, bool big)
根据给定的aid，返回其正在生产的军队数量。
big指定是否为大兵营或大马厩生产。
成功返回军队数量，失败返回-1。
big+冲撞或投石总是返回-1。
int getuplevel(int vid, int aid, int actiontype)
根据给定的aid返回其攻击、防御等级。对于研究：已经研发完成的返回1，不可研发的返回-1，可研发返回0。
actiontype:
1.攻击等级
2.防御等级
3.研究
 */