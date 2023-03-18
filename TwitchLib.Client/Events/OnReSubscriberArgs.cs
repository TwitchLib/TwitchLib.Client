using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing resubscriber event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnReSubscriberArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing resubscriber object.
        /// </summary>
        public ReSubscriber ReSubscriber { get; set; }
    }
}
