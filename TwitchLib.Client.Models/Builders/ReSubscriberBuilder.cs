using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class ReSubscriberBuilder : SubscriberBaseBuilder, IBuilder<ReSubscriber>, IFromIrcMessageBuilder<ReSubscriber>
    {
        private ReSubscriberBuilder()
        {
        }

        public static new ReSubscriberBuilder Create()
        {
            return new ReSubscriberBuilder();
        }

        public ReSubscriber BuildFromIrcMessage(IrcMessage ircMessage)
        {
            return new ReSubscriber(ircMessage);
        }

        ReSubscriber IBuilder<ReSubscriber>.Build()
        {
            return (ReSubscriber)Build();
        }

        public override SubscriberBase Build()
        {
            return new ReSubscriber(
                Badges,
                ColorHex,
                Color,
                DisplayName,
                EmoteSet,
                Id,
                Login,
                SystemMessage,
                ParsedSystemMessage,
                ResubMessage,
                SubscriptionPlan,
                SubscriptionPlanName,
                RoomId,
                UserId,
                IsModerator,
                IsTurbo,
                IsSubscriber,
                IsPartner,
                TmiSentTs,
                UserType,
                RawIrc,
                Channel,
                Months);
        }
    }
}
