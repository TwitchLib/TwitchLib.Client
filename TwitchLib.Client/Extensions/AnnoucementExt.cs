using System;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Extensions
{
    /// <summary>
    /// Extension implementing the Announcement functionality in TwitchClient.
    /// </summary>
    public static class AnnoucementExt
    {
        /// <summary>
        /// Send an Announcement to a channel using a JoinedChannel
        /// </summary>
        /// <param name="client">Client reference used to identify extension.</param>
        /// <param name="channel">JoinedChannel object to announce to</param>
        /// <param name="message">Message to announce</param>
        [Obsolete("Usage of this command through chat is not possible anymore. Use TwitchLib.Api.Helix.Chat.SendChatAnnouncementAsync() instead.")]
        public static void Announce(this ITwitchClient client, JoinedChannel channel, string message)
        {
            client.SendMessage(channel, $".announce {message}");
        }

        /// <summary>
        /// Send an Announcement to a channel using a string to represent the channel
        /// </summary>
        /// <param name="client">Client reference used to identify extension.</param>
        /// <param name="channel">Channel in string form to send announce to</param>
        /// <param name="message">Message to announce</param>
        [Obsolete("Usage of this command through chat is not possible anymore. Use TwitchLib.Api.Helix.Chat.SendChatAnnouncementAsync() instead.")]
        public static void Announce(this ITwitchClient client, string channel, string message)
        {
            client.SendMessage(channel, $".announce {message}");
        }
    }
}