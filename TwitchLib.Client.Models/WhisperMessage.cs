using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a received whisper from TwitchWhisperClient</summary>
    public class WhisperMessage : TwitchLibMessage
    {
        /// <summary>Property representing message identifier.</summary>
        public string MessageId { get; } = default!;

        /// <summary>Property representing identifier of the message thread.</summary>
        public string ThreadId { get; } = default!;

        /// <summary>Property representing identifier of the message thread.</summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhisperMessage"/> class.
        /// </summary>
        public WhisperMessage(
            List<KeyValuePair<string, string>> badges,
            string hexColor,
            string username,
            string displayName,
            EmoteSet emoteSet,
            string threadId,
            string messageId,
            string userId,
            string botUsername,
            string message,
            UserDetail userDetail,
            UserType userType)
        {
            Badges = badges;
            HexColor = hexColor;
            Username = username;
            DisplayName = displayName;
            EmoteSet = emoteSet;
            ThreadId = threadId;
            MessageId = messageId;
            UserId = userId;
            UserDetail = userDetail;
            BotUsername = botUsername;
            Message = message;
            UserType = userType;
        }

        /// <summary>
        /// WhisperMessage constructor.
        /// </summary>
        /// <param name="ircMessage">Received IRC string from Twitch server.</param>
        /// <param name="botUsername">Active bot username receiving message.</param>
        public WhisperMessage(IrcMessage ircMessage, string botUsername)
        {
            Username = ircMessage.User;
            BotUsername = botUsername;
            RawIrcMessage = ircMessage.ToString();

            Message = ircMessage.Message;
            var userDetails = UserDetails.None;
            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.Color:
                        HexColor = tagValue;
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        EmoteSet = new EmoteSet(tagValue, Message);
                        break;
                    case Tags.MessageId:
                        MessageId = tagValue;
                        break;
                    case Tags.ThreadId:
                        ThreadId = tagValue;
                        break;
                    case Tags.Turbo:
                        if (TagHelper.ToBool(tag.Value))
                            userDetails |= UserDetails.Turbo;
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    default:
                        (UndocumentedTags ??= new()).Add(tag.Key, tag.Value);
                        break;
                }
            }
            UserDetail = new(userDetails, Badges);

            EmoteSet ??= new EmoteSet(default(string), Message);
        }
    }
}
