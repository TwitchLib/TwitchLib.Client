using System;

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on channel state changed event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnSendReceiveDataArgs : EventArgs
    {
        /// <summary>
        ///     Property representing the direction of the data.
        /// </summary>
        public SendReceiveDirection Direction { get; }
        /// <summary>
        ///     Property representing the data that was either sent or received.
        /// </summary>
        public string Data { get; }
        public OnSendReceiveDataArgs(SendReceiveDirection direction, string data)
        {
            Direction = direction;
            Data = data;
        }
    }
}
