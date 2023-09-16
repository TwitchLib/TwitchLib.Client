using TwitchLib.Client.Models.Extensions;

namespace TwitchLib.Client.Models.Extractors
{
    public static class EmoteExtractor
    {
        public static List<Emote> Extract(string? rawEmoteSetString, string message)
        {
            var emotes = new List<Emote>();
            if (string.IsNullOrEmpty(rawEmoteSetString) || string.IsNullOrEmpty(message))
                return emotes;

            // 25:5-9,28-32/28087:15-21 => 25:5-9,28-32  28087:15-21
#pragma warning disable CS8604 // Possible null reference argument. false positiv in NS 2.0
            foreach (var emoteData in new SpanSliceEnumerator(rawEmoteSetString, '/'))
#pragma warning restore CS8604 // Possible null reference argument.
            {
                var index = emoteData.IndexOf(':');
                var emoteId = emoteData.Slice(0, index).ToString();
                // 25:5-9,28-32 => 5-9  28-32
                foreach (var emote in new SpanSliceEnumerator(emoteData.Slice(index + 1), ','))
                {
                    index = emote.IndexOf('-');
                    var startSlice = emote.Slice(0, index);
                    var endSlice = emote.Slice(index + 1);
#if NETSTANDARD2_0
                    var start = int.Parse(startSlice.ToString());
                    var end = int.Parse(endSlice.ToString());
#else
                    var start = int.Parse(startSlice);
                    var end = int.Parse(endSlice);
#endif
                    emotes.Add(new(emoteId, message.Substring(start, end - start + 1), start, end));
                }
            }
            return emotes;
        }
    }
}
