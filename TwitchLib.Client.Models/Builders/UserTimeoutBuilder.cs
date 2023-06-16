﻿namespace TwitchLib.Client.Models.Builders
{
    public sealed class UserTimeoutBuilder : IBuilder<UserTimeout>, IFromIrcMessageBuilder<UserTimeout>
    {
        private string _channel;
        private int _timeoutDuration;
        private string _username;
        private string _targetUserId;

        private UserTimeoutBuilder()
        {
        }

        public UserTimeoutBuilder WithChannel(string channel)
        {
            _channel = channel;
            return this;
        }

        public UserTimeoutBuilder WithTimeoutDuration(int timeoutDuration)
        {
            _timeoutDuration = timeoutDuration;
            return this;
        }

        public UserTimeoutBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public UserTimeoutBuilder WithTargetUserId(string targetUserId)
        {
            _targetUserId = targetUserId;
            return this;
        }

        public static UserTimeoutBuilder Create()
        {
            return new UserTimeoutBuilder();
        }

        public UserTimeout BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new UserTimeout(fromIrcMessageBuilderDataObject.Message);
        }

        public UserTimeout Build()
        {
            return new UserTimeout(_channel, _username, _targetUserId, _timeoutDuration);
        }
    }
}
