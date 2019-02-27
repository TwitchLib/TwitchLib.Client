using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnAnonGiftedSubscriptionArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OnAnonGiftedSubscriptionArgs : EventArgs
    {
        /// <summary>
        /// Property representing the information of the gifted subscription.
        /// </summary>
        public AnonGiftedSubscription AnonGiftedSubscription;
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel;
    }
}