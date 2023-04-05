using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing community subscription received event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnCommunitySubscriptionArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the information of the community subscription.
        /// </summary>
        public CommunitySubscription? GiftedSubscription { get; set; }
    }
}
