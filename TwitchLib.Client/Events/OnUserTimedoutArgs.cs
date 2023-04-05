using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a user was timed out event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserTimedoutArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     The user timeout
        /// </summary>
        public UserTimeout? UserTimeout { get; set; }
    }
}
