#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing Announcement in a Twitch channel.</summary>
    public class Announcement : IHexColorProperty
    {
        /// <summary>Property representing announcement message id</summary>
        public string Id { get; }

        /// <summary>Property representing list of badges assigned.</summary>
        public List<KeyValuePair<string, string>> Badges { get; }

        /// <summary>Metadata associated with each badge</summary>
        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        /// <summary>Property representing internal system message value.</summary>
        public string SystemMessage { get; }

        /// <summary>Property representing internal system message value, parsed.</summary>
        public string SystemMessageParsed { get; }

        /// <summary>Property representing whether or not the announcer is the broadcaster.</summary>
        public bool IsBroadcaster { get; }

        /// <summary>Property representing whether or not the announcer is a moderator.</summary>
        public bool IsModerator { get; }

        /// <summary>Property representing whether or not announcer is a partner.</summary>
        public bool IsPartner { get; }

        /// <summary>Property representing whether or not the announcer is a subscriber.</summary>
        public bool IsSubscriber { get; }

        /// <summary>Property representing whether or not the announcer is a twitch staff member.</summary>
        public bool IsStaff { get; }

        /// <summary>Property representing whether or not the announcer is a turbo member.</summary>
        public bool IsTurbo { get; }

        /// <summary>Property representing the user's login.</summary>
        public string Login { get; }

        /// <summary>Property representing the user's id.</summary>
        public string UserId { get; }

        /// <summary>Property representing the room id.</summary>
        public string RoomId { get; }

        /// <summary>Property representing the user type of the announcer.</summary>
        public UserType UserType { get; }

        /// <summary>Property representing the tmi-sent-ts value.</summary>
        public DateTimeOffset TmiSent { get; }

        /// <summary>Property representing emote set of announcement.</summary>
        public string EmoteSet { get; }

        /// <summary>Property representing the raw IRC message (for debugging/customized parsing)</summary>
        public string RawIrc { get; }

        /// <summary>Property representing the msg-id value.</summary>
        public string MsgId { get; }

        /// <summary>Property representing the color value of the announcement.</summary>
        public string MsgParamColor { get; }

        /// <inheritdoc/>
        public string HexColor { get; }

        /// <summary>Property representing the message of the announcement.</summary>
        public string Message { get; }

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        /// SAMPLE: @badge-info=;badges=broadcaster/1,ambassador/1;color=#033700;display-name=BarryCarlyon;emotes=;flags=;id=55d90904-e515-47d0-ac1d-879f7f1d7b01;login=barrycarlyon;mod=0;msg-id=announcement;msg-param-color=PRIMARY;room-id=15185913;subscriber=0;system-msg=;tmi-sent-ts=1648758023469;user-id=15185913;user-type= :tmi.twitch.tv USERNOTICE #barrycarlyon :test announcment main
        /// <summary>Announcement object constructor.</summary>
        /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
        public Announcement(IrcMessage ircMessage)
        {
            RawIrc = ircMessage.ToString();
            Message = ircMessage.Message;
            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tagValue);
                        foreach (var badge in Badges)
                        {
                            switch (badge.Key)
                            {
                                case "broadcaster":
                                    IsBroadcaster = true;
                                    break;
                                case "turbo":
                                    IsTurbo = true;
                                    break;
                                case "moderator":
                                    IsModerator = true;
                                    break;
                                case "subscriber":
                                    IsSubscriber = true;
                                    break;
                                case "admin":
                                    IsStaff = true;
                                    break;
                                case "staff":
                                    IsStaff = true;
                                    break;
                                case "partner":
                                    IsPartner = true;
                                    break;
                            }
                        }
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tagValue);
                        break;
                    case Tags.Color:
                        HexColor = tagValue;
                        break;
                    case Tags.MsgParamColor:
                        MsgParamColor = tagValue;
                        break;
                    case Tags.Emotes:
                        EmoteSet = tagValue;
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.SystemMsg:
                        SystemMessage = tagValue;
                        SystemMessageParsed = tagValue.Replace("\\s", " ");
                        break;
                    case Tags.TmiSentTs:
                        TmiSent = TagHelper.ToDateTimeOffsetFromUnixMs(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                    default:
                        (UndocumentedTags = new()).Add(tag.Key, tag.Value);
                        break;
                }
            }
        }
    }
}