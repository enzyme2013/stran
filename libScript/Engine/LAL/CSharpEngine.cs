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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using System.Globalization;

namespace libScriptEngine.LAL
{
	class CSharpEngine : IScriptEngine
	{
		Type t;
		object o;
		public CSharpEngine(string code)
			: base(code)
		{
			CSharpCodeProvider p = new CSharpCodeProvider();
			var cc = CSharpCodeProvider.CreateProvider("CSharp");
			CompilerParameters options = new CompilerParameters();
			options.ReferencedAssemblies.Add("System.dll");
			options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
			options.GenerateExecutable = false;
			options.GenerateInMemory = true;
			options.OutputAssembly = "script";

			CodeSnippetCompileUnit cu = new CodeSnippetCompileUnit(code);
			CompilerResults cr = cc.CompileAssemblyFromDom(options, cu);

			t = cr.CompiledAssembly.GetType("ScriptEngine.script");
			o = cr.CompiledAssembly.CreateInstance("ScriptEngine.script", false, BindingFlags.Default, null, new object[] { this }, CultureInfo.CurrentCulture, null);

		}
		public override int EvalAndRun(string funcname)
		{
			object delay = t.InvokeMember(funcname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, o, null);
			return (int)delay;
		}
	}
}
