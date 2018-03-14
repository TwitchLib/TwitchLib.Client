using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing hosting started event.</summary>
    public class OnHostingStartedArgs : EventArgs
    {
        /// <summary>Property representing hosting channel.</summary>
        public HostingStarted HostingStarted;
    }
}
