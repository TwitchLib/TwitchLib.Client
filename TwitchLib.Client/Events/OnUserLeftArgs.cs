namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing viewer left event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnUserLeftArgs : EventArgs
    {
        /// <summary>
        /// Property representing username of user that left.
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnUserLeftArgs"/> class.
        /// </summary>
        public OnUserLeftArgs(string channel, string username)
        {
            Channel = channel;
            Username = username;
        }
    }
}
