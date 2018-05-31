namespace TwitchLib.Client.Exceptions
{
    public class FailureToReceiveJoinConfirmationException
    {
        /// <summary>Exception representing failure of client to receive JOIN confirmation.</summary>
        public string Channel { get; protected set; }
        /// <summary>Extra details regarding this exception (not always set)</summary>
        public string Details { get; protected set; }

        /// <summary>Exception construtor.</summary>
        public FailureToReceiveJoinConfirmationException(string channel, string details = null)
        {
            Channel = channel;
            Details = details;
        }
    }
}
