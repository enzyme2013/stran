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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Stran
{
	public partial class NewAccount : Form
	{
		bool _ismodify;
		public TLoginInfo accountresult;
		public MUI mui { get; set; }
		public TLoginInfo logininfo { get; set; }
		public NewAccount(bool ismodify)
		{
			_ismodify = ismodify;
			InitializeComponent();
		}

		private void Btn_OK_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtProxy.Text))
			{
				var m = Regex.Match(txtProxy.Text, @"^((1?\d?\d|(2([0-4]\d|5[0-5])))\.){3}(1?\d?\d|(2([0-4]\d|5[0-5]))):\d{1,5}$");
				if (!m.Success)
				{
					MessageBox.Show("代理地址格式错误！");
					txtProxy.Text = string.Empty;
				}
			}
			if (txtServer.Text != "" && txtID.Text != "" && (_ismodify || txtPWD.Text != ""))
			{
				accountresult = new TLoginInfo()
				{
					Server = txtServer.Text.ToLower(),
					Username = txtID.Text,
					Password = txtPWD.Text,
					Tribe = comboBox1.SelectedIndex,
					Language = txtLanguage.Text.ToLower(),
					Proxy = txtProxy.Text,
					ServerLang = txtServerLang.Text
				};
			}
		}

		private void textBox3_Leave(object sender, EventArgs e)
		{
			var m = Regex.Match(txtServer.Text.ToLower(), "^([a-zA-Z]{2,3})(\\d{1,3}|x)$");
			if (m.Success)
			{
				if (m.Groups[1].Value == "de")
					txtServer.Text = string.Format("welt{0}.travian.{1}", m.Groups[2].Value[0] == 'x' ? "peed" : m.Groups[2].Value, m.Groups[1].Value);
				else
					txtServer.Text = string.Format("s{0}.travian.{1}", m.Groups[2].Value[0] == 'x' ? "peed" : m.Groups[2].Value, m.Groups[1].Value);
				if (txtLanguage.Text == "")
					txtLanguage.Text = m.Groups[1].Value;
				return;
			}
			m = Regex.Match(txtServer.Text.ToLower(), "http://(.*)");
			if (m.Success)
			{
				txtServer.Text = m.Groups[1].Value;
			}
			string slang = txtServer.Text.Substring(txtServer.Text.LastIndexOf(".") + 1, txtServer.Text.Length - txtServer.Text.LastIndexOf(".") - 1);
			if (slang == "com")
				slang = "en";
			if (slang == "org")
				slang = "de";
			if (slang == "cc")
				slang = "cn";
			txtServerLang.Text = slang;
			if (txtLanguage.Text == "")
			{
				m = Regex.Match(txtServer.Text.ToLower(), "travian.(.*)");
				if (m.Success)
				{
					string lang = m.Groups[1].Value;
					if (lang == "tw" || lang == "hk")
						txtLanguage.Text = "tw";
					else if (lang == "cn" || lang == "cc")
						txtLanguage.Text = "cn";
					else
						txtLanguage.Text = "en";
				}
			}
		}

		private void NewAccount_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			comboBox1.Items.AddRange(MainForm.tribelist);
			if (_ismodify)
				Text = mui._("edit") + Text;
			else
				Text = mui._("add") + Text;
			comboBox1.SelectedIndex = 0;
			if (logininfo != null)
			{
				txtServer.Text = logininfo.Server;
				txtID.Text = logininfo.Username;
				comboBox1.SelectedIndex = logininfo.Tribe;
				txtLanguage.Text = logininfo.Language;
				txtProxy.Text = logininfo.Proxy;
				txtServerLang.Text = logininfo.ServerLang;
			}
		}
	}
}
