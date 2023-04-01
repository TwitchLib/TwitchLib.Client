using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelpers;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    [SuppressMessage("Style", "IDE0058")]
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
            MockSequence sendMessageSequence = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence($":tmi.twitch.tv 004 {TWITCH_Username} :-", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence($":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence(userStateMessageOnJOIN, mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence(privmsgMessage, mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence(userStateMessageAfterPRIVMSG, mock, sendMessageSequence);
            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheckJOIN = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserStateChangedArgs> assertionInitial = Assert.Raises<OnUserStateChangedArgs>(
                    h => client.OnUserStateChanged += h,
                    h => client.OnUserStateChanged -= h,
                    () =>
                    {
                        client.OnUserStateChanged += (sender, args) => Assert.True(pauseCheckJOIN.Set());
                        client.JoinChannel(TWITCH_CHANNEL);
                        Assert.True(client.Connect());
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
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            MockSequence sendMessageSequence = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence($":tmi.twitch.tv 004 {TWITCH_Username} :-", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence($":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}", mock, sendMessageSequence);
            IClient communicationClient = mock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(credentials: connectionCredentials,
                                                    client: communicationClient,
                                                    sendOptions: new SendOptions(sendsAllowedInPeriod),
                                                    logger: logger);
            client.JoinChannel(TWITCH_CHANNEL);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageThrottledArgs> assertion = Assert.Raises<OnMessageThrottledArgs>(
                    h => client.OnMessageThrottled += h,
                    h => client.OnMessageThrottled -= h,
                    () =>
                    {
                        client.OnMessageThrottled += (sender, args) => Assert.True(pauseCheck.Set());
                        IClientMocker.SetIsConnected(mock, true);
                        Assert.True(client.Connect());
                        // wait to connect and join
                        Task.Delay(2000).GetAwaiter().GetResult();
                        client.SendMessage(TWITCH_CHANNEL, messageNotSent);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Reason);
            Assert.NotNull(assertion.Arguments.ItemNotSent);
            Assert.Equal(messageNotSent, assertion.Arguments.ItemNotSent.Message);
            Assert.Equal(throttlingTimeSpan, assertion.Arguments.Period);
            Assert.Equal(sendsAllowedInPeriod, assertion.Arguments.AllowedInPeriod);
        }
        [Fact]
        public void TwitchClient_Raises_OnSendFailed()
        {
            string messageNotSent = "This message has not been sent!";
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            MockSequence sendMessageSequence = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence($":tmi.twitch.tv 004 {TWITCH_Username} :-", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence($":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}", mock, sendMessageSequence);
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(false)
                .Raises(c => c.OnSendFailed += null, new OnSendFailedEventArgs() { Data = messageNotSent });
            IClient communicationClient = mock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(credentials: connectionCredentials,
                                                    client: communicationClient,
                                                    logger: logger);
            client.JoinChannel(TWITCH_CHANNEL);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnSendFailedEventArgs> assertion = Assert.Raises<OnSendFailedEventArgs>(
                    h => client.OnSendFailed += h,
                    h => client.OnSendFailed -= h,
                    () =>
                    {
                        client.OnSendFailed += (sender, args) => Assert.True(pauseCheck.Set());
                        IClientMocker.SetIsConnected(mock, true);
                        Assert.True(client.Connect());
                        // wait to connect and join
                        Task.Delay(2000).GetAwaiter().GetResult();
                        client.SendMessage(TWITCH_CHANNEL, messageNotSent);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Data);
            Assert.Equal(messageNotSent, assertion.Arguments.Data);
        }
    }
}
