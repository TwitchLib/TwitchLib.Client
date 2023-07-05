using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ContinuedGiftedSubscription
    {
        //@badge-info=subscriber/11;badges=subscriber/9;color=#DAA520;display-name=Varanid;emotes=;flags=;id=a2d384c1-c30a-409e-8001-9e7d8f9c784d;login=varanid;mod=0;msg-id=giftpaidupgrade;msg-param-sender-login=cletusbueford;msg-param-sender-name=CletusBueford;room-id=44338537;subscriber=1;system-msg=Varanid\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\sCletusBueford!;tmi-sent-ts=1612497386372;user-id=67505836;user-type= :tmi.twitch.tv USERNOTICE #burkeblack 

        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        public Color Color { get; }

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

        public string TmiSentTs { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        public ContinuedGiftedSubscription(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags)
            {
                switch (tag.Key)
                {
                    case Tags.SystemMsg:
                        SystemMsg = tag.Value;
                        break;
                    case Tags.Flags:
                        Flags = tag.Value;
                        break;
                    case Tags.MsgParamSenderLogin:
                        MsgParamSenderLogin = tag.Value;
                        break;
                    case Tags.MsgParamSenderName:
                        MsgParamSenderName = tag.Value;
                        break;
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.Color:
                        Color = TagHelper.ToColor(tag.Value);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tag.Value;
                        break;
                    case Tags.Emotes:
                        Emotes = tag.Value;
                        break;
                    case Tags.Id:
                        Id = tag.Value;
                        break;
                    case Tags.Login:
                        Login = tag.Value;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.MsgId:
                        MsgId = tag.Value;
                        break;
                    case Tags.RoomId:
                        RoomId = tag.Value;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tag.Value;
                        break;
                    case Tags.UserId:
                        UserId = tag.Value;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                }
            }
        }
    }
}
