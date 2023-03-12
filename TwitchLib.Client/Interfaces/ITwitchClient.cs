using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     Interface ITwitchClient
    /// </summary>
    public interface ITwitchClient :
        ITwitchClient_Channel,
        ITwitchClient_Client,
        ITwitchClient_Connection,
        ITwitchClient_Notice,
        ITwitchClient_NoticeUser,
        ITwitchClient_Whisper,
        ITwitchClient_BackendLogging
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
        /// Occurs when [on chat cleared].
        /// </summary>
        event EventHandler<OnChatClearedArgs> OnChatCleared;
        /// <summary>
        /// Occurs when [on chat command received].
        /// </summary>
        event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;
        /// <summary>
        /// Occurs when [on existing users detected].
        /// </summary>
        event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

        /// <summary>
        /// Occurs when [on message received].
        /// </summary>
        event EventHandler<OnMessageReceivedArgs> OnMessageReceived;
        /// <summary>
        /// Occurs when [on message sent].
        /// </summary>
        event EventHandler<OnMessageSentArgs> OnMessageSent;
        /// <summary>
        /// Occurs when [on moderator joined].
        /// </summary>
        event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;
        /// <summary>
        /// Occurs when [on moderator left].
        /// </summary>
        event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;

        /// <summary>
        /// Occurs when [on user banned].
        /// </summary>
        event EventHandler<OnUserBannedArgs> OnUserBanned;
        /// <summary>
        /// Occurs when [on user joined].
        /// </summary>
        event EventHandler<OnUserJoinedArgs> OnUserJoined;
        /// <summary>
        /// Occurs when [on user left].
        /// </summary>
        event EventHandler<OnUserLeftArgs> OnUserLeft;
        /// <summary>
        /// Occurs when [on user state changed].
        /// </summary>
        event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;
        /// <summary>
        /// Occurs when [on user timedout].
        /// </summary>
        event EventHandler<OnUserTimedoutArgs> OnUserTimedout;
        /// <summary>
        /// Occurs when [on message throttled].
        /// </summary>
        event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
        /// <summary>
        /// Occurs when [on message deleted].
        /// </summary>
        event EventHandler<OnMessageClearedArgs> OnMessageCleared;
        /// <summary>
        /// Fires when the client receives a PRIVMSG tagged as an user-intro
        /// </summary>
        event EventHandler<OnUserIntroArgs> OnUserIntro;
        /// <summary>
        /// Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="chatCommandIdentifier">The chat command identifier.</param>
        /// <param name="whisperCommandIdentifier">The whisper command identifier.</param>
        void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!');

        /// <summary>
        /// Initializes the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="channels">The channels to join once connected.</param>
        /// <param name="chatCommandIdentifier">The chat command identifier.</param>
        /// <param name="whisperCommandIdentifier">The whisper command identifier.</param>
        void Initialize(ConnectionCredentials credentials, List<string> channels, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!');

        /// <summary>
        /// Adds the chat command identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        void AddChatCommandIdentifier(char identifier);
        /// <summary>
        /// Removes the chat command identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        void RemoveChatCommandIdentifier(char identifier);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        void SendMessage(JoinedChannel channel, string message, bool dryRun = false);
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        void SendMessage(string channel, string message, bool dryRun = false);
        /// <summary>
        /// Sends a formatted Twitch chat message reply.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        void SendReply(JoinedChannel channel, string replyToId, string message, bool dryRun = false);
        /// <summary>
        /// SendReply wrapper that accepts channel in string form.
        /// </summary>
        /// <param name="channel">Channel to send Twitch chat reply to</param>
        /// <param name="replyToId">The message id that is being replied to</param>
        /// <param name="message">Reply contents</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run]</param>
        void SendReply(string channel, string replyToId, string message, bool dryRun = false);
        /// <summary>
        /// Sends the queued item.
        /// </summary>
        /// <param name="message">The message.</param>
        void SendQueuedItem(string message);
        /// <summary>
        /// Sends the raw.
        /// </summary>
        /// <param name="message">The message.</param>
        void SendRaw(string message);
    }
}
