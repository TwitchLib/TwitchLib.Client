using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing new subscriber event.</summary>
    public class OnNewSubscriberArgs : EventArgs
    {
        /// <summary>Property representing subscriber object.</summary>
        public Subscriber Subscriber;
        /// <summary>Property representing the Twitch channel this event fired from.</summary>
        public string Channel;
    }
}
