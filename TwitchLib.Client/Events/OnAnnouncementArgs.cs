using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a USERNOTICE notifying the client that an announcemet was posted
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnAnnouncementArgs : EventArgs
    {
        /// <summary>
        /// Property representing the announcement send with the USERNOTICE
        /// </summary>
        public Announcement Announcement { get; }
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnAnnouncementArgs"/> class.
        /// </summary>
        public OnAnnouncementArgs(string channel, Announcement announcement)
        {
            Channel = channel;
            Announcement = announcement;
        }
    }
}