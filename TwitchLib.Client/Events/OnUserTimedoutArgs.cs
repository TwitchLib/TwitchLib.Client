using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing a user was timed out event.</summary>
    public class OnUserTimedoutArgs : EventArgs
    {
        public UserTimeout UserTimeout;
    }
}
