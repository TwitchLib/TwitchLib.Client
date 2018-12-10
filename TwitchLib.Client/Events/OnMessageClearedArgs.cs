using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing a cleared message event.</summary>
    public class OnMessageClearedArgs : EventArgs
    {
        /// <summary>Channel that had message cleared event.</summary>
        public string Channel;

        /// <summary>Message contents that received clear message</summary>
        public string Message;

        /// <summary>Message ID representing the message that was cleared</summary>
        public string TargetMessageId;
    }
}
