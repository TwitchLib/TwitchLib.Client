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
        public Announcement Announcement;
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel;
    }
}