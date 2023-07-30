using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
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
        private readonly HashSet<string> _hasSeenJoinedChannels = new HashSet<string>();

        /// <summary>
        /// The last message sent
        /// </summary>
        private string _lastMessageSent;
        #endregion

        #region Public Variables
        /// <inheritdoc/>
        public ICollection<char> ChatCommandIdentifiers { get; } = new HashSet<char>();
        /// <inheritdoc/>
        public ICollection<char> WhisperCommandIdentifiers { get; } = new HashSet<char>();
        /// <summary>
        /// Assembly version of TwitchLib.Client.
        /// </summary>
        /// <value>The version.</value>
        public Version Version => Assembly.GetEntryAssembly()?.GetName().Version;

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
        public bool IsConnected => IsInitialized && _client?.IsConnected == true;

        /// <summary>
        /// The emotes this channel replaces.
        /// </summary>
        /// <value>The channel emotes.</value>
        /// <remarks>Twitch-handled emotes are automatically added to this collection (which also accounts for
        /// managing user emote permissions such as sub-only emotes). Third-party emotes will have to be manually
        /// added according to the availability rules defined by the third-party.</remarks>
        public MessageEmoteCollection ChannelEmotes { get; } = new MessageEmoteCollection();

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
        /// Adds to replaced Emotes their prefix. Defaults to empty string.
        /// </summary>
        public string ReplacedEmotesPrefix { get; set; } = "";

        /// <summary>
        /// Adds to replaced Emotes their suffix. Defaults to empty string.
        /// </summary>
        public string ReplacedEmotesSuffix { get; set; } = "";

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
        public event AsyncEventHandler<OnAnnouncementArgs> OnAnnouncement;

        /// <summary>
        /// Fires when VIPs are received from chat
        /// </summary>
        public event AsyncEventHandler<OnVIPsReceivedArgs> OnVIPsReceived;

        /// <summary>
        /// Fires when client connects to Twitch.
        /// </summary>
        public event AsyncEventHandler<Events.OnConnectedEventArgs> OnConnected;

        /// <summary>
        /// Fires when client joins a channel.
        /// </summary>
        public event AsyncEventHandler<OnJoinedChannelArgs> OnJoinedChannel;

        /// <summary>
        /// Fires on logging in with incorrect details, returns ErrorLoggingInException.
        /// </summary>
        public event AsyncEventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;

        /// <summary>
        /// Fires when connecting and channel state is changed, returns ChannelState.
        /// </summary>
        public event AsyncEventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;

        /// <summary>
        /// Fires when a user state is received, returns UserState.
        /// </summary>
        public event AsyncEventHandler<OnUserStateChangedArgs> OnUserStateChanged;

        /// <summary>
        /// Fires when a new chat message arrives, returns ChatMessage.
        /// </summary>
        public event AsyncEventHandler<OnMessageReceivedArgs> OnMessageReceived;

        /// <summary>
        /// Fires when a new whisper arrives, returns WhisperMessage.
        /// </summary>
        public event AsyncEventHandler<OnWhisperReceivedArgs> OnWhisperReceived;

        /// <summary>
        /// Fires when a chat message is sent, returns username, channel and message.
        /// </summary>
        public event AsyncEventHandler<OnMessageSentArgs> OnMessageSent;

        /// <summary>
        /// Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list.
        /// </summary>
        public event AsyncEventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

        /// <summary>
        /// Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
        /// </summary>
        public event AsyncEventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;

        /// <summary>
        /// Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
        /// </summary>
        public event AsyncEventHandler<OnUserJoinedArgs> OnUserJoined;

        /// <summary>
        /// Fires when a message gets deleted in chat.
        /// </summary>
        public event AsyncEventHandler<OnMessageClearedArgs> OnMessageCleared;

        /// <summary>
        /// Fires when new subscriber is announced in chat, returns Subscriber.
        /// </summary>
        public event AsyncEventHandler<OnNewSubscriberArgs> OnNewSubscriber;

        /// <summary>
        /// Fires when current subscriber renews subscription, returns ReSubscriber.
        /// </summary>
        public event AsyncEventHandler<OnReSubscriberArgs> OnReSubscriber;

        /// <summary>
        /// Fires when a current Prime gaming subscriber converts to a paid subscription.
        /// </summary>
        public event AsyncEventHandler<OnPrimePaidSubscriberArgs> OnPrimePaidSubscriber;

        /// <summary>
        /// Fires when Twitch notifies client of existing users in chat.
        /// </summary>
        public event AsyncEventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

        /// <summary>
        /// Fires when a PART message is received from Twitch regarding a particular viewer
        /// </summary>
        public event AsyncEventHandler<OnUserLeftArgs> OnUserLeft;

        /// <summary>
        /// Fires when bot has disconnected.
        /// </summary>
        public event AsyncEventHandler<OnDisconnectedEventArgs> OnDisconnected;

        /// <summary>
        /// Forces when bot suffers connection error.
        /// </summary>
        public event AsyncEventHandler<OnConnectionErrorArgs> OnConnectionError;

        /// <summary>
        /// Fires when a channel's chat is cleared.
        /// </summary>
        public event AsyncEventHandler<OnChatClearedArgs> OnChatCleared;

        /// <summary>
        /// Fires when a viewer gets timedout by any moderator.
        /// </summary>
        public event AsyncEventHandler<OnUserTimedoutArgs> OnUserTimedout;

        /// <summary>
        /// Fires when client successfully leaves a channel.
        /// </summary>
        public event AsyncEventHandler<OnLeftChannelArgs> OnLeftChannel;

        /// <summary>
        /// Fires when a viewer gets banned by any moderator.
        /// </summary>
        public event AsyncEventHandler<OnUserBannedArgs> OnUserBanned;

        /// <summary>
        /// Fires when a list of moderators is received.
        /// </summary>
        public event AsyncEventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;

        /// <summary>
        /// Fires when confirmation of a chat color change request was received.
        /// </summary>
        public event AsyncEventHandler<OnChatColorChangedArgs> OnChatColorChanged;

        /// <summary>
        /// Fires when data is either received or sent.
        /// </summary>
        public event AsyncEventHandler<OnSendReceiveDataArgs> OnSendReceiveData;

        /// <summary>
        /// Fires when a raid notification is detected in chat
        /// </summary>
        public event AsyncEventHandler<OnRaidNotificationArgs> OnRaidNotification;

        /// <summary>
        /// Fires when a subscription is gifted and announced in chat
        /// </summary>
        public event AsyncEventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

        /// <summary>
        /// Fires when a community subscription is announced in chat
        /// </summary>
        public event AsyncEventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;

        /// <summary>
        /// Fires when a gifted subscription is continued and announced in chat
        /// </summary>
        public event AsyncEventHandler<OnContinuedGiftedSubscriptionArgs> OnContinuedGiftedSubscription;

        /// <summary>
        /// Fires when a Message has been throttled.
        /// </summary>
        public event AsyncEventHandler<OnMessageThrottledArgs> OnMessageThrottled;

        /// <summary>
        /// Occurs when an Error is thrown in the protocol client
        /// </summary>
        public event AsyncEventHandler<OnErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when a reconnection occurs.
        /// </summary>
        public event AsyncEventHandler<Events.OnConnectedEventArgs> OnReconnected;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnRequiresVerifiedEmail;

        /// <summary>
        /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnRequiresVerifiedPhoneNumber;

        /// <summary>
        /// Occurs when send message rate limit has been applied to the client in a specific channel by Twitch
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnRateLimit;

        /// <summary>
        /// Occurs when sending duplicate messages and user is not permitted to do so
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnDuplicate;

        /// <summary>
        /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnBannedEmailAlias;

        /// <summary>
        /// Fires when TwitchClient attempts to host a channel it is in.
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnSelfRaidError;

        /// <summary>
        /// Fires when TwitchClient receives generic no permission error from Twitch.
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnNoPermissionError;

        /// <summary>
        /// Fires when newly raided channel is mature audience only.
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnRaidedChannelIsMatureAudience;

        /// <summary>
        /// Fires when the client was unable to join a channel.
        /// </summary>
        public event AsyncEventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in followers only mode, as a non-follower
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnFollowersOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel in subs only mode, as a non-sub
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnSubsOnly;

        /// <summary>
        /// Fires when the client attempts to send a non-emote message to a channel in emotes only mode
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnEmoteOnly;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel that has been suspended
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnSuspended;

        /// <summary>
        /// Fires when the client attempts to send a message to a channel they're banned in
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnBanned;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with slow mode enabled, without cooldown expiring
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnSlowMode;

        /// <summary>
        /// Fires when the client attempts to send a message in a channel with r9k mode enabled, and message was not permitted
        /// </summary>
        public event AsyncEventHandler<NoticeEventArgs> OnR9kMode;

        /// <summary>
        /// Fires when the client receives a PRIVMSG tagged as an user-intro
        /// </summary>
        public event AsyncEventHandler<OnUserIntroArgs> OnUserIntro;

        /// <summary>
        /// Fires when data is received from Twitch that is not able to be parsed.
        /// </summary>
        public event AsyncEventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
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
        /// <remarks>
        /// If <see cref="ChatCommandIdentifiers"/> or <see cref="WhisperCommandIdentifiers"/> dont have any command identifier the '!' is added.
        /// </remarks>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channel">The channel to connect to.</param>
        public void Initialize(ConnectionCredentials credentials, string channel = null)
        {
            if (channel?[0] == '#') channel = channel.Substring(1);
            InitializationHelper(credentials, new List<string>() { channel });
        }

        /// <summary>
        /// Initializes the TwitchChatClient class (with multiple channels).
        /// </summary>
        /// <remarks>
        /// If <see cref="ChatCommandIdentifiers"/> or <see cref="WhisperCommandIdentifiers"/> dont have any command identifier the '!' is added.
        /// </remarks>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channels">List of channels to join when connected</param>
        public void Initialize(ConnectionCredentials credentials, List<string> channels)
        {
            channels = channels.ConvertAll(x => x[0] == '#' ? x.Substring(1) : x);
            InitializationHelper(credentials, channels);
        }

        /// <summary>
        /// Runs initialization logic that is shared by the overriden Initialize methods.
        /// </summary>
        /// <remarks>
        /// If <see cref="ChatCommandIdentifiers"/> or <see cref="WhisperCommandIdentifiers"/> dont have any command identifier the '!' is added.
        /// </remarks>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="channels">List of channels to join when connected</param>
        private void InitializationHelper(
            ConnectionCredentials credentials,
            List<string> channels)
        {
            _logger?.LogInitialized(Assembly.GetExecutingAssembly().GetName().Version);
            ConnectionCredentials = credentials;
            TwitchUsername = ConnectionCredentials.TwitchUsername;

            if (ChatCommandIdentifiers.Count == 0)
                ChatCommandIdentifiers.Add('!');
            if (WhisperCommandIdentifiers.Count == 0)
                WhisperCommandIdentifiers.Add('!');

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
            _client ??= _protocol switch
            {
                ClientProtocol.TCP => new TcpClient(null, _loggerFactory?.CreateLogger<TcpClient>()),
                ClientProtocol.WebSocket => new WebSocketClient(null, _loggerFactory?.CreateLogger<WebSocketClient>()),
                _ => throw new ArgumentOutOfRangeException(nameof(_protocol), _protocol, null)
            };

            Debug.Assert(_client != null, nameof(_client) + " != null");

            _throttling = new ThrottlingService(_client, _sendOptions, _logger);
            _throttling.OnThrottled += OnThrottled;
            _throttling.OnError += ThrottlerOnError;

            _client.OnConnected += _client_OnConnectedAsync;
            _client.OnMessage += _client_OnMessage;
            _client.OnDisconnected += _client_OnDisconnected;
            _client.OnFatality += _client_OnFatality;
            _client.OnReconnected += _client_OnReconnected;
        }
        #endregion

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
            await OnSendReceiveData.TryInvoke(this, new()
            {
                Direction = SendReceiveDirection.Sent,
                Data = message
            });
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
            SendTwitchMessage(channel, message,null, dryRun);
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
        public Task SendReplyAsync(string channel, string replyToId, string message, bool dryRun = false)
        {
            return SendReplyAsync(GetJoinedChannel(channel), replyToId, message, dryRun);
        }

        #endregion

        #region Connection Calls

        /// <inheritdoc />
        public async Task<bool> ConnectAsync()
        {
            if (!IsInitialized)
                HandleNotInitialized();

            _logger?.LogConnecting();

            // Clear instance data
            _joinedChannelManager.Clear();

            if (await _client.OpenAsync())
            {
                _logger?.LogShouldBeConnected();
                return true;
            }

            return false;
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

        /// <inheritdoc />
        public async Task ReconnectAsync()
        {
            if (!IsInitialized)
                HandleNotInitialized();

            _logger?.LogReconnecting();
            await _client.ReconnectAsync();
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

        /// <inheritdoc />
        public Task JoinChannelAsync(string channel, bool overrideCheck = false)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            if (!IsConnected)
                HandleNotConnected();

            // Check to see if client is already in channel
            if (JoinedChannels.Any(x => !overrideCheck && x.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            if (channel[0] == '#')
                channel = channel.Substring(1);

            _joinChannelQueue.Enqueue(new JoinedChannel(channel));

            return !_currentlyJoiningChannels ? QueueingJoinCheckAsync() : Task.CompletedTask;
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
            {
                await _client.SendAsync(Rfc2812.Part($"#{channel}"));
                _joinedChannelManager.RemoveJoinedChannel(channel);
            }
        }

        /// <inheritdoc />
        public Task LeaveChannelAsync(JoinedChannel channel)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            return LeaveChannelAsync(channel.Channel);
        }

        #endregion

        /// <inheritdoc />
        public Task OnReadLineTestAsync(string rawIrc)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            return HandleIrcMessageAsync(IrcParser.ParseMessage(rawIrc));
        }

        #region Client Events
        private Task OnThrottled(object sender, OnMessageThrottledArgs e)
        {
            return OnMessageThrottled.TryInvoke(sender, e);
        }
        
        private Task ThrottlerOnError(object sender, OnErrorEventArgs e)
        {
            return OnError.TryInvoke(sender, e);
        }
        
        /// <summary>
        /// Handles the OnFatality event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnFatalErrorEventArgs" /> instance containing the event data.</param>
        private Task _client_OnFatality(object sender, OnFatalErrorEventArgs e)
        {
            return OnConnectionError.TryInvoke(this, new()
            {
                BotUsername = TwitchUsername,
                Error = new() { Message = e.Reason }
            });
        }

        /// <summary>
        /// Handles the OnDisconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnDisconnectedEventArgs" /> instance containing the event data.</param>
        private Task _client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            return OnDisconnected.TryInvoke(sender, e);
        }

        /// <summary>
        /// Handles the OnReconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Communication.Events.OnConnectedEventArgs" /> instance containing the event data.</param>
        private async Task _client_OnReconnected(object sender, Communication.Events.OnConnectedEventArgs e)
        {
            await SendHandshake();

            foreach (var channel in _joinedChannelManager.GetJoinedChannels())
            {
                _joinChannelQueue.Enqueue(channel);
            }

            if (_joinChannelQueue?.Count > 0)
            {
                await QueueingJoinCheckAsync();
            }

            _joinedChannelManager.Clear();
            await OnReconnected.TryInvoke(sender, new Events.OnConnectedEventArgs());
        }

        static readonly string[] NewLineSeparator = new[]
        {
            "\r\n"
        }; // dont modify!!!

        /// <summary>
        /// Handles the OnMessage event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnMessageEventArgs" /> instance containing the event data.</param>
        private async Task _client_OnMessage(object sender, OnMessageEventArgs e)
        {
            var lines = e.Message.Split(NewLineSeparator, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Length <= 1)
                    continue;

                _logger?.LogReceived(line);

                await OnSendReceiveData.TryInvoke(this, new() { Direction = SendReceiveDirection.Received, Data = line });
                await HandleIrcMessageAsync(IrcParser.ParseMessage(line));
            }
        }

        /// <summary>
        /// Clients the on connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async Task _client_OnConnectedAsync(object sender, object e)
        {
            await SendHandshake();

            if (_joinChannelQueue?.Count > 0)
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
                var expiredChannels = _awaitingJoins.Where(x => (DateTime.Now - x.Value).TotalSeconds > 5).ToList();
                if (expiredChannels.Any())
                {
                    _awaitingJoins.RemoveAll(x => (DateTime.Now - x.Value).TotalSeconds > 5);
                    foreach (var expiredChannel in expiredChannels)
                    {
                        _joinedChannelManager.RemoveJoinedChannel(expiredChannel.Key.ToLowerInvariant());
                        _ = OnFailureToReceiveJoinConfirmation?.TryInvoke(this, new()
                        {
                            Exception = new(expiredChannel.Key)
                        });
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
        private Task HandleIrcMessageAsync(IrcMessage ircMessage)
        {
            var rawMessage = ircMessage.ToString();
            if (rawMessage.StartsWith(":tmi.twitch.tv NOTICE * :Login authentication failed"))
            {
                return OnIncorrectLogin.TryInvoke(this, new() { Exception = new(rawMessage, TwitchUsername) });
            }

            return ircMessage.Command switch
            {
                IrcCommand.PrivMsg => HandlePrivMsg(ircMessage),
                IrcCommand.Join => HandleJoin(ircMessage),
                IrcCommand.Part => HandlePart(ircMessage),
                IrcCommand.Ping => !DisableAutoPong ? SendRawAsync("PONG") : Task.CompletedTask,
                IrcCommand.Notice => HandleNotice(ircMessage),
                IrcCommand.Whisper => HandleWhisper(ircMessage),
                IrcCommand.ClearChat => HandleClearChat(ircMessage),
                IrcCommand.ClearMsg => HandleClearMsg(ircMessage),
                IrcCommand.UserState => HandleUserState(ircMessage),
                IrcCommand.UserNotice => HandleUserNotice(ircMessage),
                IrcCommand.RoomState => HandleRoomState(ircMessage),
                IrcCommand.Reconnect => ReconnectAsync(),
                IrcCommand.Cap => HandleCap(ircMessage),
                IrcCommand.RPL_004 => Handle004(),
                IrcCommand.RPL_353 => Handle353(ircMessage),
                IrcCommand.RPL_366 => Handle366(),

                IrcCommand.Pong or
                IrcCommand.RPL_001 or
                IrcCommand.RPL_002 or
                IrcCommand.RPL_003 or
                IrcCommand.RPL_372 or
                IrcCommand.RPL_375 or
                IrcCommand.RPL_376 or
                IrcCommand.GlobalUserState => Task.CompletedTask,

                IrcCommand.Unknown or _ => OnUnaccountedFor?.Invoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = null,
                    Location = "HandleIrcMessage",
                    RawIRC = rawMessage
                }) ?? UnaccountedFor(rawMessage)
            };
        }

        #region IrcCommand Handling

        /// <summary>
        /// Handles the priv MSG.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandlePrivMsg(IrcMessage ircMessage)
        {
            var chatMessage = new ChatMessage(
                TwitchUsername,
                ircMessage,
                ChannelEmotes,
                WillReplaceEmotes,
                ReplacedEmotesPrefix,
                ReplacedEmotesSuffix);

            foreach (JoinedChannel joinedChannel in JoinedChannels
                .Where(x => x.Channel.Equals(ircMessage.Channel, StringComparison.InvariantCultureIgnoreCase)))
            {
                joinedChannel.HandleMessage(chatMessage);
            }

            await OnMessageReceived.TryInvoke(this, new() { ChatMessage = chatMessage });

            if (ircMessage.Tags.TryGetValue(Tags.MsgId, out var msgId)
                && msgId == MsgIds.UserIntro
                && OnUserIntro != null)
            {
                await OnUserIntro.Invoke(this, new() { ChatMessage = chatMessage });
            }

            if (ChatCommandIdentifiers.Count != 0
                && !string.IsNullOrEmpty(chatMessage.Message)
                && ChatCommandIdentifiers.Contains(chatMessage.Message[0])
                && OnChatCommandReceived != null)
            {
                await OnChatCommandReceived.Invoke(this, new() { Command = new ChatCommand(chatMessage) });
            }
        }

        /// <summary>
        /// Handles the notice.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleNotice(IrcMessage ircMessage)
        {
            var channel = ircMessage.Channel;
            var message = ircMessage.Message;
            var rawIrcMessage = ircMessage.ToString();

            // This check might be too fragile and catch false positives
            if (message.Contains("Improperly formatted auth"))
            {
                return OnIncorrectLogin.TryInvoke(this, new() { Exception = new(rawIrcMessage, TwitchUsername) });
            }

            var success = ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId);
            if (!success)
            {
                return OnUnaccountedFor?.Invoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = channel,
                    Location = "NoticeHandling",
                    RawIRC = rawIrcMessage
                }) ?? UnaccountedFor(rawIrcMessage);
            }

            var result = msgId switch
            {
                MsgIds.ColorChanged => OnChatColorChanged?.Invoke(this, new() { Channel = channel }),
                MsgIds.ModeratorsReceived => OnModeratorsReceived?.Invoke(this, new()
                {
                    Channel = channel,
                    Moderators = message.SplitFirst(':').Remainder.ToString().Replace(" ", "").Split(',')
                }),
                MsgIds.NoMods => OnModeratorsReceived?.Invoke(this, new() { Channel = channel, Moderators = Array.Empty<string>() }),
                MsgIds.NoPermission => OnNoPermissionError?.Invoke(this, new(channel, message)),
                MsgIds.RaidErrorSelf => OnSelfRaidError?.Invoke(this, new(channel, message)),
                MsgIds.RaidNoticeMature => OnRaidedChannelIsMatureAudience?.Invoke(this, new(channel, message)),
                MsgIds.MsgBannedEmailAlias => OnBannedEmailAlias?.Invoke(this, new(channel, message)),
                MsgIds.MsgChannelSuspended => HandleChannelSuspended(ircMessage),
                MsgIds.MsgRequiresVerifiedPhoneNumber => OnRequiresVerifiedPhoneNumber?.Invoke(this, new(channel, message)),
                MsgIds.MsgVerifiedEmail => OnRequiresVerifiedEmail?.Invoke(this, new(channel, message)),
                MsgIds.NoVIPs => OnVIPsReceived?.Invoke(this, new() { Channel = channel, VIPs = Array.Empty<string>() }),
                MsgIds.VIPsSuccess => OnVIPsReceived?.Invoke(this, new()
                {
                    Channel = channel,
                    // TODO: Make it less allocatey
                    VIPs = message.SplitFirst(':').Remainder.ToString().Replace(" ", "").Replace(".", "").Split(',')
                }),
                MsgIds.MsgRateLimit => OnRateLimit?.Invoke(this, new(channel, message)),
                MsgIds.MsgDuplicate => OnDuplicate?.Invoke(this, new(channel, message)),
                MsgIds.MsgFollowersOnly => OnFollowersOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgSubsOnly => OnSubsOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgEmoteOnly => OnEmoteOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgSuspended => OnSuspended?.Invoke(this, new(channel, message)),
                MsgIds.MsgBanned => OnBanned?.Invoke(this, new(channel, message)),
                MsgIds.MsgSlowMode => OnSlowMode?.Invoke(this, new(channel, message)),
                MsgIds.MsgR9k => OnR9kMode?.Invoke(this, new(channel, message)),
                _ => OnUnaccountedFor?.Invoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = channel,
                    Location = "NoticeHandling",
                    RawIRC = rawIrcMessage
                }) ?? UnaccountedFor(rawIrcMessage)
            };

            return result ?? Task.CompletedTask;
        }

        /// <summary>
        /// Handles the channel suspended message
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandleChannelSuspended(IrcMessage ircMessage)
        {
            _awaitingJoins.RemoveAll(x => x.Key.Equals(ircMessage.Channel, StringComparison.OrdinalIgnoreCase));
            _joinedChannelManager.RemoveJoinedChannel(ircMessage.Channel);

            await QueueingJoinCheckAsync();
            await OnFailureToReceiveJoinConfirmation.TryInvoke(this, new()
            {
                Exception = new(ircMessage.Channel, ircMessage.Message)
            });
        }

        /// <summary>
        /// Handles the join.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleJoin(IrcMessage ircMessage)
        {
            return OnUserJoined.TryInvoke(this,new()
            {
                Channel = ircMessage.Channel,
                Username = ircMessage.User
            });
        }

        /// <summary>
        /// Handles the part.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandlePart(IrcMessage ircMessage)
        {
            if (string.Equals(TwitchUsername, ircMessage.User, StringComparison.InvariantCultureIgnoreCase))
            {
                _joinedChannelManager.RemoveJoinedChannel(ircMessage.Channel);
                _hasSeenJoinedChannels.Remove(ircMessage.Channel);

                return OnLeftChannel.TryInvoke(this,new()
                {
                    BotUsername = TwitchUsername,
                    Channel = ircMessage.Channel
                });
            }
            else
            {
                return OnUserLeft.TryInvoke(this, new()
                {
                    Channel = ircMessage.Channel,
                    Username = ircMessage.User
                });
            }
        }

        /// <summary>
        /// Handles the clear chat.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleClearChat(IrcMessage ircMessage)
        {
            if (string.IsNullOrWhiteSpace(ircMessage.Message))
            {
                return OnChatCleared.TryInvoke(this, new() { Channel = ircMessage.Channel });
            }

            var successBanDuration = ircMessage.Tags.TryGetValue(Tags.BanDuration, out _);

            return successBanDuration
                ? OnUserTimedout.TryInvoke(this, new() { UserTimeout = new(ircMessage) })
                : OnUserBanned.TryInvoke(this, new() { UserBan = new(ircMessage) });
        }

        /// <summary>
        /// Handles the clear MSG.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleClearMsg(IrcMessage ircMessage)
        {
            var rawMessage = ircMessage.ToString();
            return OnMessageCleared.TryInvoke(this, new()
            {
                Channel = ircMessage.Channel,
                Message = ircMessage.Message,
                TargetMessageId = rawMessage.Split('=')[3].SplitFirst(';').Segment.ToString(),
                TmiSentTs = rawMessage.Split('=')[4].SplitFirst(' ').Segment.ToString()
            });
        }

        /// <summary>
        /// Handles the state of the user.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleUserState(IrcMessage ircMessage)
        {
            var userState = new UserState(ircMessage);
            var userChannel = userState.Channel.ToLowerInvariant();
            if (!_hasSeenJoinedChannels.Contains(userChannel))
            {
                _hasSeenJoinedChannels.Add(userState.Channel.ToLowerInvariant());
                return OnUserStateChanged.TryInvoke(this, new() { UserState = userState });
            }

            return OnMessageSent.TryInvoke(this, new() { SentMessage = new(userState, _lastMessageSent) });
        }

        /// <summary>
        /// Handle004s this instance.
        /// </summary>
        private Task Handle004()
        {
            return OnConnected.TryInvoke(this, new() { BotUsername = TwitchUsername });
        }

        /// <summary>
        /// Handle353s the specified irc message.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task Handle353(IrcMessage ircMessage)
        {
            return OnExistingUsersDetected.TryInvoke(this, new()
            {
                Channel = ircMessage.Channel,
                Users = ircMessage.Message.Split(' ').ToList()
            });
        }

        /// <summary>
        /// Handle366s this instance.
        /// </summary>
        private Task Handle366()
        {
            _currentlyJoiningChannels = false;
            return QueueingJoinCheckAsync();
        }

        /// <summary>
        /// Handles the whisper.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandleWhisper(IrcMessage ircMessage)
        {
            var whisperMessage = new WhisperMessage(ircMessage, TwitchUsername);
            PreviousWhisper = whisperMessage;

            await OnWhisperReceived.TryInvoke(this, new() { WhisperMessage = whisperMessage });

            if (WhisperCommandIdentifiers.Count != 0
                && !string.IsNullOrEmpty(whisperMessage.Message)
                && WhisperCommandIdentifiers.Contains(whisperMessage.Message[0]))
            {
                var whisperCommand = new WhisperCommand(whisperMessage);
                await OnWhisperCommandReceived.TryInvoke(this, new() { Command = whisperCommand });
                return;
            }

            var rawMessage = ircMessage.ToString();
            await (OnUnaccountedFor?.Invoke(this, new()
            {
                BotUsername = TwitchUsername,
                Channel = ircMessage.Channel,
                Location = "WhispergHandling",
                RawIRC = rawMessage
            }) ?? UnaccountedFor(rawMessage));
        }

        /// <summary>
        /// Handles the state of the room.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private async Task HandleRoomState(IrcMessage ircMessage)
        {
            // If ROOMSTATE is sent because a mode (subonly/slow/emote/etc) is being toggled, it has two tags: room-id, and the specific mode being toggled
            // If ROOMSTATE is sent because of a join confirmation, all tags (ie greater than 2) are sent
            if (ircMessage.Tags.Count > 2)
            {
                var channel = _awaitingJoins.Find(x => x.Key == ircMessage.Channel);
                _awaitingJoins.Remove(channel);

                await OnJoinedChannel.TryInvoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = ircMessage.Channel
                });
            }

            await OnChannelStateChanged.TryInvoke(this, new()
            {
                ChannelState = new ChannelState(ircMessage),
                Channel = ircMessage.Channel
            });
        }

        /// <summary>
        /// Handles the user notice.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleUserNotice(IrcMessage ircMessage)
        {
            var rawMessage = ircMessage.ToString();

            if (!ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId))
            {
                return OnUnaccountedFor?.Invoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = ircMessage.Channel,
                    Location = "UserNoticeHandling",
                    RawIRC = rawMessage
                }) ?? UnaccountedFor(rawMessage);
            }

            return msgId switch
            {
                MsgIds.Announcement => OnAnnouncement.TryInvoke(this, new()
                {
                    Announcement = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.Raid => OnRaidNotification.TryInvoke(this, new()
                {
                    RaidNotification = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.ReSubscription => OnReSubscriber.TryInvoke(this, new()
                {
                    ReSubscriber = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.SubGift => OnGiftedSubscription.TryInvoke(this, new()
                {
                    GiftedSubscription = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.CommunitySubscription => OnCommunitySubscription.TryInvoke(this, new()
                {
                    GiftedSubscription = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.ContinuedGiftedSubscription => OnContinuedGiftedSubscription.TryInvoke(this, new()
                {
                    ContinuedGiftedSubscription = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.Subscription => OnNewSubscriber.TryInvoke(this, new()
                {
                    Subscriber = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                MsgIds.PrimePaidUprade => OnPrimePaidSubscriber.TryInvoke(this, new()
                {
                    PrimePaidSubscriber = new(ircMessage),
                    Channel = ircMessage.Channel
                }),
                _ => OnUnaccountedFor?.Invoke(this, new()
                {
                    BotUsername = TwitchUsername,
                    Channel = ircMessage.Channel,
                    Location = "UserNoticeHandling",
                    RawIRC = rawMessage
                }) ?? UnaccountedFor(rawMessage)
            };
        }

        /// <summary>
        /// Handles the Cap
        /// </summary>
        private static Task HandleCap(IrcMessage _)
        {
            // do nothing, actually cap frfr
            return Task.CompletedTask;
        }

        #endregion

        #endregion

        private Task UnaccountedFor(string ircString)
        {
            _logger?.LogUnaccountedFor(ircString);
            return Task.CompletedTask;
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
        public Task SendQueuedItemAsync(string message)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            return _client.SendAsync(message);
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
