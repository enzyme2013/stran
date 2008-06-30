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
		int[] getcoord(int vid)
		{
			return TD.Villages[vid].Coord.getArray;
		}
		bool hasnewigm()
		{
			return false;
			//return TD.NewIGMCount != 0;
		}
		[Obsolete("Not-Implemented")]
		int[] getnewigm()
		{
			return null;
		}
		[Obsolete("Not-Implemented")]
		string[] getigmsubject(int igmid)
		{
			return null;
		}
		[Obsolete("Not-Implemented")]
		string getigmtext(int igmid)
		{
			return null;
		}
	}
}
/*
int[2] getcoord(int vid)
获取村庄的坐标。
bool hasnewigm()
返回是否有未读站内消息。
int[] getnewigm()
返回未读站内消息列表。
string[2] getigmsubject(int igmid)
返回站内消息的发送者和标题。
string getigmtext(int igmid)
返回站内消息的文本。
*/