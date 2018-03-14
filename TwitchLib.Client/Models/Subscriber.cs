using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class Subscriber : SubscriberBase
    {
        public Subscriber(IrcMessage ircMessage) : base(ircMessage) { }
    }
}
