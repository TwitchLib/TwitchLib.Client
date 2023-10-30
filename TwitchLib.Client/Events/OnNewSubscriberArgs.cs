using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing new subscriber event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnNewSubscriberArgs : EventArgs
    {
        /// <summary>
        /// Property representing subscriber object.
        /// </summary>
        public Subscriber Subscriber { get; }
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnPrimePaidSubscriberArgs"/> class.
        /// </summary>
        public OnNewSubscriberArgs(string channel, Subscriber subscriber)
        {
            Channel = channel;
            Subscriber = subscriber;
        }
    }
}
