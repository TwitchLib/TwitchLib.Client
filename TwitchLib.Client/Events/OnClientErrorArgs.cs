using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing community subscription received event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnClientErrorArgs : EventArgs
    {
        /// <summary>
        /// Property representing the kind of error the client encountered
        /// </summary>
        public ClientErrorType ErrorType;

        /// <summary>
        /// Property representing the context of the client error encountered
        /// </summary>
        public Exception Error;
    }
}
