using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing whisper command received event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnWhisperCommandReceivedArgs : OnWhisperReceivedArgs
    {
        /// <summary>
        /// Property representing received command.
        /// </summary>
        public CommandInfo Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnWhisperCommandReceivedArgs"/> class.
        /// </summary>
        public OnWhisperCommandReceivedArgs(WhisperMessage message, CommandInfo commandInfo) : base(message)
        {
            Command = commandInfo;
        }
    }
}
