using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a USERNOTICE notifying the client that an announcemet was posted
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnAnnouncementArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the announcement send with the USERNOTICE
        /// </summary>
        public Announcement Announcement { get; }
        public OnAnnouncementArgs(string channel, Announcement announcement) : base(channel)
        {
            Announcement = announcement;
        }
    }
}