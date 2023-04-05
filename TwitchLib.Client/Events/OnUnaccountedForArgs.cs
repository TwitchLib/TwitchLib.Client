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
        public string RawIRC { get; }
        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        public string Location { get; }
        /// <summary>
        ///     Gets or sets the bot username.
        /// </summary>
        public string BotUsername { get; }
        public OnUnaccountedForArgs(string channel, string botUsername, string location, string rawIrc) : base(channel)
        {
            BotUsername = botUsername;
            Location = location;
            RawIRC = rawIrc;
        }
    }
}
