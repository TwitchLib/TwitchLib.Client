using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing new subscriber event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnNewSubscriberArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing subscriber object.
        /// </summary>
        public Subscriber? Subscriber { get; set; }
    }
}
