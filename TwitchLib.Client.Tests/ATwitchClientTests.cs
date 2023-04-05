using TwitchLib.Client.Events.Abstracts;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public abstract class ATwitchClientTests<T>
    {
        /// <summary>
        ///     has to lower-case
        /// </summary>
        protected static string TWITCH_Username => "username";
        /// <summary>
        ///     has to lower-case
        /// </summary>
        protected static string TWITCH_UsernameAnother => "username_another";
        protected static string TWITCH_OAuth => "OAuth";
        /// <summary>
        ///     has to lower-case
        /// </summary>
        protected static string TWITCH_CHANNEL => "testchannel";
        protected static int WaitOneDuration => 5_000;
        protected static int WaitOneDurationShort => 500;

        protected void AssertChannel(AChannelProvidingEventArgs eventArgs)
        {
            Assert.Equal(TWITCH_CHANNEL, eventArgs.Channel);
        }
    }
}
