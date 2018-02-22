using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing resubscriber event.</summary>
    public class OnReSubscriberArgs : EventArgs
    {
        /// <summary>Property representing resubscriber object.</summary>
        public ReSubscriber ReSubscriber;
    }
}
