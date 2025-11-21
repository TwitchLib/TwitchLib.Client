using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class UserTimeout
    {
        /// <summary>Channel that had timeout event.</summary>
        public string Channel { get; }

        /// <summary>Duration of timeout</summary>
        public TimeSpan TimeoutDuration { get; }

        /// <summary>Viewer that was timed out.</summary>
        public string Username { get; }

        /// <summary>Id of Viewer that was timed out.</summary>
        public string TargetUserId { get; } = default!;

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTimeout"/> class.
        /// </summary>
        public UserTimeout(IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            Username = ircMessage.Message;

            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;

                switch (tag.Key)
                {
                    case Tags.BanDuration:
                        TimeoutDuration = TimeSpan.FromSeconds(int.Parse(tagValue));
                        break;
                    case Tags.TargetUserId:
                        TargetUserId = tagValue;
                        break;
                    default:
                        (UndocumentedTags ??= new()).Add(tag.Key, tag.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTimeout"/> class.
        /// </summary>
        public UserTimeout(
            string channel,
            string username,
            string targetUserId,
            TimeSpan timeoutDuration)
        {
            Channel = channel;
            Username = username;
            TargetUserId = targetUserId;
            TimeoutDuration = timeoutDuration;
        }
    }
}
