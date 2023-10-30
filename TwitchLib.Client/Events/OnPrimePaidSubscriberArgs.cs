using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing prime gaming sub -> paid sub event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnPrimePaidSubscriberArgs : EventArgs
    {
        /// <summary>
        /// Property representing prime gaming -> paid subscriber object.
        /// </summary>
        public PrimePaidSubscriber PrimePaidSubscriber { get; }
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnPrimePaidSubscriberArgs"/> class.
        /// </summary>
        public OnPrimePaidSubscriberArgs(string channel, PrimePaidSubscriber primePaidSubscriber)
        {
            Channel = channel;
            PrimePaidSubscriber = primePaidSubscriber;
        }
    }
}
