using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    ///     Exception representing credentials provided for logging in were bad.
    ///     Implements the <see cref="System.Exception" />
    /// </summary>
    public class ErrorLoggingInException : Exception, IAPIReferencesProvider
    {
        public IEnumerable<string> APIReferences => new[] { "https://dev.twitch.tv/docs/irc/authenticate-bot/" };
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
        [SuppressMessage("Style", "IDE0058")]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{nameof(Username)}: {Username}");
            builder.AppendLine($"{nameof(Message)}: {Message}");
            builder.AppendLine($"{nameof(APIReferences)}: {String.Join(", ", APIReferences)}");
            return builder.ToString();
        }
    }
}