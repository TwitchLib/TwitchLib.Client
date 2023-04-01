using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelpers;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    [SuppressMessage("Style", "IDE0058")]
    public class WalkThroughConnectTest : ATwitchClientTests<ITwitchClient>
    {
        [Fact]
        public void CompleteConnectionTest()
        {
            string messageLogin = $":tmi.twitch.tv 004 {TWITCH_Username} :-";
            string messageExpectedToJoin = $"JOIN #{TWITCH_CHANNEL}";
            string messageJoinCompleted = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}";
            string messageReconnect = $":tmi.twitch.tv RECONNECT";

            Mock<IClient> mock = IClientMocker.GetIClientMock();

            // IClient.Send() is/should be our trigger to raise OnMessage
            // ITwitchClient makes several calls to IClient.Send()
            // for example, to send PASS, NICK, USER, CAP, JOIN
            // IClient is only a mock and is not connected to Twitch or somewhere else
            // we assume, the implementation of IClient works fine
            // so Mock of IClient has to act as its real implementation
            // and has to handle each call to its IClient.Send() method
            MockSequence sendMessageSequence = new MockSequence();
            {
                // this block got separated
                //// after OnConnected is raise, we have to send the LogIn-Sequence
                //AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                //// then, the ITwitchClient should send a JOIN-Message, to auto-join the testchannel
                //AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
                //// now, lets reconnect
                //AddToSendMessageSequence(messageReconnect, mock, sendMessageSequence);
                //// we have to send the LogIn-Sequence again
                //AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                //// and we have to join again
                //AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
                //// we have to send the LogIn-Sequence again
                //AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                //// and we have to join again
                //AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
            }
            //
            IClient communicationClient = mock.Object;
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>(GetType());
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient twitchClient = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(mock, true);
            twitchClient.JoinChannel(TWITCH_CHANNEL);
            {
                // check OnConnected and AutoJoin
                //
                // twitchClient.Connect()
                // should make communicationClient establish a connection
                // and raise OnConnected
                // then, ITwitchClient should send the LogIn-Sequence and receive IrcCommand.RPL_004
                // receiving that IrcCommand should make the ITwitchClient raise OnConnected and start the ChannelManager
                {
                    // prepare sendMessageSequence
                    // after OnConnected is raised,
                    // we have to send the LogIn-Sequence
                    IClientMocker.AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                }
                CheckOnConnected(twitchClient);
                //
                // twitchClient.Connect() should have started ChannelManager
                // and now we expect ChannelManager to send a JOIN-message
                // IClient should raise OnMessage with the JOIN-confirmation
                // ITwitchClient should process that JOIN-Confirmation and raise OnJoinedChannel
                // then, the ITwitchClient should send a JOIN-Message, to auto-join the testchannel
                {
                    // prepare sendMessageSequence
                    IClientMocker.AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
                }
                CheckOnJoinedChannelCheck(twitchClient);
            }
            {
                // Check Reconnect and AutoJoin
                {
                    // prepare sendMessageSequence
                    // now we make IClient receive IrcCommand.Reconnect
                    IClientMocker.AddToSendMessageSequence(messageReconnect, mock, sendMessageSequence);
                    // add LogIn-Sequence
                    IClientMocker.AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                    // also add Join to Sequence
                    IClientMocker.AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
                }
                CheckOnReconnected(twitchClient, communicationClient);
                // we expect all AutoJoinChannels to be joined again after reconnecting
                CheckOnJoinedChannelCheck(twitchClient);
            }
            {
                // now we disconnect manually
                CheckOnDisconnected(twitchClient);
            }
            {
                // now connect manually again and check AutoJoin
                {
                    // prepare sendMessageSequence
                    // add LogIn-Sequence
                    IClientMocker.AddLogInToSendMessageSequence(messageLogin, mock, sendMessageSequence);
                    // also add Join to Sequence
                    IClientMocker.AddJoinToSendMessageSequence(messageExpectedToJoin, messageJoinCompleted, mock, sendMessageSequence);
                }
                CheckOnConnected(twitchClient);
                CheckOnJoinedChannelCheck(twitchClient);
            }
        }
        private static void CheckOnDisconnected(ITwitchClient twitchClient)
        {
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnDisconnectedArgs> assertion = Assert.Raises<OnDisconnectedArgs>(
                    h => twitchClient.OnDisconnected += h,
                    h => twitchClient.OnDisconnected -= h,
                    () =>
                    {
                        twitchClient.OnDisconnected += (sender, args) => Assert.True(pauseCheck.Set());
                        twitchClient.Disconnect();
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.Empty(twitchClient.JoinedChannels);
        }
        private static void CheckOnReconnected(ITwitchClient twitchClient, IClient communicationClient)
        {
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => twitchClient.OnReconnected += h,
                    h => twitchClient.OnReconnected -= h,
                    () =>
                    {
                        twitchClient.OnReconnected += (sender, args) =>
                        {
                            // we expect the ChannelManager to stop and to clear everything but WantToJoin
                            // but here we have only access to JoinedChannels
                            Assert.Empty(twitchClient.JoinedChannels);
                            Assert.True(pauseCheck.Set());
                        };
                        // we have to cheat a bit
                        // the next step with the comment "triggers communicationClient to Send messageReconnect"
                        // would make IClient raise on disconnected
                        // but here we work with a mock of an interface,
                        // that cant raise multiple events at once...
                        // so we have to make a call to close, to get OnDisconnected raised by IClient
                        communicationClient.Close();
                        // triggers communicationClient to Send messageReconnect
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.Single(assertion.Arguments.AutoJoinChannels);
            Assert.Equal(TWITCH_CHANNEL, assertion.Arguments.AutoJoinChannels.First());
        }
        private static void CheckOnConnected(ITwitchClient twitchClient)
        {
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => twitchClient.OnConnected += h,
                    h => twitchClient.OnConnected -= h,
                    () =>
                    {
                        twitchClient.OnConnected += (sender, args) => Assert.True(pauseCheck.Set());
                        Assert.True(twitchClient.Connect());
                        //Assert.True(pauseCheck.WaitOne(500000000));
                        Assert.True(pauseCheck.WaitOne(WaitOneDurationShort));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.Single(assertion.Arguments.AutoJoinChannels);
            Assert.Equal(TWITCH_CHANNEL, assertion.Arguments.AutoJoinChannels.First());
        }
        private static void CheckOnJoinedChannelCheck(ITwitchClient twitchClient)
        {
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnJoinedChannelArgs> assertion = Assert.Raises<OnJoinedChannelArgs>(
                    h => twitchClient.OnJoinedChannel += h,
                    h => twitchClient.OnJoinedChannel -= h,
                    () =>
                    {
                        twitchClient.OnJoinedChannel += (sender, args) => Assert.True(pauseCheck.Set());
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.Equal(TWITCH_CHANNEL, assertion.Arguments.Channel);
            Assert.Equal(TWITCH_Username, assertion.Arguments.BotUsername);
            Assert.Single(twitchClient.JoinedChannels);
            Assert.Equal(TWITCH_CHANNEL, twitchClient.GetJoinedChannel(TWITCH_CHANNEL).Channel);
        }
    }
}
