using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing viewer joined event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserJoinedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing username of joined viewer.
        /// </summary>
        public string Username { get; }
        public OnUserJoinedArgs(string channel, string username) : base(channel)
        {
            Username = username;
        }
    }
}
