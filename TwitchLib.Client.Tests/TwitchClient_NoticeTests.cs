using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Tests.TestHelpers;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_NoticeTests : ATwitchClientTests<ITwitchClient_Notice>
    {
        [Fact]
        public void TwitchClient_Raises_OnChatColorChanged()
        {
            string message = $"@msg-id={MsgIds.ColorChanged} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :Your color has been changed.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnChatColorChangedArgs> assertion = Assert.Raises<OnChatColorChangedArgs>(
                    h => client.OnChatColorChanged += h,
                    h => client.OnChatColorChanged -= h,
                    () =>
                    {
                        client.OnChatColorChanged += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });

            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnBanned()
        {
            string message = $"@msg-id={MsgIds.MsgBanned} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :You are permanently banned from talking in {TWITCH_CHANNEL}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnBannedArgs> assertion = Assert.Raises<OnBannedArgs>(
                    h => client.OnBanned += h,
                    h => client.OnBanned -= h,
                    () =>
                    {
                        client.OnBanned += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(MsgIds.FollowersOn, true)]
        [InlineData(MsgIds.FollowersOnZero, true)]
        [InlineData(MsgIds.FollowersOff, false)]
        public void TwitchClient_Raises_OnFollowersOnly(string msgId, bool isOn)
        {
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnFollowersOnlyArgs> assertion = Assert.Raises<OnFollowersOnlyArgs>(
                    h => client.OnFollowersOnly += h,
                    h => client.OnFollowersOnly -= h,
                    () =>
                    {
                        client.OnFollowersOnly += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            Assert.Equal(isOn, assertion.Arguments.IsOn);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(MsgIds.SubsOn, true)]
        [InlineData(MsgIds.SubsOff, false)]
        public void TwitchClient_Raises_OnSubsOnly(string msgId, bool isOn)
        {
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ....";


            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnSubsOnlyArgs> assertion = Assert.Raises<OnSubsOnlyArgs>(
                    h => client.OnSubsOnly += h,
                    h => client.OnSubsOnly -= h,
                    () =>
                    {
                        client.OnSubsOnly += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            Assert.Equal(isOn, assertion.Arguments.IsOn);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(MsgIds.EmoteOnlyOn, true)]
        [InlineData(MsgIds.EmoteOnlyOff, false)]
        public void TwitchClient_Raises_OnEmoteOnly(string msgId, bool isOn)
        {
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnEmoteOnlyArgs> assertion = Assert.Raises<OnEmoteOnlyArgs>(
                    h => client.OnEmoteOnly += h,
                    h => client.OnEmoteOnly -= h,
                    () =>
                    {
                        client.OnEmoteOnly += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            Assert.Equal(isOn, assertion.Arguments.IsOn);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnSuspended()
        {
            string message = $"@msg-id={MsgIds.MsgSuspended} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :You don’t have permission to perform that action.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnSuspendedArgs> assertion = Assert.Raises<OnSuspendedArgs>(
                    h => client.OnSuspended += h,
                    h => client.OnSuspended -= h,
                    () =>
                    {
                        client.OnSuspended += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(MsgIds.SlowOn, true)]
        [InlineData(MsgIds.SlowOff, false)]
        public void TwitchClient_Raises_OnSlowMode(string msgId, bool isOn)
        {
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is now in ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnSlowModeArgs> assertion = Assert.Raises<OnSlowModeArgs>(
                    h => client.OnSlowMode += h,
                    h => client.OnSlowMode -= h,
                    () =>
                    {
                        client.OnSlowMode += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            Assert.Equal(isOn, assertion.Arguments.IsOn);
            AssertChannel(assertion.Arguments);
        }
        [Theory]
        [InlineData(MsgIds.R9KOn, true)]
        [InlineData(MsgIds.R9KOff, false)]
        public void TwitchClient_Raises_OnR9kMode(string msgId, bool isOn)
        {
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is now in ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnR9kModeArgs> assertion = Assert.Raises<OnR9kModeArgs>(
                    h => client.OnR9kMode += h,
                    h => client.OnR9kMode -= h,
                    () =>
                    {
                        client.OnR9kMode += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            Assert.Equal(isOn, assertion.Arguments.IsOn);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnRequiresVerifiedEmail()
        {
            string message = $"@msg-id={MsgIds.MsgVerifiedEmail} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room requires a verified account to chat. Please verify your account at https://www.twitch.tv/settings/security.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnRequiresVerifiedEmailArgs> assertion = Assert.Raises<OnRequiresVerifiedEmailArgs>(
                    h => client.OnRequiresVerifiedEmail += h,
                    h => client.OnRequiresVerifiedEmail -= h,
                    () =>
                    {
                        client.OnRequiresVerifiedEmail += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnRequiresVerifiedPhoneNumber()
        {
            string message = $"@msg-id={MsgIds.MsgRequiresVerifiedPhoneNumber} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :A verified phone number is required to chat in this channel. Please visit https://www.twitch.tv/settings/security to verify your phone number.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnRequiresVerifiedPhoneNumberArgs> assertion = Assert.Raises<OnRequiresVerifiedPhoneNumberArgs>(
                    h => client.OnRequiresVerifiedPhoneNumber += h,
                    h => client.OnRequiresVerifiedPhoneNumber -= h,
                    () =>
                    {
                        client.OnRequiresVerifiedPhoneNumber += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnRateLimit()
        {
            string message = $"@msg-id={MsgIds.MsgRateLimit} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :Your message was not sent because you are sending messages too quickly.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnRateLimitArgs> assertion = Assert.Raises<OnRateLimitArgs>(
                    h => client.OnRateLimit += h,
                    h => client.OnRateLimit -= h,
                    () =>
                    {
                        client.OnRateLimit += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnDuplicate()
        {
            string message = $"@msg-id={MsgIds.MsgDuplicate} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :Your message was not sent because it is identical to the previous one you sent, less than 30 seconds ago.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnDuplicateArgs> assertion = Assert.Raises<OnDuplicateArgs>(
                    h => client.OnDuplicate += h,
                    h => client.OnDuplicate -= h,
                    () =>
                    {
                        client.OnDuplicate += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnSelfRaidError()
        {
            string message = $"@msg-id={MsgIds.RaidErrorSelf} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :A channel cannot raid itself.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<EventArgs> assertion = Assert.Raises<EventArgs>(
                    h => client.OnSelfRaidError += h,
                    h => client.OnSelfRaidError -= h,
                    () =>
                    {
                        client.OnSelfRaidError += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnNoPermissionError()
        {
            string message = $"@msg-id={MsgIds.NoPermission} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :You don’t have permission to perform that action.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<EventArgs> assertion = Assert.Raises<EventArgs>(
                    h => client.OnNoPermissionError += h,
                    h => client.OnNoPermissionError -= h,
                    () =>
                    {
                        client.OnNoPermissionError += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnRaidedChannelIsMatureAudience()
        {
            string message = $"@msg-id={MsgIds.RaidNoticeMature} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This channel is intended for mature audiences.";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<EventArgs> assertion = Assert.Raises<EventArgs>(
                    h => client.OnRaidedChannelIsMatureAudience += h,
                    h => client.OnRaidedChannelIsMatureAudience -= h,
                    () =>
                    {
                        client.OnRaidedChannelIsMatureAudience += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnBannedEmailAlias()
        {
            string message = $"@msg-id={MsgIds.MsgBannedEmailAlias} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnBannedEmailAliasArgs> assertion = Assert.Raises<OnBannedEmailAliasArgs>(
                    h => client.OnBannedEmailAlias += h,
                    h => client.OnBannedEmailAlias -= h,
                    () =>
                    {
                        client.OnBannedEmailAlias += (sender, args) => Assert.True(pauseCheck.Set());

                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        communicationClient.Send(String.Empty);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Channel);
            Assert.NotNull(assertion.Arguments.Message);
            AssertChannel(assertion.Arguments);
        }
    }
}
