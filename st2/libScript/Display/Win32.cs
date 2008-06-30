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
 * 
 * This code is modified from [SyntaxHighlightingArtice] from codeproject.
 * http://www.codeproject.com/KB/edit/SyntaxHighlighting.aspx By [Uri Guy]
 */
using System;
using System.Runtime;
using System.Runtime.InteropServices;

using HWND = System.IntPtr;

namespace libScriptEngine
{
	public class Win32
	{
		private Win32()
		{
		}

		public const int WM_USER = 0x400;
		public const int WM_PAINT = 0xF;
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_CHAR = 0x102;

		public const int EM_GETSCROLLPOS = (WM_USER + 221);
		public const int EM_SETSCROLLPOS = (WM_USER + 222);

		public const int VK_CONTROL = 0x11;
		public const int VK_UP = 0x26;
		public const int VK_DOWN = 0x28;
		public const int VK_NUMLOCK = 0x90;

		public const short KS_ON = 0x01;
		public const short KS_KEYDOWN = 0x80;

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}

		[DllImport("user32")]
		public static extern int SendMessage(HWND hwnd, int wMsg, int wParam, IntPtr lParam);
		[DllImport("user32")]
		public static extern int PostMessage(HWND hwnd, int wMsg, int wParam, int lParam);
		[DllImport("user32")]
		public static extern short GetKeyState(int nVirtKey);
		[DllImport("user32")]
		public static extern int LockWindowUpdate(HWND hwnd);

	}
}
