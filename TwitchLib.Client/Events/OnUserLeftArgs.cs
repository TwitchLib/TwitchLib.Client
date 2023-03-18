using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing viewer left event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserLeftArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing username of user that left.
        /// </summary>
        public string Username { get; set; }
    }
}
