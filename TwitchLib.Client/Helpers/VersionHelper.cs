using System;
using System.Reflection;

namespace TwitchLib.Client.Helpers
{
    internal static class VersionHelper
    {
        public static Version AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version;
    }
}
