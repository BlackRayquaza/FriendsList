using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FriendsListCmd.util
{
    public static class StringUtil
    {
        public static int FromString(string value)
        {
            if (value == null)
                throw new FormatException();
            return value.StartsWith("0x") ? int.Parse(value.Substring(2), NumberStyles.HexNumber) : int.Parse(value);
        }

        public static string FormatNumber(string value)
        {
            if (value == null)
                throw new FormatException();
            var e = new Regex("(\\d+)(\\d{3})");
            for (value += ""; e.IsMatch(value);)
                value = e.Replace(value, "$1.$2");
            return value;
        }

        public static string FormatNumber(int value)
        {
            var val = value.ToString();
            if (val == null)
                throw new FormatException();
            var e = new Regex("(\\d+)(\\d{3})");
            for (val += ""; e.IsMatch(val);)
                val = e.Replace(val, "$1.$2");
            return val;
        }
    }
}