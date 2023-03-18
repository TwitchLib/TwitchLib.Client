using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnUnaccountedForArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUnaccountedForArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Gets or sets the raw irc.
        /// </summary>
        public string RawIRC { get; set; }
        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        ///     Gets or sets the bot username.
        /// </summary>
        public string BotUsername { get; set; }
    }
}
