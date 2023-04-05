using System;

using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing client connection error event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnConnectionErrorArgs : EventArgs
    {
        /// <summary>
        ///     The error
        /// </summary>
        public ErrorEvent? Error { get; set; }
        /// <summary>
        ///     Username of the bot that suffered connection error.
        /// </summary>
        public string? BotUsername { get; set; }
    }
}
