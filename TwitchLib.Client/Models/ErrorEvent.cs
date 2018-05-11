using System;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing the error that the websocket encountered.</summary>
    public class ErrorEvent
    {
        /// <summary>Exception object representing the error.</summary>
        public Exception Exception { get; internal set; }
        /// <summary>Message pertaining to the error.</summary>
        public string Message { get; internal set; }        
    }
}
