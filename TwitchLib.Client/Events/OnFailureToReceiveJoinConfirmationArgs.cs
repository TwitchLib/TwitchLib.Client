using TwitchLib.Client.Exceptions;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Class OnFailureToReceiveJoinConfirmationArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OnFailureToReceiveJoinConfirmationArgs : EventArgs
    {
        /// <summary>
        /// The exception
        /// </summary>
        public FailureToReceiveJoinConfirmationException Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnFailureToReceiveJoinConfirmationArgs"/> class.
        /// </summary>
        public OnFailureToReceiveJoinConfirmationArgs(FailureToReceiveJoinConfirmationException exception)
        {
            Exception = exception;
        }
    }
}
