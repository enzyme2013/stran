using System;
using System.Collections.Generic;
using System.Text;

namespace libTravian
{
    public static class ExtendMethod
    {
        public static string Join(this Array array, string seperator)
        {
            if (array == null || array.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (object o in array)
                sb.Append(o).Append(seperator);

            sb.Remove(sb.Length - seperator.Length, seperator.Length);
            return sb.ToString();
        }

        public static bool Include(this string data, string value)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(value))
                return false;
            string[] tags = TagLang.Tags[value].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tag in tags)
            {
                if (data.Contains(tag))
                    return true;
            }
            return false;
        }
    }
}
