using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing moderator joined event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnModeratorJoinedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing username of joined moderator.
        /// </summary>
        public string? Username { get; set; }
    }
}
