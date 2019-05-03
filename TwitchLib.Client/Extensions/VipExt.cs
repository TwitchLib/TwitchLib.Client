using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Extensions
{
    /// <summary>Extension for using VIP related commands in TwitchClient</summary>
    public static class VIPExt
    {
        /// <summary>
        /// Creates new VIP user (REQUIRES SCOPE: channel:moderate)
        /// </summary>
        /// <param name="channel">JoinedChannel representation of the channel to send the VIP command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void VIP(this ITwitchClient client, JoinedChannel channel, string viewerToVIP)
        {
            client.SendMessage(channel, $".vip {viewerToVIP}");
        }

        /// <summary>
        /// Creates new VIP user (REQUIRES SCOPE: channel:moderate)
        /// </summary>
        /// <param name="channel">String representation of the channel to send the VIP command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void VIP(this ITwitchClient client, string channel, string viewerToVIP)
        {
            client.SendMessage(channel, $".vip {viewerToVIP}");
        }

        /// <summary>
        /// Removes VIP status from user (REQUIRES SCOPE: channel:moderate)
        /// </summary>
        /// <param name="channel">JoinedChannel representation of the channel to send the unvip command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void UnVIP(this ITwitchClient client, JoinedChannel channel, string viewerToUnVIP)
        {
            client.SendMessage(channel, $".unvip {viewerToUnVIP}");
        }

        /// <summary>
        /// Removes VIP status from user (REQUIRES SCOPE: channel:moderate)
        /// </summary>
        /// <param name="channel">String representation of the channel to send the unvip command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void UnVIP(this ITwitchClient client, string channel, string viewerToUnVIP)
        {
            client.SendMessage(channel, $".unvip {viewerToUnVIP}");
        }

        /// <summary>
        /// Asks Twitch for a list of VIPs in the channel. Listen to OnVIPsReceived event for the response.
        /// </summary>
        /// <param name="channel">JoinedChannel representation of the channel to send the vips command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void GetVIPs(this ITwitchClient client, JoinedChannel channel)
        {
            client.SendMessage(channel, ".vips");
        }

        /// <summary>
        /// Asks Twitch for a list of VIPs in the channel. Listen to OnVIPsReceived event for the response.
        /// </summary>
        /// <param name="channel">String representation of the channel to send the vips command to.</param>
        /// <param name="client">Client reference used to identify extension.</param>
        public static void GetVIPs(this ITwitchClient client, string channel)
        {
            client.SendMessage(channel, ".vips");
        }
    }
}
