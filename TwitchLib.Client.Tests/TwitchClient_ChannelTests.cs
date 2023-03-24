using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
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
            throw new NotImplementedException("has to be modified to do the whole login sequence etc");
            string message = "@emote-only=0;followers-only=0;r9k=0;room-id=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #testchannel";
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
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            AssertChannel(assertion.Arguments);
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
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnFailureToReceiveJoinConfirmationArgs> assertion = Assert.Raises<OnFailureToReceiveJoinConfirmationArgs>(
                    h => client.OnFailureToReceiveJoinConfirmation += h,
                    h => client.OnFailureToReceiveJoinConfirmation -= h,
                    () =>
                    {
                        client.OnFailureToReceiveJoinConfirmation += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        communicationClient.Send(String.Empty);
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
            mock.Setup(c => c.IsConnected).Returns(true);
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
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnJoinedChannelArgs> assertion = Assert.Raises<OnJoinedChannelArgs>(
                    h => client.OnJoinedChannel += h,
                    h => client.OnJoinedChannel -= h,
                    () =>
                    {
                        client.OnJoinedChannel += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        communicationClient.Send(String.Empty);
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
            mock.Setup(c => c.IsConnected).Returns(true);
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
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnLeftChannelArgs> assertion = Assert.Raises<OnLeftChannelArgs>(
                    h => client.OnLeftChannel += h,
                    h => client.OnLeftChannel -= h,
                    () =>
                    {
                        client.OnJoinedChannel += (sender, args) => client.LeaveChannel(TWITCH_CHANNEL);
                        client.OnLeftChannel += (sender, args) => Assert.True(pauseCheck.Set());

                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        communicationClient.Send(String.Empty);
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
