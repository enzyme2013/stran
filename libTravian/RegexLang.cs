using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using System.IO;

namespace libTravian
{
    partial class Travian
    {
        /// <summary>正则集合</summary>
        public Dictionary<string, string> RegexLang = new Dictionary<string, string>();

        /// <summary>加载正则</summary>
        /// <param name="language">服务器语言</param>
        public bool LoadRegexLang(string language)
        {
            //获取公共正则
            string lang_file = "lang\\regex_common.txt";
            if (!File.Exists(lang_file))
            {
                DebugLog("Load Regex_Common Error!", DebugLevel.E);
                return false;
            }
            string[] s = File.ReadAllLines(lang_file, Encoding.UTF8);
            foreach (var str in s)
            {
                var pairs = str.Split(new char[] { '=' }, 2);
                if (pairs.Length != 2)
                    continue;
                RegexLang.Add(pairs[0], pairs[1]);
            }

            //获取服务器语言对应的正则
            lang_file = string.Format("lang\\regex_{0}.txt", language);
            if (!File.Exists(lang_file))
                return true;
            s = File.ReadAllLines(lang_file, Encoding.UTF8);
            foreach (var str in s)
            {
                var pairs = str.Split(new char[] { '=' }, 2);
                if (pairs.Length != 2)
                    continue;
                if (RegexLang.ContainsKey(pairs[0]))
                    RegexLang[pairs[0]] = pairs[1];
                else
                    RegexLang.Add(pairs[0], pairs[1]);
            }
            return true;
        }
    }
}
