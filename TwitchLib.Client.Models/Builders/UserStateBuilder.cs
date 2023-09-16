#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class UserStateBuilder : IBuilder<UserState>, IFromIrcMessageBuilder<UserState>
    {
        private readonly List<KeyValuePair<string, string>> _badges = new List<KeyValuePair<string, string>>();
        private readonly List<KeyValuePair<string, string>> _badgeInfo = new List<KeyValuePair<string, string>>();
        private string _channel;
        private string _color;
        private string _displayName;
        private string _emoteSet;
        private string _id;
        private bool _isModerator;
        private bool _isSubscriber;
        private UserType _userType;

        private UserStateBuilder()
        {
        }

        public UserStateBuilder WithBadges(params KeyValuePair<string, string>[] badges)
        {
            _badges.AddRange(badges);
            return this;
        }

        public UserStateBuilder WithBadgeInfos(params KeyValuePair<string, string>[] badgeInfos)
        {
            _badgeInfo.AddRange(badgeInfos);
            return this;
        }

        public UserStateBuilder WithChannel(string channel)
        {
            _channel = channel;
            return this;
        }

        public UserStateBuilder WithColorHex(string color)
        {
            _color = color;
            return this;
        }

        public UserStateBuilder WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public UserStateBuilder WithEmoteSet(string emoteSet)
        {
            _emoteSet = emoteSet;
            return this;
        }

        public UserStateBuilder Id(string id)
        {
            _id = id;
            return this;
        }
        
        public UserStateBuilder WithIsModerator(bool isModerator)
        {
            _isModerator = isModerator;
            return this;
        }

        public UserStateBuilder WithIsSubscriber(bool isSubscriber)
        {
            _isSubscriber = isSubscriber;
            return this;
        }

        public UserStateBuilder WithUserType(UserType userType)
        {
            _userType = userType;
            return this;
        }

        public static UserStateBuilder Create()
        {
            return new UserStateBuilder();
        }

        public UserState BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new UserState(fromIrcMessageBuilderDataObject.Message);
        }

        public UserState Build()
        {
            return new UserState(
                _badges,
                _badgeInfo,
                _color,
                _displayName,
                _emoteSet,
                _channel,
                _id,
                _isSubscriber,
                _isModerator,
                _userType);
        }
    }
}
