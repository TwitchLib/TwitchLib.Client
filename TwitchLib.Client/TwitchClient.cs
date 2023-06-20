﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Internal.Parsing;
using TwitchLib.Client.Manager;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Throttling;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client
{
    /// <summary>
    /// Represents a client connected to a Twitch channel.
    /// Implements the <see cref="TwitchLib.Client.Interfaces.ITwitchClient" />
    /// </summary>
    /// <seealso cref="TwitchLib.Client.Interfaces.ITwitchClient" />
    public class TwitchClient : ITwitchClient
    {
        #region Private Variables
        /// <summary>
        /// The client
        /// </summary>
        private IClient _client;

        private readonly ISendOptions _sendOptions;
        private ThrottlingService _throttling;
        
        /// <summary>
        /// The channel emotes
        /// </summary>
        private MessageEmoteCollection _channelEmotes = new MessageEmoteCollection();
        /// <summary>
        /// The chat command identifiers
        /// </summary>
        private readonly ICollection<char> _chatCommandIdentifiers = new HashSet<char>();
        /// <summary>
        /// The whisper command identifiers
        /// </summary>
        private readonly ICollection<char> _whisperCommandIdentifiers = new HashSet<char>();
        /// <summary>
        /// The join channel queue
        /// </summary>
        private readonly Queue<JoinedChannel> _joinChannelQueue = new Queue<JoinedChannel>();
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<TwitchClient> _logger;
        private readonly ILoggerFactory _loggerFactory;
        /// <summary>
        /// The protocol
        /// </summary>
        private readonly ClientProtocol _protocol;
        /// <summary>
        /// The currently joining channels
        /// </summary>
        private bool _currentlyJoiningChannels;
        /// <summary>
        /// The join timer
        /// </summary>
        private System.Timers.Timer _joinTimer;
        /// <summary>
        /// The awaiting joins
        /// </summary>
        private List<KeyValuePair<string, DateTime>> _awaitingJoins;

        /// <summary>
        /// The joined channel manager
        /// </summary>
        private readonly JoinedChannelManager _joinedChannelManager = new();

        // variables used for constructing OnMessageSent properties
        /// <summary>
        /// The has seen joined channels
        /// </summary>
        private readonly List<string> _hasSeenJoinedChannels = new List<string>();
        /// <summary>
        /// The last message sent
        /// </summary>
        private string _lastMessageSent;
        #endregion

        #region Public Variables
        /// <summary>
        /// Assembly version of TwitchLib.Client.
        /// </summary>
        /// <value>The version.</value>
        public Version Version => Assembly.GetEntryAssembly().GetName().Version;
        /// <summary>
        /// Checks if underlying client has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized => _client != null;
        /// <summary>
        /// A list of all channels the client is currently in.
        /// </summary>
        /// <value>The joined channels.</value>
        public IReadOnlyList<JoinedChannel> JoinedChannels => _joinedChannelManager.GetJoinedChannels();
        /// <summary>
        /// Username of the user connected via this library.
        /// </summary>
        /// <value>The twitch username.</value>
        public string TwitchUsername { get; private set; }
        /// <summary>
        /// The most recent whisper received.
        /// </summary>
        /// <value>The previous whisper.</value>
        public WhisperMessage PreviousWhisper { get; private set; }
        /// <summary>
        /// The current connection status of the client.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected => IsInitialized && _client != null ? _client.IsConnected : false;

        /// <summary>
        /// The emotes this channel replaces.
        /// </summary>
        /// <value>The channel emotes.</value>
        /// <remarks>Twitch-handled emotes are automatically added to this collection (which also accounts for
        /// managing user emote permissions such as sub-only emotes). Third-party emotes will have to be manually
        /// added according to the availability rules defined by the third-party.</remarks>
        public MessageEmoteCollection ChannelEmotes => _channelEmotes;

        /// <summary>
        /// Will disable the client from sending automatic PONG responses to PING
        /// </summary>
        /// <value><c>true</c> if [disable automatic pong]; otherwise, <c>false</c>.</value>
        public bool DisableAutoPong { get; set; } = false;
        /// <summary>
        /// Determines whether Emotes will be replaced in messages.
        /// </summary>
        /// <value><c>true</c> if [will replace emotes]; otherwise, <c>false</c>.</value>
        public bool WillReplaceEmotes { get; set; } = false;
        /// <summary>
        /// Provides access to connection credentials object.
        /// </summary>
        /// <value>The connection credentials.</value>
        public ConnectionCredentials ConnectionCredentials { get; private set; }

        #endregion

        #region Events
        /// <summary>
        /// Fires when an Announcement is received
        /// </summary>
        public event EventHandler<OnAnnouncementArgs> OnAnnouncement;

        /// <summary>
        /// Fires when VIPs are received from chat
        /// </summary>
        public event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;

        /// <summary>
        /// Fires when client connects to Twitch.
        /// </summary>
        public event EventHandler<OnConnectedArgs> OnConnected;

        /// <summary>
        /// Fires when client joins a channel.
        /// </summary>
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;

        /// <summary>
        /// Fires on logging in with incorrect details, returns ErrorLoggingInException.
        /// </summary>
        public event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;

        /// <summary>
        /// Fires when connecting and channel state is changed, returns ChannelState.
        /// </summary>
        public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;

        /// <summary>
        /// Fires when a user state is received, returns UserState.
        /// </summary>
        public event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;

        /// <summary>
        /// Fires when a new chat message arrives, returns ChatMessage.
        /// </summary>
        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        /// <summary>
        /// Fires when a new whisper arrives, returns WhisperMessage.
        /// </summary>
        public event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;

        /// <summary>
        /// Fires when a chat message is sent, returns username, channel and message.
        /// </summary>
        public event EventHandler<OnMessageSentArgs> OnMessageSent;

        /// <summary>
        /// Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list.
        /// </summary>
        public event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

        /// <summary>
        /// Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
        /// </summary>
        public event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;

        /// <summary>
        /// Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
        /// </summary>
        public event EventHandler<OnUserJoinedArgs> OnUserJoined;

        /// <summary>
        /// Fires when a moderator joined the channel's chat room, returns username and channel.
        /// </summary>
        public event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;

        /// <summary>
        /// Fires when a moderator joins the channel's chat room, returns username and channel.
        /// </summary>
        public event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;

        /// <summary>
        /// Fires when a message gets deleted in chat.
        /// </summary>
        public event EventHandler<OnMessageClearedArgs> OnMessageCleared;

        /// <summary>
        /// Fires when new subscriber is announced in chat, returns Subscriber.
        /// </summary>
        public event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;

        /// <summary>
        /// Fires when current subscriber renews subscription, returns ReSubscriber.
        /// </summary>
        public event EventHandler<OnReSubscriberArgs> OnReSubscriber;

        /// <summary>
        /// Fires when a current Prime gaming subscriber converts to a paid subscription.
        /// </summary>
        public event EventHandler<OnPrimePaidSubscriberArgs> OnPrimePaidSubscriber;

        /// <summary>
        /// Fires when Twitch notifies client of existing users in chat.
        /// </summary>
        public event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

        /// <summary>
        /// Fires when a PART message is received from Twitch regarding a particular viewer
        /// </summary>
        public event EventHandler<OnUserLeftArgs> OnUserLeft;

        /// <summary>
        /// Fires when bot has disconnected.
        /// </summary>
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;

        /// <summary>
        /// Forces when bot suffers connection error.
        /// </summary>
        public event EventHandler<OnConnectionErrorArgs> OnConnectionError;

        /// <summary>
        /// Fires when a channel's chat is cleared.
        /// </summary>
        public event EventHandler<OnChatClearedArgs> OnChatCleared;

        /// <summary>
        /// Fires when a viewer gets timedout by any moderator.
        /// </summary>
        public event EventHandler<OnUserTimedoutArgs> OnUserTimedout;

        /// <summary>
        /// Fires when client successfully leaves a channel.
        /// </summary>
        public event EventHandler<OnLeftChannelArgs> OnLeftChannel;

        /// <summary>
        /// Fires when a viewer gets banned by any moderator.
        /// </summary>
        public event EventHandler<OnUserBannedArgs> OnUserBanned;

        /// <summary>
        /// Fires when a list of moderators is received.
        /// </summary>
        public event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;

        /// <summary>
        /// Fires when confirmation of a chat color change request was received.
        /// </summary>
        public event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;

        /// <summary>
        /// Fires when data is either received or sent.
        /// </summary>
        public event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;

        /// <summary>
        /// Fires when a raid notification is detected in chat
        /// </summary>
        public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

        /// <summary>
        /// Fires when a subscription is gifted and announced in chat
        /// </summary>
        public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

        /// <summary>
        /// Fires when a community subscription is announced in chat
        /// </summary>
        public event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<OnContinuedGiftedSubscriptionArgs> OnContinuedGiftedSubscription;

        /// <summary>
        /// Fires when a Message has been throttled.
        /// </summary>
        public event EventHandler<OnMessageThrottledArgs> OnMessageThrottled;

        /// <summary>
        /// Occurs when an Error is thrown in the protocol client
        /// </summary>
        public event EventHandler<OnErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when a reconnection occurs.
        /// </summary>
        public event EventHandler<OnConnectedArgs> OnReconnected;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
        /// </summary>
        public event EventHandler<OnRequiresVerifiedEmailArgs> OnRequiresVerifiedEmail;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
        /// </summary>
        public event EventHandler<OnRequiresVerifiedPhoneNumberArgs> OnRequiresVerifiedPhoneNumber;

        /// <summary>
        /// Occurs when send message rate limit has been applied to the client in a specific channel by Twitch
        /// </summary>
        public event EventHandler<OnRateLimitArgs> OnRateLimit;

        /// <summary>
        /// Occurs when sending duplicate messages and user is not permitted to do so
        /// </summary>
        public event EventHandler<OnDuplicateArgs> OnDuplicate;

        /// <summary>
        /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
        /// </summary>
        public event EventHandler<OnBannedEmailAliasArgs> OnBannedEmailAlias;

        /// <summary>
        /// Fires when TwitchClient attempts to host a channel it is in.
        /// </summary>
        public event EventHandler OnSelfRaidError;

        /// <summary>
        /// Fires when TwitchClient receives generic no permission error from Twitch.
        /// </summary>
        public event EventHandler OnNoPermissionError;

        /// <summary>
        /// Fires when newly raided channel is mature audience only.
        /// </summary>
        public event EventHandler OnRaidedChannelIsMatureAudience;

        /// <summary>
        /// Fires when the client was unable to join a channel.
        /// </summary>
        public event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in followers only mode, as a non-follower
        /// </summary>
        public event EventHandler<OnFollowersOnlyArgs> OnFollowersOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in subs only mode, as a non-sub
        /// </summary>
        public event EventHandler<OnSubsOnlyArgs> OnSubsOnly;

        /// <summary>
        /// Fires when the client attempts to send a non-emote message to a channel in emotes only mode
        /// </summary>
        public event EventHandler<OnEmoteOnlyArgs> OnEmoteOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel that has been suspended
        /// </summary>
        public event EventHandler<OnSuspendedArgs> OnSuspended;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel they're banned in
        /// </summary>
        public event EventHandler<OnBannedArgs> OnBanned;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with slow mode enabled, without cooldown expiring
        /// </summary>
        public event EventHandler<OnSlowModeArgs> OnSlowMode;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with r9k mode enabled, and message was not permitted
        /// </summary>
        public event EventHandler<OnR9kModeArgs> OnR9kMode;

        /// <summary>
        /// Fires when the client receives a PRIVMSG tagged as an user-intro
        /// </summary>
        public event EventHandler<OnUserIntroArgs> OnUserIntro;

        /// <summary>
        /// Fires when data is received from Twitch that is not able to be parsed.
        /// </summary>
        public event EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
        #endregion

        #region Construction Work

        /// <summary>
        /// Initializes the TwitchChatClient class.
        /// </summary>
        /// <param name="client">Protocol Client to use for connection from TwitchLib.Communication. Possible Options Are the TcpClient client or WebSocket client.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="sendOptions">Send options with throttling settings.</param>
        /// <param name="loggerFactory">Optional ILoggerFactory instance to enable logging</param>
        public TwitchClient(
            IClient client = null, 
            ClientProtocol protocol = ClientProtocol.WebSocket,
            ISendOptions sendOptions = null,
            ILoggerFactory loggerFactory = null)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory?.CreateLogger<TwitchClient>();
            _client = client;
            _protocol = protocol;
            _sendOptions = sendOptions ?? new SendOptions();
        }

        /// <summary>
        /// Initializes the TwitchChatClient class.
        /// </summary>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channel">The channel to connect to.</param>
        /// <param name="chatCommandIdentifier">The identifier to be used for reading and writing commands from chat.</param>
        /// <param name="whisperCommandIdentifier">The identifier to be used for reading and writing commands from whispers.</param>
        public void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!')
        {
            if (channel?[0] == '#') channel = channel.Substring(1);
            InitializationHelper(credentials, new List<string>() { channel }, chatCommandIdentifier, whisperCommandIdentifier);
        }

        /// <summary>
        /// Initializes the TwitchChatClient class (with multiple channels).
        /// </summary>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channels">List of channels to join when connected</param>
        /// <param name="chatCommandIdentifier">The identifier to be used for reading and writing commands from chat.</param>
        /// <param name="whisperCommandIdentifier">The identifier to be used for reading and writing commands from whispers.</param>
        public void Initialize(ConnectionCredentials credentials, List<string> channels, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!')
        {
            channels = channels.ConvertAll(x => x[0] == '#' ? x.Substring(1) : x);
            InitializationHelper(credentials, channels, chatCommandIdentifier, whisperCommandIdentifier);
        }

        /// <summary>
        /// Runs initialization logic that is shared by the overriden Initialize methods.
        /// </summary>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channels">List of channels to join when connected</param>
        /// <param name="chatCommandIdentifier">The identifier to be used for reading and writing commands from chat.</param>
        /// <param name="whisperCommandIdentifier">The identifier to be used for reading and writing commands from whispers.</param>
        private void InitializationHelper(
            ConnectionCredentials credentials, 
            List<string> channels, 
            char chatCommandIdentifier = '!', 
            char whisperCommandIdentifier = '!')
        {
            _logger?.LogInitialized(Assembly.GetExecutingAssembly().GetName().Version);
            ConnectionCredentials = credentials;
            TwitchUsername = ConnectionCredentials.TwitchUsername;
            if (chatCommandIdentifier != '\0')
                _chatCommandIdentifiers.Add(chatCommandIdentifier);
            if (whisperCommandIdentifier != '\0')
                _whisperCommandIdentifiers.Add(whisperCommandIdentifier);

            if (channels?.Count > 0)
            {
                for (var i = 0; i < channels.Count; i++)
                {
                    if (string.IsNullOrEmpty(channels[i]))
                        continue;

                    // Check to see if client is already in channel
                    if (JoinedChannels.Any(x => x.Channel.Equals(channels[i], StringComparison.OrdinalIgnoreCase)))
                        return;
                    
                    _joinChannelQueue.Enqueue(new JoinedChannel(channels[i]));
                }
            }

            InitializeClient();
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        private void InitializeClient()
        {
            if (_client == null)
            {
                switch (_protocol)
                {
                    case ClientProtocol.TCP:
                        _client = new TcpClient(null, _loggerFactory?.CreateLogger<TcpClient>());
                        break;
                    case ClientProtocol.WebSocket:
                        _client = new WebSocketClient(null, _loggerFactory?.CreateLogger<WebSocketClient>());
                        break;
                }
            }

            Debug.Assert(_client != null, nameof(_client) + " != null");

            _throttling = new ThrottlingService(_client, _sendOptions, _logger);
            _throttling.OnThrottled += OnThrottled;

            _client.OnConnected += _client_OnConnectedAsync;
            _client.OnMessage += _client_OnMessage;
            _client.OnDisconnected += _client_OnDisconnected;
            _client.OnFatality += _client_OnFatality;
            _client.OnReconnected += _client_OnReconnected;
        }
        #endregion

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="args">The arguments.</param>
        internal void RaiseEvent(string eventName, object args = null)
        {
            EventHelper.RaiseEvent(this, eventName, args);
        }

        private void OnThrottled(object sender, OnMessageThrottledArgs e)
        {
            RaiseEvent(nameof(OnMessageThrottled), e);
        }
        
        /// <summary>
        /// Sends a RAW IRC message.
        /// </summary>
        /// <param name="message">The RAW message to be sent.</param>
        public void SendRaw(string message)
        {
            SendRawAsync(message).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task SendRawAsync(string message)
        {
            if (!IsInitialized) 
                HandleNotInitialized();

            _logger?.LogWriting(message);
            await _client.SendAsync(message);
            
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }

        #region SendMessage

        private void SendTwitchMessage(JoinedChannel channel, string message, string replyToId = null, bool dryRun = false)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            if (channel == null || message == null || dryRun) 
                return;
            
            if (message.Length > 500)
            {
                _logger?.LogMessageTooLong();
                return;
            }

            var twitchMessage = new OutboundChatMessage
            {
                Channel = channel.Channel,
                Username = ConnectionCredentials.TwitchUsername,
                Message = message
            };
            
            if (replyToId != null)
            {
                twitchMessage.ReplyToId = replyToId;
            }

            _lastMessageSent = message;

            _throttling.Enqueue(twitchMessage);
        }

        /// <summary>
        /// Sends a formatted Twitch channel chat message.
        /// </summary>
        /// <param name="channel">Channel to send message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="dryRun">If set to true, the message will not actually be sent for testing purposes.</param>
        public void SendMessage(JoinedChannel channel, string message, bool dryRun = false)
        {
            SendMessageAsync(channel, message, dryRun).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public Task SendMessageAsync(JoinedChannel channel, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message, null, dryRun);
            return Task.CompletedTask;
        }

        /// <summary>
        /// SendMessage wrapper that accepts channel in string form.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            SendMessage(GetJoinedChannel(channel), message, dryRun);
        }
        
        /// <inheritdoc />
        public async Task SendMessageAsync(string channel, string message, bool dryRun = false)
        {
            await SendMessageAsync(GetJoinedChannel(channel), message, dryRun);
        }

        /// <summary>
        /// Sends a formatted Twitch chat message reply.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        public void SendReply(JoinedChannel channel, string replyToId, string message, bool dryRun = false)
        {
            SendReplyAsync(channel, replyToId, message, dryRun).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public Task SendReplyAsync(JoinedChannel channel, string replyToId, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message, replyToId, dryRun);
            return Task.CompletedTask;
        }

        /// <summary>
        /// SendReply wrapper that accepts channel in string form.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        public void SendReply(string channel, string replyToId, string message, bool dryRun = false)
        {
            SendReplyAsync(channel, replyToId, message, dryRun).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task SendReplyAsync(string channel, string replyToId, string message, bool dryRun = false)
        {
            await SendReplyAsync(GetJoinedChannel(channel), replyToId, message, dryRun);
        }

        #endregion

        #region Connection Calls

        /// <summary>
        /// Start connecting to the Twitch IRC chat.
        /// </summary>
        /// <returns>bool representing Connect() result</returns>
        public bool Connect()
        {
            return ConnectAsync().GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task<bool> ConnectAsync()
        {
            if (!IsInitialized) 
                HandleNotInitialized();

            _logger?.LogConnecting();

			// Clear instance data
            _joinedChannelManager.Clear();

            if(await _client.OpenAsync())
            {
                _logger?.LogShouldBeConnected();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Start disconnecting from the Twitch IRC chat.
        /// </summary>
        public void Disconnect()
        {
            DisconnectAsync().GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            _logger?.LogDisconnecting();

            if (!IsInitialized) 
                HandleNotInitialized();
            
            await _client.CloseAsync();

            // Clear instance data
            _joinedChannelManager.Clear();
            PreviousWhisper = null;
        }

        /// <summary>
        /// Start reconnecting to the Twitch IRC chat.
        /// </summary>
        public void Reconnect()
        {
            ReconnectAsync().GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task ReconnectAsync()
        {
            if (!IsInitialized) 
                HandleNotInitialized();

            _logger?.LogReconnecting();
            await _client.ReconnectAsync();
        }
        #endregion

        #region Command Identifiers
        /// <summary>
        /// Adds a character to a list of characters that if found at the start of a message, fires command received event.
        /// </summary>
        /// <param name="identifier">Character, that if found at start of message, fires command received event.</param>
        public void AddChatCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            _chatCommandIdentifiers.Add(identifier);
        }

        /// <summary>
        /// Removes a character from a list of characters that if found at the start of a message, fires command received event.
        /// </summary>
        /// <param name="identifier">Command identifier to removed from identifier list.</param>
        public void RemoveChatCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            _chatCommandIdentifiers.Remove(identifier);
        }

        /// <summary>
        /// Adds a character to a list of characters that if found at the start of a whisper, fires command received event.
        /// </summary>
        /// <param name="identifier">Character, that if found at start of message, fires command received event.</param>
        public void AddWhisperCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            _whisperCommandIdentifiers.Add(identifier);
        }

        /// <summary>
        /// Removes a character to a list of characters that if found at the start of a whisper, fires command received event.
        /// </summary>
        /// <param name="identifier">Command identifier to removed from identifier list.</param>
        public void RemoveWhisperCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            _whisperCommandIdentifiers.Remove(identifier);
        }
        #endregion

        #region ConnectionCredentials

        /// <summary>
        /// Sets the connection credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <exception cref="TwitchLib.Client.Exceptions.IllegalAssignmentException">While the client is connected, you are unable to change the connection credentials. Please disconnect first and then change them.</exception>
        public void SetConnectionCredentials(ConnectionCredentials credentials)
        {
            if (!IsInitialized)
                HandleNotInitialized();
            if (IsConnected)
                throw new IllegalAssignmentException("While the client is connected, you are unable to change the connection credentials. Please disconnect first and then change them.");

            ConnectionCredentials = credentials;
        }

        #endregion

        #region Channel Calls

        /// <summary>
        /// Join the Twitch IRC chat of <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">The channel to join.</param>
        /// <param name="overrideCheck">Override a join check.</param>
        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            JoinChannelAsync(channel, overrideCheck).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task JoinChannelAsync(string channel, bool overrideCheck = false)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            if (!IsConnected) 
                HandleNotConnected();
            
            // Check to see if client is already in channel
            if (JoinedChannels.FirstOrDefault(x => x.Channel.ToLower() == channel && !overrideCheck) != null)
                return;
            
            if (channel[0] == '#') 
                channel = channel.Substring(1);
            
            _joinChannelQueue.Enqueue(new JoinedChannel(channel));
            
            if (!_currentlyJoiningChannels)
                await QueueingJoinCheckAsync();
        }

        /// <summary>
        /// Returns a JoinedChannel object using a passed string/&gt;.
        /// </summary>
        /// <param name="channel">String channel to search for.</param>
        /// <returns>JoinedChannel.</returns>
        /// <exception cref="TwitchLib.Client.Exceptions.BadStateException">Must be connected to at least one channel.</exception>
        public JoinedChannel GetJoinedChannel(string channel)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            if (JoinedChannels.Count == 0)
                throw new BadStateException("Must be connected to at least one channel.");
            
            if (channel[0] == '#') 
                channel = channel.Substring(1);
            
            return _joinedChannelManager.GetJoinedChannel(channel);
        }

        /// <summary>
        /// Leaves (PART) the Twitch IRC chat of <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">The channel to leave.</param>
        /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
        public void LeaveChannel(string channel)
        {
            LeaveChannelAsync(channel).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task LeaveChannelAsync(string channel)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            // Channel MUST be lower case
            channel = channel.ToLower();
            if (channel[0] == '#') 
                channel = channel.Substring(1);

            _logger?.LogLeavingChannel(channel);
            var joinedChannel = _joinedChannelManager.GetJoinedChannel(channel);
            
            if (joinedChannel != null)
                await _client.SendAsync(Rfc2812.Part($"#{channel}"));
        }

        /// <summary>
        /// Leaves (PART) the Twitch IRC chat of <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">The JoinedChannel object to leave.</param>
        /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
        public void LeaveChannel(JoinedChannel channel)
        {
            LeaveChannelAsync(channel).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task LeaveChannelAsync(JoinedChannel channel)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            await LeaveChannelAsync(channel.Channel);
        }

        #endregion

        /// <summary>
        /// This method allows firing the message parser with a custom irc string allowing for easy testing
        /// </summary>
        /// <param name="rawIrc">This should be a raw IRC message resembling one received from Twitch IRC.</param>
        public void OnReadLineTest(string rawIrc)
        {
            OnReadLineTestAsync(rawIrc).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task OnReadLineTestAsync(string rawIrc)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            await HandleIrcMessageAsync(IrcParser.ParseMessage(rawIrc));
        }

        #region Client Events
        /// <summary>
        /// Handles the OnFatality event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnFatalErrorEventArgs" /> instance containing the event data.</param>
        private void _client_OnFatality(object sender, OnFatalErrorEventArgs e)
        {
            OnConnectionError?.Invoke(this, new OnConnectionErrorArgs { BotUsername = TwitchUsername, Error = new ErrorEvent { Message = e.Reason } });
        }

        /// <summary>
        /// Handles the OnDisconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnDisconnectedEventArgs" /> instance containing the event data.</param>
        private void _client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            OnDisconnected?.Invoke(sender, e);
        }

        /// <summary>
        /// Handles the OnReconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnConnectedEventArgs" /> instance containing the event data.</param>
        private async void _client_OnReconnected(object sender, OnConnectedEventArgs e)
        {
            await SendHandshake();

            foreach (var channel in _joinedChannelManager.GetJoinedChannels())
            {
                _joinChannelQueue.Enqueue(channel);
            }

            if(_joinChannelQueue != null && _joinChannelQueue.Count > 0)
            {
                await QueueingJoinCheckAsync();
            }
            
            _joinedChannelManager.Clear();
            OnReconnected?.Invoke(sender, new OnConnectedArgs());
        }

        static readonly string[] NewLineSeparator = new[] { "\r\n" }; // dont modify!!!

        /// <summary>
        /// Handles the OnMessage event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnMessageEventArgs" /> instance containing the event data.</param>
        private async void _client_OnMessage(object sender, OnMessageEventArgs e)
        {
            var lines = e.Message.Split(NewLineSeparator, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Length <= 1)
                    continue;

                _logger?.LogReceived(line);
                OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = SendReceiveDirection.Received, Data = line });
                await HandleIrcMessageAsync(IrcParser.ParseMessage(line));
            }
        }

        /// <summary>
        /// Clients the on connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void _client_OnConnectedAsync(object sender, object e)
        {
            await SendHandshake();

            if(_joinChannelQueue != null && _joinChannelQueue.Count > 0)
            {
                await QueueingJoinCheckAsync();
            }
        }

        /// <summary>
        /// Send the handshake for the connection.
        /// </summary>
        private async Task SendHandshake()
        {
            await _client.SendAsync(Rfc2812.Pass(ConnectionCredentials.TwitchOAuth));
            await _client.SendAsync(Rfc2812.Nick(ConnectionCredentials.TwitchUsername));
            await _client.SendAsync(Rfc2812.User(ConnectionCredentials.TwitchUsername, 0, ConnectionCredentials.TwitchUsername));

            if (ConnectionCredentials.Capabilities.Membership)
                await _client.SendAsync("CAP REQ twitch.tv/membership");
            if (ConnectionCredentials.Capabilities.Commands)
                await _client.SendAsync("CAP REQ twitch.tv/commands");
            if (ConnectionCredentials.Capabilities.Tags)
                await _client.SendAsync("CAP REQ twitch.tv/tags");
        }
        
        #endregion

        #region Joining Stuff

        /// <summary>
        /// Queueings the join check.
        /// </summary>
        private async Task QueueingJoinCheckAsync()
        {
            if (_joinChannelQueue.Count > 0)
            {
                _currentlyJoiningChannels = true;
                var channelToJoin = _joinChannelQueue.Dequeue();
                _logger?.LogJoiningChannel(channelToJoin.Channel);
                // important we set channel to lower case when sending join message
                await _client.SendAsync(Rfc2812.Join($"#{channelToJoin.Channel.ToLower()}"));
                _joinedChannelManager.AddJoinedChannel(new JoinedChannel(channelToJoin.Channel));
                StartJoinedChannelTimer(channelToJoin.Channel);
            }
            else
            {
                _logger?.LogChannelJoiningFinished();
            }
        }

        /// <summary>
        /// Starts the joined channel timer.
        /// </summary>
        /// <param name="channel">The channel.</param>
        private void StartJoinedChannelTimer(string channel)
        {
            if (_joinTimer == null)
            {
                _joinTimer = new System.Timers.Timer(1000);
                _joinTimer.Elapsed += JoinChannelTimeout;
                _awaitingJoins = new List<KeyValuePair<string, DateTime>>();
            }
            
            // channel is ToLower()'d because ROOMSTATE (which is the event the client uses to remove
            // this channel from _awaitingJoins list) contains the username as always lowercase. This means
            // if we don't ToLower(), the channel never gets removed, and FailureToReceiveJoinConfirmation
            // fires.
            _awaitingJoins.Add(new KeyValuePair<string, DateTime>(channel.ToLower(), DateTime.Now));
            if (!_joinTimer.Enabled)
                _joinTimer.Start();
        }

        /// <summary>
        /// Joins the channel timeout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void JoinChannelTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_awaitingJoins.Any())
            {
                List<KeyValuePair<string, DateTime>> expiredChannels = _awaitingJoins.Where(x => (DateTime.Now - x.Value).TotalSeconds > 5).ToList();
                if (expiredChannels.Any())
                {
                    _awaitingJoins.RemoveAll(x => (DateTime.Now - x.Value).TotalSeconds > 5);
                    foreach (KeyValuePair<string, DateTime> expiredChannel in expiredChannels)
                    {
                        _joinedChannelManager.RemoveJoinedChannel(expiredChannel.Key.ToLowerInvariant());
                        OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs { Exception = new FailureToReceiveJoinConfirmationException(expiredChannel.Key) });
                    }
                }
            }
            else
            {
                _joinTimer.Stop();
                _currentlyJoiningChannels = false;
                QueueingJoinCheckAsync().GetAwaiter().GetResult();
            }
        }

        #endregion

        #region IrcMessage Handling

        /// <summary>
        /// Handles the irc message.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandleIrcMessageAsync(IrcMessage ircMessage)
        {
            if (ircMessage.ToString().StartsWith(":tmi.twitch.tv NOTICE * :Login authentication failed"))
            {
                OnIncorrectLogin?.Invoke(this, new OnIncorrectLoginArgs { Exception = new ErrorLoggingInException(ircMessage.ToString(), TwitchUsername) });
                return;
            }
            
            switch (ircMessage.Command)
            {
                case IrcCommand.PrivMsg:
                    HandlePrivMsg(ircMessage);
                    return;
                case IrcCommand.Notice:
                    await HandleNoticeAsync(ircMessage);
                    break;
                case IrcCommand.Ping:
                    if (!DisableAutoPong)
                        SendRaw("PONG");
                    return;
                case IrcCommand.Pong:
                    return;
                case IrcCommand.Join:
                    HandleJoin(ircMessage);
                    break;
                case IrcCommand.Part:
                    HandlePart(ircMessage);
                    break;
                case IrcCommand.ClearChat:
                    HandleClearChat(ircMessage);
                    break;
                case IrcCommand.ClearMsg:
                    HandleClearMsg(ircMessage);
                    break;
                case IrcCommand.UserState:
                    HandleUserState(ircMessage);
                    break;
                case IrcCommand.GlobalUserState:
                    break;
                case IrcCommand.RPL_001:
                    break;
                case IrcCommand.RPL_002:
                    break;
                case IrcCommand.RPL_003:
                    break;
                case IrcCommand.RPL_004:
                    Handle004();
                    break;
                case IrcCommand.RPL_353:
                    Handle353(ircMessage);
                    break;
                case IrcCommand.RPL_366:
                    await Handle366Async();
                    break;
                case IrcCommand.RPL_372:
                    break;
                case IrcCommand.RPL_375:
                    break;
                case IrcCommand.RPL_376:
                    break;
                case IrcCommand.Whisper:
                    HandleWhisper(ircMessage);
                    break;
                case IrcCommand.RoomState:
                    HandleRoomState(ircMessage);
                    break;
                case IrcCommand.Reconnect:
                    Reconnect();
                    break;
                case IrcCommand.UserNotice:
                    HandleUserNotice(ircMessage);
                    break;
                case IrcCommand.Mode:
                    HandleMode(ircMessage);
                    break;
                case IrcCommand.Cap:
                    HandleCap(ircMessage);
                    break;
                case IrcCommand.Unknown:
                    // fall through
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = null, Location = "HandleIrcMessage", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }

        #region IrcCommand Handling

        /// <summary>
        /// Handles the priv MSG.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandlePrivMsg(IrcMessage ircMessage)
        {
            ChatMessage chatMessage = new ChatMessage(TwitchUsername, ircMessage, ref _channelEmotes, WillReplaceEmotes);
            foreach (JoinedChannel joinedChannel in JoinedChannels.Where(x => string.Equals(x.Channel, ircMessage.Channel, StringComparison.InvariantCultureIgnoreCase)))
                joinedChannel.HandleMessage(chatMessage);

            OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs { ChatMessage = chatMessage });

            if (ircMessage.Tags.TryGetValue(Tags.MsgId, out var msgId))
                if (msgId == MsgIds.UserIntro)
                    OnUserIntro?.Invoke(this, new OnUserIntroArgs { ChatMessage = chatMessage });

            if (_chatCommandIdentifiers != null && _chatCommandIdentifiers.Count != 0 && !string.IsNullOrEmpty(chatMessage.Message))
            {
                if (_chatCommandIdentifiers.Contains(chatMessage.Message[0]))
                {
                    ChatCommand chatCommand = new ChatCommand(chatMessage);
                    OnChatCommandReceived?.Invoke(this, new OnChatCommandReceivedArgs { Command = chatCommand });
                    return;
                }
            }
        }

        /// <summary>
        /// Handles the notice.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandleNoticeAsync(IrcMessage ircMessage)
        {
            if (ircMessage.Message.Contains("Improperly formatted auth"))
            {
                OnIncorrectLogin?.Invoke(this, new OnIncorrectLoginArgs { Exception = new ErrorLoggingInException(ircMessage.ToString(), TwitchUsername) });
                return;
            }

            bool success = ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId);
            if (!success)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                UnaccountedFor(ircMessage.ToString());
            }

            switch (msgId)
            {
                case MsgIds.ColorChanged:
                    OnChatColorChanged?.Invoke(this, new OnChatColorChangedArgs { Channel = ircMessage.Channel });
                    break;
                case MsgIds.ModeratorsReceived:
                    OnModeratorsReceived?.Invoke(this, new OnModeratorsReceivedArgs { Channel = ircMessage.Channel, Moderators = ircMessage.Message.Replace(" ", "").Split(':')[1].Split(',').ToList() });
                    break;
                case MsgIds.NoMods:
                    OnModeratorsReceived?.Invoke(this, new OnModeratorsReceivedArgs { Channel = ircMessage.Channel, Moderators = new List<string>() });
                    break;
                case MsgIds.NoPermission:
                    OnNoPermissionError?.Invoke(this, null);
                    break;
                case MsgIds.RaidErrorSelf:
                    OnSelfRaidError?.Invoke(this, null);
                    break;
                case MsgIds.RaidNoticeMature:
                    OnRaidedChannelIsMatureAudience?.Invoke(this, null);
                    break;
                case MsgIds.MsgBannedEmailAlias:
                    OnBannedEmailAlias?.Invoke(this, new OnBannedEmailAliasArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgChannelSuspended:
                    _awaitingJoins.RemoveAll(x => x.Key.ToLower() == ircMessage.Channel);
                    _joinedChannelManager.RemoveJoinedChannel(ircMessage.Channel);
                    await QueueingJoinCheckAsync();
                    OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs
                    {
                        Exception = new FailureToReceiveJoinConfirmationException(ircMessage.Channel, ircMessage.Message)
                    });
                    break;
                case MsgIds.MsgRequiresVerifiedPhoneNumber:
                    OnRequiresVerifiedPhoneNumber?.Invoke(this, new OnRequiresVerifiedPhoneNumberArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgVerifiedEmail:
                    OnRequiresVerifiedEmail?.Invoke(this, new OnRequiresVerifiedEmailArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.NoVIPs:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = new List<string>() });
                    break;
                case MsgIds.VIPsSuccess:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = ircMessage.Message.Replace(" ", "").Replace(".", "").Split(':')[1].Split(',').ToList() });
                    break;
                case MsgIds.MsgRateLimit:
                    OnRateLimit?.Invoke(this, new OnRateLimitArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgDuplicate:
                    OnDuplicate?.Invoke(this, new OnDuplicateArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgFollowersOnly:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSubsOnly:
                    OnSubsOnly?.Invoke(this, new OnSubsOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgEmoteOnly:
                    OnEmoteOnly?.Invoke(this, new OnEmoteOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSuspended:
                    OnSuspended?.Invoke(this, new OnSuspendedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgBanned:
                    OnBanned?.Invoke(this, new OnBannedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSlowMode:
                    OnSlowMode?.Invoke(this, new OnSlowModeArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgR9k:
                    OnR9kMode?.Invoke(this, new OnR9kModeArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;

                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }

        /// <summary>
        /// Handles the join.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleJoin(IrcMessage ircMessage)
        {
            OnUserJoined?.Invoke(this, new OnUserJoinedArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
        }

        /// <summary>
        /// Handles the part.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandlePart(IrcMessage ircMessage)
        {
            if (string.Equals(TwitchUsername, ircMessage.User, StringComparison.InvariantCultureIgnoreCase))
            {
                _joinedChannelManager.RemoveJoinedChannel(ircMessage.Channel);
                _hasSeenJoinedChannels.Remove(ircMessage.Channel);
                OnLeftChannel?.Invoke(this, new OnLeftChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
            }
            else
            {
                OnUserLeft?.Invoke(this, new OnUserLeftArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
            }
        }

        /// <summary>
        /// Handles the clear chat.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleClearChat(IrcMessage ircMessage)
        {
            if (string.IsNullOrWhiteSpace(ircMessage.Message))
            {
                OnChatCleared?.Invoke(this, new OnChatClearedArgs { Channel = ircMessage.Channel });
                return;
            }

            bool successBanDuration = ircMessage.Tags.TryGetValue(Tags.BanDuration, out _);
            if (successBanDuration)
            {
                UserTimeout userTimeout = new UserTimeout(ircMessage);
                OnUserTimedout?.Invoke(this, new OnUserTimedoutArgs { UserTimeout = userTimeout });
                return;
            }

            UserBan userBan = new UserBan(ircMessage);
            OnUserBanned?.Invoke(this, new OnUserBannedArgs { UserBan = userBan });
        }

        /// <summary>
        /// Handles the clear MSG.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleClearMsg(IrcMessage ircMessage)
        {
            OnMessageCleared?.Invoke(this, new OnMessageClearedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message, TargetMessageId = ircMessage.ToString().Split('=')[3].Split(';')[0], TmiSentTs = ircMessage.ToString().Split('=')[4].Split(' ')[0] });
        }

        /// <summary>
        /// Handles the state of the user.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleUserState(IrcMessage ircMessage)
        {
            var userState = new UserState(ircMessage);
            if (!_hasSeenJoinedChannels.Contains(userState.Channel.ToLowerInvariant()))
            {
                _hasSeenJoinedChannels.Add(userState.Channel.ToLowerInvariant());
                OnUserStateChanged?.Invoke(this, new OnUserStateChangedArgs { UserState = userState });
            }
            else
                OnMessageSent?.Invoke(this, new OnMessageSentArgs { SentMessage = new SentMessage(userState, _lastMessageSent) });
        }

        /// <summary>
        /// Handle004s this instance.
        /// </summary>
        private void Handle004()
        {
            OnConnected?.Invoke(this, new OnConnectedArgs { BotUsername = TwitchUsername });
        }

        /// <summary>
        /// Handle353s the specified irc message.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void Handle353(IrcMessage ircMessage)
        {
            OnExistingUsersDetected?.Invoke(this, new OnExistingUsersDetectedArgs { Channel = ircMessage.Channel, Users = ircMessage.Message.Split(' ').ToList() });
        }

        /// <summary>
        /// Handle366s this instance.
        /// </summary>
        private async Task Handle366Async()
        {
            _currentlyJoiningChannels = false;
            await QueueingJoinCheckAsync();
        }

        /// <summary>
        /// Handles the whisper.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleWhisper(IrcMessage ircMessage)
        {
            WhisperMessage whisperMessage = new WhisperMessage(ircMessage, TwitchUsername);
            PreviousWhisper = whisperMessage;
            OnWhisperReceived?.Invoke(this, new OnWhisperReceivedArgs { WhisperMessage = whisperMessage });

            if (_whisperCommandIdentifiers != null && _whisperCommandIdentifiers.Count != 0 && !string.IsNullOrEmpty(whisperMessage.Message)) 
            {
                if (_whisperCommandIdentifiers.Contains(whisperMessage.Message[0]))
                {
                    WhisperCommand whisperCommand = new WhisperCommand(whisperMessage);
                    OnWhisperCommandReceived?.Invoke(this, new OnWhisperCommandReceivedArgs { Command = whisperCommand });
                    return;
                }
            }
            
            OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "WhispergHandling", RawIRC = ircMessage.ToString() });
            UnaccountedFor(ircMessage.ToString());
        }

        /// <summary>
        /// Handles the state of the room.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleRoomState(IrcMessage ircMessage)
        {
            // If ROOMSTATE is sent because a mode (subonly/slow/emote/etc) is being toggled, it has two tags: room-id, and the specific mode being toggled
            // If ROOMSTATE is sent because of a join confirmation, all tags (ie greater than 2) are sent
            if (ircMessage.Tags.Count > 2)
            {
                KeyValuePair<string, DateTime> channel = _awaitingJoins.FirstOrDefault(x => x.Key == ircMessage.Channel);
                _awaitingJoins.Remove(channel);
                OnJoinedChannel?.Invoke(this, new OnJoinedChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
            }

            OnChannelStateChanged?.Invoke(this, new OnChannelStateChangedArgs { ChannelState = new ChannelState(ircMessage), Channel = ircMessage.Channel });
        }

        /// <summary>
        /// Handles the user notice.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleUserNotice(IrcMessage ircMessage)
        {
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
                    ReSubscriber resubscriber = new ReSubscriber(ircMessage);
                    OnReSubscriber?.Invoke(this, new OnReSubscriberArgs { ReSubscriber = resubscriber, Channel = ircMessage.Channel });
                    break;
                case MsgIds.SubGift:
                    GiftedSubscription giftedSubscription = new GiftedSubscription(ircMessage);
                    OnGiftedSubscription?.Invoke(this, new OnGiftedSubscriptionArgs { GiftedSubscription = giftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.CommunitySubscription:
                    CommunitySubscription communitySubscription = new CommunitySubscription(ircMessage);
                    OnCommunitySubscription?.Invoke(this, new OnCommunitySubscriptionArgs { GiftedSubscription = communitySubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.ContinuedGiftedSubscription:
                    ContinuedGiftedSubscription continuedGiftedSubscription = new ContinuedGiftedSubscription(ircMessage);
                    OnContinuedGiftedSubscription?.Invoke(this, new OnContinuedGiftedSubscriptionArgs { ContinuedGiftedSubscription = continuedGiftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.Subscription:
                    Subscriber subscriber = new Subscriber(ircMessage);
                    OnNewSubscriber?.Invoke(this, new OnNewSubscriberArgs { Subscriber = subscriber, Channel = ircMessage.Channel });
                    break;
                case MsgIds.PrimePaidUprade:
                    PrimePaidSubscriber primePaidSubscriber = new PrimePaidSubscriber(ircMessage);
                    OnPrimePaidSubscriber?.Invoke(this, new OnPrimePaidSubscriberArgs { PrimePaidSubscriber = primePaidSubscriber, Channel = ircMessage.Channel });
                    break;
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }

        /// <summary>
        /// Handles the mode.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private void HandleMode(IrcMessage ircMessage)
        {
            if (ircMessage.Message.StartsWith("+o"))
            {
                OnModeratorJoined?.Invoke(this, new OnModeratorJoinedArgs { Channel = ircMessage.Channel, Username = ircMessage.Message.Split(' ')[1] });
                return;
            }

            if (ircMessage.Message.StartsWith("-o"))
            {
                OnModeratorLeft?.Invoke(this, new OnModeratorLeftArgs { Channel = ircMessage.Channel, Username = ircMessage.Message.Split(' ')[1] });
            }
        }

        /// <summary>
        /// Handles the Cap
        /// </summary>
        /// <param name="ircMessage">The irc message</param>
        private void HandleCap(IrcMessage ircMessage)
        {
            // do nothing
        }

        #endregion

        #endregion

        private void UnaccountedFor(string ircString)
        {
            _logger?.LogUnaccountedFor(ircString);
        }

        /// <summary>
        /// Sends the queued item.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendQueuedItem(string message)
        {
            SendQueuedItemAsync(message).GetAwaiter().GetResult();
        }
        
        /// <inheritdoc />
        public async Task SendQueuedItemAsync(string message)
        {
            if (!IsInitialized) 
                HandleNotInitialized();
            
            await _client.SendAsync(message);
        }

        /// <summary>
        /// Handles the not initialized.
        /// </summary>
        /// <exception cref="TwitchLib.Client.Exceptions.ClientNotInitializedException">The twitch client has not been initialized and cannot be used. Please call Initialize();</exception>
        protected static void HandleNotInitialized()
        {
            throw new ClientNotInitializedException("The twitch client has not been initialized and cannot be used. Please call Initialize();");
        }

        /// <summary>
        /// Handles the not connected.
        /// </summary>
        /// <exception cref="TwitchLib.Client.Exceptions.ClientNotConnectedException">In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.</exception>
        protected static void HandleNotConnected()
        {
            throw new ClientNotConnectedException("In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.");
        }
    }
}
