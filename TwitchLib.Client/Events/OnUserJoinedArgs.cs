namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing viewer joined event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnUserJoinedArgs : EventArgs
    {
        /// <summary>
        /// Property representing username of joined viewer.
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnUserJoinedArgs"/> class.
        /// </summary>
        public OnUserJoinedArgs(string channel, string username)
        {
            Channel = channel;
            Username = username;
        }
    }
}
