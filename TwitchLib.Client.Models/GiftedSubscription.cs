using System;
using System.Collections.Generic;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class GiftedSubscription
    {
        public string Badges { get; }
        public string Color { get; }
        public string DisplayName { get; }
        public string Emotes { get; }
        public string Id { get; }
        public bool IsModerator { get; }
        public bool IsSubscriber { get; }
        public bool IsTurbo { get; }
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

        public GiftedSubscription(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        Badges = tagValue;
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
                            default:
                                throw new ArgumentOutOfRangeException(nameof(tagValue.ToLower));
                        }
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
        public GiftedSubscription(string badges, string color, string displayName, string emotes, string id, string login, bool isModerator,
            string msgId, string msgParamMonths, string msgParamRecipientDisplayName, string msgParamRecipientId, string msgParamRecipientUserName,
            string msgParamSubPlanName, SubscriptionPlan msgParamSubPlan, string roomId, bool isSubscriber, string systemMsg, string systemMsgParsed,
            string tmiSentTs, bool isTurbo, UserType userType)
        {
            Badges = badges;
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
            RoomId = roomId;
            IsSubscriber = isSubscriber;
            SystemMsg = systemMsg;
            SystemMsgParsed = systemMsgParsed;
            TmiSentTs = tmiSentTs;
            IsTurbo = isTurbo;
            UserType = userType;
        }
    }
}
