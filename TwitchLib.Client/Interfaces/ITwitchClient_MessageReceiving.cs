﻿using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to receiving messages
    ///     <br></br>
    ///     <br></br>
    ///     a bit unspecified...
    /// </summary>
    public interface ITwitchClient_MessageReceiving
    {
        #region events public
        /// <summary>
        ///     Occurs when [on chat cleared].
        /// </summary>
        event EventHandler<OnChatClearedArgs> OnChatCleared;
        /// <summary>
        ///     Occurs when [on chat command received].
        /// </summary>
        event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;
        /// <summary>
        ///     Occurs when [on existing users detected].
        /// </summary>
        event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;
        /// <summary>
        ///     Occurs when [on message received].
        /// </summary>
        event EventHandler<OnMessageReceivedArgs> OnMessageReceived;
        /// <summary>
        ///     Occurs when [on moderator joined].
        /// </summary>
        event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;
        /// <summary>
        ///     Occurs when [on moderator left].
        /// </summary>
        event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;
        /// <summary>
        ///     Occurs when [on user banned].
        /// </summary>
        event EventHandler<OnUserBannedArgs> OnUserBanned;
        /// <summary>
        ///     Occurs when [on user joined].
        /// </summary>
        event EventHandler<OnUserJoinedArgs> OnUserJoined;
        /// <summary>
        ///     Occurs when [on user left].
        /// </summary>
        event EventHandler<OnUserLeftArgs> OnUserLeft;
        /// <summary>
        ///     Occurs when [on user state changed].
        /// </summary>
        event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;
        /// <summary>
        ///     Occurs when [on user timedout].
        /// </summary>
        event EventHandler<OnUserTimedoutArgs> OnUserTimedout;
        /// <summary>
        ///     Occurs when [on message deleted].
        /// </summary>
        event EventHandler<OnMessageClearedArgs> OnMessageCleared;
        /// <summary>
        ///     Fires when the client receives a PRIVMSG tagged as an user-intro
        /// </summary>
        event EventHandler<OnUserIntroArgs> OnUserIntro;
        #endregion events public


        #region methods public
        /// <summary>
        ///     parses the given <paramref name="ircMessage"/> and passes it to <see cref="HandleIrcMessage(IrcMessage)"/>
        /// </summary>
        /// <param name="ircMessage">
        ///     raw irc-message as <see cref="String"/>
        /// </param>
        /// <returns>
        ///     <see langword="false"/>, if the message was <see cref="ITwitchClient.OnUnaccountedFor"/>
        ///     <br></br>
        ///     <see langword="true"/> otherwise
        /// </returns>
        bool HandleIrcMessage(string ircMessage);
        /// <summary>
        ///     tries to handle the given <see cref="IrcMessage"/>
        /// </summary>
        /// <param name="ircMessage">
        ///     <see cref="IrcMessage"/>
        /// </param>
        /// <returns>
        ///     <see langword="false"/>, if the message was <see cref="ITwitchClient.OnUnaccountedFor"/>
        ///     <br></br>
        ///     <see langword="true"/> otherwise
        /// </returns>
        bool HandleIrcMessage(IrcMessage ircMessage);
        #endregion methods public
    }
}
