using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing whisper command received event.</summary>
    public class OnWhisperCommandReceivedArgs : EventArgs
    {
        /// <summary>Property representing received command.</summary>
        public WhisperCommand Command;
    }
}
