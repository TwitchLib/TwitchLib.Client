using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing the client left a channel event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnLeftChannelArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     The username of the bot that left the channel.
        /// </summary>
        public string BotUsername { get; }
        public OnLeftChannelArgs(string channel, string botUsername) : base(channel)
        {
            BotUsername = botUsername;
        }
    }
}
