using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnContinuedGiftedSubscriptionArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnContinuedGiftedSubscriptionArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the information of the subscription that was originally gifted, and is now continued by the user.
        /// </summary>
        public ContinuedGiftedSubscription ContinuedGiftedSubscription { get; }
        public OnContinuedGiftedSubscriptionArgs(string channel, ContinuedGiftedSubscription continuedGiftedSubscription) : base(channel)
        {
            ContinuedGiftedSubscription = continuedGiftedSubscription;
        }
    }
}