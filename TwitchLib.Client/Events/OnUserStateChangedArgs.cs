using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on user state changed event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserStateChangedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing user state object.
        /// </summary>
        public UserState? UserState { get; set; }
    }
}
