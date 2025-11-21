namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing client disconnect event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnDisconnectedArgs : EventArgs
    {
        /// <summary>
        /// Username of the bot that was disconnected.
        /// </summary>
        public string BotUsername { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDisconnectedArgs"/> class.
        /// </summary>
        public OnDisconnectedArgs(string botUsername)
        {
            BotUsername = botUsername;
        }
    }
}
