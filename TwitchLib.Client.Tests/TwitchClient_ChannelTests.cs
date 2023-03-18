using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_ChannelTests : ATwitchClientTests<ITwitchClient_Channel>
    {
        [Fact]
        public void TwitchClient_Raises_OnJoinedChannel() { throw new NotImplementedException(); }
        [Theory]
        [InlineData("@emote-only=0;followers-only=0;r9k=0;room-id=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=1;followers-only=0;r9k=0;room-id=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=1;r9k=0;room-id=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=0;r9k=1;room-id=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=0;r9k=0;room-id=0;slow=1;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=0;r9k=0;room-id=0;slow=0;subs-only=1 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=0;r9k=0;room-id=0;slow=1;subs-only=1 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=0;r9k=1;room-id=0;slow=1;subs-only=1 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=0;followers-only=1;r9k=1;room-id=0;slow=1;subs-only=1 :tmi.twitch.tv ROOMSTATE #testchannel")]
        [InlineData("@emote-only=1;followers-only=1;r9k=1;room-id=0;slow=1;subs-only=1 :tmi.twitch.tv ROOMSTATE #testchannel")]
        public void TwitchClient_Raises_OnChannelStateChanged(string message)
        {
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChannelStateChangedArgs> assertion = Assert.Raises<OnChannelStateChangedArgs>(
                    h => client.OnChannelStateChanged += h,
                    h => client.OnChannelStateChanged -= h,
                    () =>
                    {
                        client.OnChannelStateChanged += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnLeftChannel() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnFailureToReceiveJoinConfirmation()
        {
            string message = $"@msg-id={MsgIds.MsgChannelSuspended} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This channel does not exist or has been suspended.";
            throw new NotImplementedException();
        }
    }
}
