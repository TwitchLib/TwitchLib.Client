using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client a message was not sent because the channel is suspended.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnSuspendedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; set; }
    }
}
