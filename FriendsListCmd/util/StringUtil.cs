using System;
using System.Globalization;
using System.Text;

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

        public static string Format(string value)
        {
            if (value == null)
                throw new FormatException();
            var sb = new StringBuilder();


            return sb.ToString();
        }
    }
}