using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <inheritdoc />
    /// <summary>Exception thrown when attempting to perform an actino that is only available when the client is connected.</summary>
    public class ClientNotConnectedException : Exception
    {
        /// <inheritdoc />
        /// <summary>Exception constructor</summary>
        public ClientNotConnectedException(string description)
            : base(description)
        {
        }
    }
}
