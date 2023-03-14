using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Tests.Helpers;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Tests.Helper;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_NoticeUserTests : ATwitchClientTests<ITwitchClient_NoticeUser>
    {
        [Fact]
        public void TwitchClient_Raises_OnAnnouncement()
        {
            string message = $"@badge-info=;badges=moderator/1,partner/1;color=#5B99FF;display-name=StreamElements;emotes=;flags=;id=an_id;login=streamelements;mod=1;msg-id=announcement;msg-param-color=PRIMARY;room-id=0;subscriber=0;system-msg=;tmi-sent-ts=1678800000000;user-id=0;user-type=mod :tmi.twitch.tv USERNOTICE #{TWITCH_CHANNEL}: this is an announcement";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnAnnouncementArgs>(
                    h => client.OnAnnouncement += h,
                    h => client.OnAnnouncement -= h,
                    () =>
                    {
                        client.OnAnnouncement += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.Announcement);
                            Assert.NotNull(args.Announcement.Id);
                            Assert.Equal("an_id", args.Announcement.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnRaidNotification()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnRaidNotificationArgs>(
                    h => client.OnRaidNotification += h,
                    h => client.OnRaidNotification -= h,
                    () =>
                    {
                        client.OnRaidNotification += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.RaidNotification);
                            Assert.NotNull(args.RaidNotification.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.RaidNotification.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnNewSubscriber()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnNewSubscriberArgs>(
                    h => client.OnNewSubscriber += h,
                    h => client.OnNewSubscriber -= h,
                    () =>
                    {
                        client.OnNewSubscriber += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.Subscriber);
                            Assert.NotNull(args.Subscriber.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.Subscriber.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnReSubscriber()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnReSubscriberArgs>(
                    h => client.OnReSubscriber += h,
                    h => client.OnReSubscriber -= h,
                    () =>
                    {
                        client.OnReSubscriber += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.ReSubscriber);
                            Assert.NotNull(args.ReSubscriber.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.ReSubscriber.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnPrimePaidSubscriber()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnPrimePaidSubscriberArgs>(
                    h => client.OnPrimePaidSubscriber += h,
                    h => client.OnPrimePaidSubscriber -= h,
                    () =>
                    {
                        client.OnPrimePaidSubscriber += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.PrimePaidSubscriber);
                            Assert.NotNull(args.PrimePaidSubscriber.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.PrimePaidSubscriber.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnGiftedSubscription()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnGiftedSubscriptionArgs>(
                    h => client.OnGiftedSubscription += h,
                    h => client.OnGiftedSubscription -= h,
                    () =>
                    {
                        client.OnGiftedSubscription += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.GiftedSubscription);
                            Assert.NotNull(args.GiftedSubscription.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.GiftedSubscription.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnCommunitySubscription()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnCommunitySubscriptionArgs>(
                    h => client.OnCommunitySubscription += h,
                    h => client.OnCommunitySubscription -= h,
                    () =>
                    {
                        client.OnCommunitySubscription += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.GiftedSubscription);
                            Assert.NotNull(args.GiftedSubscription.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.GiftedSubscription.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
        [Fact]
        public void TwitchClient_Raises_OnContinuedGiftedSubscription()
        {
            string message = $"";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnContinuedGiftedSubscriptionArgs>(
                    h => client.OnContinuedGiftedSubscription += h,
                    h => client.OnContinuedGiftedSubscription -= h,
                    () =>
                    {
                        client.OnContinuedGiftedSubscription += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.ContinuedGiftedSubscription);
                            Assert.NotNull(args.ContinuedGiftedSubscription.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.ContinuedGiftedSubscription.Id);
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
