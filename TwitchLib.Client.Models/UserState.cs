using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing state of a specific user.</summary>
    public class UserState : IHexColorProperty
    {
        /// <summary>Properrty representing the chat badges a specific user has.</summary>
        public List<KeyValuePair<string, string>> Badges { get; } = new List<KeyValuePair<string, string>>();

        /// <summary>Metadata associated badgest</summary>
        public List<KeyValuePair<string, string>> BadgeInfo { get; } = new List<KeyValuePair<string, string>>();

        /// <summary>Property representing channel.</summary>
        public string Channel { get; }

        /// <summary>Properrty representing HEX user's name.</summary>
        public string HexColor { get; } = default!;

        /// <summary>Property representing user's display name.</summary>
        public string DisplayName { get; } = default!;

        /// <summary>Property representing emote sets available to user.</summary>
        public string EmoteSet { get; } = default!;

        /// <summary>Property representing the user's Id.</summary>
        public string Id { get; } = default!;

        /// <summary>Property representing Turbo status.</summary>
        public bool IsModerator { get; }

        /// <summary>Property representing subscriber status.</summary>
        public bool IsSubscriber { get; }

        /// <summary>
        /// A Boolean value that indicates whether the user has site-wide commercial free mode enabled. 
        /// </summary>
        public bool Turbo { get; }

        /// <summary>Property representing returned user type of user.</summary>
        public UserType UserType { get; }

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        /// <summary>
        /// Constructor for UserState.
        /// </summary>
        /// <param name="ircMessage"></param>
        public UserState(IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;

            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.Color:
                        HexColor = tagValue;
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.EmotesSets:
                        EmoteSet = tagValue;
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.Turbo:
                        Turbo = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    default:
                        (UndocumentedTags ??= new()).Add(tag.Key, tag.Value);
                        break;
                }
            }

            if (string.Equals(ircMessage.User, Channel, StringComparison.InvariantCultureIgnoreCase))
                UserType = UserType.Broadcaster;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserState"/> class.
        /// </summary>
        public UserState(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            string hexColor,
            string displayName,
            string emoteSet,
            string channel,
            string id,
            bool isSubscriber,
            bool isModerator,
            UserType userType)
        {
            Badges = badges;
            BadgeInfo = badgeInfo;
            HexColor = hexColor;
            DisplayName = displayName;
            EmoteSet = emoteSet;
            Channel = channel;
            Id = id;
            IsSubscriber = isSubscriber;
            IsModerator = isModerator;
            UserType = userType;
        }
    }
}
