#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class CommunitySubscription : IHexColorProperty
    {
        private const string AnonymousGifterUserId = "274598607";

        public List<KeyValuePair<string, string>> Badges;
        public List<KeyValuePair<string, string>> BadgeInfo;
        
        /// <inheritdoc/>
        public string HexColor { get; }
        public string DisplayName;
        public string Emotes;
        public string Id;
        public string Login;
        public bool IsModerator;
        public bool IsAnonymous;
        public string MsgId;
        public int MsgParamMassGiftCount;
        public int MsgParamSenderCount;
        public SubscriptionPlan MsgParamSubPlan;
        public string RoomId;
        public bool IsSubscriber;
        public string SystemMsg;
        public string SystemMsgParsed;
        public DateTimeOffset TmiSent;
        public bool IsTurbo;
        public string UserId;
        public UserType UserType;
        public string MsgParamMultiMonthGiftDuration;

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        public CommunitySubscription(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags)
            {
                var (tagKey, tagValue) = (tag.Key, tag.Value);
                switch (tagKey)
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
                    case Tags.MsgParamSubPlan:
                        MsgParamSubPlan  = TagHelper.ToSubscriptionPlan(tag.Value);
                        break;
                    case Tags.MsgParamMassGiftCount:
                        MsgParamMassGiftCount = int.Parse(tag.Value);
                        break;
                    case Tags.MsgParamSenderCount:
                        MsgParamSenderCount = int.Parse(tag.Value);
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
                        if(UserId == AnonymousGifterUserId)
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
    }
}
