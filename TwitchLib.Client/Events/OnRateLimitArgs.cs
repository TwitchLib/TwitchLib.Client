using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client that a rate limit has been hit.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnRateLimitArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string? Message { get; set; }
    }
}