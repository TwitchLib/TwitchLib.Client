using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client hosting failed because of a host error
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnBadHostErrorArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; set; }
    }
}
