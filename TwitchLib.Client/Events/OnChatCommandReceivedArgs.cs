using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing chat command received event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnChatCommandReceivedArgs : OnMessageReceivedArgs
    {
        /// <summary>
        /// Property representing received command.
        /// </summary>
        public CommandInfo Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnChatCommandReceivedArgs"/> class.
        /// </summary>
        public OnChatCommandReceivedArgs(ChatMessage message, CommandInfo commandInfo) : base(message)
        {
            Command = commandInfo;
        }
    }
}