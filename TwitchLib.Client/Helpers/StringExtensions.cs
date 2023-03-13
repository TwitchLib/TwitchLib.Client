using System;

namespace TwitchLib.Client.Helpers
{
    internal static class StringExtensions
    {
        public static bool IsNullOrEmptyOrWhitespace(this string s)
        {
            return String.IsNullOrEmpty(s) || String.IsNullOrWhiteSpace(s);
        }
    }
}
