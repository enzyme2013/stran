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
using LuaInterface;
using System.Reflection;

namespace libScriptEngine.LAL
{
	class LuaEngine : IScriptEngine
	{
		Lua lua;
		public LuaEngine(string code)
			: base(code)
		{
			lua = new Lua();
			Register("resamount", Template.t<int, int>());
			Register("rescapacity", Template.t<int, int>());
			Register("resproduce", Template.t<int, int>());
			Register("getlevel", Template.t<int, int>());
			Register("getgid", Template.t<int, int>());
			Register("gettaskdelay", Template.t<int, int>());
			Register("getmerchants", Template.t<int>());
			Register("getsinglecarry", Template.t<int>());
			Register("gettroop", Template.t<int, int>());
			Register("gettroopinbuild", Template.t<int, int, bool>());
			Register("getuplevel", Template.t<int, int, int>());
			Register("getcoord", Template.t<int, int>());
			Register("hasnewigm", Template.t());
			Register("getnewigm", Template.t());
			Register("getigmsubject", Template.t<int>());
			Register("getigmtext", Template.t<int>());
			lua.DoString(code);
		}
		private LuaFunction Register(string FuncName, Type[] Params)
		{
			return lua.RegisterFunction(FuncName, this, base.GetType().GetMethod(FuncName, Params));

		}
		public override int EvalAndRun(string funcname)
		{
			var func = lua.GetFunction(funcname);
			int delay = -1;
			if(func != null)
				delay = Convert.ToInt32((double)func.Call()[0]);
			return delay;
		}
	}
}
