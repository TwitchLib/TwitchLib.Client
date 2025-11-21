using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing on channel state changed event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnChannelStateChangedArgs : EventArgs
    {
        /// <summary>
        /// Property representing the current channel state.
        /// </summary>
        public ChannelState ChannelState { get; }
        /// <summary>
        /// Property representing the channel received state from.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnChannelStateChangedArgs"/> class.
        /// </summary>
        public OnChannelStateChangedArgs(string channel, ChannelState channelState)
        {
            Channel = channel;
            ChannelState = channelState;
        }
    }
}
