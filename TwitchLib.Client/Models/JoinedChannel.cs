using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Helpers;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    // TODO: Missing builder
    /// <summary>
    ///     Class representing a joined channel.
    /// </summary>
    public class JoinedChannel
    {
        #region properties private
        private ConcurrentDictionary<string, ChatMessage> BotMessages { get; } = new ConcurrentDictionary<string, ChatMessage>();
        private ChannelState State { get; set; }
        #endregion properties private


        #region properties public
        /// <summary>
        ///     name of the channel
        /// </summary>
        public string Channel { get; }
        /// <summary>
        ///     Name of the Bot
        /// </summary>
        public string BotUsername { get; }
        /// <summary>
        ///     <see cref="ChatMessage"/>s that the Bot sent, but <see cref="ITwitchClient"/> did not received a corresponding <see cref="Enums.IrcCommand.UserState"/> yet
        /// </summary>
        public ICollection<ChatMessage> UnRaisedSentMessages => BotMessages.Values;
        #endregion properties public


        #region ctor
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="channel">
        ///     <inheritdoc cref="Channel"/>
        /// </param>
        /// <param name="botUsername">
        ///     <inheritdoc cref="BotUsername"/>
        /// </param>
        public JoinedChannel(string channel, string botUsername)
        {
            Channel = channel;
            BotUsername = botUsername;
        }
        #endregion ctor


        #region methods public
        /// <summary>Handles a message</summary>
        internal void HandlePRIVMSG(ChatMessage message)
        {
            if (String.Equals(message?.Username?.ToLower(), BotUsername))
            {
                BotMessages.TryAdd(message.Id, message);
            }
        }
        /// <summary>
        ///     <see href="https://dev.twitch.tv/docs/irc/commands/#userstate"/>
        ///     <br></br>
        ///     <see href="https://dev.twitch.tv/docs/irc/tags/#userstate-tags"/>
        /// </summary>
        internal void HandleUSERSTATE(UserState userState, ITwitchClient twitchClient)
        {
            if (userState.Id.IsNullOrEmptyOrWhitespace())
            {
                OnUserStateChangedArgs args = new OnUserStateChangedArgs { UserState = userState, Channel = Channel };
                RaiseEventHelper.RaiseEvent(twitchClient, nameof(twitchClient.OnUserStateChanged), args);
                return;
            }
            bool gotValue = BotMessages.TryRemove(userState.Id, out ChatMessage chatMessage);
            if (gotValue && chatMessage != null)
            {
                OnMessageSentArgs args = new OnMessageSentArgs() { SentMessage = chatMessage, Channel = Channel };
                RaiseEventHelper.RaiseEvent(twitchClient, nameof(twitchClient.OnMessageSent), args);
            }
        }
        /// <summary>
        ///     <see href="https://dev.twitch.tv/docs/irc/tags/#roomstate-tags"/>
        ///     <br></br>
        ///     <br></br>
        ///     <b>2nd paragraph:</b>
        ///     <br></br>
        ///     For JOIN messages, the message contains all chat room setting tags,
        ///     <br></br>
        ///     but for actions that change a single chat room setting,
        ///     <br></br>
        ///     the message includes only that chat room setting tag.
        ///     <br></br>
        ///     For example,
        ///     <br></br>
        ///     if the moderator turned on unique chat,
        ///     <br></br>
        ///     the message includes only the r9k tag.
        /// </summary>
        /// <param name="ircMessage">
        ///     <see cref="IrcMessage"/>
        /// </param>
        /// <param name="twitchClient">
        ///     <see cref="ITwitchClient"/>
        /// </param>
        internal void HandleROOMSTATE(IrcMessage ircMessage, ITwitchClient twitchClient)
        {
            if (State == null)
            {
                State = new ChannelState(ircMessage); ;
            }
            else
            {
                State.Apply(ircMessage);
            }
            OnChannelStateChangedArgs args = new OnChannelStateChangedArgs { ChannelState = State, Channel = Channel };
            RaiseEventHelper.RaiseEvent(twitchClient, nameof(twitchClient.OnChannelStateChanged), args);
        }
        #endregion methods public
    }
}
