using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a cleared message event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnMessageClearedArgs : AChannelProvidingEventArgs
    {

        /// <summary>
        ///     Message contents that received clear message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Message ID representing the message that was cleared
        /// </summary>
        public string TargetMessageId { get; set; }

        /// <summary>
        ///     Timestamp of when message was sent
        /// </summary>
        public string TmiSentTs { get; set; }
        public OnMessageClearedArgs(string channel, string message, string targetMessageId, string tmiSentTs) : base(channel)
        {
            Message = message;
            TargetMessageId = targetMessageId;
            TmiSentTs = tmiSentTs;
        }
    }
}
