using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client
{
    /// <summary>
    ///     Represents a client connected to a Twitch channel.
    ///     Implements the <see cref="TwitchLib.Client.Interfaces.ITwitchClient" />
    /// </summary>
    [SuppressMessage("Style", "IDE0058")]
    public partial class TwitchClient : ITwitchClient
    {
        #region Private Variables
        private ISet<char> ChatCommandIdentifiers { get; } = new HashSet<char>();
        private ILogger<TwitchClient> LOGGER { get; }
        private string LastMessageSent { get; set; }
        #endregion

        #region Public Variables
        public Version Version => Assembly.GetEntryAssembly().GetName().Version;
        public string TwitchUsername { get; private set; }
        public bool DisableAutoPong { get; set; } = false;
        public bool WillReplaceEmotes { get; set; } = false;
        #endregion

        #region Events


        public event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;

        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public event EventHandler<OnMessageSentArgs> OnMessageSent;

        public event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

        public event EventHandler<OnUserJoinedArgs> OnUserJoined;
        public event EventHandler<OnUserLeftArgs> OnUserLeft;
        public event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;
        public event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;



        public event EventHandler<OnMessageClearedArgs> OnMessageCleared;

        public event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;


        public event EventHandler<OnChatClearedArgs> OnChatCleared;

        public event EventHandler<OnUserTimedoutArgs> OnUserTimedout;

        public event EventHandler<OnUserBannedArgs> OnUserBanned;

        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;

        public event EventHandler<OnUserIntroArgs> OnUserIntro;

        #endregion

        #region Construction Work

        public TwitchClient(IClient client = null, ClientProtocol protocol = ClientProtocol.WebSocket, ILogger<TwitchClient> logger = null)
        {
            LOGGER = logger;
            Protocol = protocol;
            Client = client;
            if (Client == null)
            {
                switch (Protocol)
                {
                    case ClientProtocol.TCP:
                        Client = new TcpClient();
                        break;
                    case ClientProtocol.WebSocket:
                        Client = new WebSocketClient();
                        break;
                }
            }
            Debug.Assert(Client != null, nameof(Client) + " != null");
            InitializeClient();
            ChannelManager = new ChannelManager(Client, Log, LogError);
        }

        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        public void Initialize(ConnectionCredentials credentials,
                               string channel = null,
                               char chatCommandIdentifier = '!',
                               char whisperCommandIdentifier = '!')
        {
            if (channel != null && channel[0] == '#') channel = channel.Substring(1);
            InitializeHelper(credentials, new List<string>() { channel }, chatCommandIdentifier);
        }

        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        public void Initialize(ConnectionCredentials credentials,
                               List<string> channels,
                               char chatCommandIdentifier = '!',
                               char whisperCommandIdentifier = '!')
        {
            channels = channels.Select(x => x[0] == '#' ? x.Substring(1) : x).ToList();
            InitializeHelper(credentials, channels, chatCommandIdentifier);
        }

        private void InitializeHelper(ConnectionCredentials credentials,
                                      List<string> channels,
                                      char chatCommandIdentifier = '!')
        {
            Log($"TwitchLib-TwitchClient initialized, assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");
            ConnectionCredentials = credentials;
            TwitchUsername = ConnectionCredentials.TwitchUsername;
            if (chatCommandIdentifier != '\0')
                ChatCommandIdentifiers.Add(chatCommandIdentifier);

            ChannelManager.JoinChannels(channels);
        }

        #endregion

        #region SendMessage

        public void SendRaw(string message)
        {
            if (!IsInitialized) HandleNotInitialized();

            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }

        private void SendPONG()
        {
            if (!IsInitialized) HandleNotInitialized();
            string message = "PONG";
            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.SendPONG();
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }

        private void SendTwitchMessage(JoinedChannel channel, string message, string replyToId = null, bool dryRun = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (channel == null || message == null || dryRun) return;
            if (message.Length > 500)
            {
                LogError("Message length has exceeded the maximum character count. (500)");
                return;
            }

            OutboundChatMessage twitchMessage = new OutboundChatMessage
            {
                Channel = channel.Channel,
                Username = ConnectionCredentials.TwitchUsername,
                Message = message
            };
            if (replyToId != null)
            {
                twitchMessage.ReplyToId = replyToId;
            }

            LastMessageSent = message;

            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(twitchMessage.ToString());
        }

        public void SendMessage(JoinedChannel channel, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message, null, dryRun);
        }

        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            SendMessage(GetJoinedChannel(channel), message, dryRun);
        }

        public void SendReply(JoinedChannel channel, string replyToId, string message, bool dryRun = false)
        {
            SendTwitchMessage(channel, message, replyToId, dryRun);
        }

        public void SendReply(string channel, string replyToId, string message, bool dryRun = false)
        {
            SendReply(GetJoinedChannel(channel), replyToId, message, dryRun);
        }

        #endregion

        #region Command Identifiers

        public void AddChatCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            ChatCommandIdentifiers.Add(identifier);
        }

        public void RemoveChatCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
            ChatCommandIdentifiers.Remove(identifier);
        }
        #endregion

        #region IrcMessage Handling

        private void HandleIrcMessage(IrcMessage ircMessage)
        {
            if (ircMessage.Message.Contains("Login authentication failed"))
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
                    HandleNotice(ircMessage);
                    break;
                case IrcCommand.Ping:
                    if (!DisableAutoPong)
                        SendPONG();
                    return;
                case IrcCommand.Pong:
                    return;
                case IrcCommand.Join:
                    if (ircMessage.User == TwitchUsername)
                    {
                        // TODO: lets see if thats correct
                        ChannelManager.JoinCompleted(ircMessage.Channel);
                        OnJoinedChannel?.Invoke(this, new OnJoinedChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
                    }
                    OnUserJoined?.Invoke(this, new OnUserJoinedArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
                    break;
                case IrcCommand.Part:
                    HandlePart(ircMessage);
                    break;
                case IrcCommand.ClearChat:
                    HandleClearChat(ircMessage);
                    break;
                case IrcCommand.ClearMsg:
                    OnMessageCleared?.Invoke(this, new OnMessageClearedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message, TargetMessageId = ircMessage.ToString().Split('=')[3].Split(';')[0], TmiSentTs = ircMessage.ToString().Split('=')[4].Split(' ')[0] });
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
                    ChannelManager.Start();
                    OnConnected?.Invoke(this, new OnConnectedArgs { BotUsername = TwitchUsername });
                    break;
                case IrcCommand.RPL_353:
                    OnExistingUsersDetected?.Invoke(this, new OnExistingUsersDetectedArgs { Channel = ircMessage.Channel, Users = ircMessage.Message.Split(' ').ToList() });
                    break;
                case IrcCommand.RPL_366:
                    break;
                case IrcCommand.RPL_372:
                    break;
                case IrcCommand.RPL_375:
                    break;
                case IrcCommand.RPL_376:
                    break;
                case IrcCommand.Whisper:
                    break;
                case IrcCommand.RoomState:
                    OnChannelStateChanged?.Invoke(this, new OnChannelStateChangedArgs { ChannelState = new ChannelState(ircMessage), Channel = ircMessage.Channel });
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
                    // do nothing
                    break;
                case IrcCommand.Unknown:
                // fall through
                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = null, Location = "HandleIrcMessage", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }
        #endregion

        #region IrcCommand Handling

        private void HandlePrivMsg(IrcMessage ircMessage)
        {
            ChatMessage chatMessage = new ChatMessage(TwitchUsername, ircMessage, WillReplaceEmotes);
            foreach (JoinedChannel joinedChannel in JoinedChannels.Where(x => String.Equals(x.Channel, ircMessage.Channel, StringComparison.InvariantCultureIgnoreCase)))
                joinedChannel.HandleMessage(chatMessage);

            OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs { ChatMessage = chatMessage });

            if (ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId))
            {
                if (msgId == MsgIds.UserIntro)
                    OnUserIntro?.Invoke(this, new OnUserIntroArgs { ChatMessage = chatMessage });
            }

            if (ChatCommandIdentifiers != null && ChatCommandIdentifiers.Count != 0 && !String.IsNullOrEmpty(chatMessage.Message))
            {
                if (ChatCommandIdentifiers.Contains(chatMessage.Message[0]))
                {
                    ChatCommand chatCommand = new ChatCommand(chatMessage);
                    OnChatCommandReceived?.Invoke(this, new OnChatCommandReceivedArgs { Command = chatCommand });
                    return;
                }
            }
        }

        private void HandlePart(IrcMessage ircMessage)
        {
            if (String.Equals(TwitchUsername, ircMessage.User, StringComparison.InvariantCultureIgnoreCase))
            {
                ChannelManager.LeaveChannel(ircMessage.Channel);
                // IDE0058
                HasSeenJoinedChannels.Remove(ircMessage.Channel);
                OnLeftChannel?.Invoke(this, new OnLeftChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
            }
            else
            {
                OnUserLeft?.Invoke(this, new OnUserLeftArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
            }
        }

        private void HandleClearChat(IrcMessage ircMessage)
        {
            if (String.IsNullOrWhiteSpace(ircMessage.Message))
            {
                OnChatCleared?.Invoke(this, new OnChatClearedArgs { Channel = ircMessage.Channel });
                return;
            }

            bool successBanDuration = ircMessage.Tags.ContainsKey(Tags.BanDuration);
            if (successBanDuration)
            {
                UserTimeout userTimeout = new UserTimeout(ircMessage);
                OnUserTimedout?.Invoke(this, new OnUserTimedoutArgs { UserTimeout = userTimeout });
                return;
            }

            UserBan userBan = new UserBan(ircMessage);
            OnUserBanned?.Invoke(this, new OnUserBannedArgs { UserBan = userBan });
        }

        private void HandleUserState(IrcMessage ircMessage)
        {
            UserState userState = new UserState(ircMessage);
            if (!HasSeenJoinedChannels.Contains(userState.Channel.ToLowerInvariant()))
            {
                HasSeenJoinedChannels.Add(userState.Channel.ToLowerInvariant());
                OnUserStateChanged?.Invoke(this, new OnUserStateChangedArgs { UserState = userState });
            }
            else
            {
                OnMessageSent?.Invoke(this, new OnMessageSentArgs { SentMessage = new SentMessage(userState, LastMessageSent) });
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

        public void SendQueuedItem(string message)
        {
            if (!IsInitialized) HandleNotInitialized();
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
        }

    }
}
