using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     Interface ITwitchClient
    /// </summary>
    public interface ITwitchClient :
        ITwitchClient_BackendLogging,
        ITwitchClient_Channel,
        ITwitchClient_Client,
        ITwitchClient_Connection,
        ITwitchClient_MessageSending,
        ITwitchClient_Notice,
        ITwitchClient_NoticeUser,
        ITwitchClient_Whisper
    {
        /// <summary>
        /// Gets or sets a value indicating whether [disable automatic pong].
        /// </summary>
        /// <value><c>true</c> if [disable automatic pong]; otherwise, <c>false</c>.</value>
        bool DisableAutoPong { get; set; }
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
        /// <summary>
        ///     Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">
        ///     <see cref="ConnectionCredentials"/>
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <param name="chatCommandIdentifier">
        ///     The chat command identifier.
        /// </param>
        /// <param name="whisperCommandIdentifier">
        ///     The whisper command identifier.
        /// </param>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!');

        /// <summary>
        ///     Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">
        ///     <see cref="ConnectionCredentials"/>
        /// </param>
        /// <param name="channels">
        ///     The channels to join once connected.
        /// </param>
        /// <param name="chatCommandIdentifier">
        ///     The chat command identifier.
        /// </param>
        /// <param name="whisperCommandIdentifier">
        ///     The whisper command identifier.
        /// </param>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        void Initialize(ConnectionCredentials credentials, List<string> channels, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!');

        /// <summary>
        ///     Adds the chat command identifier.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier.
        /// </param>
        void AddChatCommandIdentifier(char identifier);
        /// <summary>
        ///     Removes the chat command identifier.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier.
        /// </param>
        void RemoveChatCommandIdentifier(char identifier);
    }
}
