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
	}
}
/*
int sendtroop(int vid, int[2] target, int sendtype, int[11] amount, int[2] kata)
出兵到target，出兵方式为sendtype: 1.支援 2.攻击 3.抢夺，kata 投石机目标可空。
成功返回行军往返时间，失败返回-1。
amount若给定-1则派出所有此种部队。
关于1兵的问题：脚本应事先获取兵种的数量。若脚本决定派出一兵，程序应当无条件执行。
*/