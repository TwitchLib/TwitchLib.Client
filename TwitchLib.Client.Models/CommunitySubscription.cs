using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class CommunitySubscription : ASubscriptionBase
    {
        public int MsgParamMassGiftCount { get; }
        public int MsgParamSenderCount { get; }
        public SubscriptionPlan MsgParamSubPlan { get; }
        public string MsgParamMultiMonthGiftDuration { get; }
        [SuppressMessage("Style", "IDE0058")]
        public CommunitySubscription(IrcMessage ircMessage, ILogger logger = null) : base(ircMessage, logger)
        {
            foreach (string tag in ircMessage.Tags.Keys)
            {
                string tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
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
                    case Tags.MsgParamMassGiftCount:
                        // Suppressed IDE0058
                        Int32.TryParse(tagValue, out int massGiftCount);
                        MsgParamMassGiftCount = massGiftCount;
                        break;
                    case Tags.MsgParamSenderCount:
                        // Suppressed IDE0058
                        Int32.TryParse(tagValue, out int senderCount);
                        MsgParamSenderCount = senderCount;
                        break;
                    case Tags.MsgParamMultiMonthGiftDuration:
                        MsgParamMultiMonthGiftDuration = tagValue;
                        break;
                }
            }
        }
    }
}
