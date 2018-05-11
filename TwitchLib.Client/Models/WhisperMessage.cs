#if NETSTANDARD
using TwitchLib.Client.Extensions.NetCore;
#endif
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TwitchLib.Client.Common;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

#if NET452

#endif

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a received whisper from TwitchWhisperClient</summary>
    public class WhisperMessage
    {
        /// <summary>Property representing dynamic badges assigned to message.</summary>
        public List<KeyValuePair<string, string>> Badges { get; }
        /// <summary>Property representing bot's username.</summary>
        public string BotUsername { get; }
        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public Color Color { get; }
        /// <summary>Property representing HEX representation of color of username.</summary>
        public string ColorHex { get; }
        /// <summary>Property representing sender DisplayName (can be null/blank).</summary>
        public string DisplayName { get; }
        /// <summary>Property representing list of string emotes in message.</summary>
        public EmoteSet EmoteSet { get; }
        /// <summary>Property representing whether or not sender has Turbo.</summary>
        public bool IsTurbo { get; }
        /// <summary>Property representing message contents.</summary>
        public string Message { get; }
        /// <summary>Property representing message identifier.</summary>
        public string MessageId { get; }
        /// <summary>Property representing identifier of the message thread.</summary>
        public string ThreadId { get; }
        /// <summary>Property representing sender identifier.</summary>
        public string UserId { get; }
        /// <summary>Property representing sender Username.</summary>
        public string Username { get; }
        /// <summary>Property representing user type of sender.</summary>
        public UserType UserType { get; }

        internal WhisperMessage(List<KeyValuePair<string, string>> badges, string colorHex, Color color, string username, string displayName, EmoteSet emoteSet, string threadId, string messageId,
            string userId, bool isTurbo, string botUsername, string message, UserType userType)
        {
            Badges = badges;
            ColorHex = colorHex;
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

            Message = ircMessage.Message;
            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];
                switch (tag)
                {
                    case Tags.Badges:
                        Badges = new List<KeyValuePair<string, string>>();
                        if (tagValue.Contains('/'))
                        {
                            if (!tagValue.Contains(","))
                                Badges.Add(new KeyValuePair<string, string>(tagValue.Split('/')[0], tagValue.Split('/')[1]));
                            else
                                foreach (var badge in tagValue.Split(','))
                                    Badges.Add(new KeyValuePair<string, string>(badge.Split('/')[0], badge.Split('/')[1]));
                        }
                        break;
                    case Tags.Color:
                        ColorHex = tagValue;
                        if (!string.IsNullOrEmpty(ColorHex))
                            Color = ColorTranslator.FromHtml(ColorHex);
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
                        IsTurbo = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        switch (tagValue)
                        {
                            case "global_mod":
                                UserType = UserType.GlobalModerator;
                                break;
                            case "admin":
                                UserType = UserType.Admin;
                                break;
                            case "staff":
                                UserType = UserType.Staff;
                                break;
                            default:
                                UserType = UserType.Viewer;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
