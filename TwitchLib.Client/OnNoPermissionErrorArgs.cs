using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a NOTICE when the client receives generic no permission error from Twitch.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnNoPermissionErrorArgs : EventArgs
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
