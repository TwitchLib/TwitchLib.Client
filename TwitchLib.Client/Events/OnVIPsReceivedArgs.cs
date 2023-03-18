using System.Collections.Generic;

using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a list of VIPs received from chat.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnVIPsReceivedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the list of VIPs.
        /// </summary>
        public ICollection<string> VIPs { get; set; }
    }
}
