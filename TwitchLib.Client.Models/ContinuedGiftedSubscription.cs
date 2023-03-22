using Microsoft.Extensions.Logging;

using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ContinuedGiftedSubscription : ASubscriptionBase
    {
        //@badge-info=subscriber/11;badges=subscriber/9;color=#DAA520;display-name=Varanid;emotes=;flags=;id=a2d384c1-c30a-409e-8001-9e7d8f9c784d;login=varanid;mod=0;msg-id=giftpaidupgrade;msg-param-sender-login=cletusbueford;msg-param-sender-name=CletusBueford;room-id=44338537;subscriber=1;system-msg=Varanid\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\sCletusBueford!;tmi-sent-ts=1612497386372;user-id=67505836;user-type= :tmi.twitch.tv USERNOTICE #burkeblack 

        public string MsgParamSenderLogin { get; }

        public string MsgParamSenderName { get; }


        public ContinuedGiftedSubscription(IrcMessage ircMessage, ILogger logger = null) : base(ircMessage, logger)
        {
            foreach (string tag in ircMessage.Tags.Keys)
            {
                string tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.MsgParamSenderLogin:
                        MsgParamSenderLogin = tagValue;
                        break;
                    case Tags.MsgParamSenderName:
                        MsgParamSenderName = tagValue;
                        break;
                }
            }
        }
    }
}
