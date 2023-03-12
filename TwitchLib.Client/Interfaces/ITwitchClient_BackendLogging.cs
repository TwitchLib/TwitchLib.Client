using System;

using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to backend and logging
    /// </summary>
    public interface ITwitchClient_BackendLogging
    {
        /// <summary>
        ///     Occurs when [on unaccounted for].
        /// </summary>
        event EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
        /// <summary>
        ///     Occurs when [on log].
        /// </summary>
        event EventHandler<OnLogArgs> OnLog;
        /// <summary>
        ///     Occurs when [on send receive data].
        /// </summary>
        event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        event EventHandler<OnErrorEventArgs> OnError;
    }
}
