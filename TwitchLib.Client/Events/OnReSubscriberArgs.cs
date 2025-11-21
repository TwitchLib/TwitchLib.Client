using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing resubscriber event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnReSubscriberArgs : EventArgs
    {
        /// <summary>
        /// Property representing resubscriber object.
        /// </summary>
        public ReSubscriber ReSubscriber { get; }
        /// <summary>
        /// Property representing the Twitch channel this event fired from.
        /// </summary>
        public string Channel {  get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnReSubscriberArgs"/> class.
        /// </summary>
        public OnReSubscriberArgs(string channel, ReSubscriber reSubscriber)
        {
            Channel = channel;
            ReSubscriber = reSubscriber;
        }
    }
}
