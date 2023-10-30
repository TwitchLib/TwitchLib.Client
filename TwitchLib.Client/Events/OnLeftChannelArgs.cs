namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing the client left a channel event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnLeftChannelArgs : EventArgs
    {
        /// <summary>
        /// The username of the bot that left the channel.
        /// </summary>
        public string BotUsername { get; }
        /// <summary>
        /// Channel that bot just left (parted).
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnLeftChannelArgs"/> class.
        /// </summary>
        public OnLeftChannelArgs(string channel, string botUsername)
        {
            Channel = channel;
            BotUsername = botUsername;
        }
    }
}
