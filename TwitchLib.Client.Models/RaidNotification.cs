using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class RaidNotification
    {
        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        public Color Color { get; }

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

        public string TmiSentTs { get; }

        public bool Turbo { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        // @badges=;color=#FF0000;display-name=Heinki;emotes=;id=4fb7ab2d-aa2c-4886-a286-46e20443f3d6;login=heinki;mod=0;msg-id=raid;msg-param-displayName=Heinki;msg-param-login=heinki;msg-param-viewerCount=4;room-id=27229958;subscriber=0;system-msg=4\sraiders\sfrom\sHeinki\shave\sjoined\n!;tmi-sent-ts=1510249711023;turbo=0;user-id=44110799;user-type= :tmi.twitch.tv USERNOTICE #pandablack
        public RaidNotification(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags)
            {
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.Color:
                        Color = TagHelper.ToColor(tag.Value);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tag.Value;
                        break;
                    case Tags.Emotes:
                        Emotes = tag.Value;
                        break;
                    case Tags.Login:
                        Login = tag.Value;
                        break;
                    case Tags.Mod:
                        Moderator = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.MsgId:
                        MsgId = tag.Value;
                        break;
                    case Tags.MsgParamDisplayname:
                        MsgParamDisplayName = tag.Value;
                        break;
                    case Tags.MsgParamLogin:
                        MsgParamLogin = tag.Value;
                        break;
                    case Tags.MsgParamViewerCount:
                        MsgParamViewerCount = tag.Value;
                        break;
                    case Tags.RoomId:
                        RoomId = tag.Value;
                        break;
                    case Tags.Subscriber:
                        Subscriber = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tag.Value;
                        SystemMsgParsed = tag.Value.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tag.Value;
                        break;
                    case Tags.Turbo:
                        Turbo = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.UserId:
                        UserId = tag.Value;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                }
            }
        }

        public RaidNotification(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            Color color,
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
            string tmiSentTs,
            bool turbo,
            UserType userType,
            string userId)
        {
            Badges = badges;
            BadgeInfo = badgeInfo;
            Color = color;
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
            TmiSentTs = tmiSentTs;
            Turbo = turbo;
            UserType = userType;
            UserId = userId;
        }
    }
}
