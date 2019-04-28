using System.Collections.Generic;

using TwitchLib.Client.Models.Builders;

namespace TwitchLib.Client.Models
{
    /// <summary>Object representing emote set from a chat message.</summary>
    public class EmoteSet
    {
        /// <summary>List containing all emotes in the message.</summary>
        public List<Emote> Emotes { get; }

        /// <summary>The raw emote set string obtained from Twitch, for legacy purposes.</summary>
        public string RawEmoteSetString { get; }

        /// <summary>Constructor for ChatEmoteSet object.</summary>
        /// <param name="emoteSetData"></param>
        /// <param name="message"></param>
        public EmoteSet(string emoteSetData, string message)
        {
            Emotes = new List<Emote>();
            RawEmoteSetString = emoteSetData;
            if (string.IsNullOrEmpty(emoteSetData)
               || string.IsNullOrEmpty(message))
            {
                return;
            }

            if (emoteSetData.Contains("/"))
            {
                // Message contains multiple different emotes, first parse by unique emotes: 28087:15-21/25:5-9,28-32
                foreach (var emoteData in emoteSetData.Split('/'))
                {
                    var emoteId = int.Parse(emoteData.Split(':')[0]);
                    if (emoteData.Contains(","))
                    {
                        // Multiple copies of a single emote: 25:5-9,28-32
                        foreach (var emote in emoteData.Replace($"{emoteId}:", "").Split(','))
                            AddEmote(emote, emoteId, message);
                    }
                    else
                    {
                        // Single copy of single emote: 25:5-9/28087:16-22
                        AddEmote(emoteData, emoteId, message, true);
                    }
                }
            }
            else
            {
                var emoteId = int.Parse(emoteSetData.Split(':')[0]);
                // Message contains a single, or multiple of the same emote
                if (emoteSetData.Contains(","))
                {
                    // Multiple copies of a single emote: 25:5-9,28-32
                    foreach (var emote in emoteSetData.Replace($"{emoteId}:", "").Split(','))
                        AddEmote(emote, emoteId, message);
                }
                else
                {
                    // Single copy of single emote: 25:5-9
                    AddEmote(emoteSetData, emoteId, message, true);
                }
            }
        }

        private void AddEmote(string emoteData, int emoteId, string message, bool single = false)
        {
            int startIndex = -1;
            int endIndex = -1;

            if (single)
            {
                startIndex = int.Parse(emoteData.Split(':')[1].Split('-')[0]);
                endIndex = int.Parse(emoteData.Split(':')[1].Split('-')[1]);
            }
            else
            {
                startIndex = int.Parse(emoteData.Split('-')[0]);
                endIndex = int.Parse(emoteData.Split('-')[1]);
            }

            string name = message.Substring(startIndex, (endIndex - startIndex) + 1);

            EmoteBuilder emoteBuilder = EmoteBuilder.Create()
                                                    .WithId(emoteId)
                                                    .WithName(name)
                                                    .WithStartIndex(startIndex)
                                                    .WithEndIndex(endIndex);

            Emotes.Add(emoteBuilder.Build());
        }

        /// <summary>
        /// Object representing an emote in an EmoteSet in a chat message.
        /// </summary>
    }
}
