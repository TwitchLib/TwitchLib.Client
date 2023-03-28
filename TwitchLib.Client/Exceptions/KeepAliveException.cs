using System;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    ///     <see cref="Exception"/> for that case, that Twicht-Server failed to receive a KeepAlive-Message.
    /// </summary>
    public class KeepAliveException : Exception
    {
        /// <summary>
        ///     <see langword="default"/> Message
        /// </summary>
        private static readonly string MESSAGE = "Twitch-Server failed to receive KeepAlive-Message (PONG). Twitch-Server terminates the connection.";
        /// <summary>
        ///     Reference to the TwitchAPI
        /// </summary>
        public string APIReference => "https://dev.twitch.tv/docs/irc/#keepalive-messages";
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
            builder.AppendLine($"{nameof(APIReference)}: {APIReference}");
            return builder.ToString();
        }
    }
}
