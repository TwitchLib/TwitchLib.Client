using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing on channel state changed event.</summary>
    public class OnChannelStateChangedArgs : EventArgs
    {
        /// <summary>Property representing the current channel state.</summary>
        public ChannelState ChannelState;
        /// <summary>Property representing the channel received state from.</summary>
        public string Channel;
    }
}
