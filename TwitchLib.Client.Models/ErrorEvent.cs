namespace TwitchLib.Client.Models
{
    /// <summary>Class representing the error that the websocket encountered.</summary>
    public class ErrorEvent
    {
        /// <summary>Message pertaining to the error.</summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEvent"/> class.
        /// </summary>
        public ErrorEvent(string message)
        {
            Message = message;
        }
    }
}
