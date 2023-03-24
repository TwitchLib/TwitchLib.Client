using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_MessageSendingTests : ATwitchClientTests<ITwitchClient_MessageSending>
    {
        /// <summary>
        ///     <seealso cref="Models.JoinedChannelTests.UserState_And_SentMessage_Test(String, String, Int32)"/>
        ///     <br></br>
        ///     <br></br>
        ///     from the test mentioned above,
        ///     <br></br>
        ///     we know,
        ///     <br></br>
        ///     if someone else sends a <see cref="IrcCommand.PrivMsg"/>,
        ///     <br></br>
        ///     it is not added to <see cref="JoinedChannel.BotMessages"/>,
        ///     <br></br>
        ///     whats not added, cant be raised...
        /// </summary>
        [Fact]
        public void TwitchClient_Raises_OnMessageSent()
        {
            string userStateMessageOnJOIN = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv USERSTATE #{TWITCH_CHANNEL}";
            string messageId = "some_message_id_hash";
            string privmsgMessage = $"@badge-info=subscriber/22;badges=subscriber/18,bits/1000;client-nonce=a_hash;color=#1E90FF;display-name=testuser;emote-only=1;emotes=1:0-1,8-9/555555584:4-5;first-msg=0;flags=;id={messageId};mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=1;user-type= :{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv PRIVMSG #{TWITCH_CHANNEL} ::)  <3  :)";
            string userStateMessageAfterPRIVMSG = $"@id={messageId} :{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv USERSTATE #{TWITCH_CHANNEL}";
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            MockSequence sendMessageSequnce = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence($":tmi.twitch.tv 004 {TWITCH_Username} :-", mock, sendMessageSequnce);
            IClientMocker.AddToSendMessageSequence($":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}", mock, sendMessageSequnce);
            IClientMocker.AddToSendMessageSequence(userStateMessageOnJOIN, mock, sendMessageSequnce);
            IClientMocker.AddToSendMessageSequence(privmsgMessage, mock, sendMessageSequnce);
            IClientMocker.AddToSendMessageSequence(userStateMessageAfterPRIVMSG, mock, sendMessageSequnce);
            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheckJOIN = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserStateChangedArgs> assertionInitial = Assert.Raises<OnUserStateChangedArgs>(
                    h => client.OnUserStateChanged += h,
                    h => client.OnUserStateChanged -= h,
                    () =>
                    {
                        client.OnUserStateChanged += (sender, args) => Assert.True(pauseCheckJOIN.Set());
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth), TWITCH_CHANNEL);
                        client.Connect();
                        // lets give the ITwitchClient some time to handle auth and autojoin ...
                        Task.Delay(2000).GetAwaiter().GetResult();
                        // we have to cheat a bit
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheckJOIN.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertionInitial.Arguments);
            AssertChannel(assertionInitial.Arguments);
            Assert.NotNull(assertionInitial.Arguments.UserState);

            client.SendRaw(privmsgMessage);

            ManualResetEvent pauseCheckUpdate = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageSentArgs> assertionUpdate = Assert.Raises<OnMessageSentArgs>(
                    h => client.OnMessageSent += h,
                    h => client.OnMessageSent -= h,
                    () =>
                    {
                        client.OnMessageSent += (sender, args) => Assert.True(pauseCheckUpdate.Set());
                        // we have to cheat a bit
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheckUpdate.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertionUpdate.Arguments);
            AssertChannel(assertionUpdate.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnMessageThrottled()
        {
            uint sendsAllowedInPeriod = 0;
            TimeSpan throttlingTimeSpan = TimeSpan.FromSeconds(30);
            string messageNotSent = "This message has not been sent!";
            string reason = "Some reason";

            OnThrottledEventArgs onThrottledEventArgs = new OnThrottledEventArgs()
            {
                AllowedInPeriod = sendsAllowedInPeriod,
                SentCount = sendsAllowedInPeriod,
                Period = throttlingTimeSpan,
                ItemNotSent = messageNotSent,
                Reason = reason
            };

            OnMessageThrottledEventArgs onMessageThrottledEventArgs = new OnMessageThrottledEventArgs(onThrottledEventArgs);

            Mock<IClient> clientMock = new Mock<IClient>();

            clientMock.SetupAdd(c => c.OnMessageThrottled += It.IsAny<EventHandler<OnMessageThrottledEventArgs>>());
            clientMock.Setup(c => c.Send(It.IsAny<string>()))
                .Returns(false)
                .Raises(c => c.OnMessageThrottled += null, onMessageThrottledEventArgs);

            IClient communicationClient = clientMock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageThrottledEventArgs> assertion = Assert.Raises<OnMessageThrottledEventArgs>(
                    h => client.OnMessageThrottled += h,
                    h => client.OnMessageThrottled -= h,
                    () =>
                    {
                        client.OnMessageThrottled += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        communicationClient.Send(messageNotSent);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Reason);

            Assert.Equal(messageNotSent, assertion.Arguments.ItemNotSent);
            Assert.Equal(reason, assertion.Arguments.Reason);
            Assert.Equal(throttlingTimeSpan, assertion.Arguments.Period);
            Assert.Equal(sendsAllowedInPeriod, assertion.Arguments.AllowedInPeriod);
        }
    }
}
