using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnCommunitySubscriptionArgs.
    /// </summary>
    public class OnCommunitySubscriptionArgs
    {
        /// <summary>
        /// Property representing the information of the community subscription.
        /// </summary>
        public CommunitySubscription GiftedSubscription;
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel;
    }
}
