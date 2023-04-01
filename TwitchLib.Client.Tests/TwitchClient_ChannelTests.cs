using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Tests.TestHelpers;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    [SuppressMessage("Style", "IDE0058")]
    public class TwitchClient_ChannelTests : ATwitchClientTests<ITwitchClient_Channel>
    {
        /// <summary>
        ///     only tests, if <see cref="ITwitchClient.OnChannelStateChanged"/> gets raised
        ///     <br></br>
        ///     <br></br>
        ///     see also
        ///     <br></br>
        ///     <seealso cref="Models.JoinedChannelTests"/>
        ///     <br></br>
        ///     <seealso cref="TwitchLib.Client.Models.Tests.ChannelStateTests"/>
        /// </summary>
        [Fact]
        public void TwitchClient_Raises_OnChannelStateChanged()
        {
            string channelStateInitialMessage = $"@emote-only=1;followers-only=0;r9k=0;room-id=0;slow=60;subs-only=1 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}";
            string channelStateUpdateMessage = $"@followers-only=-1;room-id=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}";
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            MockSequence sendMessageSequence = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence($":tmi.twitch.tv 004 {TWITCH_Username} :-", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence($":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}", mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence(channelStateInitialMessage, mock, sendMessageSequence);
            IClientMocker.AddToSendMessageSequence(channelStateUpdateMessage, mock, sendMessageSequence);
            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheckInitial = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChannelStateChangedArgs> assertionInitial = Assert.Raises<OnChannelStateChangedArgs>(
                    h => client.OnChannelStateChanged += h,
                    h => client.OnChannelStateChanged -= h,
                    () =>
                    {
                        client.OnChannelStateChanged += (sender, args) => Assert.True(pauseCheckInitial.Set());
                        client.JoinChannel(TWITCH_CHANNEL);
                        Assert.True(client.Connect());
                        // lets give the ITwitchClient some time to handle auth and autojoin ...
                        Task.Delay(2000).GetAwaiter().GetResult();
                        // we have to cheat a bit
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheckInitial.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertionInitial.Arguments);
            AssertChannel(assertionInitial.Arguments);
            Assert.NotNull(assertionInitial.Arguments.ChannelState);
            ChannelState channelStateInitial = assertionInitial.Arguments.ChannelState;
            Assert.Equal(TimeSpan.FromMinutes(0), channelStateInitial.FollowersOnly);
            Assert.True(channelStateInitial.EmoteOnly);
            Assert.False(channelStateInitial.R9K);
            Assert.Equal(60, channelStateInitial.SlowMode);
            Assert.True(channelStateInitial.SubOnly);

            ManualResetEvent pauseCheckUpdate = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChannelStateChangedArgs> assertionUpdate = Assert.Raises<OnChannelStateChangedArgs>(
                    h => client.OnChannelStateChanged += h,
                    h => client.OnChannelStateChanged -= h,
                    () =>
                    {
                        client.OnChannelStateChanged += (sender, args) => Assert.True(pauseCheckUpdate.Set());
                        // we have to cheat a bit
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheckUpdate.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertionUpdate.Arguments);
            AssertChannel(assertionUpdate.Arguments);
            Assert.NotNull(assertionUpdate.Arguments.ChannelState);
            ChannelState? channelStateUpdate = assertionUpdate.Arguments.ChannelState;
            Assert.Null(channelStateUpdate.FollowersOnly);
            Assert.Equal(channelStateInitial.EmoteOnly, channelStateUpdate.EmoteOnly);
            Assert.Equal(channelStateInitial.R9K, channelStateUpdate.R9K);
            Assert.Equal(channelStateInitial.SlowMode, channelStateUpdate.SlowMode);
            Assert.Equal(channelStateInitial.SubOnly, channelStateUpdate.SubOnly);
        }
        [Fact]
        public void TwitchClient_Raises_OnFailureToReceiveJoinConfirmation()
        {
            Mock<IClient> mock = new Mock<IClient>();
            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            MockSequence sequence = new MockSequence();
            // to make the ITwitchClient call ChannelManager.Start()
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":tmi.twitch.tv 004 {TWITCH_Username} :-" });


            string message = $"@msg-id={MsgIds.MsgChannelSuspended} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This channel does not exist or has been suspended.";
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = message });

            IClient communicationClient = mock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnFailureToReceiveJoinConfirmationArgs> assertion = Assert.Raises<OnFailureToReceiveJoinConfirmationArgs>(
                    h => client.OnFailureToReceiveJoinConfirmation += h,
                    h => client.OnFailureToReceiveJoinConfirmation -= h,
                    () =>
                    {
                        client.OnFailureToReceiveJoinConfirmation += (sender, args) => Assert.True(pauseCheck.Set());
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        Assert.True(communicationClient.Send(String.Empty));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Exception);
            Assert.NotNull(assertion.Arguments.Channel);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnJoinedChannel()
        {
            Mock<IClient> mock = new Mock<IClient>();
            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            MockSequence sequence = new MockSequence();
            // to make the ITwitchClient call ChannelManager.Start()
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":tmi.twitch.tv 004 {TWITCH_Username} :-" });

            // after calling IClient.JoinChannel([...]), the ChannelManager should send "JOIN #[...]"
            // and we want to raise the Join-Confirmation-Message, that we recveive from twitch
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.Is<string>(s => String.Equals($"JOIN #{TWITCH_CHANNEL}", s))))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}" });

            IClient communicationClient = mock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnJoinedChannelArgs> assertion = Assert.Raises<OnJoinedChannelArgs>(
                    h => client.OnJoinedChannel += h,
                    h => client.OnJoinedChannel -= h,
                    () =>
                    {
                        client.OnJoinedChannel += (sender, args) => Assert.True(pauseCheck.Set());
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        Assert.True(communicationClient.Send(String.Empty));
                        client.JoinChannel(TWITCH_CHANNEL);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnLeftChannel()
        {
            Mock<IClient> mock = new Mock<IClient>();
            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            MockSequence sequence = new MockSequence();
            // to make the ITwitchClient call ChannelManager.Start()
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":tmi.twitch.tv 004 {TWITCH_Username} :-" });

            // after calling IClient.JoinChannel([...]), the ChannelManager should send "JOIN #[...]"
            // and we want to raise the Join-Confirmation-Message, that we recveive from twitch
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.Is<string>(s => String.Equals($"JOIN #{TWITCH_CHANNEL}", s))))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}" });

            // 
            mock.InSequence(sequence)
                .Setup(c => c.Send(It.Is<string>(s => String.Equals($"PART #{TWITCH_CHANNEL}", s))))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv PART #{TWITCH_CHANNEL}" });

            IClient communicationClient = mock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnLeftChannelArgs> assertion = Assert.Raises<OnLeftChannelArgs>(
                    h => client.OnLeftChannel += h,
                    h => client.OnLeftChannel -= h,
                    () =>
                    {
                        client.OnJoinedChannel += (sender, args) => client.LeaveChannel(TWITCH_CHANNEL);
                        client.OnLeftChannel += (sender, args) => Assert.True(pauseCheck.Set());
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        Assert.True(communicationClient.Send(String.Empty));
                        client.JoinChannel(TWITCH_CHANNEL);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            AssertChannel(assertion.Arguments);

        }
    }
}
