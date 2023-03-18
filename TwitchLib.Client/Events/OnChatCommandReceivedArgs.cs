using System;

using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing chat command received event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnChatCommandReceivedArgs : EventArgs
    {
        /// <summary>
        ///     The command
        /// </summary>
        public ChatCommand Command { get; set; }
    }
}
