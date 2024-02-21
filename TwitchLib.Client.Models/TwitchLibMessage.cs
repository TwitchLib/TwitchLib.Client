using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Models
{
    /// <summary>Class represents Message.</summary>
    public abstract class TwitchLibMessage : IHexColorProperty
    {
        /// <summary>List of key-value pair badges.</summary>
        public List<KeyValuePair<string, string>> Badges { get; protected set; } = default!;

        /// <summary>Twitch username of the bot that received the message.</summary>
        public string BotUsername { get; protected set; } = default!;

        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public string HexColor { get; protected set; } = default!;

        /// <summary>Case-sensitive username of sender of chat message.</summary>
        public string DisplayName { get; protected set; } = default!;

        /// <summary>Emote Ids that exist in message.</summary>
        public EmoteSet EmoteSet { get; protected set; } = default!;

        /// <summary>Twitch-unique integer assigned on per account basis.</summary>
        public string UserId { get; protected set; } = default!;

        /// <summary>Username of sender of chat message.</summary>
        public string Username { get; protected set; } = default!;

        public UserDetail UserDetail { get; protected set; }

        /// <summary>User type can be viewer, moderator, global mod, admin, or staff</summary>
        public UserType UserType { get; protected set; }
        
        /// <summary>Raw IRC-style text received from Twitch.</summary>
        public string RawIrcMessage { get; protected set; } = default!;

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; protected set; }
    }
}
