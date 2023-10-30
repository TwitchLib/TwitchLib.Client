namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a cleared chat event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnChatClearedArgs : EventArgs
    {
        /// <summary>
        /// Channel that had chat cleared event.
        /// </summary>
        public string Channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnChatClearedArgs"/> class.
        /// </summary>
        public OnChatClearedArgs(string channel)
        {
            Channel = channel;
        }
    }
}
