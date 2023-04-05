using System;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnLogArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnLogArgs : EventArgs
    {
        /// <summary>
        ///     The bot username
        /// </summary>
        public string BotUsername { get; set; }
        /// <summary>
        ///     The data
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        ///     The date time
        /// </summary>
        public DateTime DateTime { get; set; }
        public OnLogArgs(string botUsername, string data, DateTime dateTime)
        {
            BotUsername = botUsername;
            Data = data;
            DateTime = dateTime;
        }
    }
}
