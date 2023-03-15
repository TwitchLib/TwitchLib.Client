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
            string message = $"@badge-info=;badges=premium/1;color=#4E3696;display-name={TWITCH_Username};emotes=;flags=;id=msg_id_hash;login={TWITCH_Username};mod=0;msg-id=sub;msg-param-cumulative-months=1;msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\s(Name);msg-param-sub-plan=Prime;msg-param-was-gifted=false;room-id=0;subscriber=1;system-msg={TWITCH_Username}\\ssubscribed\\swith\\sPrime.;tmi-sent-ts=1678000000000;user-id=1;user-type= :tmi.twitch.tv USERNOTICE #{TWITCH_CHANNEL}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnNewSubscriberArgs> assertion = Assert.Raises<OnNewSubscriberArgs>(
                    h => client.OnNewSubscriber += h,
                    h => client.OnNewSubscriber -= h,
                    () =>
                    {
                        client.OnNewSubscriber += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnReSubscriber()
        {
            string message = $"@badge-info=subscriber/13;badges=subscriber/12,moments/1;color=#1BFF00;display-name={TWITCH_Username};emotes=;flags=;id=msg_id_hash;login={TWITCH_Username};mod=0;msg-id=resub;msg-param-cumulative-months=13;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\s(Name);msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=0;subscriber=1;system-msg={TWITCH_Username}\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s13\\smonths!;tmi-sent-ts=1678800000000;user-id=1;user-type= :tmi.twitch.tv USERNOTICE #{TWITCH_CHANNEL} :Test";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnReSubscriberArgs> assertion = Assert.Raises<OnReSubscriberArgs>(
                    h => client.OnReSubscriber += h,
                    h => client.OnReSubscriber -= h,
                    () =>
                    {
                        client.OnReSubscriber += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });

            Assert.NotNull(assertion.Arguments);
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
            string message = $"@badge-info=;badges=vip/1,bits/1000;color=#DAA520;display-name={TWITCH_Username};emotes=;flags=;id=msg_id_hash;login={TWITCH_Username};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-goal-contribution-type=NEW_SUBS;msg-param-goal-current-contributions=6;msg-param-goal-description=Test;msg-param-goal-target-contributions=20;msg-param-goal-user-contributions=1;msg-param-months=4;msg-param-origin-id=0;msg-param-recipient-display-name=testuser;msg-param-recipient-id=2;msg-param-recipient-user-name=testuser;msg-param-sender-count=3;msg-param-sub-plan-name=Channel\\sSubscription\\s(name);msg-param-sub-plan=1000;room-id=1;subscriber=0;system-msg={TWITCH_Username}Test!;tmi-sent-ts=1678800000000;user-id=1;user-type= :tmi.twitch.tv USERNOTICE #{TWITCH_CHANNEL}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnGiftedSubscriptionArgs> assertion = Assert.Raises<OnGiftedSubscriptionArgs>(
                    h => client.OnGiftedSubscription += h,
                    h => client.OnGiftedSubscription -= h,
                    () =>
                    {
                        client.OnGiftedSubscription += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
        }
        [Fact]
        public void TwitchClient_Raises_OnCommunitySubscription()
        {
            string message = $"@badge-info=subscriber/4;badges=vip/1,subscriber/3,hype-train/1;color=#FF69B4;display-name={TWITCH_Username};emotes=;flags=;id=msg_id_hash;login={TWITCH_Username};mod=0;msg-id=submysterygift;msg-param-mass-gift-count=1;msg-param-origin-id=0;msg-param-sender-count=30;msg-param-sub-plan=1000;room-id=0;subscriber=1;system-msg={TWITCH_Username}\\sis\\sgifting\\s1\\sTier\\s1\\sSubs\\sto\\s{TWITCH_CHANNEL}'s\\scommunity!\\sThey've\\sgifted\\sa\\stotal\\sof\\s30\\sin\\sthe\\schannel!;tmi-sent-ts=1678800000000;user-id=0;user-type= :tmi.twitch.tv USERNOTICE #{TWITCH_CHANNEL}";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnCommunitySubscriptionArgs> assertion = Assert.Raises<OnCommunitySubscriptionArgs>(
                    h => client.OnCommunitySubscription += h,
                    h => client.OnCommunitySubscription -= h,
                    () =>
                    {
                        client.OnCommunitySubscription += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
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
