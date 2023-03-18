using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on channel joined event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnJoinedChannelArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing bot username.
        /// </summary>
        public string BotUsername { get; set; }
    }
}
