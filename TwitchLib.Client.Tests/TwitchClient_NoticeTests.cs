using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_NoticeTests : ATwitchClientTests<ITwitchClient_Notice>
    {

        [Fact]
        public void TwitchClient_Raises_OnModeratorsReceived()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnModeratorsReceivedArgs>(
                    h => client.OnModeratorsReceived += h,
                    h => client.OnModeratorsReceived -= h,
                    () =>
                    {
                        client.OnModeratorsReceived += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Moderators);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnChatColorChanged()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnChatColorChangedArgs>(
                    h => client.OnChatColorChanged += h,
                    h => client.OnChatColorChanged -= h,
                    () =>
                    {
                        client.OnChatColorChanged += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Channel);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnBanned()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnBannedArgs>(
                    h => client.OnBanned += h,
                    h => client.OnBanned -= h,
                    () =>
                    {
                        client.OnBanned += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(MsgIds.FollowersOn)]
        [InlineData(MsgIds.FollowersOnZero)]
        [InlineData(MsgIds.FollowersOff)]
        public void TwitchClient_Raises_OnFollowersOnly(string msgId)
        {
            // TODO: i think it never gets handled
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnFollowersOnlyArgs>(
                    h => client.OnFollowersOnly += h,
                    h => client.OnFollowersOnly -= h,
                    () =>
                    {
                        client.OnFollowersOnly += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(MsgIds.SubsOn)]
        [InlineData(MsgIds.SubsOff)]
        public void TwitchClient_Raises_OnSubsOnly(string msgId)
        {
            // TODO: i think it never gets handled
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ....";


            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnSubsOnlyArgs>(
                    h => client.OnSubsOnly += h,
                    h => client.OnSubsOnly -= h,
                    () =>
                    {
                        client.OnSubsOnly += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(MsgIds.EmoteOnlyOn)]
        [InlineData(MsgIds.EmoteOnlyOff)]
        public void TwitchClient_Raises_OnEmoteOnly(string msgId)
        {
            // TODO: i think it never gets handled
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnEmoteOnlyArgs>(
                    h => client.OnEmoteOnly += h,
                    h => client.OnEmoteOnly -= h,
                    () =>
                    {
                        client.OnEmoteOnly += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnSuspended()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnSuspendedArgs>(
                    h => client.OnSuspended += h,
                    h => client.OnSuspended -= h,
                    () =>
                    {
                        client.OnSuspended += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(MsgIds.SlowOn)]
        [InlineData(MsgIds.SlowOff)]
        public void TwitchClient_Raises_OnSlowMode(string msgId)
        {
            // TODO: i think it never gets handled
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is now in ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnSlowModeArgs>(
                    h => client.OnSlowMode += h,
                    h => client.OnSlowMode -= h,
                    () =>
                    {
                        client.OnSlowMode += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Theory]
        [InlineData(MsgIds.R9KOn)]
        [InlineData(MsgIds.R9KOff)]
        public void TwitchClient_Raises_OnR9kMode(string msgId)
        {
            // TODO: i think it never gets handled
            string message = $"@msg-id={msgId} :tmi.twitch.tv NOTICE #{TWITCH_CHANNEL} :This room is now in ...";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnR9kModeArgs>(
                    h => client.OnR9kMode += h,
                    h => client.OnR9kMode -= h,
                    () =>
                    {
                        client.OnR9kMode += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnRequiresVerifiedEmail()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnRequiresVerifiedEmailArgs>(
                    h => client.OnRequiresVerifiedEmail += h,
                    h => client.OnRequiresVerifiedEmail -= h,
                    () =>
                    {
                        client.OnRequiresVerifiedEmail += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnRequiresVerifiedPhoneNumber()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnRequiresVerifiedPhoneNumberArgs>(
                    h => client.OnRequiresVerifiedPhoneNumber += h,
                    h => client.OnRequiresVerifiedPhoneNumber -= h,
                    () =>
                    {
                        client.OnRequiresVerifiedPhoneNumber += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnRateLimit()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnRateLimitArgs>(
                    h => client.OnRateLimit += h,
                    h => client.OnRateLimit -= h,
                    () =>
                    {
                        client.OnRateLimit += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnDuplicate()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnDuplicateArgs>(
                    h => client.OnDuplicate += h,
                    h => client.OnDuplicate -= h,
                    () =>
                    {
                        client.OnDuplicate += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnSelfRaidError()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<EventArgs>(
                    h => client.OnSelfRaidError += h,
                    h => client.OnSelfRaidError -= h,
                    () =>
                    {
                        client.OnSelfRaidError += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnNoPermissionError()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<EventArgs>(
                    h => client.OnNoPermissionError += h,
                    h => client.OnNoPermissionError -= h,
                    () =>
                    {
                        client.OnNoPermissionError += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnRaidedChannelIsMatureAudience()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<EventArgs>(
                    h => client.OnRaidedChannelIsMatureAudience += h,
                    h => client.OnRaidedChannelIsMatureAudience -= h,
                    () =>
                    {
                        client.OnRaidedChannelIsMatureAudience += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnBannedEmailAlias()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnBannedEmailAliasArgs>(
                    h => client.OnBannedEmailAlias += h,
                    h => client.OnBannedEmailAlias -= h,
                    () =>
                    {
                        client.OnBannedEmailAlias += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.Message);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnVIPsReceived()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnVIPsReceivedArgs>(
                    h => client.OnVIPsReceived += h,
                    h => client.OnVIPsReceived -= h,
                    () =>
                    {
                        client.OnVIPsReceived += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.VIPs);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
    }
}
