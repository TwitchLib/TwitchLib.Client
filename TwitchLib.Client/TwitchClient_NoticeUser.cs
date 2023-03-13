using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Communication.Extensions;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_NoticeUser
    {
        public event EventHandler<OnAnnouncementArgs> OnAnnouncement;
        public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

        public event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;
        public event EventHandler<OnReSubscriberArgs> OnReSubscriber;
        public event EventHandler<OnPrimePaidSubscriberArgs> OnPrimePaidSubscriber;

        public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;
        public event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;
        public event EventHandler<OnContinuedGiftedSubscriptionArgs> OnContinuedGiftedSubscription;

        private void HandleUserNotice(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
            bool successMsgId = ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId);
            if (!successMsgId)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                UnaccountedFor(ircMessage.ToString());
                return;
            }

            switch (msgId)
            {
                case MsgIds.Announcement:
                    Announcement announcement = new Announcement(ircMessage);
                    OnAnnouncement?.Invoke(this, new OnAnnouncementArgs { Announcement = announcement, Channel = ircMessage.Channel });
                    break;
                case MsgIds.Raid:
                    RaidNotification raidNotification = new RaidNotification(ircMessage);
                    OnRaidNotification?.Invoke(this, new OnRaidNotificationArgs { Channel = ircMessage.Channel, RaidNotification = raidNotification });
                    break;
                case MsgIds.ReSubscription:
                    ReSubscriber resubscriber = new ReSubscriber(ircMessage, LOGGER);
                    OnReSubscriber?.Invoke(this, new OnReSubscriberArgs { ReSubscriber = resubscriber, Channel = ircMessage.Channel });
                    break;
                case MsgIds.SubGift:
                    GiftedSubscription giftedSubscription = new GiftedSubscription(ircMessage, LOGGER);
                    OnGiftedSubscription?.Invoke(this, new OnGiftedSubscriptionArgs { GiftedSubscription = giftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.CommunitySubscription:
                    CommunitySubscription communitySubscription = new CommunitySubscription(ircMessage, LOGGER);
                    OnCommunitySubscription?.Invoke(this, new OnCommunitySubscriptionArgs { GiftedSubscription = communitySubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.ContinuedGiftedSubscription:
                    ContinuedGiftedSubscription continuedGiftedSubscription = new ContinuedGiftedSubscription(ircMessage);
                    OnContinuedGiftedSubscription?.Invoke(this, new OnContinuedGiftedSubscriptionArgs { ContinuedGiftedSubscription = continuedGiftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.Subscription:
                    Subscriber subscriber = new Subscriber(ircMessage, LOGGER);
                    OnNewSubscriber?.Invoke(this, new OnNewSubscriberArgs { Subscriber = subscriber, Channel = ircMessage.Channel });
                    break;
                case MsgIds.PrimePaidUprade:
                    PrimePaidSubscriber primePaidSubscriber = new PrimePaidSubscriber(ircMessage, LOGGER);
                    OnPrimePaidSubscriber?.Invoke(this, new OnPrimePaidSubscriberArgs { PrimePaidSubscriber = primePaidSubscriber, Channel = ircMessage.Channel });
                    break;
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }
    }
}
