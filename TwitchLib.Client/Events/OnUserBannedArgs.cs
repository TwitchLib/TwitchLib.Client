using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a user was banned event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserBannedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     The user ban
        /// </summary>
        public UserBan? UserBan { get; set; }
    }
}
