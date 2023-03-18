using System.Collections.Generic;

using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a list of moderators received from chat.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnModeratorsReceivedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the list of moderators.
        /// </summary>
        public ICollection<string> Moderators { get; set; }
    }
}
