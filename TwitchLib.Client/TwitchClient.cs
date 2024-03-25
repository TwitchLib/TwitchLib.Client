using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Parsing;
using TwitchLib.Client.Manager;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Throttling;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Client.Models.Extensions;

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
        private IClient? _client;

        private readonly ISendOptions _sendOptions;
        private ThrottlingService? _throttling;

        /// <summary>
        /// The join channel queue
        /// </summary>
        private readonly Queue<JoinedChannel> _joinChannelQueue = new Queue<JoinedChannel>();

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<TwitchClient>? _logger;

        private readonly ILoggerFactory? _loggerFactory;

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
        private System.Timers.Timer? _joinTimer;

        /// <summary>
        /// The awaiting joins
        /// </summary>
        private readonly List<KeyValuePair<string, DateTime>> _awaitingJoins = new();

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
        private string _lastMessageSent = string.Empty;
        #endregion

        #region Public Variables
        /// <inheritdoc/>
        public ICollection<char> ChatCommandIdentifiers { get; } = new HashSet<char>();

        /// <inheritdoc/>
        public ICollection<char> WhisperCommandIdentifiers { get; } = new HashSet<char>();

        /// <inheritdoc/>
#if NET
        [MemberNotNullWhen(true, nameof(_client))]
#endif
        public bool IsInitialized => _client != null;

        /// <inheritdoc/>
        public IReadOnlyList<JoinedChannel> JoinedChannels => _joinedChannelManager.GetJoinedChannels();

        /// <inheritdoc/>
        public string TwitchUsername => ConnectionCredentials?.TwitchUsername ?? string.Empty;

        /// <inheritdoc/>
        public WhisperMessage? PreviousWhisper { get; private set; }

        /// <inheritdoc/>
        public bool IsConnected => IsInitialized && _client.IsConnected == true;

        /// <inheritdoc/>
        /// <remarks>
        /// Twitch-handled emotes are automatically added to this collection (which also accounts for
        /// managing user emote permissions such as sub-only emotes). Third-party emotes will have to be manually
        /// added according to the availability rules defined by the third-party.
        /// </remarks>
        public MessageEmoteCollection ChannelEmotes { get; } = new MessageEmoteCollection();

        /// <inheritdoc/>
        public bool DisableAutoPong { get; set; } = false;

        /// <inheritdoc/>
        public bool WillReplaceEmotes { get; set; } = false;

        /// <summary>
        /// Adds to replaced Emotes their prefix. Defaults to empty string.
        /// </summary>
        public string ReplacedEmotesPrefix { get; set; } = "";

        /// <summary>
        /// Adds to replaced Emotes their suffix. Defaults to empty string.
        /// </summary>
        public string ReplacedEmotesSuffix { get; set; } = "";

        /// <inheritdoc/>
        public ConnectionCredentials? ConnectionCredentials { get; private set; }
        #endregion

        #region Events
        /// <inheritdoc/>
        public event AsyncEventHandler<OnAnnouncementArgs>? OnAnnouncement;

        /// <inheritdoc/>
        public event AsyncEventHandler<Events.OnConnectedEventArgs>? OnConnected;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnJoinedChannelArgs>? OnJoinedChannel;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnIncorrectLoginArgs>? OnIncorrectLogin;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnChannelStateChangedArgs>? OnChannelStateChanged;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserStateChangedArgs>? OnUserStateChanged;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnMessageReceivedArgs>? OnMessageReceived;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnWhisperReceivedArgs>? OnWhisperReceived;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnMessageSentArgs>? OnMessageSent;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnChatCommandReceivedArgs>? OnChatCommandReceived;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnWhisperCommandReceivedArgs>? OnWhisperCommandReceived;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserJoinedArgs>? OnUserJoined;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnMessageClearedArgs>? OnMessageCleared;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnNewSubscriberArgs>? OnNewSubscriber;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnReSubscriberArgs>? OnReSubscriber;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnPrimePaidSubscriberArgs>? OnPrimePaidSubscriber;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnExistingUsersDetectedArgs>? OnExistingUsersDetected;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserLeftArgs>? OnUserLeft;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnDisconnectedEventArgs>? OnDisconnected;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnConnectionErrorArgs>? OnConnectionError;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnChatClearedArgs>? OnChatCleared;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserTimedoutArgs>? OnUserTimedout;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnLeftChannelArgs>? OnLeftChannel;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserBannedArgs>? OnUserBanned;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnSendReceiveDataArgs>? OnSendReceiveData;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnRaidNotificationArgs>? OnRaidNotification;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnGiftedSubscriptionArgs>? OnGiftedSubscription;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnCommunitySubscriptionArgs>? OnCommunitySubscription;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnContinuedGiftedSubscriptionArgs>? OnContinuedGiftedSubscription;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnMessageThrottledArgs>? OnMessageThrottled;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnErrorEventArgs>? OnError;

        /// <inheritdoc/>
        public event AsyncEventHandler<Events.OnConnectedEventArgs>? OnReconnected;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnRequiresVerifiedEmail;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnRequiresVerifiedPhoneNumber;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnRateLimit;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnDuplicate;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnBannedEmailAlias;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnSelfRaidError;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnNoPermissionError;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnRaidedChannelIsMatureAudience;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnFailureToReceiveJoinConfirmationArgs>? OnFailureToReceiveJoinConfirmation;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnFollowersOnly;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnSubsOnly;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnEmoteOnly;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnSuspended;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnBanned;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnSlowMode;

        /// <inheritdoc/>
        public event AsyncEventHandler<NoticeEventArgs>? OnR9kMode;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUserIntroArgs>? OnUserIntro;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnUnaccountedForArgs>? OnUnaccountedFor;

        /// <inheritdoc/>
        public event AsyncEventHandler<OnAnonGiftPaidUpgradeArgs>? OnAnonGiftPaidUpgrade;
       
        /// <inheritdoc/>
        public event AsyncEventHandler<OnUnraidNotificationArgs>? OnUnraidNotification;
       
        /// <inheritdoc/>
        public event AsyncEventHandler<OnRitualArgs>? OnRitual;
        
        /// <inheritdoc/>
        public event AsyncEventHandler<OnBitsBadgeTierArgs>? OnBitsBadgeTier;
        
        /// <inheritdoc/>
        public event AsyncEventHandler<OnCommunityPayForwardArgs>? OnCommunityPayForward;
       
        /// <inheritdoc/>
        public event AsyncEventHandler<OnStandardPayForwardArgs>? OnStandardPayForward;
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
            IClient? client = null,
            ClientProtocol protocol = ClientProtocol.WebSocket,
            ISendOptions? sendOptions = null,
            ILoggerFactory? loggerFactory = null)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory?.CreateLogger<TwitchClient>();
            _client = client;
            _protocol = protocol;
            _sendOptions = sendOptions ?? new SendOptions();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// If <see cref="ChatCommandIdentifiers"/> or <see cref="WhisperCommandIdentifiers"/> dont have any command identifier the '!' is added.
        /// </remarks>
        public void Initialize(ConnectionCredentials credentials, string? channel = null)
        {
            var channels = new List<string>();
            if(channel is not null)
                channels.Add(channel);
            Initialize(credentials, channels);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// If <see cref="ChatCommandIdentifiers"/> or <see cref="WhisperCommandIdentifiers"/> dont have any command identifier the '!' is added.
        /// </remarks>
        public void Initialize(ConnectionCredentials credentials, List<string> channels)
        {
            channels = channels.ConvertAll(x => x.StartsWith("#") ? x.Substring(1) : x);
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
            _logger?.LogInitialized(Assembly.GetExecutingAssembly().GetName().Version!);
            ConnectionCredentials = credentials;

            if (ChatCommandIdentifiers.Count == 0)
                ChatCommandIdentifiers.Add('!');
            if (WhisperCommandIdentifiers.Count == 0)
                WhisperCommandIdentifiers.Add('!');

            for (var i = 0; i < channels.Count; i++)
            {
                if (string.IsNullOrEmpty(channels[i]))
                    continue;

                // Check to see if client is already in channel
                if (JoinedChannels.Any(x => x.Channel.Equals(channels[i], StringComparison.OrdinalIgnoreCase)))
                    return;

                _joinChannelQueue.Enqueue(new JoinedChannel(channels[i]));
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

        /// <inheritdoc />
        public async Task SendRawAsync(string message)
        {
            if (!IsInitialized)
                HandleNotInitialized();

            _logger?.LogWriting(message);
            await _client.SendAsync(message);
            await OnSendReceiveData.TryInvoke(this, new(SendReceiveDirection.Sent, message));
        }

        #region SendMessage

        private void SendTwitchMessage(JoinedChannel? channel, string? message, string? replyToId = null, bool dryRun = false)
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

            var twitchMessage = new OutboundChatMessage(channel.Channel, message);

            if (replyToId != null)
            {
                twitchMessage.ReplyToId = replyToId;
            }

            _lastMessageSent = message;
            _throttling!.Enqueue(twitchMessage);
        }

        /// <inheritdoc />
        public Task SendMessageAsync(JoinedChannel channel, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message,null, dryRun);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SendMessageAsync(string channel, string message, bool dryRun = false)
        {
            return SendMessageAsync(GetJoinedChannel(channel), message, dryRun);
        }

        /// <inheritdoc />
        public Task SendReplyAsync(JoinedChannel channel, string replyToId, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message, replyToId, dryRun);
            return Task.CompletedTask;
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
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        /// <exception cref="TwitchLib.Client.Exceptions.BadStateException">Must be connected to at least one channel.</exception>
        public JoinedChannel? GetJoinedChannel(string channel)
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
        private Task OnThrottled(object? sender, OnMessageThrottledArgs e)
        {
            return OnMessageThrottled.TryInvoke(sender, e);
        }
        
        private Task ThrottlerOnError(object? sender, OnErrorEventArgs e)
        {
            return OnError.TryInvoke(sender, e);
        }

        /// <summary>
        /// Handles the OnFatality event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnFatalErrorEventArgs" /> instance containing the event data.</param>
        private Task _client_OnFatality(object? sender, OnFatalErrorEventArgs e)
        {
            return OnConnectionError.TryInvoke(this, new(TwitchUsername, new(e.Reason)));
        }

        /// <summary>
        /// Handles the OnDisconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OnDisconnectedEventArgs" /> instance containing the event data.</param>
        private Task _client_OnDisconnected(object? sender, OnDisconnectedEventArgs e)
        {
            return OnDisconnected.TryInvoke(sender, e);
        }

        /// <summary>
        /// Handles the OnReconnected event of the _client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Communication.Events.OnConnectedEventArgs" /> instance containing the event data.</param>
        private async Task _client_OnReconnected(object? sender, Communication.Events.OnConnectedEventArgs e)
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
            await OnReconnected.TryInvoke(sender, new Events.OnConnectedEventArgs(TwitchUsername));
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
        private async Task _client_OnMessage(object? sender, OnMessageEventArgs e)
        {
            var lines = e.Message.Split(NewLineSeparator, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Length <= 1)
                    continue;

                _logger?.LogReceived(line);

                await OnSendReceiveData.TryInvoke(this, new(SendReceiveDirection.Received, line));
                IrcMessage ircMessage;
                try
                {
                    ircMessage = IrcParser.ParseMessage(line);
                }
                catch (Exception ex)
                {
                    _logger?.LogParsingError(line, ex);
                    OnError?.Invoke(this, new(ex));
                    continue;
                }
                await HandleIrcMessageAsync(ircMessage);
            }
        }

        /// <summary>
        /// Clients the on connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async Task _client_OnConnectedAsync(object? sender, EventArgs e)
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
            await _client!.SendAsync(Rfc2812.Pass(ConnectionCredentials!.TwitchOAuth));
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
                await _client!.SendAsync(Rfc2812.Join($"#{channelToJoin.Channel.ToLower()}"));
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
        private void JoinChannelTimeout(object? sender, System.Timers.ElapsedEventArgs e)
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
                        _ = OnFailureToReceiveJoinConfirmation?.TryInvoke(this, new(new(expiredChannel.Key)));
                    }
                }
            }
            else
            {
                _joinTimer!.Stop();
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
                return OnIncorrectLogin.TryInvoke(this, new(new(rawMessage, TwitchUsername)));
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

                IrcCommand.Unknown or _ => OnUnaccountedFor?.Invoke(this, new(TwitchUsername, null, "HandleIrcMessage", rawMessage)) ?? UnaccountedFor(rawMessage)
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

            await OnMessageReceived.TryInvoke(this, new(chatMessage));

            if (ircMessage.Tags.TryGetValue(Tags.MsgId, out var msgId)
                && msgId == MsgIds.UserIntro
                && OnUserIntro != null)
            {
                await OnUserIntro.Invoke(this, new(chatMessage));
            }

            if (OnChatCommandReceived is not null
                && !string.IsNullOrEmpty(chatMessage.Message)
                && ChatCommandIdentifiers.Contains(chatMessage.Message[0])
                && CommandInfo.TryParse(chatMessage.Message.AsSpan(), out var commandInfo)
                )
            {
                await OnChatCommandReceived.Invoke(this, new(chatMessage, commandInfo));
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
                return OnIncorrectLogin.TryInvoke(this, new(new(rawIrcMessage, TwitchUsername)));
            }

            var success = ircMessage.Tags.TryGetValue(Tags.MsgId, out string? msgId);
            //if (!success)
            //{
            //    return OnUnaccountedFor?.Invoke(this, new()
            //    {
            //        BotUsername = TwitchUsername,
            //        Channel = channel,
            //        Location = "NoticeHandling",
            //        RawIRC = rawIrcMessage
            //    }) ?? UnaccountedFor(rawIrcMessage);
            //}

            var result = msgId switch
            {
                MsgIds.NoPermission => OnNoPermissionError?.Invoke(this, new(channel, message)),
                MsgIds.RaidErrorSelf => OnSelfRaidError?.Invoke(this, new(channel, message)),
                MsgIds.RaidNoticeMature => OnRaidedChannelIsMatureAudience?.Invoke(this, new(channel, message)),
                MsgIds.MsgBannedEmailAlias => OnBannedEmailAlias?.Invoke(this, new(channel, message)),
                MsgIds.MsgChannelSuspended => HandleChannelSuspended(ircMessage),
                MsgIds.MsgRequiresVerifiedPhoneNumber => OnRequiresVerifiedPhoneNumber?.Invoke(this, new(channel, message)),
                MsgIds.MsgVerifiedEmail => OnRequiresVerifiedEmail?.Invoke(this, new(channel, message)),
                MsgIds.MsgRateLimit => OnRateLimit?.Invoke(this, new(channel, message)),
                MsgIds.MsgDuplicate => OnDuplicate?.Invoke(this, new(channel, message)),
                MsgIds.MsgFollowersOnly => OnFollowersOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgSubsOnly => OnSubsOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgEmoteOnly => OnEmoteOnly?.Invoke(this, new(channel, message)),
                MsgIds.MsgSuspended => OnSuspended?.Invoke(this, new(channel, message)),
                MsgIds.MsgBanned => OnBanned?.Invoke(this, new(channel, message)),
                MsgIds.MsgSlowMode => OnSlowMode?.Invoke(this, new(channel, message)),
                MsgIds.MsgR9k => OnR9kMode?.Invoke(this, new(channel, message)),
                _ => OnUnaccountedFor?.Invoke(this, new(TwitchUsername, channel, "NoticeHandling", rawIrcMessage)) ?? UnaccountedFor(rawIrcMessage)
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
            await OnFailureToReceiveJoinConfirmation.TryInvoke(this, new(
                new(ircMessage.Channel, ircMessage.Message)));
        }

        /// <summary>
        /// Handles the join.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleJoin(IrcMessage ircMessage)
        {
            if (string.Equals(TwitchUsername, ircMessage.User, StringComparison.InvariantCultureIgnoreCase))
            {
                var channel = _awaitingJoins.Find(x => x.Key == ircMessage.Channel);
                _awaitingJoins.Remove(channel);

                return OnJoinedChannel.TryInvoke(this, new(ircMessage.Channel, TwitchUsername));
            }
            else 
            {
                return OnUserJoined.TryInvoke(this, new(ircMessage.Channel, ircMessage.User));
            } 
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

                return OnLeftChannel.TryInvoke(this,new(ircMessage.Channel, TwitchUsername));
            }
            else
            {
                return OnUserLeft.TryInvoke(this, new(ircMessage.Channel, ircMessage.User));
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
                return OnChatCleared.TryInvoke(this, new(ircMessage.Channel));
            }

            var successBanDuration = ircMessage.Tags.ContainsKey(Tags.BanDuration);

            return successBanDuration
                ? OnUserTimedout.TryInvoke(this, new(new(ircMessage)))
                : OnUserBanned.TryInvoke(this, new(new(ircMessage)));
        }

        /// <summary>
        /// Handles the clear MSG.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleClearMsg(IrcMessage ircMessage)
        {
            var tmiSent = ircMessage.Tags.TryGetValue("tmi-sent-ts", out var tmiSentTs)
                ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(tmiSentTs))
                : default;
            return OnMessageCleared.TryInvoke(this, new(
                ircMessage.Channel,
                ircMessage.Message,
                ircMessage.Tags.GetValueOrDefault("target-msg-id", string.Empty),
                tmiSent
                ));
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
                return OnUserStateChanged.TryInvoke(this, new(userState));
            }

            return OnMessageSent.TryInvoke(this, new(new(userState, _lastMessageSent)));
        }

        /// <summary>
        /// Handle004s this instance.
        /// </summary>
        private Task Handle004()
        {
            return OnConnected.TryInvoke(this, new(TwitchUsername));
        }

        /// <summary>
        /// Handle353s the specified irc message.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task Handle353(IrcMessage ircMessage)
        {
            return OnExistingUsersDetected.TryInvoke(this, new(ircMessage.Channel, ircMessage.Message.Split(' ').ToList()));
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

            await OnWhisperReceived.TryInvoke(this, new(whisperMessage));

            if (OnWhisperCommandReceived is not null
                && !string.IsNullOrEmpty(whisperMessage.Message)
                && WhisperCommandIdentifiers.Contains(whisperMessage.Message[0])
                && CommandInfo.TryParse(whisperMessage.Message.AsSpan(), out var commandInfo)
                )
            {
                await OnWhisperCommandReceived.Invoke(this, new(whisperMessage, commandInfo));
            }
        }

        /// <summary>
        /// Handles the state of the room.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleRoomState(IrcMessage ircMessage)
        {
            return OnChannelStateChanged.TryInvoke(this, new(ircMessage.Channel, new ChannelState(ircMessage)));
        }

        /// <summary>
        /// Handles the user notice.
        /// </summary>
        /// <param name="ircMessage">The irc message.</param>
        private Task HandleUserNotice(IrcMessage ircMessage)
        {
            var rawMessage = ircMessage.ToString();

            ircMessage.Tags.TryGetValue(Tags.MsgId, out string? msgId);
            //if(!ircMessage.Tags.TryGetValue(Tags.MsgId, out string? msgId))
            //{
            //    return OnUnaccountedFor?.Invoke(this, new()
            //    {
            //        BotUsername = TwitchUsername,
            //        Channel = ircMessage.Channel,
            //        Location = "UserNoticeHandling",
            //        RawIRC = rawMessage
            //    }) ?? UnaccountedFor(rawMessage);
            //}

            return msgId switch
            {
                MsgIds.Announcement => OnAnnouncement.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.Raid => OnRaidNotification.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.ReSubscription => OnReSubscriber.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.SubGift => OnGiftedSubscription.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.CommunitySubscription => OnCommunitySubscription.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.ContinuedGiftedSubscription => OnContinuedGiftedSubscription.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.Subscription => OnNewSubscriber.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.PrimePaidUprade => OnPrimePaidSubscriber.TryInvoke(this, new(ircMessage.Channel, new(ircMessage))),
                MsgIds.AnonGiftPaidUpgrade => OnAnonGiftPaidUpgrade.TryInvoke(this, new(ircMessage)),
                MsgIds.Unraid => OnUnraidNotification.TryInvoke(this, new(ircMessage)),
                MsgIds.Ritual => OnRitual.TryInvoke(this, new(ircMessage)),
                MsgIds.BitsBadgeTier => OnBitsBadgeTier.TryInvoke(this, new(ircMessage)),
                MsgIds.CommunityPayForward => OnCommunityPayForward.TryInvoke(this, new(ircMessage)),
                MsgIds.StandardPayForward => OnStandardPayForward.TryInvoke(this, new(ircMessage)),
                _ => OnUnaccountedFor?.Invoke(this, new(TwitchUsername, ircMessage.Channel, "UserNoticeHandling", rawMessage)) ?? UnaccountedFor(rawMessage)
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
#if NETSTANDARD2_1 || NET
        [DoesNotReturn]
#endif
        protected static void HandleNotInitialized()
        {
            throw new ClientNotInitializedException("The twitch client has not been initialized and cannot be used. Please call Initialize();");
        }

        /// <summary>
        /// Handles the not connected.
        /// </summary>
        /// <exception cref="TwitchLib.Client.Exceptions.ClientNotConnectedException">In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.</exception>
#if NETSTANDARD2_1 || NET
        [DoesNotReturn]
#endif
        protected static void HandleNotConnected()
        {
            throw new ClientNotConnectedException("In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.");
        }
    }
}
