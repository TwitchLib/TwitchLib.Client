﻿using System;
using System.Collections.Generic;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on connected event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnConnectedArgs : EventArgs
    {
        /// <summary>
        ///     Property representing bot username.
        /// </summary>
        public string BotUsername { get; }
        /// <summary>
        ///     <see cref="IEnumerable{T}"/> with channel-names to join when connected
        ///     <br></br>
        ///     <br></br>
        ///     the presence of this <see cref="IEnumerable{T}"/> <b>does not</b> indicate they are already joined!
        /// </summary>
        public IEnumerable<string> AutoJoinChannels { get; set; } = new List<string>();
        public OnConnectedArgs(string botUsername, IEnumerable<string> autoJoinChannels)
        {
            BotUsername = botUsername;
            AutoJoinChannels = autoJoinChannels;
        }
    }
}
