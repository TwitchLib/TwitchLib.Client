using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class GiftedSubscription
    {
        private const string AnonymousGifterUserId = "274598607";

        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        public Color Color { get; }

        public string DisplayName { get; }

        public string Emotes { get; }

        public string Id { get; }

        public bool IsModerator { get; }

        public bool IsSubscriber { get; }

        public bool IsTurbo { get; }

        public bool IsAnonymous { get; }

        public string Login { get; }

        public string MsgId { get; }

        public string MsgParamMonths { get; }

        public string MsgParamRecipientDisplayName { get; }

        public string MsgParamRecipientId { get; }

        public string MsgParamRecipientUserName { get; }

        public string MsgParamSubPlanName { get; }

        public SubscriptionPlan MsgParamSubPlan { get; }

        public string RoomId { get; }

        public string SystemMsg { get; }

        public string SystemMsgParsed { get; }

        public string TmiSentTs { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        public string MsgParamMultiMonthGiftDuration { get; }

        public GiftedSubscription(IrcMessage ircMessage)
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
                    case Tags.Id:
                        Id = tag.Value;
                        break;
                    case Tags.Login:
                        Login = tag.Value;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.MsgId:
                        MsgId = tag.Value;
                        break;
                    case Tags.MsgParamMonths:
                        MsgParamMonths = tag.Value;
                        break;
                    case Tags.MsgParamRecipientDisplayname:
                        MsgParamRecipientDisplayName = tag.Value;
                        break;
                    case Tags.MsgParamRecipientId:
                        MsgParamRecipientId = tag.Value;
                        break;
                    case Tags.MsgParamRecipientUsername:
                        MsgParamRecipientUserName = tag.Value;
                        break;
                    case Tags.MsgParamSubPlanName:
                        MsgParamSubPlanName = tag.Value;
                        break;
                    case Tags.MsgParamSubPlan:
                        MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                        break;
                    case Tags.RoomId:
                        RoomId = tag.Value;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tag.Value;
                        SystemMsgParsed = tag.Value.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tag.Value;
                        break;
                    case Tags.Turbo:
                        IsTurbo = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.UserId:
                        UserId = tag.Value;
                        if (UserId == AnonymousGifterUserId)
                        {
                            IsAnonymous = true;
                        }
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    case Tags.MsgParamMultiMonthGiftDuration:
                        MsgParamMultiMonthGiftDuration = tag.Value;
                        break;
                }
            }
        }

        public GiftedSubscription(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            Color color,
            string displayName,
            string emotes,
            string id,
            string login,
            bool isModerator,
            string msgId,
            string msgParamMonths,
            string msgParamRecipientDisplayName,
            string msgParamRecipientId,
            string msgParamRecipientUserName,
            string msgParamSubPlanName,
            string msgMultiMonthDuration,
            SubscriptionPlan msgParamSubPlan,
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
            MsgParamMonths = msgParamMonths;
            MsgParamRecipientDisplayName = msgParamRecipientDisplayName;
            MsgParamRecipientId = msgParamRecipientId;
            MsgParamRecipientUserName = msgParamRecipientUserName;
            MsgParamSubPlanName = msgParamSubPlanName;
            MsgParamSubPlan = msgParamSubPlan;
            MsgParamMultiMonthGiftDuration = msgMultiMonthDuration;
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
