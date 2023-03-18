using System;

using TwitchLib.Client.Consts.Internal;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing whisper sent event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
    public class OnWhisperSentArgs : EventArgs
    {
        /// <summary>
        /// Property representing username of bot.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Property representing receiver of the whisper.
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// Property representing sent message contents.
        /// </summary>
        public string Message { get; set; }
    }
}
