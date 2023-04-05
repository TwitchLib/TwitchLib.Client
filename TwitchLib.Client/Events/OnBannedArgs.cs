using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client a message was not sent because the bot user is banned.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnBannedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; }
        public OnBannedArgs(string channel, string message) : base(channel)
        {
            Message = message;
        }
    }
}
