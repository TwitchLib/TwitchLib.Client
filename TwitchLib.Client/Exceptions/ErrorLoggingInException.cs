using System;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    ///     Exception representing credentials provided for logging in were bad.
    ///     Implements the <see cref="System.Exception" />
    /// </summary>
    public class ErrorLoggingInException : Exception
    {
        /// <summary>
        ///     Reference to TwitchAPI
        /// </summary>
        public string APIReference => "https://dev.twitch.tv/docs/irc/authenticate-bot/";
        /// <summary>
        ///     Exception representing username associated with bad login.
        /// </summary>
        public string Username { get; protected set; }

        /// <summary>
        ///     Exception construtor.
        /// </summary>
        /// <param name="ircData">
        ///     The irc data.
        /// </param>
        /// <param name="twitchUsername">
        ///     The twitch username.
        /// </param>
        public ErrorLoggingInException(string ircData, string twitchUsername) : base(ircData)
        {
            Username = twitchUsername;
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{nameof(Username)}: {Username}");
            builder.AppendLine($"{nameof(Message)}: {Message}");
            builder.AppendLine($"{nameof(APIReference)}: {APIReference}");
            return builder.ToString();
        }
    }
}