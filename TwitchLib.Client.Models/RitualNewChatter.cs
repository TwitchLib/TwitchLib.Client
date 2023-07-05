using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class RitualNewChatter
    {
        public List<KeyValuePair<string, string>> Badges { get; }

        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        public string Color { get; }

        public string DisplayName { get; }

        public string Emotes { get; }

        public string Id { get; }

        public bool IsModerator { get; }

        public bool IsSubscriber { get; }

        public bool IsTurbo { get; }

        public string Login { get; }

        public string Message { get; }

        public string MsgId { get; }

        public string MsgParamRitualName { get; }

        public string RoomId { get; }

        public string SystemMsgParsed { get; }

        public string SystemMsg { get; }

        public string TmiSentTs { get; }

        public string UserId { get; }

        public UserType UserType { get; }

        // badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;
        // login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;
        // system-msg=@KittyJinxu\sis\snew\shere.\sSay\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;
        // user-type= USERNOTICE #thorlar kittyjinxu > #thorlar: HeyGuys
        public RitualNewChatter(IrcMessage ircMessage)
        {
            Message = ircMessage.Message;
            foreach (var tag in ircMessage.Tags)
            {
                switch (tag.Key)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.Color:
                        Color = tag.Value;
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
                    case Tags.MsgParamRitualName:
                        MsgParamRitualName = tag.Value;
                        break;
                    case Tags.RoomId:
                        RoomId = tag.Value;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tag.Value;
                        SystemMsgParsed = tag.Value.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tag.Value;
                        break;
                    case Tags.Turbo:
                        IsTurbo = TagHelper.ToBool(tag.Value);
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
