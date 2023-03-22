using System;
using System.Text;
using System.Threading;

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
    public class TwitchClient_MessageReceivingTests : ATwitchClientTests<ITwitchClient_MessageReceiving>
    {
        [Fact]
        public void TwitchClient_Raises_OnMessageReceived()
        {
            string message = $"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageReceivedArgs> assertion = Assert.Raises<OnMessageReceivedArgs>(
                    h => client.OnMessageReceived += h,
                    h => client.OnMessageReceived -= h,
                    () =>
                    {
                        client.OnMessageReceived += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.ChatMessage);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(":tmi.twitch.tv 001 [a users name] :Welcome, GLHF!", IrcCommand.RPL_001)]
        [InlineData(":tmi.twitch.tv 002 [a users name] :Your host is tmi.twitch.tv", IrcCommand.RPL_002)]
        [InlineData(":tmi.twitch.tv 003 [a users name] :This server is rather new", IrcCommand.RPL_003)]
        [InlineData(":tmi.twitch.tv 004 [a users name] :-", IrcCommand.RPL_004, Skip = "this should raise OnConnected and is tested somewhere else")]
        [InlineData(":tmi.twitch.tv 375 [a users name] :-", IrcCommand.RPL_375)]
        [InlineData(":tmi.twitch.tv 372 [a users name] :You are in a maze of twisty passages, all alike.", IrcCommand.RPL_372)]
        [InlineData(":tmi.twitch.tv 376 [a users name] :>", IrcCommand.RPL_376)]
        [InlineData("PING :tmi.twitch.tv", IrcCommand.Ping)]
        [InlineData("PONG :tmi.twitch.tv", IrcCommand.Pong)]
        [InlineData(":[a users name].tmi.twitch.tv 366 [a users name] #testchannel :End of /NAMES list", IrcCommand.RPL_366)]
        public void TwitchClient_Raises_Nothing_OnMessageReceived(string message, IrcCommand ircCommand)
        {
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            try
            {
                Assert.RaisesAny<OnMessageReceivedArgs>(
                        h => client.OnMessageReceived += h,
                        h => client.OnMessageReceived -= h,
                        () =>
                        {
                            client.Initialize(new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                            // send is our trigger, to make the IClient-Mock raise OnMessage!
                            Assert.True(communicationClient.Send(String.Empty));
                            // we dont need to wait so long as `WaitOneDuration` specifies
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
        public void TwitchClient_Raises_OnUserStateChanged()
        {
            string message = $":{TWITCH_UsernameAnother}!{TWITCH_UsernameAnother}@{TWITCH_UsernameAnother}.tmi.twitch.tv USERSTATE #{TWITCH_CHANNEL}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserStateChangedArgs> assertion = Assert.Raises<OnUserStateChangedArgs>(
                    h => client.OnUserStateChanged += h,
                    h => client.OnUserStateChanged -= h,
                    () =>
                    {
                        client.OnUserStateChanged += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.UserState);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnChatCommandReceived()
        {
            string message = $"@badge-info=subscriber/1;badges=subscriber/0,premium/1;color=#00FF7F;display-name={TWITCH_Username};emotes=;first-msg=0;flags=;id=msg_id_hash;mod=0;returning-chatter=0;room-id=1;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=0;user-type= :{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv PRIVMSG #{TWITCH_CHANNEL} :!chat_command";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChatCommandReceivedArgs> assertion = Assert.Raises<OnChatCommandReceivedArgs>(
                    h => client.OnChatCommandReceived += h,
                    h => client.OnChatCommandReceived -= h,
                    () =>
                    {
                        client.OnChatCommandReceived += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Command);
        }
        [Fact]
        public void TwitchClient_Raises_OnUserJoined()
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
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = $":{TWITCH_UsernameAnother}!{TWITCH_UsernameAnother}@{TWITCH_UsernameAnother}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}" });

            IClient communicationClient = mock.Object;


            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserJoinedArgs> assertion = Assert.Raises<OnUserJoinedArgs>(
                    h => client.OnUserJoined += h,
                    h => client.OnUserJoined -= h,
                    () =>
                    {
                        client.OnUserJoined += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // make the client raise OnConnected and ITwitchClient start ChannelManager
                        communicationClient.Send(String.Empty);
                        // trigger the client to raise OnMessage ...
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Username);
            Assert.Equal(TWITCH_UsernameAnother, assertion.Arguments.Username);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnUserLeft()
        {
            string message = $":{TWITCH_UsernameAnother}!{TWITCH_UsernameAnother}@{TWITCH_UsernameAnother}.tmi.twitch.tv PART #{TWITCH_CHANNEL}";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserLeftArgs> assertion = Assert.Raises<OnUserLeftArgs>(
                    h => client.OnUserLeft += h,
                    h => client.OnUserLeft -= h,
                    () =>
                    {
                        client.OnUserLeft += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Username);
            Assert.Equal(TWITCH_UsernameAnother, assertion.Arguments.Username);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnModeratorJoined()
        {
            string message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv MODE #{TWITCH_CHANNEL} :+o {TWITCH_UsernameAnother}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnModeratorJoinedArgs> assertion = Assert.Raises<OnModeratorJoinedArgs>(
                    h => client.OnModeratorJoined += h,
                    h => client.OnModeratorJoined -= h,
                    () =>
                    {
                        client.OnModeratorJoined += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Username);
            Assert.Equal(TWITCH_UsernameAnother, assertion.Arguments.Username);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnModeratorLeft()
        {
            string message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv MODE #{TWITCH_CHANNEL} :-o {TWITCH_UsernameAnother}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnModeratorLeftArgs> assertion = Assert.Raises<OnModeratorLeftArgs>(
                    h => client.OnModeratorLeft += h,
                    h => client.OnModeratorLeft -= h,
                    () =>
                    {
                        client.OnModeratorLeft += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Username);
            Assert.Equal(TWITCH_UsernameAnother, assertion.Arguments.Username);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnMessageCleared()
        {
            string message = $"@login={TWITCH_Username};room-id=;target-msg-id=msg_id_hash;tmi-sent-ts=1678800000000 :tmi.twitch.tv CLEARMSG #{TWITCH_CHANNEL} :This message is going to be cleared.";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageClearedArgs> assertion = Assert.Raises<OnMessageClearedArgs>(
                    h => client.OnMessageCleared += h,
                    h => client.OnMessageCleared -= h,
                    () =>
                    {
                        client.OnMessageCleared += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnExistingUsersDetected()
        {
            string[] users = new string[] {
                "testuser0",
                "testuser1",
                "testuser2"
            };
            StringBuilder builder = new StringBuilder($":{TWITCH_Username}.tmi.twitch.tv 353 {TWITCH_Username} = #{TWITCH_CHANNEL} :");
            builder.Append(String.Join(' ', users));
            string message = builder.ToString();
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnExistingUsersDetectedArgs> assertion = Assert.Raises<OnExistingUsersDetectedArgs>(
                    h => client.OnExistingUsersDetected += h,
                    h => client.OnExistingUsersDetected -= h,
                    () =>
                    {
                        client.OnExistingUsersDetected += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Users);
            Assert.NotEmpty(assertion.Arguments.Users);
            Assert.Equal(users.Length, assertion.Arguments.Users.Count);
            Assert.NotNull(assertion.Arguments.Channel);
            AssertChannel(assertion.Arguments);

        }
        [Fact]
        public void TwitchClient_Raises_OnChatCleared()
        {
            string message = $"@room-id=0;target-user-id=0;tmi-sent-ts=1678800000000 :tmi.twitch.tv CLEARCHAT #{TWITCH_CHANNEL} :";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChatClearedArgs> assertion = Assert.Raises<OnChatClearedArgs>(
                    h => client.OnChatCleared += h,
                    h => client.OnChatCleared -= h,
                    () =>
                    {
                        client.OnChatCleared += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnUserTimedout()
        {
            string message = $"@ban-duration=60;room-id=0;target-user-id=0;tmi-sent-ts=1678800000000 :tmi.twitch.tv CLEARCHAT #{TWITCH_CHANNEL} :{TWITCH_Username}";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserTimedoutArgs> assertion = Assert.Raises<OnUserTimedoutArgs>(
                    h => client.OnUserTimedout += h,
                    h => client.OnUserTimedout -= h,
                    () =>
                    {
                        client.OnUserTimedout += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.UserTimeout);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnUserBanned()
        {
            string message = $"@room-id=0;target-user-id=0;tmi-sent-ts=1678800000000 :tmi.twitch.tv CLEARCHAT #{TWITCH_CHANNEL} :{TWITCH_Username}";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserBannedArgs> assertion = Assert.Raises<OnUserBannedArgs>(
                    h => client.OnUserBanned += h,
                    h => client.OnUserBanned -= h,
                    () =>
                    {
                        client.OnUserBanned += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.UserBan);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnUserIntro()
        {
            string message = $"@badge-info=;badges=premium/1;color=#0000FF;display-name={TWITCH_Username};emotes=1:24-25;first-msg=1;flags=;id=msg_id_hash;mod=0;msg-id=user-intro;returning-chatter=0;room-id=1;subscriber=0;tmi-sent-ts=1678800000000;turbo=0;user-id=0;user-type= :{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv PRIVMSG #{TWITCH_CHANNEL} :let me introduce myself :)";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnUserIntroArgs> assertion = Assert.Raises<OnUserIntroArgs>(
                    h => client.OnUserIntro += h,
                    h => client.OnUserIntro -= h,
                    () =>
                    {
                        client.OnUserIntro += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.ChatMessage);
            AssertChannel(assertion.Arguments);
        }
    }
}
