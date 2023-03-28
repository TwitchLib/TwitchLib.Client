﻿using System;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to notice the User <see cref="IrcCommand.UserNotice"/>
    /// </summary>
    public interface ITwitchClient_NoticeUser
    {
        #region events public
        /// <summary>
        ///     Occurs when [on gifted subscription].
        /// </summary>
        event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;
        /// <summary>
        ///     Occurs when [on continued gifted subscription].
        /// </summary>
        event EventHandler<OnContinuedGiftedSubscriptionArgs> OnContinuedGiftedSubscription;
        /// <summary>
        ///     Occurs when [on new subscriber].
        /// </summary>
        event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;
        /// <summary>
        ///     Occurs when [on prime paid subscriber].
        /// </summary>
        event EventHandler<OnPrimePaidSubscriberArgs> OnPrimePaidSubscriber;
        /// <summary>
        ///     Occurs when [on raid notification].
        /// </summary>
        event EventHandler<OnRaidNotificationArgs> OnRaidNotification;
        /// <summary>
        ///     Occurs on <see cref="Models.Internal.MsgIds.UnRaid"/>
        /// </summary>
        event EventHandler<OnRaidNotificationArgs> OnUnRaidNotification;
        /// <summary>
        ///     Occurs when [on re subscriber].
        /// </summary>
        event EventHandler<OnReSubscriberArgs> OnReSubscriber;
        /// <summary>
        ///     Occurs when [on community subscription announcement received].
        /// </summary>
        event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;
        /// <summary>
        ///     Fires when the client receives a USERNOTICE tagged as an announcement
        /// </summary>
        event EventHandler<OnAnnouncementArgs> OnAnnouncement;
        #endregion events public
    }
}
