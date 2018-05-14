using System.Net;
using System.Text.RegularExpressions;
using TwitchLib.Client.Exceptions;

namespace TwitchLib.Client.Models
{
    /// <summary>Class used to store credentials used to connect to Twitch chat/whisper.</summary>
    public class ConnectionCredentials
    {        
        /// <summary>Property IP/port of a proxy.</summary>
        public IPEndPoint Proxy { get; }
        /// <summary>Property representing Twitch's host address</summary>
        public string TwitchHost { get; }
        /// <summary>Property representing bot's oauth.</summary>
        public string TwitchOAuth { get; internal set; }
        /// <summary>Property representing Twitch's host port</summary>
        public int TwitchPort { get; }
        /// <summary>Property representing bot's username.</summary>
        public string TwitchUsername { get; }
        

        /// <summary>Constructor for ConnectionCredentials object.</summary>
        public ConnectionCredentials(string twitchUsername, string twitchOAuth, string twitchHost = "irc-ws.chat.twitch.tv", int twitchPort = 80, bool disableUsernameCheck = false, string proxyIP = null, int? proxyPort = null)
        {
            if (!disableUsernameCheck && !new Regex("^([a-zA-Z0-9][a-zA-Z0-9_]{3,25})$").Match(twitchUsername).Success)
                throw new ErrorLoggingInException("Twitch username does not appear to be valid.", twitchUsername);
            TwitchUsername = twitchUsername.ToLower();
            TwitchOAuth = twitchOAuth;
            TwitchHost = twitchHost;
            TwitchPort = twitchPort;

            if (proxyIP != null && proxyPort != null)
                Proxy = new IPEndPoint(IPAddress.Parse(proxyIP), (int)proxyPort);
        }
    }
}
