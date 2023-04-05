using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Exceptions;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnFailureToReceiveJoinConfirmationArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnFailureToReceiveJoinConfirmationArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     The exception
        /// </summary>
        public FailureToReceiveJoinConfirmationException Exception { get; }
        public OnFailureToReceiveJoinConfirmationArgs(string channel, FailureToReceiveJoinConfirmationException exception) : base(channel)
        {
            Exception = exception;
        }
    }
}
