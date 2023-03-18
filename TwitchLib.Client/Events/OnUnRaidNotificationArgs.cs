using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnRaidNotificationArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUnRaidNotificationArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     The raid notification
        /// </summary>
        public RaidNotification RaidNotification { get; set; }
    }
}
