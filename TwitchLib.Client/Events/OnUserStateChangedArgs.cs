using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing on user state changed event.</summary>
    public class OnUserStateChangedArgs : EventArgs
    {
        /// <summary>Property representing user state object.</summary>
        public UserState UserState;
    }
}
