using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client hosting failed because rate limit for hosting was encountered
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnBadHostRateExceededArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; set; }
    }
}
