using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client that the user is banned to chat bcs of an already banned alias with the same Email.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnBannedEmailAliasArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string? Message { get; set; }
    }
}