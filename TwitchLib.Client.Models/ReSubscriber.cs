using System.Collections.Generic;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ReSubscriber : SubscriberBase
    {
        public int Months { get; }

        public ReSubscriber(IrcMessage ircMessage) : base(ircMessage) {
            Months = months;
        }
        public ReSubscriber(List<KeyValuePair<string, string>> badges, string colorHex, Color color, string displayName, string emoteSet, string id, string login, string systemMessage,
            string systemMessageParsed, string resubMessage, SubscriptionPlan subscriptionPlan, string subscriptionPlanName, string roomId, string userId, bool isModerator, bool isTurbo,
            bool isSubscriber, bool isPartner, string tmiSentTs, UserType userType, string rawIrc, string channel) : base(badges, colorHex, color, displayName, emoteSet, id, login, systemMessage,
                systemMessageParsed, resubMessage, subscriptionPlan, subscriptionPlanName, roomId, userId, isModerator, isTurbo, isSubscriber, isPartner, tmiSentTs, userType, rawIrc, channel) { }
    }
}
