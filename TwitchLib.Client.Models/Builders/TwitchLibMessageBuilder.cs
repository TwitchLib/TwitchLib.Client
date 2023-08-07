#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class TwitchLibMessageBuilder : TwitchLibMessage, IBuilder<TwitchLibMessage>
    {
        private TwitchLibMessageBuilder()
        {
        }

        public TwitchLibMessageBuilder WithBadges(List<KeyValuePair<string, string>> badges)
        {
            Badges = badges;
            return this;
        }

        public TwitchLibMessageBuilder WithColorHex(string color)
        {
            HexColor = color;
            return this;
        }

        public TwitchLibMessageBuilder WithUsername(string username)
        {
            Username = username;
            return this;
        }

        public TwitchLibMessageBuilder WithDisplayName(string displayName)
        {
            DisplayName = displayName;
            return this;
        }

        public TwitchLibMessageBuilder WithEmoteSet(EmoteSet emoteSet)
        {
            EmoteSet = emoteSet;
            return this;
        }

        public TwitchLibMessageBuilder WithUserId(string userId)
        {
            UserId = userId;
            return this;
        }

        public TwitchLibMessageBuilder WithIsTurbo(bool isTurbo)
        {
            IsTurbo = isTurbo;
            return this;
        }

        public TwitchLibMessageBuilder WithBotUserName(string botUserName)
        {
            BotUsername = botUserName;
            return this;
        }

        public TwitchLibMessageBuilder WithUserType(UserType userType)
        {
            UserType = userType;
            return this;
        }

        public static TwitchLibMessageBuilder Create()
        {
            return new TwitchLibMessageBuilder();
        }

        public TwitchLibMessage Build()
        {
            return this;
        }
    }
}
