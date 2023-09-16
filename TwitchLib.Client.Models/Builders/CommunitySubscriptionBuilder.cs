#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public sealed class CommunitySubscriptionBuilder : IFromIrcMessageBuilder<CommunitySubscription>
    {
        private CommunitySubscriptionBuilder()
        {
        }

        public static CommunitySubscriptionBuilder Create()
        {
            return new CommunitySubscriptionBuilder();
        }

        public CommunitySubscription BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new CommunitySubscription(fromIrcMessageBuilderDataObject.Message);
        }
    }
}
