using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a cleared chat event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnChatClearedArgs : AChannelProvidingEventArgs
    {
        public OnChatClearedArgs(string channel) : base(channel) { }
    }
}
