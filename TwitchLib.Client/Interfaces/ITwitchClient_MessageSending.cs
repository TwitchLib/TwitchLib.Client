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
        /// <summary>
        ///     Occurs when [on message sent].
        /// </summary>
        event EventHandler<OnMessageSentArgs> OnMessageSent;
        /// <summary>
        /// Occurs when [on message throttled].
        /// </summary>
        event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
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
