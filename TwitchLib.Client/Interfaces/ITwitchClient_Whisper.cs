﻿using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to Whispers
    /// </summary>
    public interface ITwitchClient_Whisper
    {
        /// <summary>
        ///     Gets the previous whisper.
        /// </summary>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        WhisperMessage PreviousWhisper { get; }
        /// <summary>
        ///     Occurs when [on whisper command received].
        /// </summary>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;
        /// <summary>
        ///     Occurs when [on whisper received].
        /// </summary>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;
        /// <summary>
        ///     Occurs when [on whisper sent].
        /// </summary>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        event EventHandler<OnWhisperSentArgs> OnWhisperSent;
        /// <summary>
        ///     Occurs when [on whisper throttled].
        /// </summary>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;
        /// <summary>
        ///     Adds the whisper command identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        void AddWhisperCommandIdentifier(char identifier);
        /// <summary>
        ///     Removes the whisper command identifier.
        /// </summary>
        /// <param name="identifier">
        ///     <see langword="char"/>
        /// </param>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        void RemoveWhisperCommandIdentifier(char identifier);

        /// <summary>
        /// Sends the whisper.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        void SendWhisper(string receiver, string message, bool dryRun = false);
    }
}