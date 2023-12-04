using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    /// Interface ITwitchClient
    /// </summary>
    public interface ITwitchClient
    {
        /// <summary>
        /// Gets the channel emotes.
        /// </summary>
        /// <value>The channel emotes.</value>
        MessageEmoteCollection ChannelEmotes { get; }
        /// <summary>
        /// Gets the connection credentials.
        /// </summary>
        /// <value>The connection credentials.</value>
        ConnectionCredentials? ConnectionCredentials { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [disable automatic pong].
        /// </summary>
        /// <value><c>true</c> if [disable automatic pong]; otherwise, <c>false</c>.</value>
        bool DisableAutoPong { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        bool IsConnected { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        bool IsInitialized { get; }
        /// <summary>
        /// Gets the joined channels.
        /// </summary>
        /// <value>The joined channels.</value>
        IReadOnlyList<JoinedChannel> JoinedChannels { get; }
        /// <summary>
        /// Gets the previous whisper.
        /// </summary>
        /// <value>The previous whisper.</value>
        WhisperMessage? PreviousWhisper { get; }
        /// <summary>
        /// Gets the twitch username.
        /// </summary>
        /// <value>The twitch username.</value>
        string TwitchUsername { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [will replace emotes].
        /// </summary>
        /// <value><c>true</c> if [will replace emotes]; otherwise, <c>false</c>.</value>
        bool WillReplaceEmotes { get; set; }
        /// <summary>
        /// The chat command identifiers
        /// </summary>
        ICollection<char> ChatCommandIdentifiers { get; }
        /// <summary>
        /// The whisper command identifiers
        /// </summary>
        ICollection<char> WhisperCommandIdentifiers { get; }

        /// <summary>
        /// Fires when an Announcement is received
        /// </summary>
        event AsyncEventHandler<OnAnnouncementArgs>? OnAnnouncement;

        /// <summary>
        /// Fires when client connects to Twitch.
        /// </summary>
        event AsyncEventHandler<Events.OnConnectedEventArgs>? OnConnected;

        /// <summary>
        /// Fires when client joins a channel.
        /// </summary>
        event AsyncEventHandler<OnJoinedChannelArgs>? OnJoinedChannel;

        /// <summary>
        /// Fires on logging in with incorrect details, returns ErrorLoggingInException.
        /// </summary>
        event AsyncEventHandler<OnIncorrectLoginArgs>? OnIncorrectLogin;

        /// <summary>
        /// Fires when connecting and channel state is changed, returns ChannelState.
        /// </summary>
        event AsyncEventHandler<OnChannelStateChangedArgs>? OnChannelStateChanged;

        /// <summary>
        /// Fires when a user state is received, returns UserState.
        /// </summary>
        event AsyncEventHandler<OnUserStateChangedArgs>? OnUserStateChanged;

        /// <summary>
        /// Fires when a new chat message arrives, returns ChatMessage.
        /// </summary>
        event AsyncEventHandler<OnMessageReceivedArgs>? OnMessageReceived;

        /// <summary>
        /// Fires when a new whisper arrives, returns WhisperMessage.
        /// </summary>
        event AsyncEventHandler<OnWhisperReceivedArgs>? OnWhisperReceived;

        /// <summary>
        /// Fires when a chat message is sent, returns username, channel and message.
        /// </summary>
        event AsyncEventHandler<OnMessageSentArgs>? OnMessageSent;

        /// <summary>
        /// Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list.
        /// </summary>
        event AsyncEventHandler<OnChatCommandReceivedArgs>? OnChatCommandReceived;

        /// <summary>
        /// Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
        /// </summary>
        event AsyncEventHandler<OnWhisperCommandReceivedArgs>? OnWhisperCommandReceived;

        /// <summary>
        /// Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
        /// </summary>
        event AsyncEventHandler<OnUserJoinedArgs>? OnUserJoined;

        /// <summary>
        /// Fires when a message gets deleted in chat.
        /// </summary>
        event AsyncEventHandler<OnMessageClearedArgs>? OnMessageCleared;

        /// <summary>
        /// Fires when new subscriber is announced in chat, returns Subscriber.
        /// </summary>
        event AsyncEventHandler<OnNewSubscriberArgs>? OnNewSubscriber;

        /// <summary>
        /// Fires when current subscriber renews subscription, returns ReSubscriber.
        /// </summary>
        event AsyncEventHandler<OnReSubscriberArgs>? OnReSubscriber;

        /// <summary>
        /// Fires when a current Prime gaming subscriber converts to a paid subscription.
        /// </summary>
        event AsyncEventHandler<OnPrimePaidSubscriberArgs>? OnPrimePaidSubscriber;

        /// <summary>
        /// Fires when Twitch notifies client of existing users in chat.
        /// </summary>
        event AsyncEventHandler<OnExistingUsersDetectedArgs>? OnExistingUsersDetected;

        /// <summary>
        /// Fires when a PART message is received from Twitch regarding a particular viewer
        /// </summary>
        event AsyncEventHandler<OnUserLeftArgs>? OnUserLeft;

        /// <summary>
        /// Fires when bot has disconnected.
        /// </summary>
        event AsyncEventHandler<OnDisconnectedEventArgs>? OnDisconnected;

        /// <summary>
        /// Forces when bot suffers connection error.
        /// </summary>
        event AsyncEventHandler<OnConnectionErrorArgs>? OnConnectionError;

        /// <summary>
        /// Fires when a channel's chat is cleared.
        /// </summary>
        event AsyncEventHandler<OnChatClearedArgs>? OnChatCleared;

        /// <summary>
        /// Fires when a viewer gets timedout by any moderator.
        /// </summary>
        event AsyncEventHandler<OnUserTimedoutArgs>? OnUserTimedout;

        /// <summary>
        /// Fires when client successfully leaves a channel.
        /// </summary>
        event AsyncEventHandler<OnLeftChannelArgs>? OnLeftChannel;

        /// <summary>
        /// Fires when a viewer gets banned by any moderator.
        /// </summary>
        event AsyncEventHandler<OnUserBannedArgs>? OnUserBanned;

        /// <summary>
        /// Fires when data is either received or sent.
        /// </summary>
        event AsyncEventHandler<OnSendReceiveDataArgs>? OnSendReceiveData;

        /// <summary>
        /// Fires when a raid notification is detected in chat
        /// </summary>
        event AsyncEventHandler<OnRaidNotificationArgs>? OnRaidNotification;

        /// <summary>
        /// Fires when a subscription is gifted and announced in chat
        /// </summary>
        event AsyncEventHandler<OnGiftedSubscriptionArgs>? OnGiftedSubscription;

        /// <summary>
        /// Fires when a community subscription is announced in chat
        /// </summary>
        event AsyncEventHandler<OnCommunitySubscriptionArgs>? OnCommunitySubscription;

        /// <summary>
        /// Fires when a gifted subscription is continued and announced in chat
        /// </summary>
        event AsyncEventHandler<OnContinuedGiftedSubscriptionArgs>? OnContinuedGiftedSubscription;

        public event AsyncEventHandler<OnAnonGiftPaidUpgradeArgs>? OnAnonGiftPaidUpgrade;
        public event AsyncEventHandler<OnUnraidNotificationArgs>? OnUnraidNotification;
        public event AsyncEventHandler<OnRitualArgs>? OnRitual;
        public event AsyncEventHandler<OnBitsBadgeTierArgs>? OnBitsBadgeTier;
        public event AsyncEventHandler<OnCommunityPayForwardArgs>? OnCommunityPayForward;
        public event AsyncEventHandler<OnStandardPayForwardArgs>? OnStandardPayForward;

        /// <summary>
        /// Fires when a Message has been throttled.
        /// </summary>
        event AsyncEventHandler<OnMessageThrottledArgs>? OnMessageThrottled;

        /// <summary>
        /// Occurs when an Error is thrown in the protocol client
        /// </summary>
        event AsyncEventHandler<OnErrorEventArgs>? OnError;

        /// <summary>
        /// Occurs when a reconnection occurs.
        /// </summary>
        event AsyncEventHandler<Events.OnConnectedEventArgs>? OnReconnected;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnRequiresVerifiedEmail;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnRequiresVerifiedPhoneNumber;

        /// <summary>
        /// Occurs when send message rate limit has been applied to the client in a specific channel by Twitch
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnRateLimit;

        /// <summary>
        /// Occurs when sending duplicate messages and user is not permitted to do so
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnDuplicate;

        /// <summary>
        /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnBannedEmailAlias;

        /// <summary>
        /// Fires when TwitchClient attempts to host a channel it is in.
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnSelfRaidError;

        /// <summary>
        /// Fires when TwitchClient receives generic no permission error from Twitch.
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnNoPermissionError;

        /// <summary>
        /// Fires when newly raided channel is mature audience only.
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnRaidedChannelIsMatureAudience;

        /// <summary>
        /// Fires when the client was unable to join a channel.
        /// </summary>
        event AsyncEventHandler<OnFailureToReceiveJoinConfirmationArgs>? OnFailureToReceiveJoinConfirmation;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in followers only mode, as a non-follower
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnFollowersOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in subs only mode, as a non-sub
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnSubsOnly;

        /// <summary>
        /// Fires when the client attempts to send a non-emote message to a channel in emotes only mode
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnEmoteOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel that has been suspended
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnSuspended;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel they're banned in
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnBanned;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with slow mode enabled, without cooldown expiring
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnSlowMode;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with r9k mode enabled, and message was not permitted
        /// </summary>
        event AsyncEventHandler<NoticeEventArgs>? OnR9kMode;

        /// <summary>
        /// Fires when the client receives a PRIVMSG tagged as an user-intro
        /// </summary>
        event AsyncEventHandler<OnUserIntroArgs>? OnUserIntro;

        /// <summary>
        /// Fires when data is received from Twitch that is not able to be parsed.
        /// </summary>
        event AsyncEventHandler<OnUnaccountedForArgs>? OnUnaccountedFor;

        /// <summary>
        /// Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="channel">The channel.</param>
        void Initialize(ConnectionCredentials credentials, string? channel = null);

        /// <summary>
        /// Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="channels">The channels to join once connected.</param>
        void Initialize(ConnectionCredentials credentials, List<string> channels);

        /// <summary>
        /// Sets the connection credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        void SetConnectionCredentials(ConnectionCredentials credentials);

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>bool representing Connect() result</returns>
        Task<bool> ConnectAsync();
        
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        Task DisconnectAsync();
        
        /// <summary>
        /// Reconnects this instance.
        /// </summary>
        Task ReconnectAsync();

        /// <summary>
        /// Gets the joined channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>JoinedChannel.</returns>
        JoinedChannel? GetJoinedChannel(string channel);
        
        /// <summary>
        /// Joins the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="overrideCheck">if set to <c>true</c> [override check].</param>
        Task JoinChannelAsync(string channel, bool overrideCheck = false);
        
        /// <summary>
        /// Leaves the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        Task LeaveChannelAsync(JoinedChannel channel);
        
        /// <summary>
        /// Leaves the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        Task LeaveChannelAsync(string channel);
        
        /// <summary>
        /// Called when [read line test].
        /// </summary>
        /// <param name="rawIrc">The raw irc.</param>
        Task OnReadLineTestAsync(string rawIrc);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        Task SendMessageAsync(JoinedChannel channel, string message, bool dryRun = false);
        
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        Task SendMessageAsync(string channel, string message, bool dryRun = false);
        
        /// <summary>
        /// Sends a formatted Twitch chat message reply.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        Task SendReplyAsync(JoinedChannel channel, string replyToId, string message, bool dryRun = false);
        
        /// <summary>
        /// SendReply wrapper that accepts channel in string form.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        Task SendReplyAsync(string channel, string replyToId, string message, bool dryRun = false);
        
        /// <summary>
        /// Sends the queued item.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SendQueuedItemAsync(string message);
        
        /// <summary>
        /// Sends the raw.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SendRawAsync(string message);
    }
}
