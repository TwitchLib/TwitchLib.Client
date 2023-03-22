using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public abstract class ASubscriptionBase
    {
        protected const string AnonymousGifterUserId = "274598607";
        public List<KeyValuePair<string, string>> Badges { get; }
        public List<KeyValuePair<string, string>> BadgeInfo { get; }
        public string Color { get; }
        public string DisplayName { get; }
        public string Emotes { get; }
        public string Flags { get; }
        public string Id { get; }
        public string Login { get; }
        public string MsgId { get; }
        public UserType UserType { get; }
        public string SystemMsg { get; }
        public string SystemMsgParsed { get; }
        public string TmiSentTs { get; }
        public string RoomId { get; }
        public string UserId { get; }
        public bool IsModerator { get; }
        public bool IsTurbo { get; }
        public bool IsSubscriber { get; }
        public bool IsAnonymous { get; }
        protected ASubscriptionBase(IrcMessage ircMessage, ILogger logger = null)
        {
            foreach (string tag in ircMessage.Tags.Keys)
            {
                string tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        Badges = Common.Helpers.ParseBadges(tagValue);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = Common.Helpers.ParseBadges(tagValue);
                        break;
                    case Tags.Color:
                        Color = tagValue;
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        Emotes = tagValue;
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tagValue;
                        SystemMsgParsed = tagValue.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tagValue;
                        break;
                    case Tags.Turbo:
                        IsTurbo = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        if (UserId == AnonymousGifterUserId)
                        {
                            IsAnonymous = true;
                        }
                        break;
                    case Tags.UserType:
                        switch (tagValue)
                        {
                            case "mod":
                                UserType = UserType.Moderator;
                                break;
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
        public ASubscriptionBase(List<KeyValuePair<string, string>> badges,
                                 List<KeyValuePair<string, string>> badgeInfo,
                                 string color,
                                 string displayName,
                                 string emotes,
                                 string id,
                                 string login,
                                 bool isModerator,
                                 string msgId,
                                 string roomId,
                                 bool isSubscriber,
                                 string systemMsg,
                                 string systemMsgParsed,
                                 string tmiSentTs,
                                 bool isTurbo,
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
            IsModerator = isModerator;
            MsgId = msgId;
            RoomId = roomId;
            IsSubscriber = isSubscriber;
            SystemMsg = systemMsg;
            SystemMsgParsed = systemMsgParsed;
            TmiSentTs = tmiSentTs;
            IsTurbo = isTurbo;
            UserType = userType;
            UserId = userId;
        }
    }
}
