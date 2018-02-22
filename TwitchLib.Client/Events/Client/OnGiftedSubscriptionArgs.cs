using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events.Client
{
    public class OnGiftedSubscriptionArgs : EventArgs
    {
        public GiftedSubscription GiftedSubscription;
    }
}
