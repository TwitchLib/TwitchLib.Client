namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a chat reply/thread</summary>
    public class ChatReply
    {
        /// <summary>
        /// The display name of the sender of the direct parent message.
        /// </summary>
        public string ParentDisplayName { get; internal set; }

        /// <summary>
        /// The text of the direct parent message.
        /// </summary>
        public string ParentMsgBody { get; internal set; }

        /// <summary>
        /// An ID that uniquely identifies the direct parent message that this message is replying to.
        /// </summary>
        public string ParentMsgId { get; internal set; }

        /// <summary>
        /// An ID that identifies the sender of the direct parent message.
        /// </summary>
        public string ParentUserId { get; internal set; }

        /// <summary>
        /// The login name of the sender of the direct parent message.
        /// </summary>
        public string ParentUserLogin { get; internal set; }

        /// <summary>
        /// An ID that uniquely identifies the top-level parent message of the reply thread that this message is replying to.
        /// </summary>
        public string ThreadParentMsgId { get; internal set; }

        /// <summary>
        /// The login name of the sender of the top-level parent message.
        /// </summary>
        public string ThreadParentUserLogin { get; internal set; }
    }
}
