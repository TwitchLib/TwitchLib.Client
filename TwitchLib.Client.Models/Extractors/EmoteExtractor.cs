using System;
using System.Collections.Generic;
using TwitchLib.Client.Models.Extensions;

namespace TwitchLib.Client.Models.Extractors
{
    public static class EmoteExtractor
    {
        public static List<Emote> Extract(string rawEmoteSetString, string message)
        {
            var emotes = new List<Emote>();
            if (string.IsNullOrEmpty(rawEmoteSetString) || string.IsNullOrEmpty(message))
                return emotes;

            // 25:5-9,28-32/28087:15-21 => 25:5-9,28-32  28087:15-21
            foreach (var emoteData in new SpanSliceEnumerator(rawEmoteSetString, '/'))
            {
                var index = emoteData.IndexOf(':');
                var emoteId = emoteData.Slice(0, index).ToString();
                // 25:5-9,28-32 => 5-9  28-32
                foreach (var emote in new SpanSliceEnumerator(emoteData.Slice(index + 1), ','))
                {
                    index = emote.IndexOf('-');
                    var start = int.Parse(emote.Slice(0, index).ToString());
                    var end = int.Parse(emote.Slice(index + 1).ToString());
                    emotes.Add(new(emoteId, message.Substring(start, end - start + 1), start, end));
                }
            }
            return emotes;
        }
    }
}
