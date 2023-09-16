using System.Text.RegularExpressions;

namespace TwitchLib.Client.Models
{
    /// <summary>Class used to store credentials used to connect to Twitch chat/whisper.</summary>
    public partial class ConnectionCredentials
    {
#if NET7_0_OR_GREATER
        [GeneratedRegex("^([a-zA-Z0-9][a-zA-Z0-9_]{4,25})$")]
        private static partial Regex GetUsernameCheckRegex();
#else
        private static Regex GetUsernameCheckRegex() => UsernameCheckRegex;
        private static readonly Regex UsernameCheckRegex = new("^([a-zA-Z0-9][a-zA-Z0-9_]{4,25})$");
#endif
        /// <summary>Property representing bot's oauth.</summary>
        public string TwitchOAuth { get; }

        /// <summary>Property representing bot's username.</summary>
        public string TwitchUsername { get; }

        /// <summary>Property representing capability requests sent to twitch.</summary>
        public Capabilities Capabilities { get; }

        /// <summary>Constructor for ConnectionCredentials object.</summary>
        public ConnectionCredentials(
            string twitchUsername,
            string twitchOAuth,
            bool disableUsernameCheck = false,
            Capabilities? capabilities = null)
        {
            if (!disableUsernameCheck && !GetUsernameCheckRegex().Match(twitchUsername).Success)
                throw new Exception($"Twitch username does not appear to be valid. {twitchUsername}");

            TwitchUsername = twitchUsername.ToLower();
            TwitchOAuth = twitchOAuth;

            // Make sure proper formatting is applied to oauth
            if (!twitchOAuth.Contains(":"))
            {
                TwitchOAuth = $"oauth:{twitchOAuth.Replace("oauth", "")}";
            }

            Capabilities = capabilities ?? new Capabilities();
        }
    }

    /// <summary>Class used to store capacity request settings used when connecting to Twitch</summary>
    public class Capabilities
    {
        /// <summary>Adds membership state event data. By default, we do not send this data to clients without this capability.</summary>
        public bool Membership { get; }

        /// <summary>Adds IRC V3 message tags to several commands, if enabled with the commands capability.</summary>
        public bool Tags { get; }

        /// <summary>Enables several Twitch-specific commands.</summary>
        public bool Commands { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Capabilities"/> class.
        /// </summary>
        public Capabilities(bool membership = true, bool tags = true, bool commands = true)
        {
            Membership = membership;
            Tags = tags;
            Commands = commands;
        }
    }
}
