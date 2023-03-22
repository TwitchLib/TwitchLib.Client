using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class GiftedSubscription : ASubscriptionBase
    {

        public string MsgParamMonths { get; }

        public string MsgParamRecipientDisplayName { get; }

        public string MsgParamRecipientId { get; }

        public string MsgParamRecipientUserName { get; }

        public string MsgParamSubPlanName { get; }

        public SubscriptionPlan MsgParamSubPlan { get; }

        public string MsgParamMultiMonthGiftDuration { get; }

        public GiftedSubscription(IrcMessage ircMessage, ILogger logger = null) : base(ircMessage, logger)
        {
            foreach (string tag in ircMessage.Tags.Keys)
            {
                string tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
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
                            case "":
                                break;
                            default:
                                Exception ex = new ArgumentOutOfRangeException(nameof(tagValue),
                                                                               tagValue,
                                                                               $"switch-case and/or {nameof(Enums.SubscriptionPlan)} have/has to be extended.");
                                logger?.LogExceptionAsError(GetType(), ex);
                                break;
                        }
                        break;
                    case Tags.MsgParamMultiMonthGiftDuration:
                        MsgParamMultiMonthGiftDuration = tagValue;
                        break;
                }
            }
        }

        public GiftedSubscription(List<KeyValuePair<string, string>> badges,
                                  List<KeyValuePair<string, string>> badgeInfo,
                                  string color,
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
                                  string userId) : base(badges,
                                                        badgeInfo,
                                                        color,
                                                        displayName,
                                                        emotes,
                                                        id,
                                                        login,
                                                        isModerator,
                                                        msgId,
                                                        roomId,
                                                        isSubscriber,
                                                        systemMsg,
                                                        systemMsgParsed,
                                                        tmiSentTs,
                                                        isTurbo,
                                                        userType,
                                                        userId)
        {
            MsgParamMonths = msgParamMonths;
            MsgParamRecipientDisplayName = msgParamRecipientDisplayName;
            MsgParamRecipientId = msgParamRecipientId;
            MsgParamRecipientUserName = msgParamRecipientUserName;
            MsgParamSubPlanName = msgParamSubPlanName;
            MsgParamSubPlan = msgParamSubPlan;
            MsgParamMultiMonthGiftDuration = msgMultiMonthDuration;
        }
    }
}
