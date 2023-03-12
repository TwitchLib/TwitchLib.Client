using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Whisper
    {
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
            if (!IsInitialized) HandleNotInitialized();
            if (dryRun) return;
        }
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public void AddWhisperCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
        }
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        public void RemoveWhisperCommandIdentifier(char identifier)
        {
            if (!IsInitialized) HandleNotInitialized();
        }
    }
}
