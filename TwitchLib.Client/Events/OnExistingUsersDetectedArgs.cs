namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing existing user(s) detected event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnExistingUsersDetectedArgs : EventArgs
    {
        /// <summary>
        /// Property representing string list of existing users.
        /// </summary>
        public List<string> Users { get; }
        /// <summary>
        /// Property representing channel bot is connected to.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnExistingUsersDetectedArgs"/> class.
        /// </summary>
        public OnExistingUsersDetectedArgs(string channel, List<string> users)
        {
            Channel = channel;
            Users = users;
        }
    }
}
