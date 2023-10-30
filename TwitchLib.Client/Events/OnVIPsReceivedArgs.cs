using TwitchLib.Client.Extensions;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a list of VIPs received from chat.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnVIPsReceivedArgs : NoticeEventArgs
    {
        /// <summary>
        /// Property representing an array of VIPs.
        /// </summary>
        public string[] VIPs { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnVIPsReceivedArgs"/> class.
        /// </summary>
        public OnVIPsReceivedArgs(string channel, string message) : base(channel, message)
        {
            // TODO: Make it less allocatey
            VIPs = string.IsNullOrEmpty(message)
                ? Array.Empty<string>()
                : message.SplitFirst(':').Remainder.ToString().Replace(" ", "").Replace(".", "").Split(',');
        }
    }
}
