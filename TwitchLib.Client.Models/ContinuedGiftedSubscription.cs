#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ContinuedGiftedSubscription : IHexColorProperty
    {
        //@badge-info=subscriber/11;badges=subscriber/9;color=#DAA520;display-name=Varanid;emotes=;flags=;id=a2d384c1-c30a-409e-8001-9e7d8f9c784d;login=varanid;mod=0;msg-id=giftpaidupgrade;msg-param-sender-login=cletusbueford;msg-param-sender-name=CletusBueford;room-id=44338537;subscriber=1;system-msg=Varanid\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\sCletusBueford!;tmi-sent-ts=1612497386372;user-id=67505836;user-type= :tmi.twitch.tv USERNOTICE #burkeblack 

        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }


        /// <inheritdoc/>
        public string HexColor { get; }

        public string DisplayName { get; }

        public string Emotes { get; }

        public string Flags { get; }

        public string Id { get; }

        public string Login { get; }

        public bool IsModerator { get; }

        public string MsgId { get; }

        public string MsgParamSenderLogin { get; }

        public string MsgParamSenderName { get; }

        public string RoomId { get; }

        public bool IsSubscriber { get; }

        public string SystemMsg { get; }

        public DateTimeOffset TmiSent { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        /// <summary>
        /// Contains undocumented tags.
        /// </summary>
        public Dictionary<string, string>? UndocumentedTags { get; }

        public ContinuedGiftedSubscription(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags)
            {
                var tagValue = tag.Value;
                switch (tag.Key)
                {
                    case Tags.SystemMsg:
                        SystemMsg = tagValue;
                        break;
                    case Tags.Flags:
                        Flags = tagValue;
                        break;
                    case Tags.MsgParamSenderLogin:
                        MsgParamSenderLogin = tagValue;
                        break;
                    case Tags.MsgParamSenderName:
                        MsgParamSenderName = tagValue;
                        break;
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
                    case Tags.Emotes:
                        Emotes = tagValue;
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tagValue);
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tagValue);
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
