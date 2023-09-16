#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class GiftedSubscription : IHexColorProperty
    {
        private const string AnonymousGifterUserId = "274598607";

        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        /// <inheritdoc/>
        public string HexColor { get; }

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

        public DateTimeOffset TmiSent { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        public string MsgParamMultiMonthGiftDuration { get; }

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        public GiftedSubscription(IrcMessage ircMessage)
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
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.MsgParamMonths:
                        MsgParamMonths = tagValue;
                        break;
                    case Tags.MsgParamRecipientDisplayname:
                        MsgParamRecipientDisplayName = tagValue;
                        break;
                    case Tags.MsgParamRecipientId:
                        MsgParamRecipientId = tagValue;
                        break;
                    case Tags.MsgParamRecipientUsername:
                        MsgParamRecipientUserName = tagValue;
                        break;
                    case Tags.MsgParamSubPlanName:
                        MsgParamSubPlanName = tagValue;
                        break;
                    case Tags.MsgParamSubPlan:
                        MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tagValue;
                        SystemMsgParsed = tagValue.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSent = TagHelper.ToDateTimeOffsetFromUnixMs(tagValue);
                        break;
                    case Tags.Turbo:
                        IsTurbo = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        if (UserId == AnonymousGifterUserId)
                        {
                            IsAnonymous = true;
                        }
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    case Tags.MsgParamMultiMonthGiftDuration:
                        MsgParamMultiMonthGiftDuration = tagValue;
                        break;
                    default:
                        (UndocumentedTags = new()).Add(tag.Key, tag.Value);
                        break;
                }
            }
        }

        public GiftedSubscription(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            string hexColor,
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
            DateTimeOffset tmiSent,
            bool isTurbo,
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
            TmiSent = tmiSent;
            IsTurbo = isTurbo;
            UserType = userType;
            UserId = userId;
        }
    }
}
