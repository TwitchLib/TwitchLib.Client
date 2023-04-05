using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to send messages
    /// </summary>
    public interface ITwitchClient_MessageSending
    {
        #region events public
        /// <summary>
        ///     Occurs when [on message sent].
        /// </summary>
        event EventHandler<OnMessageSentArgs>? OnMessageSent;
        /// <summary>
        ///     Occurs when [on message send failed].
        /// </summary>
        event EventHandler<OnSendFailedEventArgs>? OnSendFailed;
        /// <summary>
        ///     Occurs when [on message throttled].
        /// </summary>
        event EventHandler<OnMessageThrottledArgs>? OnMessageThrottled;
        #endregion events public


        #region methods public
        /// <summary>
        ///     Sends the given <paramref name="message"/> in the given <paramref name="channel"/>
        ///     <br></br>
        ///     <b>take care</b>,
        ///     <br></br>
        ///     if the message has more than 500 <see langword="char"/>s,
        ///     <br></br>
        ///     it wont get sent
        /// </summary>
        /// <param name="channel">
        ///     channel-name
        /// </param>
        /// <param name="message">
        ///     'normal' message to send
        ///     <br></br>
        ///     <b>no</b> raw irc-message
        ///     <br></br>
        ///     therefore use <see cref="SendRaw(String)"/> instead
        /// </param>
        /// <param name="dryRun">
        ///     if set to <see langword="true"/>, the created <see cref="OutboundChatMessage"/> wont be added to send-queue
        /// </param>
        void SendMessage(JoinedChannel channel, string message, bool dryRun = false);
        /// <inheritdoc cref="SendMessage(JoinedChannel, String, Boolean)"/>
        void SendMessage(string channel, string message, bool dryRun = false);
        /// <summary>
        ///     Sends the given <paramref name="message"/> in the given <paramref name="channel"/>
        ///     as reply to a message with the given <paramref name="replyToId"/>
        ///     <br></br>
        ///     <b>take care</b>,
        ///     <br></br>
        ///     if the message has more than 500 <see langword="char"/>s,
        ///     <br></br>
        ///     it wont get sent
        /// </summary>
        /// <param name="channel">
        ///     channel-name
        /// </param>
        /// <param name="replyToId">
        ///     <inheritdoc cref="ChatMessage.Id"/>
        ///     see also
        ///     <br></br>
        ///     <see cref="ChatMessage.Id"/>
        /// </param>
        /// <param name="message">
        ///     'normal' message to send
        ///     <br></br>
        ///     <b>no</b> raw irc-message
        ///     <br></br>
        ///     therefore use <see cref="SendRaw(String)"/> instead
        /// </param>
        /// <param name="dryRun">
        ///     if set to <see langword="true"/>, the created <see cref="OutboundChatMessage"/> wont be added to send-queue
        /// </param>
        void SendReply(JoinedChannel channel, string replyToId, string message, bool dryRun = false);
        /// <inheritdoc cref="SendReply(JoinedChannel, String, String, Boolean)"/>
        void SendReply(string channel, string replyToId, string message, bool dryRun = false);
        /// <summary>
        ///     sends an raw irc-message
        ///     <br></br>
        ///     <b>take care</b>,
        ///     <br></br>
        ///     it bypasses the integrated <see cref="Services.ThrottlerService"/>
        /// </summary>
        /// <param name="message">
        ///     an raw irc-message
        /// </param>
        void SendRaw(string message);
        #endregion methods public
    }
}
