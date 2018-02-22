using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnGiftedSubscriptionArgs : EventArgs
    {
        public GiftedSubscription GiftedSubscription;
    }
}
