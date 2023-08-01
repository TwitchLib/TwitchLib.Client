using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnWhisperReceivedArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnWhisperReceivedArgs : EventArgs
    {
        /// <summary>
        /// The whisper message
        /// </summary>
        public WhisperMessage WhisperMessage { get; }

        public bool IsCommand => CommandInfo is not null;

        public CommandInfo CommandInfo { get; }

        public OnWhisperReceivedArgs(WhisperMessage message, CommandInfo commandInfo = null)
        {
            WhisperMessage = message;
            CommandInfo = commandInfo;
        }
    }
}
