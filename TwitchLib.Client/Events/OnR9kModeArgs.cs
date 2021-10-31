using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a NOTICE telling the client a message was not sent because the channel is in r9k mode
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnR9kModeArgs : EventArgs
    {
        /// <summary>
        /// Property representing message send with the NOTICE
        /// </summary>
        public string Message;
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel;
    }
}