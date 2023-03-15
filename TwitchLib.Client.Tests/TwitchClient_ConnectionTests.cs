using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.Helpers;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Tests.Helper;

using Xunit;

namespace TwitchLib.Client.Tests
{

    public class TwitchClient_ConnectionTests : ATwitchClientTests<ITwitchClient_Connection>
    {
        public TwitchClient_ConnectionTests() { }
        /// <summary>
        ///     <see cref="ITwitchClient"/> raises <see cref="ITwitchClient_Connection.OnConnected"/>
        ///     if it receives an <see cref="Models.Internal.IrcMessage"/>
        ///     with <see cref="IrcCommand.RPL_004"/>
        ///     as <see cref="Models.Internal.IrcMessage.Command"/>
        ///     <br></br>
        ///     <br></br>
        ///     this test case also checks, that ITwitchClient.Client_OnConnected is added to IClient.OnConnected
        /// </summary>
        [Fact]
        public void TwitchClient_Raises_OnConnected_Advanced()
        {
            string message = $":tmi.twitch.tv 004 {TWITCH_Username} :-";
            Mock<IClient> mock = new Mock<IClient>();

            // make IClient-Mock raise OnConnectedEventArgs when Open() is called
            // that should trigger ITwitchClient to send Credentials (we want to test it)
            // ITwitchClient's call to IClient.Send() makes IClient-Mock raise the message above
            // and ITwitchClient.OnConnected should be raise (we want to test it)
            mock.SetupAdd(c => c.OnConnected += It.IsAny<EventHandler<OnConnectedEventArgs>>());
            mock.Setup<bool>(c => c.Open())
                .Returns(true)
                .Raises(c => c.OnConnected += null, new OnConnectedEventArgs());


            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            // IClient.Send() is/should be our trigger to raise OnMessage
            // ITwitchClient.Client_OnConnected calls IClient.Send() six times
            // so we have to setup a sequence, cause we want to raise IClient.OnMessage only one time
            MockSequence sequence = new MockSequence();
            // ITwichClient.Client_OnConnected sends PASS
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true);
            // ITwichClient.Client_OnConnected sends NICK
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true);
            // ITwichClient.Client_OnConnected sends USER
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true);
            // ITwichClient.Client_OnConnected sends CAP membership
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true);
            // ITwichClient.Client_OnConnected sends CAP commands
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true);
            // ITwichClient.Client_OnConnected sends CAP tags
            // only this last call to IClient.Send() has to trigger raise OnMessage
            mock.InSequence(sequence).Setup(c => c.Send(It.IsAny<string>())).Returns(true).Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = message });


            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.OnConnected += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        Assert.True(client.Connect());
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        /// <summary>
        ///     <see cref="ITwitchClient"/> raises <see cref="ITwitchClient_Connection.OnConnected"/>
        ///     if it receives an <see cref="Models.Internal.IrcMessage"/>
        ///     with <see cref="IrcCommand.RPL_004"/>
        ///     as <see cref="Models.Internal.IrcMessage.Command"/>
        /// </summary>
        [Fact]
        public void TwitchClient_Raises_OnConnected_Simple()
        {
            string message = $":tmi.twitch.tv 004 {TWITCH_Username} :-";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.OnConnected += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(":tmi.twitch.tv 001 [a users name] :Welcome, GLHF!", IrcCommand.RPL_001)]
        [InlineData(":tmi.twitch.tv 002 [a users name] :Your host is tmi.twitch.tv", IrcCommand.RPL_002)]
        [InlineData(":tmi.twitch.tv 003 [a users name] :This server is rather new", IrcCommand.RPL_003)]
        [InlineData(":tmi.twitch.tv 004 [a users name] :-", IrcCommand.RPL_004, Skip = "this should raise OnConnected and is tested somewhere else")]
        [InlineData(":tmi.twitch.tv 375 [a users name] :-", IrcCommand.RPL_375)]
        [InlineData(":tmi.twitch.tv 372 [a users name] :You are in a maze of twisty passages, all alike.", IrcCommand.RPL_372)]
        [InlineData(":tmi.twitch.tv 376 [a users name] :>", IrcCommand.RPL_376)]
        public void TwitchClient_Raises_Nothing_OnConnected(string message, IrcCommand ircCommand)
        {
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            try
            {
                Assert.RaisesAny<OnConnectedArgs>(
                        h => client.OnConnected += h,
                        h => client.OnConnected -= h,
                        () =>
                        {
                            client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                            // send is our trigger, to make the IClient-Mock raise OnMessage!
                            Assert.True(communicationClient.Send(String.Empty));
                            Assert.False(pauseCheck.WaitOne(WaitOneDuration));
                        });
                Assert.Fail("RaisesAny should throw an Exception!");
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }
        [Fact]
        public void TwitchClient_Raises_OnDisconnected()
        {
            Mock<IClient> mock = new Mock<IClient>();
            mock.SetupAdd(c => c.OnDisconnected += It.IsAny<EventHandler<OnDisconnectedEventArgs>>());
            mock.Setup(c => c.Close())
                .Raises(c => c.OnDisconnected += null, new OnDisconnectedEventArgs());

            mock.Setup(c => c.IsConnected).Returns(true);

            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnDisconnectedArgs>(
                    h => client.OnDisconnected += h,
                    h => client.OnDisconnected -= h,
                    () =>
                    {
                        client.OnDisconnected += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        client.Disconnect();
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnIncorrectLogin() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnConnectionError() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnReconnected() { throw new NotImplementedException(); }
    }
}
