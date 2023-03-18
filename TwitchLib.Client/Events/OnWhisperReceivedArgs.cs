using System;

using TwitchLib.Client.Consts.Internal;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Class OnWhisperReceivedArgs.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
    public class OnWhisperReceivedArgs : EventArgs
    {
        /// <summary>
        ///     The whisper message
        /// </summary>
        public WhisperMessage WhisperMessage { get; set; }
    }
}
