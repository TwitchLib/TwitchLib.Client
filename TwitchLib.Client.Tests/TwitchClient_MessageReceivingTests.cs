using System;
using System.Text;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.Helpers;
using TwitchLib.Communication.Interfaces;

using TwitchLib.Communication.Tests.Helper;

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
            Assert.Raises<OnMessageReceivedArgs>(
                    h => client.OnMessageReceived += h,
                    h => client.OnMessageReceived -= h,
                    () =>
                    {
                        client.OnMessageReceived += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.ChatMessage);
                            Assert.NotNull(args.ChatMessage.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.ChatMessage.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
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
                            Assert.False(pauseCheck.WaitOne(500));
                        });
                Assert.Fail("RaisesAny should throw an Exception!");
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }
        [Fact]
        public void TwitchClient_Raises_OnUserStateChanged() { throw new NotImplementedException(); }
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
        }
        [Fact]
        public void TwitchClient_Raises_OnUserJoined()
        {
            string message = $":{TWITCH_Username}!{TWITCH_Username}@{TWITCH_Username}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
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
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
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
        }
        [Fact]
        public void TwitchClient_Raises_OnModeratorJoined() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnModeratorLeft() { throw new NotImplementedException(); }
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
        }
    }
}
