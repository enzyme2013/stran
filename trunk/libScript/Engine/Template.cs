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
	static class Template
	{
		static public Type[] t()
		{
			return new Type[] { };
		}
		static public Type[] t<T1>()
		{
			return new Type[] { typeof(T1) };
		}
		static public Type[] t<T1, T2>()
		{
			return new Type[] { typeof(T1), typeof(T2) };
		}
		static public Type[] t<T1, T2, T3>()
		{
			return new Type[] { typeof(T1), typeof(T2), typeof(T3) };
		}
		static public Type[] t<T1, T2, T3, T4>()
		{
			return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
		}
		static public Type[] t<T1, T2, T3, T4, T5>()
		{
			return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
		}
	}
}
