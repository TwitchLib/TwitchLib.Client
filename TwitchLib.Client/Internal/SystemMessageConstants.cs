namespace TwitchLib.Client.Internal
{

    internal static class SystemMessageConstants
    {
        public const string ObsoleteWhisperMessage = "Whispers are no longer part of IRC."
            + "\r\n\r\nhttps://dev.twitch.tv/docs/api/reference/#send-whisper"
            + "\r\nTwitchLib.Api.Helix.Whispers"
            + "\r\n\r\nhttps://dev.twitch.tv/docs/pubsub/#topics"
            + "\r\nTwitchLib.PubSub";
        public const string ObsoleteWhisperMessageParameter = ObsoleteWhisperMessage
            + "\r\nThe parameter 'whisperCommandIdentifier' is going to be removed within one of the next releases."
            + "\r\nYou could use named parameters to leave out the parameter 'whisperCommandIdentifier'.";
    }
}
