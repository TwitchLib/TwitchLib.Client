using System;
using System.Net;
using System.Text.RegularExpressions;

namespace TwitchLib.Client.Models
{
    /// <summary>Class used to store credentials used to connect to Twitch chat/whisper.</summary>
    public class ConnectionCredentials
    {
        /// <summary>Property representing URI used to connect to Twitch websocket service.</summary>
        public string TwitchWebsocketURI { get; }
        /// <summary>Property representing bot's oauth.</summary>
        public string TwitchOAuth { get; }
        /// <summary>Property representing bot's username.</summary>
        public string TwitchUsername { get; }
        

        /// <summary>Constructor for ConnectionCredentials object.</summary>
        public ConnectionCredentials(string twitchUsername, string twitchOAuth, string twitchWebsocketURI = "wss://irc-ws.chat.twitch.tv:443", bool disableUsernameCheck = false)
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
            TwitchWebsocketURI = twitchWebsocketURI;
        }
        
    }
}
