using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnAnonGiftedSubscriptionArgs : EventArgs
    {
        /// <summary>Property representing the information of the gifted subscription.</summary>
        public AnonGiftedSubscription AnonGiftedSubscription;
        /// <summary>Property representing the Twitch channel this event fired from.</summary>
        public string Channel;
    }
}