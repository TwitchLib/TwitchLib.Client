#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public sealed class ConnectionCredentialsBuilder : IBuilder<ConnectionCredentials>
    {
        private string _twitchUsername;
        private string _twitchOAuth;
        private bool _disableUsernameCheck;

        private ConnectionCredentialsBuilder()
        {
        }

        public ConnectionCredentialsBuilder WithTwitchUsername(string twitchUsername)
        {
            _twitchUsername = twitchUsername;
            return this;
        }

        public ConnectionCredentialsBuilder WithTwitchOAuth(string twitchOAuth)
        {
            _twitchOAuth = twitchOAuth;
            return this;
        }

        public ConnectionCredentialsBuilder WithDisableUsernameCheck(bool disableUsernameCheck)
        {
            _disableUsernameCheck = disableUsernameCheck;
            return this;
        }

        public static ConnectionCredentialsBuilder Create()
        {
            return new ConnectionCredentialsBuilder();
        }

        public ConnectionCredentials Build()
        {
            return new ConnectionCredentials(
                _twitchUsername,
                _twitchOAuth,
                disableUsernameCheck: _disableUsernameCheck);
        }
    }
}
