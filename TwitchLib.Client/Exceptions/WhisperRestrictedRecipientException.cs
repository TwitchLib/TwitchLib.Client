using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    /// Exception representing Twitch telling client it is unable to send whispers to the target user id
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <inheritdoc />
    public class WhisperRestrictedRecipientException : Exception
    {
        /// <summary>
        /// Bot username that had the exception.
        /// </summary>
        /// <value>The bot username.</value>
        public string BotUsername { get; protected set; }

        /// <summary>
        /// Userid of user that was sent the whisper.
        /// </summary>
        /// <value>The bot username.</value>
        public string TargetUserId { get; protected set; }

        /// <summary>
        /// The raw IRC as received from Twitch
        /// </summary>
        /// <value>Raw IRC</value>
        public string RawIRC { get; protected set; }

        public WhisperRestrictedRecipientException(string botUsername, string targetUserId, string rawIrc) : base("See raw IRC.")
        {
            BotUsername = botUsername;
            TargetUserId = targetUserId;
            RawIRC = rawIrc;
        }
    }
}
