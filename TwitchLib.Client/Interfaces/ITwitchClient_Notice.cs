using System;

using TwitchLib.Client.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything realted to general Notifications
    /// </summary>
    public interface ITwitchClient_Notice
    {
        #region events public
        /// <summary>
        ///     Occurs when [on chat color changed].
        /// </summary>
        event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;
        /// <summary>
        ///     Occurs when [on no permission].
        /// </summary>
        event EventHandler<EventArgs> OnNoPermissionError;
        /// <summary>
        ///     Occurs when [on moderators received].
        /// </summary>
        event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;
        /// <summary>
        ///     Occurs when [on vip received].
        /// </summary>
        event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;
        /// <summary>
        ///     Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
        /// </summary>
        event EventHandler<OnRequiresVerifiedEmailArgs> OnRequiresVerifiedEmail;
        /// <summary>
        ///     Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
        /// </summary>
        event EventHandler<OnRequiresVerifiedPhoneNumberArgs> OnRequiresVerifiedPhoneNumber;
        /// <summary>
        ///     Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
        /// </summary>
        event EventHandler<OnBannedEmailAliasArgs> OnBannedEmailAlias;
        /// <summary>
        ///     Fires when TwitchClient attempts to host a channel it is in.
        /// </summary>
        event EventHandler<EventArgs> OnSelfRaidError;
        /// <summary>
        ///     Fires when newly raided channel is mature audience only.
        /// </summary>
        event EventHandler<EventArgs> OnRaidedChannelIsMatureAudience;
        /// <summary>
        ///     Fires when the client attempts to send a message to a channel in followers only mode, as a non-follower
        /// </summary>
        event EventHandler<OnFollowersOnlyArgs> OnFollowersOnly;
        /// <summary>
        ///     Fires when the client attempts to send a message to a channel in subs only mode, as a non-sub
        /// </summary>
        event EventHandler<OnSubsOnlyArgs> OnSubsOnly;
        /// <summary>
        ///     Fires when the client attempts to send a non-emote message to a channel in emotes only mode
        /// </summary>
        event EventHandler<OnEmoteOnlyArgs> OnEmoteOnly;
        /// <summary>
        ///     Fires when the client attempts to send a message to a channel that has been suspended
        /// </summary>
        event EventHandler<OnSuspendedArgs> OnSuspended;
        /// <summary>
        ///     Fires when the client attempts to send a message to a channel they're banned in
        /// </summary>
        event EventHandler<OnBannedArgs> OnBanned;
        /// <summary>
        ///     Fires when the client attempts to send a message in a channel with slow mode enabled, without cooldown expiring
        /// </summary>
        event EventHandler<OnSlowModeArgs> OnSlowMode;
        /// <summary>
        ///     Fires when the client attempts to send a message in a channel with r9k mode enabled, and message was not permitted
        /// </summary>
        event EventHandler<OnR9kModeArgs> OnR9kMode;
        /// <summary>
        ///     Occurs when send message rate limit has been applied to the client in a specific channel by Twitch
        /// </summary>
        event EventHandler<OnRateLimitArgs> OnRateLimit;
        /// <summary>
        ///     Occurs when sending duplicate messages and user is not permitted to do so
        /// </summary>
        event EventHandler<OnDuplicateArgs> OnDuplicate;
        #endregion events public
    }
}
