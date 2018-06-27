using System;
using System.Text.RegularExpressions;
#if NET452
using System.Net;
#endif

namespace TwitchLib.Client.Models
{
    /// <summary>Class used to store credentials used to connect to Twitch chat/whisper.</summary>
    public class ConnectionCredentials
    {        
#if NET452
        /// <summary>Property representing Twitch's host port</summary>
        public int TwitchPort { get; }
        /// <summary>Property representing Twitch's host address</summary>
        public string TwitchHost { get; }
        /// <summary>Property IP/port of a proxy.</summary>
        public IPEndPoint Proxy { get; }
#endif
        /// <summary>Property representing bot's username.</summary>
        public string TwitchUsername { get; }
        /// <summary>Property representing bot's oauth.</summary>
        public string TwitchOAuth { get; }

        /// <summary>Constructor for ConnectionCredentials object.</summary>
#if NET452
        public ConnectionCredentials(string twitchUsername, string twitchOAuth, string twitchHost = "irc-ws.chat.twitch.tv", int twitchPort = 443, bool disableUsernameCheck = false, string proxyIP = null, int? proxyPort = null)

#elif NETSTANDARD
        public ConnectionCredentials(string twitchUsername, string twitchOAuth, bool disableUsernameCheck = false)
#endif
        {
            if (!disableUsernameCheck && !new Regex("^([a-zA-Z0-9][a-zA-Z0-9_]{3,25})$").Match(twitchUsername).Success)
                throw new Exception($"Twitch username does not appear to be valid. {twitchUsername}");
            TwitchUsername = twitchUsername.ToLower();
            TwitchOAuth = twitchOAuth;
            // Make sure proper formatting is applied to oauth
            if (!twitchOAuth.Contains(":"))
            {
                TwitchOAuth = $"oauth:{twitchOAuth.Replace("oauth", "")}";
            }
#if NET452
            TwitchHost = twitchHost;
            TwitchPort = twitchPort;

            if (proxyIP != null && proxyPort != null)
                Proxy = new IPEndPoint(IPAddress.Parse(proxyIP), (int)proxyPort);
#endif
        }

    }
}
