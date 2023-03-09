namespace TwitchLib.Client.Internal
{

    internal static class SystemMessageConstants
    {
        public const string ObsoleteWhisperMessage = "Whispers are no longer part of IRC.";
        public const string ObsoleteWhisperMessageParameter = ObsoleteWhisperMessage
            + "\r\nThe parameter 'whisperCommandIdentifier' is going to be removed within one of the next releases."
            + "\r\nYou could use named parameters to leave out the parameter 'whisperCommandIdentifier'.";
    }
}
