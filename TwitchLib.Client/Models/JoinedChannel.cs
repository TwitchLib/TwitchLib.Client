using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Helpers;
using TwitchLib.Client.Interfaces;

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
                RaiseEventHelper.RaiseEvent(twitchClient, nameof(twitchClient.OnUserStateChanged), new OnUserStateChangedArgs { UserState = userState, Channel = Channel });
                return;
            }
            bool gotValue = BotMessages.TryRemove(userState.Id, out ChatMessage chatMessage);
            if (gotValue && chatMessage != null)
            {
                RaiseEventHelper.RaiseEvent(twitchClient, nameof(twitchClient.OnMessageSent), new OnMessageSentArgs() { SentMessage = chatMessage });
            }
        }
        #endregion methods public
    }
}
