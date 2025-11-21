namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing on channel joined event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnJoinedChannelArgs : EventArgs
    {
        /// <summary>
        /// Property representing bot username.
        /// </summary>
        public string BotUsername { get; }
        /// <summary>
        /// Property representing the channel that was joined.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnJoinedChannelArgs"/> class.
        /// </summary>
        public OnJoinedChannelArgs(string channel, string botUsername)
        {
            Channel = channel;
            BotUsername = botUsername;
        }
    }
}
