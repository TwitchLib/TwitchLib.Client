using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a NOTICE telling the client the newly raided channel is mature audience only.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnRaidedChannelIsMatureAudienceArgs : EventArgs
    {
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel;

        /// <summary>
        /// Property representing message send with the NOTICE
        /// </summary>
        public string Message;
    }
}
