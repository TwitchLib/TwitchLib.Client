using System.Collections.Generic;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Extensions.NetCore;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing Announcement in a Twitch channel.</summary>
    public class Announcement
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
        public string TmiSentTs { get; }

        /// <summary>Property representing emote set of announcement.</summary>
        public string EmoteSet { get; }

        /// <summary>Property representing the raw IRC message (for debugging/customized parsing)</summary>
        public string RawIrc { get; }

        /// <summary>Property representing the msg-id value.</summary>
        public string MsgId { get; }

        /// <summary>Property representing the color value of the announcement.</summary>
        public string MsgParamColor { get; }

        /// <summary>Property representing the colorhex of the announcer.</summary>
        public string ColorHex { get; }

        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public Color Color { get; }

        /// <summary>Property representing the message of the announcement.</summary>
        public string Message { get; }

        /// SAMPLE: @badge-info=;badges=broadcaster/1,ambassador/1;color=#033700;display-name=BarryCarlyon;emotes=;flags=;id=55d90904-e515-47d0-ac1d-879f7f1d7b01;login=barrycarlyon;mod=0;msg-id=announcement;msg-param-color=PRIMARY;room-id=15185913;subscriber=0;system-msg=;tmi-sent-ts=1648758023469;user-id=15185913;user-type= :tmi.twitch.tv USERNOTICE #barrycarlyon :test announcment main
        /// <summary>Announcement object constructor.</summary>
        /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
        public Announcement(IrcMessage ircMessage)
        {
            RawIrc = ircMessage.ToString();
            Message = ircMessage.Message;

            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        Badges = Common.Helpers.ParseBadges(tagValue);
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
                        BadgeInfo = Common.Helpers.ParseBadges(tagValue);
                        break;
                    case Tags.Color:
                        ColorHex = tagValue;
                        if (!string.IsNullOrEmpty(ColorHex))
                            Color = ColorTranslator.FromHtml(ColorHex);
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
                        TmiSentTs = tagValue;
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        switch (tagValue)
                        {
                            case "mod":
                                UserType = UserType.Moderator;
                                break;
                            case "global_mod":
                                UserType = UserType.GlobalModerator;
                                break;
                            case "admin":
                                UserType = UserType.Admin;
                                break;
                            case "staff":
                                UserType = UserType.Staff;
                                break;
                            default:
                                UserType = UserType.Viewer;
                                break;
                        }
                        break;
                }
            }
        }
    }
}