﻿using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnRaidNotificationArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OnRaidNotificationArgs : EventArgs
    {
        /// <summary>
        /// The raid notificaiton
        /// </summary>
        public RaidNotification RaidNotificaiton;
        /// <summary>
        /// The channel
        /// </summary>
        public string Channel;
    }
}
