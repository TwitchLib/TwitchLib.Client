using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing moderator leave event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnModeratorLeftArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing username of moderator that left..
        /// </summary>
        public string Username { get; }
        public OnModeratorLeftArgs(string channel, string username) : base(channel)
        {
            Username = username;
        }
    }
}
