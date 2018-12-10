using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TwitchLib.Client.Internal;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal.Parsing;
using TwitchLib.Client.Manager;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Communication;
using TwitchLib.Communication.Events;
using TwitchLib.Client.Enums;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Clients;

namespace TwitchLib.Client
{
    /// <summary>Represents a client connected to a Twitch channel.</summary>
    public class TwitchClient : ITwitchClient 
    {
        #region Private Variables
        private IClient _client;
        private MessageEmoteCollection _channelEmotes = new MessageEmoteCollection();
        private readonly ICollection<char> _chatCommandIdentifiers = new HashSet<char>();
        private readonly ICollection<char> _whisperCommandIdentifiers = new HashSet<char>();
        private readonly Queue<JoinedChannel> _joinChannelQueue = new Queue<JoinedChannel>();
        private readonly ILogger<TwitchClient> _logger;
        private readonly ClientProtocol _protocol;
        private string _autoJoinChannel;
        private bool _currentlyJoiningChannels;
        private System.Timers.Timer _joinTimer;
        private List<KeyValuePair<string, DateTime>> _awaitingJoins;

        private readonly IrcParser _ircParser;
        private readonly JoinedChannelManager _joinedChannelManager;

        // variables used for constructing OnMessageSent properties
        private readonly List<string> _hasSeenJoinedChannels = new List<string>();
        private string _lastMessageSent;
        #endregion

        #region Public Variables
        /// <summary>Assembly version of TwitchLib.Client.</summary>
        public Version Version => Assembly.GetEntryAssembly().GetName().Version;
        /// <summary>Checks if underlying client has been initialized.</summary>
        public bool IsInitialized => _client != null;
        /// <summary>A list of all channels the client is currently in.</summary>
        public IReadOnlyList<JoinedChannel> JoinedChannels => _joinedChannelManager.GetJoinedChannels();
        /// <summary>Username of the user connected via this library.</summary>
        public string TwitchUsername { get; private set; }
        /// <summary>The most recent whisper received.</summary>
        public WhisperMessage PreviousWhisper { get; private set; }
        /// <summary>The current connection status of the client.</summary>
        public bool IsConnected => IsInitialized && _client != null ? _client.IsConnected : false;
       
        /// <summary>The emotes this channel replaces.</summary>
        /// <remarks>
        ///     Twitch-handled emotes are automatically added to this collection (which also accounts for
        ///     managing user emote permissions such as sub-only emotes). Third-party emotes will have to be manually
        ///     added according to the availability rules defined by the third-party.
        /// </remarks>
        public MessageEmoteCollection ChannelEmotes => _channelEmotes;

        /// <summary>Will disable the client from sending automatic PONG responses to PING</summary>
        public bool DisableAutoPong { get; set; } = false;
        /// <summary>Determines whether Emotes will be replaced in messages.</summary>
        public bool WillReplaceEmotes { get; set; } = false;
        /// <summary>If set to true, the library will not check upon channel join that if BeingHosted event is subscribed, that the bot is connected as broadcaster. Only override if the broadcaster is joining multiple channels, including the broadcaster's.</summary>
        public bool OverrideBeingHostedCheck { get; set; } = false;
        /// <summary>Provides access to connection credentials object.</summary>
        public ConnectionCredentials ConnectionCredentials { get; private set; }
        /// <summary>Provides access to autorelistiononexception on off boolean.</summary>
        public bool AutoReListenOnException { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Fires when VIPs are received from chat
        /// </summary>
        public event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;

        /// <summary>
        /// Fires whenever a log write happens.
        /// </summary>
        public event EventHandler<OnLogArgs> OnLog;

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
        /// Fires when a whisper message is sent, returns username and message.
        /// </summary>
        public event EventHandler<OnWhisperSentArgs> OnWhisperSent;

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
        /// Fires when a hosted streamer goes offline and hosting is killed.
        /// </summary>
        public event EventHandler OnHostLeft;

        /// <summary>
        /// Fires when Twitch notifies client of existing users in chat.
        /// </summary>
        public event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

        /// <summary>
        /// Fires when a PART message is received from Twitch regarding a particular viewer
        /// </summary>
        public event EventHandler<OnUserLeftArgs> OnUserLeft;

        /// <summary>
        /// Fires when the joined channel begins hosting another channel.
        /// </summary>
        public event EventHandler<OnHostingStartedArgs> OnHostingStarted;

        /// <summary>
        /// Fires when the joined channel quits hosting another channel.
        /// </summary>
        public event EventHandler<OnHostingStoppedArgs> OnHostingStopped;

        /// <summary>
        /// Fires when bot has disconnected.
        /// </summary>
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;

        /// <summary>
        /// Forces when bot suffers conneciton error.
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
        /// Fires when client receives notice that a joined channel is hosting another channel.
        /// </summary>
        public event EventHandler<OnNowHostingArgs> OnNowHosting;

        /// <summary>
        /// Fires when the library detects another channel has started hosting the broadcaster's stream. MUST BE CONNECTED AS BROADCASTER.
        /// </summary>
        public event EventHandler<OnBeingHostedArgs> OnBeingHosted;

        /// <summary>
        /// Fires when a raid notification is detected in chat
        /// </summary>
        public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

        /// <summary>
        /// Fires when a subscription is gifted  and anonymously in chat
        /// </summary>
        public event EventHandler<OnAnonGiftedSubscriptionArgs> OnAnonGiftedSubscription;

        /// <summary>
        /// Fires when a subscription is gifted and announced in chat
        /// </summary>
        public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

        /// <summary>
        /// Fires when a community subscription is announced in chat
        /// </summary>
        public event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;

        /// <summary>
        /// Fires when a Message has been throttled.
        /// </summary>
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;

        /// <summary>
        /// Fires when a Whisper has been throttled.
        /// </summary>
        public event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;

        /// <summary>
        /// Occurs when an Error is thrown in the protocol client
        /// </summary>
        public event EventHandler<OnErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when a reconnection occurs.
        /// </summary>
        public event EventHandler<OnReconnectedEventArgs> OnReconnected;

        /// <summary>Fires when TwitchClient attempts to host a channel it is in.</summary>
        public EventHandler OnSelfRaidError;

        /// <summary>Fires when TwitchClient receives generic no permission error from Twitch.</summary>
        public EventHandler OnNoPermissionError;

        /// <summary>Fires when newly raided channel is mature audience only.</summary>
        public EventHandler OnRaidedChannelIsMatureAudience;

        /// <summary>Fires when a ritual for a new chatter is received.</summary>
        public EventHandler<OnRitualNewChatterArgs> OnRitualNewChatter;

        /// <summary>Fires when the client was unable to join a channel.</summary>
        public EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

        /// <summary>Fires when data is received from Twitch that is not able to be parsed.</summary>
        public EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
        #endregion

        #region Construction Work

        /// <summary>
        /// Initializes the TwitchChatClient class.
        /// </summary>
        /// <param name="client">Protocol Client to use for connection from TwitchLib.Communication. Possible Options Are the TcpClient client or WebSocket client.</param>
        /// <param name="logger">Optional ILogger instance to enable logging</param>
        public TwitchClient(IClient client = null, ClientProtocol protocol = ClientProtocol.WebSocket, ILogger < TwitchClient> logger = null)
        {
            _logger = logger;
            _client = client;
            _protocol = protocol;
            _joinedChannelManager = new JoinedChannelManager();
            _ircParser = new IrcParser();
        }

        /// <summary>
        /// Initializes the TwitchChatClient class.
        /// </summary>
        /// <param name="channel">The channel to connect to.</param>
        /// <param name="credentials">The credentials to use to log in.</param>
        /// <param name="chatCommandIdentifier">The identifier to be used for reading and writing commands from chat.</param>
        /// <param name="whisperCommandIdentifier">The identifier to be used for reading and writing commands from whispers.</param>
        /// <param name="autoReListenOnExceptions">By default, TwitchClient will silence exceptions and auto-relisten for overall stability. For debugging, you may wish to have the exception bubble up, set this to false.</param>
        public void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!', bool autoReListenOnExceptions = true)
        {
            Log($"TwitchLib-TwitchClient initialized, assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");
            ConnectionCredentials = credentials;
            TwitchUsername = ConnectionCredentials.TwitchUsername;
            _autoJoinChannel = channel?.ToLower();
            if (chatCommandIdentifier != '\0')
                _chatCommandIdentifiers.Add(chatCommandIdentifier);
            if (whisperCommandIdentifier != '\0')
                _whisperCommandIdentifiers.Add(whisperCommandIdentifier);

            AutoReListenOnException = autoReListenOnExceptions;

            InitializeClient();
        }

        private void InitializeClient()
        {
            if (_client == null)
            {
                switch (_protocol)
                {
                    case ClientProtocol.TCP:
                        _client = new TcpClient();
                        break;
                    case ClientProtocol.WebSocket:
                        _client = new WebSocketClient();
                        break;
                }
            }

            Debug.Assert(_client != null, nameof(_client) + " != null");

            _client.OnConnected += _client_OnConnected;
            _client.OnMessage += _client_OnMessage;
            _client.OnDisconnected += _client_OnDisconnected;
            _client.OnFatality += _client_OnFatality;
            _client.OnMessageThrottled += _client_OnMessageThrottled;
            _client.OnWhisperThrottled += _client_OnWhisperThrottled;
            _client.OnReconnected += _client_OnReconnected;
        }

        #endregion

        internal void RaiseEvent(string eventName, object args = null)
        {
            var fInfo = GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic) as FieldInfo;
            var multi = fInfo.GetValue(this) as MulticastDelegate;
            foreach (Delegate del in multi.GetInvocationList())
            {
                del.Method.Invoke(del.Target, args == null ? new object[] {this, new EventArgs()} : new[] {this, args});
            }
        }

        /// <summary>
        /// Sends a RAW IRC message.
        /// </summary>
        /// <param name="message">The RAW message to be sent.</param>
        public void SendRaw(string message)
        {
            if (!IsInitialized) HandleNotInitialized();

            Log($"Writing: {message}");
            _client.Send(message);
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }

        #region SendMessage

        /// <summary>
        /// Sends a formatted Twitch channel chat message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="dryRun">If set to true, the message will not actually be sent for testing purposes.</param>
        /// <param name="channel">Channel to send message to.</param>
        public void SendMessage(JoinedChannel channel, string message, bool dryRun = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (channel == null || message == null || dryRun) return;
            if (message.Length > 500)
            {
                LogError("Message length has exceeded the maximum character count. (500)");
                return;
            }

            var twitchMessage = new OutboundChatMessage
            {
                Channel = channel.Channel,
                Username = ConnectionCredentials.TwitchUsername,
                Message = message
            };

            _lastMessageSent = message;


            _client.Send(twitchMessage.ToString());
        }

        /// <summary>
        /// SendMessage wrapper that accepts channel in string form.
        /// </summary>
        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            SendMessage(GetJoinedChannel(channel), message, dryRun);
        }

        #endregion

        #region Whispers
        /// <summary>
        /// Sends a formatted whisper message to someone.
        /// </summary>
        /// <param name="receiver">The receiver of the whisper.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="dryRun">If set to true, the message will not actually be sent for testing purposes.</param>
        public void SendWhisper(string receiver, string message, bool dryRun = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (dryRun) return;

            var twitchMessage = new OutboundWhisperMessage
            {
                Receiver = receiver,
                Username = ConnectionCredentials.TwitchUsername,
                Message = message
            };
            
            _client.SendWhisper(twitchMessage.ToString());

            OnWhisperSent?.Invoke(this, new OnWhisperSentArgs { Receiver = receiver, Message = message });
        }

        #endregion

        #region Connection Calls
        /// <summary>
        /// Start connecting to the Twitch IRC chat.
        /// </summary>
        public void Connect()
        {
            if (!IsInitialized) HandleNotInitialized();
            Log($"Connecting to: {ConnectionCredentials.TwitchWebsocketURI}");

            _client.Open();

            Log("Should be connected!");
        }

        /// <summary>
        /// Start disconnecting from the Twitch IRC chat.
        /// </summary>
        public void Disconnect()
        {
            Log("Disconnect Twitch Chat Client...");

            if (!IsInitialized) HandleNotInitialized();
            _client.Close();

            // Clear instance data
            _joinedChannelManager.Clear();
            PreviousWhisper = null;
        }

        /// <summary>
        /// Start reconnecting to the Twitch IRC chat.
        /// </summary>
        public void Reconnect()
        {
            if (!IsInitialized) HandleNotInitialized();
            Log($"Reconnecting to Twitch");
            _joinedChannelManager.Clear();
            _client.Reconnect();
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
        /// Join the Twitch IRC chat of <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The channel to join.</param>
        /// <param name="overrideCheck">Override a join check.</param>
        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (!IsConnected) HandleNotConnected();
            // Check to see if client is already in channel
            if (JoinedChannels.FirstOrDefault(x => x.Channel.ToLower() == channel && !overrideCheck) != null)
                return;
            _joinChannelQueue.Enqueue(new JoinedChannel(channel));
            if (!_currentlyJoiningChannels)
                QueueingJoinCheck();
        }

        public void JoinRoom(string channelId, string roomId, bool overrideCheck = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            // Check to see if client is already in channel
            if (JoinedChannels.FirstOrDefault(x => x.Channel.ToLower() == $"chatrooms:{channelId}:{roomId}" && !overrideCheck) != null)
                return;
            _joinChannelQueue.Enqueue(new JoinedChannel($"chatrooms:{channelId}:{roomId}"));
            if (!_currentlyJoiningChannels)
                QueueingJoinCheck();
        }
        /// <summary>
        /// Returns a JoinedChannel object using a passed string/>.
        /// </summary>
        /// <param name="channel">String channel to search for.</param>
        public JoinedChannel GetJoinedChannel(string channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (JoinedChannels.Count == 0)
                throw new BadStateException("Must be connected to at least one channel.");
            return _joinedChannelManager.GetJoinedChannel(channel);
        }

        /// <summary>
        /// Leaves (PART) the Twitch IRC chat of <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The channel to leave.</param>
        /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
        public void LeaveChannel(string channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            // Channel MUST be lower case
            channel = channel.ToLower();
            Log($"Leaving channel: {channel}");
            var joinedChannel = _joinedChannelManager.GetJoinedChannel(channel);
            if (joinedChannel != null)
                _client.Send(Rfc2812.Part($"#{channel}"));
        }

        public void LeaveRoom(string channelId, string roomId)
        {
            if (!IsInitialized) HandleNotInitialized();
            var room = $"chatrooms:{channelId}:{roomId}";
            Log($"Leaving channel: {room}");
            var joinedChannel = _joinedChannelManager.GetJoinedChannel(room);
            if (joinedChannel != null)
                _client.Send(Rfc2812.Part($"#{room}"));
        }

        /// <summary>
        /// Leaves (PART) the Twitch IRC chat of <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The JoinedChannel object to leave.</param>
        /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
        public void LeaveChannel(JoinedChannel channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            LeaveChannel(channel.Channel);
        }

        #endregion

        /// <summary>
        /// This method allows firing the message parser with a custom irc string allowing for easy testing
        /// </summary>
        /// <param name="rawIrc">This should be a raw IRC message resembling one received from Twitch IRC.</param>
        public void OnReadLineTest(string rawIrc)
        {
            if (!IsInitialized) HandleNotInitialized();
            HandleIrcMessage(_ircParser.ParseIrcMessage(rawIrc));
        }

        #region Client Events

        private void _client_OnWhisperThrottled(object sender, OnWhisperThrottledEventArgs e)
        {
            OnWhisperThrottled?.Invoke(sender, e);
        }

        private void _client_OnMessageThrottled(object sender, OnMessageThrottledEventArgs e)
        {
            OnMessageThrottled?.Invoke(sender, e);
        }

        private void _client_OnFatality(object sender, OnFatalErrorEventArgs e)
        {
            OnConnectionError?.Invoke(this, new OnConnectionErrorArgs { BotUsername = TwitchUsername, Error = new ErrorEvent { Message = e.Reason } });
        }

        private void _client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            OnDisconnected?.Invoke(sender, e);
            _joinedChannelManager.Clear();
        }

        private void _client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            OnReconnected?.Invoke(sender, e);
        }

        private void _client_OnMessage(object sender, OnMessageEventArgs e)
        {
            var stringSeparators = new[] { "\r\n" };
            var lines = e.Message.Split(stringSeparators, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Length <= 1)
                    continue;

                Log($"Received: {line}");
                OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Received, Data = line });
                HandleIrcMessage(_ircParser.ParseIrcMessage(line));
            }
        }

        private void _client_OnConnected(object sender, object e)
        {
            _client.Send(Rfc2812.Pass(ConnectionCredentials.TwitchOAuth));
            _client.Send(Rfc2812.Nick(ConnectionCredentials.TwitchUsername));
            _client.Send(Rfc2812.User(ConnectionCredentials.TwitchUsername, 0, ConnectionCredentials.TwitchUsername));

            _client.Send("CAP REQ twitch.tv/membership");
            _client.Send("CAP REQ twitch.tv/commands");
            _client.Send("CAP REQ twitch.tv/tags");

            if (_autoJoinChannel != null)
            {
                JoinChannel(_autoJoinChannel);
            }
        }

        #endregion

        #region Joining Stuff

        private void QueueingJoinCheck()
        {
            if (_joinChannelQueue.Count > 0)
            {
                _currentlyJoiningChannels = true;
                var channelToJoin = _joinChannelQueue.Dequeue();
                Log($"Joining channel: {channelToJoin.Channel}");
                // important we set channel to lower case when sending join message
                _client.Send(Rfc2812.Join($"#{channelToJoin.Channel.ToLower()}"));
                _joinedChannelManager.AddJoinedChannel(new JoinedChannel(channelToJoin.Channel));
                StartJoinedChannelTimer(channelToJoin.Channel);
            }
            else
            {
                Log("Finished channel joining queue.");
            }
        }

        private void StartJoinedChannelTimer(string channel)
        {
            if (_joinTimer == null)
            {
                _joinTimer = new System.Timers.Timer(1000);
                _joinTimer.Elapsed += JoinChannelTimeout;
                _awaitingJoins = new List<KeyValuePair<string, DateTime>>();
            }
            _awaitingJoins.Add(new KeyValuePair<string, DateTime>(channel, DateTime.Now));
            if (!_joinTimer.Enabled)
                _joinTimer.Start();
        }

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
                        OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs { Exception = new FailureToReceiveJoinConfirmationException(expiredChannel.Key) });
                    }
                }
            }
            else
            {
                _joinTimer.Stop();
                _currentlyJoiningChannels = false;
                QueueingJoinCheck();
            }
        }

        #endregion

        #region IrcMessage Handling

        private void HandleIrcMessage(IrcMessage ircMessage)
        {
            if (ircMessage.Message.Contains("Login authentication failed"))
            {
                OnIncorrectLogin?.Invoke(this, new OnIncorrectLoginArgs { Exception = new ErrorLoggingInException(ircMessage.ToString(), TwitchUsername) });
            }

            switch (ircMessage.Command)
            {
                case IrcCommand.PrivMsg:
                    HandlePrivMsg(ircMessage);
                    return;
                case IrcCommand.Notice:
                    HandleNotice(ircMessage);
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
                case IrcCommand.HostTarget:
                    HandleHostTarget(ircMessage);
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
                    Handle366();
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
                case IrcCommand.Unknown:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = null, Location = "HandleIrcMessage", RawIRC = ircMessage.ToString() });
                    Log($"Unaccounted for: {ircMessage.ToString()}");
                    break;
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = null, Location = "HandleIrcMessage", RawIRC = ircMessage.ToString() });
                    Log($"Unaccounted for: {ircMessage.ToString()}");
                    break;
            }
        }

        #region IrcCommand Handling

        private void HandlePrivMsg(IrcMessage ircMessage)
        {
            if (ircMessage.Hostmask.Equals("jtv!jtv@jtv.tmi.twitch.tv"))
            {
                var hostNotification = new BeingHostedNotification(TwitchUsername, ircMessage);
                OnBeingHosted?.Invoke(this, new OnBeingHostedArgs { BeingHostedNotification = hostNotification });
                return;
            }

            var chatMessage = new ChatMessage(TwitchUsername, ircMessage, ref _channelEmotes, WillReplaceEmotes);
            foreach (var joinedChannel in JoinedChannels.Where(x => string.Equals(x.Channel, ircMessage.Channel, StringComparison.InvariantCultureIgnoreCase)))
                joinedChannel.HandleMessage(chatMessage);
            OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs { ChatMessage = chatMessage });

            if (_chatCommandIdentifiers != null && _chatCommandIdentifiers.Count != 0 && !string.IsNullOrEmpty(chatMessage.Message))
            {
                if (_chatCommandIdentifiers.Contains(chatMessage.Message[0]))
                {
                    var chatCommand = new ChatCommand(chatMessage);
                    OnChatCommandReceived?.Invoke(this, new OnChatCommandReceivedArgs { Command = chatCommand });
                    return;
                }
            }
        }

        private void HandleNotice(IrcMessage ircMessage)
        {
            if (ircMessage.Message.Contains("Improperly formatted auth"))
            {
                OnIncorrectLogin?.Invoke(this, new OnIncorrectLoginArgs { Exception = new ErrorLoggingInException(ircMessage.ToString(), TwitchUsername) });
                return;
            }

            var success = ircMessage.Tags.TryGetValue(Tags.MsgId, out var msgId);
            if (!success)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                Log($"Unaccounted for: {ircMessage.ToString()}");
            }

            switch (msgId)
            {
                case MsgIds.ColorChanged:
                    OnChatColorChanged?.Invoke(this, new OnChatColorChangedArgs { Channel = ircMessage.Channel });
                    break;
                case MsgIds.HostOn:
                    OnNowHosting?.Invoke(this, new OnNowHostingArgs { Channel = ircMessage.Channel, HostedChannel = ircMessage.Message.Split(' ')[2].Replace(".", "") });
                    break;
                case MsgIds.HostOff:
                    OnHostLeft?.Invoke(this, null);
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
                case MsgIds.MsgChannelSuspended:
                    _awaitingJoins.RemoveAll(x => x.Key.ToLower() == ircMessage.Channel);
                    _joinedChannelManager.RemoveJoinedChannel(ircMessage.Channel);
                    QueueingJoinCheck();
                    OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs {
                        Exception = new FailureToReceiveJoinConfirmationException(ircMessage.Channel, ircMessage.Message)
                        });
                    break;
                case MsgIds.NoVIPs:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = new List<string>() });
                    break;
                case MsgIds.VIPsSuccess:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = ircMessage.Message.Replace(" ", "").Replace(".", "").Split(':')[1].Split(',').ToList() });
                    break;
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                    Log($"Unaccounted for: {ircMessage.ToString()}");
                    break;
            }
        }

        private void HandleJoin(IrcMessage ircMessage)
        {
            OnUserJoined?.Invoke(this, new OnUserJoinedArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
        }

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

        private void HandleHostTarget(IrcMessage ircMessage)
        {
            if (ircMessage.Message.StartsWith("-"))
            {
                var hostingStopped = new HostingStopped(ircMessage);
                OnHostingStopped?.Invoke(this, new OnHostingStoppedArgs { HostingStopped = hostingStopped });
            }
            else
            {
                var hostingStarted = new HostingStarted(ircMessage);
                OnHostingStarted?.Invoke(this, new OnHostingStartedArgs { HostingStarted = hostingStarted });
            }
        }

        private void HandleClearChat(IrcMessage ircMessage)
        {
            if (string.IsNullOrWhiteSpace(ircMessage.Message))
            {
                OnChatCleared?.Invoke(this, new OnChatClearedArgs { Channel = ircMessage.Channel });
                return;
            }

            var successBanDuration = ircMessage.Tags.TryGetValue(Tags.BanDuration, out _);
            if (successBanDuration)
            {
                var userTimeout = new UserTimeout(ircMessage);
                OnUserTimedout?.Invoke(this, new OnUserTimedoutArgs { UserTimeout = userTimeout });
                return;
            }

            var userBan = new UserBan(ircMessage);
            OnUserBanned?.Invoke(this, new OnUserBannedArgs { UserBan = userBan });
        }

        private void HandleClearMsg(IrcMessage ircMessage)
        {
            OnMessageCleared?.Invoke(this, new OnMessageClearedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message, TargetMessageId = ircMessage.ToString().Split('=')[2].Split(' ')[0] });
        }

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

        private void Handle004()
        {
            OnConnected?.Invoke(this, new OnConnectedArgs { AutoJoinChannel = _autoJoinChannel, BotUsername = TwitchUsername });
        }

        private void Handle353(IrcMessage ircMessage)
        {
            if (string.Equals(ircMessage.Channel, TwitchUsername, StringComparison.InvariantCultureIgnoreCase))
            {
                OnExistingUsersDetected?.Invoke(this, new OnExistingUsersDetectedArgs { Channel = ircMessage.Channel, Users = ircMessage.Message.Split(' ').ToList() });
            }
        }

        private void Handle366()
        {
            _currentlyJoiningChannels = false;
            QueueingJoinCheck();
        }

        private void HandleWhisper(IrcMessage ircMessage)
        {
            var whisperMessage = new WhisperMessage(ircMessage, TwitchUsername);
            PreviousWhisper = whisperMessage;
            OnWhisperReceived?.Invoke(this, new OnWhisperReceivedArgs { WhisperMessage = whisperMessage });

            if (_whisperCommandIdentifiers != null && _whisperCommandIdentifiers.Count != 0 && !string.IsNullOrEmpty(whisperMessage.Message))
                if (_whisperCommandIdentifiers.Contains(whisperMessage.Message[0]))
                {
                    var whisperCommand = new WhisperCommand(whisperMessage);
                    OnWhisperCommandReceived?.Invoke(this, new OnWhisperCommandReceivedArgs { Command = whisperCommand });
                    return;
                }
            OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "WhispergHandling", RawIRC = ircMessage.ToString() });
            Log($"Unaccounted for: {ircMessage.ToString()}");
        }

        private void HandleRoomState(IrcMessage ircMessage)
        {
            // If ROOMSTATE is sent because a mode (subonly/slow/emote/etc) is being toggled, it has two tags: room-id, and the specific mode being toggled
            // If ROOMSTATE is sent because of a join confirmation, all tags (ie greater than 2) are sent
            if (ircMessage.Tags.Count > 2)
            {
                var channel = _awaitingJoins.FirstOrDefault(x => x.Key == ircMessage.Channel);
                _awaitingJoins.Remove(channel);

                OnJoinedChannel?.Invoke(this, new OnJoinedChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
                if (OnBeingHosted != null)
                if (ircMessage.Channel.ToLowerInvariant() != TwitchUsername && !OverrideBeingHostedCheck)
                    Log("[OnBeingHosted] OnBeingHosted will only be fired while listening to this event as the broadcaster's channel. You do not appear to be connected as the broadcaster. To hide this warning, set TwitchClient property OverrideBeingHostedCheck to true.");
            }

            OnChannelStateChanged?.Invoke(this, new OnChannelStateChangedArgs { ChannelState = new ChannelState(ircMessage), Channel = ircMessage.Channel });
        }

        private void HandleUserNotice(IrcMessage ircMessage)
        {
            var successMsgId = ircMessage.Tags.TryGetValue(Tags.MsgId, out var msgId);
            if (!successMsgId)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                Log($"Unaccounted for: {ircMessage.ToString()}");
                return;
            }

            switch (msgId)
            {
                case MsgIds.Raid:
                    var raidNotification = new RaidNotification(ircMessage);
                    OnRaidNotification?.Invoke(this, new OnRaidNotificationArgs { Channel = ircMessage.Channel, RaidNotificaiton = raidNotification });
                    break;
                case MsgIds.ReSubscription:
                    var resubscriber = new ReSubscriber(ircMessage);
                    OnReSubscriber?.Invoke(this, new OnReSubscriberArgs { ReSubscriber = resubscriber, Channel = ircMessage.Channel });
                    break;
                case MsgIds.Ritual:
                    var successRitualName = ircMessage.Tags.TryGetValue(Tags.MsgParamRitualName, out var ritualName);
                    if (!successRitualName)
                    {
                        OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeRitualHandling", RawIRC = ircMessage.ToString() });
                        Log($"Unaccounted for: {ircMessage.ToString()}");
                        return;
                    }
                    switch (ritualName)
                    {
                        case "new_chatter": // In case there will be more Rituals we should do a "string enum" for them too but for now this will do
                            OnRitualNewChatter?.Invoke(this, new OnRitualNewChatterArgs { RitualNewChatter = new RitualNewChatter(ircMessage) });
                            break;
                        default:
                            OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                            Log($"Unaccounted for: {ircMessage.ToString()}");
                            break;
                    }
                    break;
                case MsgIds.AnonSubGift:
                var anonGiftedSubscription = new AnonGiftedSubscription(ircMessage);
                OnAnonGiftedSubscription?.Invoke(this, new OnAnonGiftedSubscriptionArgs { AnonGiftedSubscription = anonGiftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.SubGift:
                    var giftedSubscription = new GiftedSubscription(ircMessage);
                    OnGiftedSubscription?.Invoke(this, new OnGiftedSubscriptionArgs { GiftedSubscription = giftedSubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.CommunitySubscription:
                    var communitySubscription = new CommunitySubscription(ircMessage);
                    OnCommunitySubscription?.Invoke(this, new OnCommunitySubscriptionArgs { GiftedSubscription = communitySubscription, Channel = ircMessage.Channel });
                    break;
                case MsgIds.Subscription:
                    var subscriber = new Subscriber(ircMessage);
                    OnNewSubscriber?.Invoke(this, new OnNewSubscriberArgs { Subscriber = subscriber, Channel = ircMessage.Channel });
                    break;
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "UserNoticeHandling", RawIRC = ircMessage.ToString() });
                    Log($"Unaccounted for: {ircMessage.ToString()}");
                    break;
            }
        }

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

        #endregion

        #endregion

        private void Log(string message, bool includeDate = false, bool includeTime = false)
        {
            string dateTimeStr;
            if (includeDate && includeTime)
                dateTimeStr = $"{DateTime.UtcNow}";
            else if (includeDate)
                dateTimeStr = $"{DateTime.UtcNow.ToShortDateString()}";
            else
                dateTimeStr = $"{DateTime.UtcNow.ToShortTimeString()}";

            if (includeDate || includeTime)
                _logger?.LogInformation($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version} - {dateTimeStr}] {message}");
            else
                _logger?.LogInformation($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version}] {message}");

            OnLog?.Invoke(this, new OnLogArgs { BotUsername = ConnectionCredentials?.TwitchUsername, Data = message, DateTime = DateTime.UtcNow });
        }
        
        private void LogError(string message, bool includeDate = false, bool includeTime = false)
        {
            string dateTimeStr;
            if (includeDate && includeTime)
                dateTimeStr = $"{DateTime.UtcNow}";
            else if (includeDate)
                dateTimeStr = $"{DateTime.UtcNow.ToShortDateString()}";
            else
                dateTimeStr = $"{DateTime.UtcNow.ToShortTimeString()}";

            if (includeDate || includeTime)
                _logger?.LogError($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version} - {dateTimeStr}] {message}");
            else
                _logger?.LogError($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version}] {message}");

            OnLog?.Invoke(this, new OnLogArgs { BotUsername = ConnectionCredentials?.TwitchUsername, Data = message, DateTime = DateTime.UtcNow });
        }
        
        public void SendQueuedItem(string message)
        {
            if (!IsInitialized) HandleNotInitialized();
            _client.Send(message);
        }

        protected static void HandleNotInitialized()
        {
            throw new ClientNotInitializedException("The twitch client has not been initialized and cannot be used. Please call Initialize();");
        }

        protected static void HandleNotConnected()
        {
            throw new ClientNotConnectedException("In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.");
        }
    }
}
