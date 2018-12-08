using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing a list of VIPs received from chat.</summary>
    public class OnVIPsReceivedArgs : EventArgs
    {
        /// <summary>Property representing the channel the VIPs list came from.</summary>
        public string Channel;
        /// <summary>Property representing the list of VIPs.</summary>
        public List<string> VIPs;
    }
}
