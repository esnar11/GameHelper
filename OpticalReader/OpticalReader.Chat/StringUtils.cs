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
    }
}
