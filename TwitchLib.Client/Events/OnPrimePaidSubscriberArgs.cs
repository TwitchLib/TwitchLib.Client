using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing prime gaming sub -> paid sub event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnPrimePaidSubscriberArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing prime gaming -> paid subscriber object.
        /// </summary>
        public PrimePaidSubscriber? PrimePaidSubscriber { get; set; }
    }
}
