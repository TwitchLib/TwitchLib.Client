namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a successful chat color change request.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnChatColorChangedArgs : EventArgs
    {
        /// <summary>
        /// Property reprenting the channel the event was received in.
        /// </summary>
        public string Channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnChatColorChangedArgs"/> class.
        /// </summary>
        public OnChatColorChangedArgs(string channel)
        {
            Channel = channel;
        }
    }
}
