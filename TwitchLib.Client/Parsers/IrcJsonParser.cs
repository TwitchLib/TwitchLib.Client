using System;

using Newtonsoft.Json.Linq;

using TwitchLib.Client.Extensions.Internal;

namespace TwitchLib.Client.Parsers {
    internal class IrcJsonParser {
        /// <summary>
        ///     parses the given <paramref name="irc"/>-message as <see cref="JObject"/>
        /// </summary>
        /// <param name="irc">
        ///     raw irc-message as <see langword="string"/>
        /// </param>
        /// <returns>
        ///     for easier access, almost everything is a <see cref="JObject"/> having <see cref="JObject"/>s as <see cref="JProperty"/>s
        ///     <br></br>
        ///     <br></br>
        ///     <see cref="JObject.Properties()"/> are <see cref="IJEnumerable{T}"/> and can be accessed by their names, too, eg.:
        ///     <code>
        ///         ircObject["command"]
        ///     </code>
        ///     <code>
        ///         ircObject.Value&lt;string&gt;("command")
        ///     </code>
        ///     <br></br>
        ///     <br></br>
        ///     only emote-indices are <see cref="JArray"/>s of <see cref="JObject"/>s having <see langword="from"/> and <see langword="to"/> as <see cref="JProperty"/>s
        ///     <code>
        ///         ircObject.Value&lt;JObject&gt;("tags").Value&lt;JObject&gt;("emotes").Value&lt;JArray&gt;("2")
        ///     </code>
        ///     <br></br>
        ///     <br></br>
        ///     tags, that are present within the raw irc, but have no value return an empty <see langword="string"/>
        ///     <br></br>
        ///     <br></br>
        ///     example:
        ///     <code>
        ///     {
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
        ///                 "id of an emote, eg 1": [
        ///                     {
        ///                         "from": "1",
        ///                         "to": "2"
        ///                     }
        ///                 ],
        ///                 "id of an emote, eg 2": [
        ///                     {
        ///                         "from": "4",
        ///                         "to": "5"
        ///                     },
        ///                     {
        ///                         "from": "7",
        ///                         "to": "8"
        ///                     }
        ///                 ]
        ///             },
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
            string user = GetUser(messageParts, command, hasTags, isPingOrPong);
            if (!user.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(user), new JValue(user));
            }
            string channel = GetChannel(messageParts, hasTags, isPingOrPong);
            if (!channel.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(channel), new JValue(channel));
            }
            string message = GetMessage(messageParts, hasTags, isPingOrPong);
            if (!message.IsNullOrEmptyOrWhitespace()) {
                ircObject.Add(nameof(message), new JValue(message));
            }
            if (hasTags) {
                string tagPart = messageParts[0].Substring(1);

                JObject tagObject = new JObject();

                string[] tags = tagPart.Split(';');
                foreach (string tag in tags) {
                    string[] keyValue = tag.Split('=');

                    string key = keyValue[0];
                    string value = keyValue[1];
                    JToken tagValue = new JValue("");
                    if (String.Equals("emotes", key)) {
                        if (!value.IsNullOrEmptyOrWhitespace()) {
                            tagValue = GetEmotes(value);
                        }
                    } else if (key.StartsWith("badge")) {
                        if (!value.IsNullOrEmptyOrWhitespace()) {
                            tagValue = GetBadgeStuff(value);
                        }
                    } else {
                        tagValue = new JValue(value);
                    }

                    tagObject.Add(key, tagValue);
                }

                ircObject.Add("tags", tagObject);
            }

            ircObject.Add(nameof(irc), irc);
            return ircObject;
        }

        private static string GetMessage(string[] messageParts, bool hasTags, bool isPingOrPong) {
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

        private static string GetChannel(string[] messageParts, bool hasTags, bool isPingOrPong) {
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

        private static string GetUser(string[] messageParts, string command, bool hasTags, bool isPingOrPong) {
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

        private static JToken GetEmotes(string value) {
            //emotes
            //=
            //1:0-1,8-9/555555584:4-5;
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
                    indexObject.Add("from", fromTo[0]);
                    indexObject.Add("to", fromTo[1]);
                    indiceArray.Add(indexObject);
                }
                emotesObject.Add(id, indiceArray);
            }
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
