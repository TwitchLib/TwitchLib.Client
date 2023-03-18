using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing the detected hosted channel.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnNowHostingArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing channel that is being hosted.
        /// </summary>
        public string HostedChannel { get; set; }
    }
}
