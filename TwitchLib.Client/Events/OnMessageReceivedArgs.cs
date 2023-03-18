using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing message received event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnMessageReceivedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing received chat message.
        /// </summary>
        public ChatMessage ChatMessage { get; set; }
    }
}
