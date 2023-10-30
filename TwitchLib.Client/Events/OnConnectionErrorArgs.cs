using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing client connection error event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnConnectionErrorArgs : EventArgs
    {
        /// <summary>
        /// The error
        /// </summary>
        public ErrorEvent Error { get; }
        /// <summary>
        /// Username of the bot that suffered connection error.
        /// </summary>
        public string BotUsername { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnConnectionErrorArgs"/> class.
        /// </summary>
        public OnConnectionErrorArgs(string botUsername, ErrorEvent error)
        {
            BotUsername = botUsername;
            Error = error;
        }
    }
}
