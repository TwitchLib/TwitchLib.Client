using System;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Extensions
{
    /// <summary>
    /// Extension for implementing marker functionality
    /// </summary>
    public static class MarkerExt
    {
        /// <summary>
        /// Sends command to create a marker using a JoinedChannel object.
        /// </summary>
        /// <param name="client">Client reference used to identify extension.</param>
        /// <param name="channel">JoinedChannel representation of the channel to send the marker command to.</param>
        [Obsolete("Usage of this command through chat is not possible anymore. Use TwitchLib.Api.Helix.Streams.CreateStreamMarkerAsync() instead.")]
        public static void Marker(this ITwitchClient client, JoinedChannel channel)
        {
            client.SendMessage(channel, ".marker");
        }

        /// <summary>
        /// Sends command to create a marker using a string.
        /// </summary>
        /// <param name="client">Client reference used to identify extension.</param>
        /// <param name="channel">String representation of the channel to send the marker command to.</param>
        [Obsolete("Usage of this command through chat is not possible anymore. Use TwitchLib.Api.Helix.Streams.CreateStreamMarkerAsync() instead.")]
        public static void Marker(this ITwitchClient client, string channel)
        {
            client.SendMessage(channel, ".marker");
        }
    }
}
