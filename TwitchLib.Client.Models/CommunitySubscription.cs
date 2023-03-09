using System;
using System.Collections.Generic;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
#pragma warning disable 1591
    public class CommunitySubscription
    {
        private const string AnonymousGifterUserId = "274598607";

        public List<KeyValuePair<string, string>> Badges { get; }
        public List<KeyValuePair<string, string>> BadgeInfo { get; }
        public string Color { get; }
        public string DisplayName { get; }
        public string Emotes { get; }
        public string Id { get; }
        public string Login { get; }
        public bool IsModerator { get; }
        public bool IsAnonymous { get; }
        public string MsgId { get; }
        public int MsgParamMassGiftCount { get; }
        public int MsgParamSenderCount { get; }
        public SubscriptionPlan MsgParamSubPlan { get; }
        public string RoomId { get; }
        public bool IsSubscriber { get; }
        public string SystemMsg { get; }
        public string SystemMsgParsed { get; }
        public string TmiSentTs { get; }
        public bool IsTurbo { get; }
        public string UserId { get; }
        public UserType UserType { get; }
        public string MsgParamMultiMonthGiftDuration { get; }

        public CommunitySubscription(IrcMessage ircMessage)
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
                    case Tags.MsgParamSubPlan:
                        switch (tagValue)
                        {
                            case "prime":
                                MsgParamSubPlan = SubscriptionPlan.Prime;
                                break;
                            case "1000":
                                MsgParamSubPlan = SubscriptionPlan.Tier1;
                                break;
                            case "2000":
                                MsgParamSubPlan = SubscriptionPlan.Tier2;
                                break;
                            case "3000":
                                MsgParamSubPlan = SubscriptionPlan.Tier3;
                                break;
                            case "":
                                break;
                            default:
                                // TODO: rather logging than throwing an exception
                                throw new ArgumentOutOfRangeException(nameof(tagValue));
                        }
                        break;
                    case Tags.MsgParamMassGiftCount:
                        Int32.TryParse(tagValue, out int massGiftCount);
                        MsgParamMassGiftCount = massGiftCount;
                        break;
                    case Tags.MsgParamSenderCount:
                        Int32.TryParse(tagValue, out int senderCount);
                        MsgParamSenderCount = senderCount;
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
                    case Tags.MsgParamMultiMonthGiftDuration:
                        MsgParamMultiMonthGiftDuration = tagValue;
                        break;
                }
            }
        }
    }
#pragma warning restore 1591
}
