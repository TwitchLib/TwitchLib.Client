using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnRaidNotificationArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OnRaidNotificationArgs : EventArgs
    {
        /// <summary>
        /// The raid notification
        /// </summary>
        public RaidNotification RaidNotification { get; }
        /// <summary>
        /// The channel
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnRaidNotificationArgs"/> class.
        /// </summary>
        public OnRaidNotificationArgs(string channel, RaidNotification raidNotification)
        {
            Channel = channel;
            RaidNotification = raidNotification;
        }
    }
}
