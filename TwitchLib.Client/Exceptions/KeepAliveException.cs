using System;
using System.Collections.Generic;
using System.Text;

using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    ///     <see cref="Exception"/> for that case, that Twicht-Server failed to receive a KeepAlive-Message.
    /// </summary>
    public class KeepAliveException : Exception, IAPIReferencesProvider
    {
        /// <summary>
        ///     <see langword="default"/> Message
        /// </summary>
        private static readonly string MESSAGE = "Twitch-Server failed to receive KeepAlive-Message (PONG). Twitch-Server terminates the connection.";
        public IEnumerable<string> APIReferences => new[] { "https://dev.twitch.tv/docs/irc/#keepalive-messages" };
        /// <summary>
        ///     Bot-Username
        /// </summary>
        public string Username { get; }
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="username">
        ///     Bot-Username
        /// </param>
        public KeepAliveException(string username) : base(MESSAGE) { Username = username; }
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
