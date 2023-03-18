using System;

using TwitchLib.Client.Consts.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Extensions;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Whisper
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public WhisperMessage PreviousWhisper { get; private set; }
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public event EventHandler<OnWhisperSentArgs> OnWhisperSent;
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public void SendWhisper(string receiver, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Whisper));
            if (!IsInitialized) HandleNotInitialized();
            if (dryRun) return;
        }
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public void AddWhisperCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Whisper));
            if (!IsInitialized) HandleNotInitialized();
        }
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public void RemoveWhisperCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Whisper));
            if (!IsInitialized) HandleNotInitialized();
        }
    }
}
