using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing an event where another channel has started hosting the broadcaster's channel.</summary>
    public class OnBeingHostedArgs : EventArgs
    {
        /// <summary>Property representing the Host notification</summary>
        public BeingHostedNotification BeingHostedNotification;
    }
}
