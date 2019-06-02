using System;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Builders;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models.Extractors
{
    public class AnonGiftedSubscriptionExtractor : IExtractor<AnonGiftedSubscription>
    {
        public AnonGiftedSubscription Extract(IrcMessage ircMessage)
        {
            AnonGiftedSubscriptionBuilder builder = AnonGiftedSubscriptionBuilder.Create();

            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        builder.WithBadges(Common.Helpers.ParseBadges(tagValue).ToArray());
                        break;
                    case Tags.BadgeInfo:
                        builder.WithBadgeInfos(Common.Helpers.ParseBadges(tagValue).ToArray());
                        break;
                    case Tags.Color:
                        builder.WithColor(tagValue);
                        break;
                    case Tags.DisplayName:
                        builder.WithDisplayName(tagValue);
                        break;
                    case Tags.Emotes:
                        builder.WithEmotes(tagValue);
                        break;
                    case Tags.Id:
                        builder.WithId(tagValue);
                        break;
                    case Tags.Login:
                        builder.WithLogin(tagValue);
                        break;
                    case Tags.Mod:
                        builder.WithIsModerator(Common.Helpers.ConvertToBool(tagValue));
                        break;
                    case Tags.MsgId:
                        builder.WithMessageId(tagValue);
                        break;
                    case Tags.MsgParamCumulativeMonths:
                        builder.WithMsgParamCumulativeMonths(tagValue);
                        break;
                    case Tags.MsgParamRecipientDisplayname:
                        builder.WithMsgParamRecipientDisplayName(tagValue);
                        break;
                    case Tags.MsgParamRecipientId:
                        builder.WithMsgParamRecipientId(tagValue);
                        break;
                    case Tags.MsgParamRecipientUsername:
                        builder.WithMsgParamRecipientUserName(tagValue);
                        break;
                    case Tags.MsgParamSubPlanName:
                        builder.WithMsgParamSubPlanName(tagValue);
                        break;
                    case Tags.MsgParamSubPlan:
                        switch (tagValue)
                        {
                            case "prime":
                                builder.WithMsgParamSubPlan(SubscriptionPlan.Prime);
                                break;
                            case "1000":
                                builder.WithMsgParamSubPlan(SubscriptionPlan.Tier1);
                                break;
                            case "2000":
                                builder.WithMsgParamSubPlan(SubscriptionPlan.Tier2);
                                break;
                            case "3000":
                                builder.WithMsgParamSubPlan(SubscriptionPlan.Tier3);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(tagValue.ToLower));
                        }
                        break;
                    case Tags.RoomId:
                        builder.WithRoomId(tagValue);
                        break;
                    case Tags.Subscriber:
                        builder.WithIsSubscriber(Common.Helpers.ConvertToBool(tagValue));
                        break;
                    case Tags.SystemMsg:
                        builder.WithSystemMsg(tagValue);
                        builder.WithSystemMsgParsed(tagValue.Replace("\\s", " ").Replace("\\n", ""));
                        break;
                    case Tags.TmiSentTs:
                        builder.WithTmiSentTs(tagValue);
                        break;
                    case Tags.Turbo:
                        builder.WithIsTurbo(Common.Helpers.ConvertToBool(tagValue));
                        break;
                    case Tags.UserId:
                        builder.WithUserId(tagValue);
                        break;
                    case Tags.UserType:
                        switch (tagValue)
                        {
                            case "mod":
                                builder.WithUserType(UserType.Moderator);
                                break;
                            case "global_mod":
                                builder.WithUserType(UserType.GlobalModerator);
                                break;
                            case "admin":
                                builder.WithUserType(UserType.Admin);
                                break;
                            case "staff":
                                builder.WithUserType(UserType.Staff);
                                break;
                            default:
                                builder.WithUserType(UserType.Viewer);
                                break;
                        }
                        break;
                }
            }

            return builder.Build();
        }
    }
}
