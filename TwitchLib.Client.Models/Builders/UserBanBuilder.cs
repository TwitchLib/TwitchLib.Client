#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public sealed class UserBanBuilder : IBuilder<UserBan>, IFromIrcMessageBuilder<UserBan>
    {
        private string _channelName;
        private string _userName;
        private string _roomId;
        private string _targetUserId;

        public static UserBanBuilder Create()
        {
            return new UserBanBuilder();
        }

        private UserBanBuilder()
        {
        }

        public UserBanBuilder WithChannel(string channel)
        {
            _channelName = channel;
            return this;
        }

        public UserBanBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public UserBanBuilder WithRoomId(string roomId)
        {
            _roomId = roomId;
            return this;
        }

        public UserBanBuilder WithTargetUserId(string targetUserId)
        {
            _targetUserId = targetUserId;
            return this;
        }

        public UserBan BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new UserBan(fromIrcMessageBuilderDataObject.Message);
        }

        public UserBan Build()
        {
            return new UserBan(_channelName, _userName, _roomId, _targetUserId);
        }
    }
}
