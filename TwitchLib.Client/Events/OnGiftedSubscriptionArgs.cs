using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnGiftedSubscriptionArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OnGiftedSubscriptionArgs : EventArgs
    {
        /// <summary>
        /// Property representing the information of the gifted subscription.
        /// </summary>
        public GiftedSubscription GiftedSubscription { get; }
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnGiftedSubscriptionArgs"/> class.
        /// </summary>
        public OnGiftedSubscriptionArgs(string channel, GiftedSubscription giftedSubscription)
        {
            Channel = channel;
            GiftedSubscription = giftedSubscription;
        }
    }
}
