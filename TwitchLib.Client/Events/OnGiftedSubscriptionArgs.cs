using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnGiftedSubscriptionArgs : EventArgs
    {
        /// <summary>Property representing the information of the gifted subscription.</summary>
        public GiftedSubscription GiftedSubscription;
        /// <summary>Property representing the Twitch channel this event fired from.</summary>
        public string Channel;
    }
}
