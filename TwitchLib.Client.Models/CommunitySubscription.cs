using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class CommunitySubscription
    {
        private const string AnonymousGifterUserId = "274598607";

        public List<KeyValuePair<string, string>> Badges;
        public List<KeyValuePair<string, string>> BadgeInfo;
        public Color Color;
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
        public string TmiSentTs;
        public bool IsTurbo;
        public string UserId;
        public UserType UserType;
        public string MsgParamMultiMonthGiftDuration;

        public CommunitySubscription(IrcMessage ircMessage)
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
                        if(UserId == AnonymousGifterUserId)
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
    }
}
