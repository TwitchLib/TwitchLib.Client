#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class RaidNotification : IHexColorProperty
    {
        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        /// <inheritdoc/>
        public string HexColor { get; }

        public string DisplayName { get; }

        public string Emotes { get; }

        public string Id { get; }

        public string Login { get; }

        public bool Moderator { get; }

        public string MsgId { get; }

        public string MsgParamDisplayName { get; }

        public string MsgParamLogin { get; }

        public string MsgParamViewerCount { get; }

        public string RoomId { get; }

        public bool Subscriber { get; }

        public string SystemMsg { get; }

        public string SystemMsgParsed { get; }

        public DateTimeOffset TmiSent { get; }

        public bool Turbo { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        // @badges=;color=#FF0000;display-name=Heinki;emotes=;id=4fb7ab2d-aa2c-4886-a286-46e20443f3d6;login=heinki;mod=0;msg-id=raid;msg-param-displayName=Heinki;msg-param-login=heinki;msg-param-viewerCount=4;room-id=27229958;subscriber=0;system-msg=4\sraiders\sfrom\sHeinki\shave\sjoined\n!;tmi-sent-ts=1510249711023;turbo=0;user-id=44110799;user-type= :tmi.twitch.tv USERNOTICE #pandablack
        public RaidNotification(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.Color:
                        HexColor = tagValue;
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        Emotes = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        Moderator = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.MsgParamDisplayname:
                        MsgParamDisplayName = tagValue;
                        break;
                    case Tags.MsgParamLogin:
                        MsgParamLogin = tagValue;
                        break;
                    case Tags.MsgParamViewerCount:
                        MsgParamViewerCount = tagValue;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        Subscriber = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tagValue;
                        SystemMsgParsed = tagValue.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSent = TagHelper.ToDateTimeOffsetFromUnixMs(tagValue);
                        break;
                    case Tags.Turbo:
                        Turbo = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    default:
                        (UndocumentedTags = new()).Add(tag.Key, tag.Value);
                        break;
                }
            }
        }

        public RaidNotification(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            string hexColor,
            string displayName,
            string emotes,
            string id,
            string login,
            bool moderator,
            string msgId,
            string msgParamDisplayName,
            string msgParamLogin,
            string msgParamViewerCount,
            string roomId,
            bool subscriber,
            string systemMsg,
            string systemMsgParsed,
            DateTimeOffset tmiSent,
            bool turbo,
            UserType userType,
            string userId)
        {
            Badges = badges;
            BadgeInfo = badgeInfo;
            HexColor = hexColor;
            DisplayName = displayName;
            Emotes = emotes;
            Id = id;
            Login = login;
            Moderator = moderator;
            MsgId = msgId;
            MsgParamDisplayName = msgParamDisplayName;
            MsgParamLogin = msgParamLogin;
            MsgParamViewerCount = msgParamViewerCount;
            RoomId = roomId;
            Subscriber = subscriber;
            SystemMsg = systemMsg;
            SystemMsgParsed = systemMsgParsed;
            TmiSent = tmiSent;
            Turbo = turbo;
            UserType = userType;
            UserId = userId;
        }
    }
}
