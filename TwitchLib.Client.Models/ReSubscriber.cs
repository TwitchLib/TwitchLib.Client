using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ReSubscriber : SubscriberBase
    {
        public int Months => monthsInternal;

        public ReSubscriber(IrcMessage ircMessage) : base(ircMessage)
        {
        }

        public ReSubscriber(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            string hexColor,
            string displayName,
            string emoteSet,
            string id,
            string login,
            string systemMessage,
            string msgId,
            string msgParamCumulativeMonths,
            string msgParamStreakMonths,
            bool msgParamShouldShareStreak,
            string systemMessageParsed,
            string resubMessage,
            SubscriptionPlan subscriptionPlan,
            string subscriptionPlanName,
            string roomId,
            string userId,
            bool isModerator,
            bool isTurbo,
            bool isSubscriber,
            bool isPartner,
            DateTimeOffset tmiSent,
            UserType userType,
            string rawIrc,
            string channel,
            int months = 0)
            : base(badges,
                  badgeInfo,
                  hexColor,
                  displayName,
                  emoteSet,
                  id,
                  login,
                  systemMessage,
                  msgId,
                  msgParamCumulativeMonths,
                  msgParamStreakMonths,
                  msgParamShouldShareStreak,
                  systemMessageParsed,
                  resubMessage,
                  subscriptionPlan,
                  subscriptionPlanName,
                  roomId,
                  userId,
                  isModerator,
                  isTurbo,
                  isSubscriber,
                  isPartner,
                  tmiSent,
                  userType,
                  rawIrc,
                  channel,
                  months)
        {
        }
    }
}
