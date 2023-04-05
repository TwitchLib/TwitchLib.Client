using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing message sent event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnMessageSentArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing a chat message that was just sent (check null on properties before using).
        /// </summary>
        public ChatMessage? SentMessage { get; set; }
    }
}
