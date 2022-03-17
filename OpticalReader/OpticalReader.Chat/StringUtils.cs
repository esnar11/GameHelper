using System;
using System.Linq;

namespace OpticalReader.Chat
{
    internal static class StringUtils
    {
        public static bool Contains_IgnoreCase(this string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;

            var ss1 = s1.ToLowerInvariant();
            var ss2 = s2.ToLowerInvariant();
            return ss1.Contains(ss2);
        }

        public static double EqualsRatio(this string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                return 1;

            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            var c1 = s1.Where(char.IsLetterOrDigit).Sum(ch => ch);
            var c2 = s2.Where(char.IsLetterOrDigit).Sum(ch => ch);
            var min = Math.Min(c1, c2);
            var max = Math.Max(c1, c2);
            return (double)min / max;
        }
    }
}
