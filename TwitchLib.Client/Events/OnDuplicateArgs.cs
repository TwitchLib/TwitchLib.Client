using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client that duplicate messages are not allowed.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnDuplicateArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; set; }
    }
}
