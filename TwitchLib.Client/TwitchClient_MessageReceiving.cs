using System;
using System.Linq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_MessageReceiving
    {
        #region events public
        public event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;
        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;
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
        public event EventHandler<OnUserIntroArgs> OnUserIntro;
        #endregion events public


        #region methods private
        private void HandleIrcMessage(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
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
                    OnError?.Invoke(this, new OnErrorEventArgs() { Exception = new KeepAliveException(TwitchUsername) });
                    return;
                case IrcCommand.Join:
                    if (ircMessage.User == TwitchUsername)
                    {
                        // join is only completed, whenever its ourselves
                        // ChannelManager manages channels we want to join!
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
                    OnConnectedArgs args = new OnConnectedArgs { BotUsername = TwitchUsername, AutoJoinChannels = ChannelManager.AutoJoinChannels };
                    if (ConnectionStateManager.WasConnected)
                    {
                        OnReconnected?.Invoke(this, args);
                    }
                    else
                    {
                        ConnectionStateManager.SetConnected();
                        OnConnected?.Invoke(this, args);
                    }
                    ChannelManager.Start();
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
                case IrcCommand.RoomState:
                    JoinedChannel joinedChannel = GetJoinedChannel(ircMessage.Channel);
                    joinedChannel?.HandleROOMSTATE(ircMessage, this);
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
        private void HandlePrivMsg(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
            ChatMessage chatMessage = new ChatMessage(TwitchUsername, ircMessage, WillReplaceEmotes);
            JoinedChannel joinedChannel = GetJoinedChannel(ircMessage.Channel);
            joinedChannel?.HandlePRIVMSG(chatMessage);

            OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs { ChatMessage = chatMessage, Channel = ircMessage.Channel });

            if (ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId))
            {
                if (msgId == MsgIds.UserIntro)
                    OnUserIntro?.Invoke(this, new OnUserIntroArgs { ChatMessage = chatMessage, Channel = ircMessage.Channel });
            }

            if (ChatCommandIdentifiers != null && ChatCommandIdentifiers.Count != 0 && !String.IsNullOrEmpty(chatMessage.Message))
            {
                if (ChatCommandIdentifiers.Contains(chatMessage.Message[0]))
                {
                    ChatCommand chatCommand = new ChatCommand(chatMessage);
                    OnChatCommandReceived?.Invoke(this, new OnChatCommandReceivedArgs { Command = chatCommand });
                }
            }
        }
        private void HandlePart(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (String.Equals(TwitchUsername, ircMessage.User, StringComparison.InvariantCultureIgnoreCase))
            {
                ChannelManager.LeaveChannel(ircMessage.Channel);
                OnLeftChannel?.Invoke(this, new OnLeftChannelArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel });
            }
            else
            {
                OnUserLeft?.Invoke(this, new OnUserLeftArgs { Channel = ircMessage.Channel, Username = ircMessage.User });
            }
        }
        private void HandleClearChat(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (String.IsNullOrWhiteSpace(ircMessage.Message))
            {
                OnChatCleared?.Invoke(this, new OnChatClearedArgs { Channel = ircMessage.Channel });
                return;
            }

            bool successBanDuration = ircMessage.Tags.ContainsKey(Tags.BanDuration);
            if (successBanDuration)
            {
                UserTimeout userTimeout = new UserTimeout(ircMessage);
                OnUserTimedout?.Invoke(this, new OnUserTimedoutArgs { UserTimeout = userTimeout, Channel = ircMessage.Channel });
                return;
            }

            UserBan userBan = new UserBan(ircMessage);
            OnUserBanned?.Invoke(this, new OnUserBannedArgs { UserBan = userBan, Channel = ircMessage.Channel });
        }
        /// <summary>
        ///     <see href="https://dev.twitch.tv/docs/irc/commands/#userstate"/>
        ///     <br></br>
        ///     <see href="https://dev.twitch.tv/docs/irc/tags/#userstate-tags"/>
        /// </summary>
        /// <param name="ircMessage">
        ///     <see cref="IrcMessage"/>
        /// </param>
        private void HandleUserState(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
            UserState userState = new UserState(ircMessage, LOGGER);
            JoinedChannel joinedChannel = GetJoinedChannel(userState.Channel);
            joinedChannel?.HandleUSERSTATE(userState, this);
        }
        private void HandleMode(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(GetType());
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
        #endregion methods private

    }
}
