using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a NOTICE telling the client hosting failed because of a host error
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnBadHostErrorArgs : EventArgs
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
