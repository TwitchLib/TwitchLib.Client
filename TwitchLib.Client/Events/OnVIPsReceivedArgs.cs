using System;
using System.Collections.Generic;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a list of VIPs received from chat.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnVIPsReceivedArgs : EventArgs
    {
        /// <summary>
        /// Property representing the channel the VIPs array came from.
        /// </summary>
        public string Channel;
        /// <summary>
        /// Property representing an array of VIPs.
        /// </summary>
        public string[] VIPs;
    }
}
