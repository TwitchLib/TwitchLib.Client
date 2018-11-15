using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnCommunitySubscriptionArgs
    {
        /// <summary>Property representing the information of the community subscription.</summary>
        public CommunitySubscription GiftedSubscription;
        /// <summary>Property representing the Twitch channel this event fired from.</summary>
        public string Channel;
    }
}
