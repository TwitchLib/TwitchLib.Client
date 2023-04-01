using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelpers;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    [SuppressMessage("Style", "IDE0058")]
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
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            // IClient.Send() is/should be our trigger to raise OnMessage
            // ITwitchClient.Client_OnConnected calls IClient.Send() six times
            // so we have to setup a sequence, cause we want to raise IClient.OnMessage only one time
            MockSequence sendMessageSequence = new MockSequence();
            IClientMocker.AddLogInToSendMessageSequence(message, mock, sendMessageSequence);


            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.OnConnected += (sender, args) => Assert.True(pauseCheck.Set());
                        Assert.True(client.Connect());
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.NotNull(assertion.Arguments.AutoJoinChannels);
            Assert.Empty(assertion.Arguments.AutoJoinChannels);
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
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(Mock.Get(communicationClient), true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.OnConnected += (sender, args) => Assert.True(pauseCheck.Set());
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.NotNull(assertion.Arguments.AutoJoinChannels);
            Assert.Empty(assertion.Arguments.AutoJoinChannels);
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
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(Mock.Get(communicationClient), true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            try
            {
                Assert.RaisesAny<OnConnectedArgs>(
                        h => client.OnConnected += h,
                        h => client.OnConnected -= h,
                        () =>
                        {
                            // send is our trigger, to make the IClient-Mock raise OnMessage!
                            Assert.True(communicationClient.Send(String.Empty));
                            // we dont need to wait to long, we expect it to fail
                            Assert.False(pauseCheck.WaitOne(WaitOneDurationShort));
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
            Mock<IClient> mock = IClientMocker.GetIClientMock();

            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(Mock.Get(communicationClient), true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnDisconnectedArgs> assertion = Assert.Raises<OnDisconnectedArgs>(
                    h => client.OnDisconnected += h,
                    h => client.OnDisconnected -= h,
                    () =>
                    {
                        client.OnDisconnected += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Disconnect();
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
        }
        [Theory]
        [InlineData(":tmi.twitch.tv NOTICE * :Login authentication failed")]
        [InlineData(":tmi.twitch.tv NOTICE * :Improperly formatted auth")]
        public void TwitchClient_Raises_OnIncorrectLogin(string message)
        {
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(Mock.Get(communicationClient), true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnIncorrectLoginArgs> assertion = Assert.Raises<OnIncorrectLoginArgs>(
                    h => client.OnIncorrectLogin += h,
                    h => client.OnIncorrectLogin -= h,
                    () =>
                    {
                        client.OnIncorrectLogin += (sender, args) => Assert.True(pauseCheck.Set());
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Exception);
            Assert.Equal(TWITCH_Username, assertion.Arguments.Exception.Username);
        }
        [Fact]
        public void TwitchClient_Raises_OnConnectionError()
        {
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            mock.Setup(c => c.Send(It.IsAny<string>()))
                .Returns(false)
                .Raises(c => c.OnFatality += null, new OnFatalErrorEventArgs("Fatal network error."));
            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectionErrorArgs> assertion = Assert.Raises<OnConnectionErrorArgs>(
                    h => client.OnConnectionError += h,
                    h => client.OnConnectionError -= h,
                    () =>
                    {
                        client.OnConnectionError += (sender, args) => Assert.True(pauseCheck.Set());
                        Assert.False(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });

            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.BotUsername);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
        }
        [Fact]
        public void TwitchClient_Raises_OnReconnected()
        {
            string messageLogin = $":tmi.twitch.tv 004 {TWITCH_Username} :-";
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            MockSequence sendMessageSequence = new MockSequence();
            // for call to ITwitchClient.Connect()
            IClientMocker.AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
            // for call to ITwitchClient.Reconnect()
            IClientMocker.AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
            IClient communicationClient = mock.Object;
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => client.OnReconnected += h,
                    h => client.OnReconnected -= h,
                    () =>
                    {
                        client.OnReconnected += (sender, args) => Assert.True(pauseCheck.Set());
                        // first connect, to get ConnectionStateManager in correct state
                        Assert.True(client.Connect());
                        // cheat a bit
                        // the 'real' IClient.Reconnect()
                        // would make a call to IClient.Close()
                        // but here,
                        // we only have a Mock of it
                        communicationClient.Close();
                        // lets take a breath and let ITwitchClient gets its work done...
                        Task.Delay(WaitOneDuration).GetAwaiter().GetResult();
                        client.Reconnect();
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void SetConnectionCredentialsNullTest()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(credentials);
            try
            {
                client.SetConnectionCredentials(null);
                Assert.Fail($"{typeof(ArgumentNullException)} expected!");
            }
            catch (Exception e)
            {
                Assert.NotNull(e);
                Assert.IsType<ArgumentNullException>(e);
            }
        }
        [Fact]
        public void SetConnectionCredentialsConnectedTest()
        {
            Mock<IClient> mock = IClientMocker.GetIClientMock();
            IClient communicationClient = mock.Object;
            ConnectionCredentials credentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(credentials, communicationClient);
            IClientMocker.SetIsConnected(mock, true);
            try
            {
                client.SetConnectionCredentials(credentials);
                Assert.Fail($"{typeof(IllegalAssignmentException)} expected!");
            }
            catch (Exception e)
            {
                Assert.NotNull(e);
                Assert.IsType<IllegalAssignmentException>(e);
            }
        }
    }
}
