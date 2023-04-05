using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using TwitchLib.Client.Extensions.Internal;

namespace TwitchLib.Client.Parsers {
    /// <summary>
    ///     <see langword="class"/> to parse raw irc-messages as <see cref="JObject"/>s
    /// </summary>
    public static class IrcJsonParser {
        /// <summary>
        ///     parses the given <paramref name="irc"/>-message as <see cref="JObject"/>
        /// </summary>
        /// <param name="irc">
        ///     raw irc-message as <see langword="string"/>
        /// </param>
        /// <returns>
        ///     for easier access, almost everything is a <see cref="JObject"/> having <see cref="JObject"/>s as <see cref="JProperty"/>s
        ///     <br></br>
        ///     <see cref="JObject.Properties()"/> are <see cref="IJEnumerable{T}"/> and can be accessed by their names, too, eg.:
        ///     <code>
        ///         ircObject["command"]
        ///     </code>
        ///     <code>
        ///         ircObject.Value&lt;string&gt;("command")
        ///     </code>
        ///     <br></br>
        ///     emote-indices are <see cref="JArray"/>s of <see cref="JObject"/>s having <see langword="from"/> and <see langword="to"/> as <see cref="JProperty"/>s
        ///     <code>
        ///         ircObject.Value&lt;JObject&gt;("tags").Value&lt;JObject&gt;("emotes").Value&lt;JArray&gt;("2")
        ///     </code>
        ///     <br></br>
        ///     emotes has a property called "orderedIndexObjects" that contains all emote-indices ordered ascending by "from"-index
        ///     <br></br>
        ///     emote-sets is an <see cref="JArray"/> of <see cref="JValue"/>s; each <see cref="JValue"/> represents one ID of an emote-set
        ///     <br></br>
        ///     tags, that are present within the raw irc, but have no value return <see langword="null"/>
        ///     <br></br>
        ///     example:
        ///     <code>{
        ///         "command": "irc command",
        ///         "user": "user-login",
        ///         "channel": "channel-name",
        ///         "message": "the message",
        ///         "irc": "the given raw irc-message",
        ///         "tags": {
        ///             "badge-info": {
        ///                 "subscriber": "22"
        ///             },
        ///             "badges": {
        ///                 "subscriber": "18",
        ///                 "bits": "1000"
        ///             },
        ///             "emotes": {
        ///                 "id of an emote, eg 1": [ {
        ///                         "id": "id of an emote, eg 1",
        ///                         "name": "name of an emote, eg '&lt;3'",
        ///                         "from": "4",
        ///                         "to": "5"
        ///                     }
        ///                 ],
        ///                 "id of an emote, eg 2": [ {
        ///                         "id": "id of an emote, eg 2",
        ///                         "name": "name of an emote, eg ':)'",
        ///                         "from": "1",
        ///                         "to": "2"
        ///                     }, {
        ///                         "id": "id of an emote, eg 2",
        ///                         "name": "name of an emote, eg ':)'",
        ///                         "from": "7",
        ///                         "to": "8"
        ///                     }
        ///                 ],
        ///                 "orderedIndexObjects": [ {
        ///                         "id": "id of an emote, eg 2",
        ///                         "name": "name of an emote, eg ':)'",
        ///                         "from": "1",
        ///                         "to": "2"
        ///                     }, {
        ///                         "id": "id of an emote, eg 1",
        ///                         "name": "name of an emote, eg '&lt;3'",
        ///                         "from": "4",
        ///                         "to": "5"
        ///                     }, {
        ///                         "id": "id of an emote, eg 2"
        ///                         "name": "name of an emote, eg ':)'",
        ///                         "from": "7",
        ///                         "to": "8"
        ///                     }
        ///                 ]
        ///             },
        ///             "emote-sets": [
        ///                 "id of an emote-set, eg 1",
        ///                 "id of an emote-set, eg 2"
        ///             ]
        ///             "some tag name": "some tag value"
        ///         }
        ///     }
        ///     </code>
        /// </returns>
        public static JObject Parse(string irc) {
            JObject ircObject = new JObject();
            bool hasTags = irc.StartsWith("@");
            bool isPingOrPong = irc.StartsWith("PING") || irc.StartsWith("PONG");
            string[] messageParts = GetMessagePartsFromRawIRC(irc);
            string command = GetIRCCommand(messageParts, hasTags, isPingOrPong);
            if (!command.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(command), new JValue(command));
            }
            string? user = GetUser(messageParts, command, hasTags, isPingOrPong);
            if (user != null && !user.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(user), new JValue(user));
            }
            string? channel = GetChannel(messageParts, hasTags, isPingOrPong);
            if (channel != null && !channel.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(channel), new JValue(channel));
            }
            string? message = GetMessage(messageParts, hasTags);
            if (message != null && !message.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(message), new JValue(message));
            }
            if (hasTags) {
                // substring, to get rid of @-sign at the beginning
                string tagPart = messageParts[0].Substring(1);
                JObject tagObject = GetTags(tagPart, message);
                ircObject.Add("tags", tagObject);
            }

            ircObject.Add(nameof(irc), irc);
            return ircObject;
        }

        /// <summary>
        ///     handles the tags of an irc-message
        /// </summary>
        /// <param name="tagPart">
        ///     that part of an raw irc-message, that starts with @-sign
        ///     <br></br>
        ///     @-sign should be removed before calling this method!
        /// </param>
        /// <param name="message">
        ///     the users message
        /// </param>
        /// <returns>
        ///     <see cref="JObject"/>
        /// </returns>
        private static JObject GetTags(string tagPart, string? message) {
            JObject tagObject = new JObject();
            string[] tags = tagPart.Split(';');
            foreach (string tag in tags) {
                string[] keyValue = tag.Split('=');
                string key = keyValue[0];
                string value = keyValue[1];
                JToken? tagValue = null;
                if (!value.IsNullOrEmptyOrWhitespace()) {
                    tagValue = GetTagValue(key, value, message);
                }
                tagObject.Add(key, tagValue);
            }
            return tagObject;
        }

        /// <summary>
        ///     handles a tag and its value
        ///     <code>
        ///         emotes=1:0-1,8-9/555555584:4-5
        ///     </code>
        /// </summary>
        /// <param name="key">
        ///     eg
        ///     <code>
        ///         emotes
        ///     </code>
        /// </param>
        /// <param name="value">
        ///     eg
        ///     <code>
        ///         1:0-1,8-9/555555584:4-5
        ///     </code>
        /// </param>
        /// <param name="message">
        ///     eg
        ///     <code>
        ///         :) &lt;3 :)
        ///     </code>
        /// </param>
        /// <returns></returns>
        private static JToken GetTagValue(string key, string value, string? message) {
            if (String.Equals("emotes", key)) {
                return GetEmotes(value, message);
            } else if (String.Equals("emote-sets", key)) {
                return GetEmoteSets(value);
            } else if (key.StartsWith("badge")) {
                return GetBadgeStuff(value);
            }
            return new JValue(value);
        }

        private static JToken GetEmoteSets(string value) {
            JArray emoteSets = new JArray();
            string[] emoteSetIds = value.Split(',');
            foreach (string emoteSetId in emoteSetIds) {
                emoteSets.Add(new JValue(emoteSetId));
            }
            return emoteSets;
        }

        private static string? GetMessage(string[] messageParts, bool hasTags) {
            if (hasTags) {
                if (messageParts.Length < 3) {
                    return null;
                }
                return messageParts[2];
            }
            if (messageParts.Length < 2) {
                return null;
            }
            return messageParts[1];
        }

        private static string? GetChannel(string[] messageParts, bool hasTags, bool isPingOrPong) {
            if (isPingOrPong) {
                return null;
            }
            string meta;
            if (hasTags) {
                meta = messageParts[1];
            } else {
                meta = messageParts[0];
            }
            if (!meta.Contains("#")) {
                return null;
            }
            return meta.Substring(meta.IndexOf("#") + 1);
        }

        private static string? GetUser(string[] messageParts, string command, bool hasTags, bool isPingOrPong) {
            if (isPingOrPong) {
                return null;
            }
            string meta;
            if (hasTags) {
                meta = messageParts[1];
            } else if (Int32.TryParse(command, out _)) {
                meta = messageParts[0];
                return meta.Split(' ')[2];
            } else {
                meta = messageParts[0];
            }
            string potentialUser = meta.Split(' ')[0];
            if (!potentialUser.Contains("@")) {
                return null;
            }
            return potentialUser.Split('@')[1].Split('.')[0];
        }

        private static string GetIRCCommand(string[] messageParts, bool hasTags, bool isPingOrPong) {
            if (isPingOrPong) {
                return messageParts[0];
            }
            string meta;
            if (hasTags) {
                meta = messageParts[1];
            } else {
                meta = messageParts[0];
            }
            return meta.Split(' ')[1];
        }

        /// <summary>
        ///     handles the emotes-tags value
        /// </summary>
        /// <param name="value">
        ///     tag-value
        ///     <br></br>
        ///     for
        ///     <code>emotes=1:0-1,8-9/555555584:4-5</code>
        ///     it would be
        ///     <code>1:0-1,8-9/555555584:4-5</code>
        /// </param>
        /// <param name="message">
        ///     the users message
        /// </param>
        /// <returns>
        ///     <see cref="JToken"/>
        /// </returns>
        private static JToken GetEmotes(string value, string? message) {
            //
            // collect all index objects to easily replace emotes within message
            IList<JToken> indexObjects = new List<JToken>();

            JObject emotesObject = new JObject();
            string[] entries = value.Split('/');
            foreach (string entry in entries) {
                string[] idAndIndices = entry.Split(':');
                string id = idAndIndices[0];
                JArray indiceArray = new JArray();
                string[] indices = idAndIndices[1].Split(',');
                foreach (string index in indices) {
                    JObject indexObject = new JObject();
                    string[] fromTo = index.Split('-');
                    bool fromParsed = Int32.TryParse(fromTo[0], out int fromIndex);
                    bool toParsed = Int32.TryParse(fromTo[1], out int toIndex);
                    // indexObject also gets the emote-id,
                    // to easily replace emotes within message
                    indexObject.Add("id", id);
                    if (fromParsed && toParsed && message != null) {
                        int emoteNameLength = toIndex - fromIndex + 1;
                        string name = message.Substring(fromIndex, emoteNameLength);
                        indexObject.Add(nameof(name), name);
                        // if it got parsed,
                        // use parsed values
                        indexObject.Add("from", fromIndex);
                        indexObject.Add("to", toIndex);
                    } else {
                        // if its not parsed,
                        // use raw values
                        indexObject.Add("from", fromTo[0]);
                        indexObject.Add("to", fromTo[1]);
                    }
                    indexObjects.Add(indexObject);
                    indiceArray.Add(indexObject);
                }
                emotesObject.Add(id, indiceArray);
            }
            // order indexObjects by from-index to easily replace emotes within message
            IOrderedEnumerable<JToken> orderedIndexObjects = indexObjects.OrderBy(indexObject => indexObject["from"]);
            emotesObject.Add(nameof(orderedIndexObjects), new JArray(orderedIndexObjects));
            return emotesObject;
        }

        private static JToken GetBadgeStuff(string raw) {
            string[] entries = raw.Split(',');
            JObject badgeObject = new JObject();
            foreach (string entry in entries) {
                if (entry.IsNullOrEmptyOrWhitespace()) {
                    continue;
                }
                string[] keyValuePair = entry.Split('/');
                badgeObject.Add(keyValuePair[0], new JValue(keyValuePair[1]));
            }
            return badgeObject;
        }

        private static string[] GetMessagePartsFromRawIRC(string rawIrc) {
            string[] messageParts = rawIrc.Split(new string[] { " :" }, StringSplitOptions.None);
            if (messageParts.Length > 3) {
                messageParts = new string[]
                {
                    messageParts[0],
                    messageParts[1],
                    messageParts[2] = String.Join(" :", messageParts, 2, messageParts.Length-2),
                };
            }
            return messageParts;
        }
    }
}
