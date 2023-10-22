using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a chat reply/thread</summary>
    public class ChatReply
    {
        /// <summary>
        /// The display name of the sender of the direct parent message.
        /// </summary>
        public string ParentDisplayName { get; internal set; } = default!;

        /// <summary>
        /// The text of the direct parent message.
        /// </summary>
        public string ParentMsgBody { get; internal set; } = default!;

        /// <summary>
        /// An ID that uniquely identifies the direct parent message that this message is replying to.
        /// </summary>
        public string ParentMsgId { get; internal set; } = default!;

        /// <summary>
        /// An ID that identifies the sender of the direct parent message.
        /// </summary>
        public string ParentUserId { get; internal set; } = default!;

        /// <summary>
        /// The login name of the sender of the direct parent message.
        /// </summary>
        public string ParentUserLogin { get; internal set; } = default!;

        /// <summary>
        /// An ID that uniquely identifies the top-level parent message of the reply thread that this message is replying to.
        /// </summary>
        public string ThreadParentMsgId { get; internal set; } = default!;

        /// <summary>
        /// The login name of the sender of the top-level parent message.
        /// </summary>
        public string ThreadParentUserLogin { get; internal set; } = default!;

        internal static bool TrySetTag(ref ChatReply? reply, KeyValuePair<string, string> tag)
        {
            switch (tag.Key)
            {
                case Tags.ReplyParentDisplayName:
                    (reply ??= new()).ParentDisplayName = tag.Value;
                    break;
                case Tags.ReplyParentMsgBody:
                    (reply ??= new()).ParentMsgBody = tag.Value;
                    break;
                case Tags.ReplyParentMsgId:
                    (reply ??= new()).ParentMsgId = tag.Value;
                    break;
                case Tags.ReplyParentUserId:
                    (reply ??= new()).ParentUserId = tag.Value;
                    break;
                case Tags.ReplyParentUserLogin:
                    (reply ??= new()).ParentUserLogin = tag.Value;
                    break;
                case Tags.ReplyThreadParentMsgId:
                    (reply ??= new()).ThreadParentMsgId = tag.Value;
                    break;
                case Tags.ReplyThreadParentUserLogin:
                    (reply ??= new()).ThreadParentUserLogin = tag.Value;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
