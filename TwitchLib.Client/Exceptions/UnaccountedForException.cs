using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    /// Exception representing values the client was not expecting
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <inheritdoc />
    public class UnaccountedForException : Exception
    {
        /// <summary>
        /// Bot username that had the exception.
        /// </summary>
        /// <value>The bot username.</value>
        public string BotUsername { get; protected set; }

        /// <summary>
        /// Channel client was joined in
        /// </summary>
        /// <value>The channel the the unaccounted for value was sent in</value>
        public string Channel { get; protected set; }

        /// <summary>
        /// Location within source code that had unaccounted for 
        /// </summary>
        /// <value>Source code location</value>
        public string Location { get; protected set; }

        /// <summary>
        /// The raw IRC as received from Twitch
        /// </summary>
        /// <value>Raw IRC</value>
        public string RawIRC { get; protected set; }

        public UnaccountedForException(string botUsername, string channel, string location, string rawIrc) : base("See raw IRC.")
        {
            BotUsername = botUsername;
            Channel = channel;
            Location = location;
            RawIRC = rawIrc;
        }
    }
}
