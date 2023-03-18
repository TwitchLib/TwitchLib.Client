using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on channel state changed event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnChannelStateChangedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the current channel state.
        /// </summary>
        public ChannelState ChannelState { get; set; }
    }
}
