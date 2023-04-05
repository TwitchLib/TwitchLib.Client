using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing client disconnect event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnDisconnectedArgs : EventArgs
    {
        /// <summary>
        ///     Username of the bot that was disconnected.
        /// </summary>
        public string? BotUsername;
    }
}
