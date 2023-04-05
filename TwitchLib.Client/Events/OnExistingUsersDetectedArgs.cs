using System.Collections.Generic;

using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing existing user(s) detected event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnExistingUsersDetectedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing string list of existing users.
        /// </summary>
        public IEnumerable<string> Users { get; } = new List<string>();
        public OnExistingUsersDetectedArgs(string channel, IEnumerable<string> users) : base(channel)
        {
            Users = users;
        }
    }
}
