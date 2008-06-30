using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Stravian
{
	public partial class NewAccount : Form
	{
		bool _ismodify;
		public logininfo accountresult;
		public NewAccount(bool ismodify)
		{
			_ismodify = ismodify;
			InitializeComponent();
			if(ismodify)
				Text = "±à¼­" + Text;
			else
				Text = "Ìí¼Ó" + Text;
			comboBox1.SelectedIndex = 0;
		}

		public void init(logininfo l)
		{
			textBox3.Text = l.Server;
			textBox2.Text = l.Username;
			comboBox1.SelectedIndex = l.Tribe;
			textBox4.Text = l.Language;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(textBox3.Text != "" && textBox2.Text != "" && (_ismodify || textBox1.Text != ""))
			{
				accountresult = new logininfo() { Server = textBox3.Text, Username = textBox2.Text, Password = textBox1.Text, Tribe = comboBox1.SelectedIndex, Language = textBox4.Text };
			}
		}

		private void textBox3_Leave(object sender, EventArgs e)
		{
			var m = Regex.Match(textBox3.Text.ToLower(), "^([a-zA-Z]{2,3})(\\d{1,3}|x)$");
			if(m.Success)
			{
				textBox3.Text = string.Format("s{0}.travian.{1}", m.Groups[2].Value[0] == 'x' ? "peed" : m.Groups[2].Value, m.Groups[1].Value);
				if(textBox4.Text == "")
					textBox4.Text = m.Groups[1].Value;
				return;
			}
			m = Regex.Match(textBox3.Text.ToLower(), "http://(.*)");
			if(m.Success)
			{
				textBox3.Text = m.Groups[1].Value;
			}
			if(textBox4.Text == "")
			{
				m = Regex.Match(textBox3.Text.ToLower(), "travian.(.*)");
				if(m.Success)
					textBox4.Text = m.Groups[1].Value;
			}
		}

	}
}
