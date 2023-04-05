using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnGiftedSubscriptionArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnGiftedSubscriptionArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the information of the gifted subscription.
        /// </summary>
        public GiftedSubscription? GiftedSubscription { get; set; }
    }
}
