using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a received whisper from TwitchWhisperClient</summary>
    public class WhisperMessage : TwitchLibMessage
    {
        /// <summary>Property representing message identifier.</summary>
        public string MessageId { get; }

        /// <summary>Property representing identifier of the message thread.</summary>
        public string ThreadId { get; }

        /// <summary>Property representing identifier of the message thread.</summary>
        public string Message { get; }

        public WhisperMessage(
            List<KeyValuePair<string, string>> badges,
            Color color,
            string username,
            string displayName,
            EmoteSet emoteSet,
            string threadId,
            string messageId,
            string userId,
            bool isTurbo,
            string botUsername,
            string message,
            UserType userType)
        {
            Badges = badges;
            Color = color;
            Username = username;
            DisplayName = displayName;
            EmoteSet = emoteSet;
            ThreadId = threadId;
            MessageId = messageId;
            UserId = userId;
            IsTurbo = isTurbo;
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
            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.Color:
                        Color = TagHelper.ToColor(tagValue);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        EmoteSet = new EmoteSet(tag.Value, Message);
                        break;
                    case Tags.MessageId:
                        MessageId = tagValue;
                        break;
                    case Tags.ThreadId:
                        ThreadId = tagValue;
                        break;
                    case Tags.Turbo:
                        IsTurbo = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                }
            }

            EmoteSet ??= new EmoteSet(default(string), Message);
        }
    }
}
