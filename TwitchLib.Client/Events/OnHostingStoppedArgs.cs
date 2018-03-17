using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing hosting stopped event.</summary>
    public class OnHostingStoppedArgs : EventArgs
    {
        /// <summary>Property representing hosting channel.</summary>
        public HostingStopped HostingStopped;
    }
}
