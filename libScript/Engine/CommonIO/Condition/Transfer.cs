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
		int getmerchants(int vid)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			return TD.Villages[vid].Market.ActiveMerchant;
		}
		int getsinglecarry(int vid)
		{
			if(!TD.Villages.ContainsKey(vid))
				return int.MinValue;
			return TD.Villages[vid].Market.SingleCarry;
		}
	}
}
/*
int getmerchants(int vid)
获取剩余的商人数量。
int getsinglecarry(int vid)
获取单个商人的运输量。
*/