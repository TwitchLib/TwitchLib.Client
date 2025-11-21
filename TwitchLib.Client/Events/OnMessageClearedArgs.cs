namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a cleared message event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnMessageClearedArgs : EventArgs
    {
        /// <summary>
        /// Channel that had message cleared event.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Message contents that received clear message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Message ID representing the message that was cleared
        /// </summary>
        public string TargetMessageId { get; }

        /// <summary>
        /// Timestamp of when message was sent
        /// </summary>
        public DateTimeOffset TmiSent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnMessageClearedArgs"/> class.
        /// </summary>
        public OnMessageClearedArgs(string channel, string message, string targetMessageId, DateTimeOffset tmiSent)
        {
            Channel = channel;
            Message = message;
            TargetMessageId = targetMessageId;
            TmiSent = tmiSent;
        }
    }
}
