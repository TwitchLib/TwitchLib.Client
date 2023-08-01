using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing message received event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnMessageReceivedArgs : EventArgs
    {
        /// <summary>
        /// Property representing received chat message.
        /// </summary>
        public ChatMessage ChatMessage { get; }

        public bool IsCommand => CommandInfo is not null;

        public CommandInfo CommandInfo  { get; }

        public OnMessageReceivedArgs(ChatMessage message, CommandInfo commandInfo = null)
        {
            ChatMessage = message;
            CommandInfo = commandInfo;
        }
    }
}
