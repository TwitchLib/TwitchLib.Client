using TwitchLib.Client.Extensions;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a list of moderators received from chat.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnModeratorsReceivedArgs : NoticeEventArgs
    {
        /// <summary>
        /// Property representing an array of moderators.
        /// </summary>
        public string[] Moderators { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnModeratorsReceivedArgs"/> class.
        /// </summary>
        public OnModeratorsReceivedArgs(string channel, string message) : base(channel, message)
        {
            Moderators = string.IsNullOrEmpty(message)
                ? Array.Empty<string>()
                : message.SplitFirst(':').Remainder.ToString().Replace(" ", "").Split(',');
        }
    }
}
