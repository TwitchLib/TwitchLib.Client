using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class ReSubscriber : SubscriberBase
    {
        public int Months { get; }

        public ReSubscriber(IrcMessage ircMessage) : base(ircMessage) {
            Months = months;
        }
    }
}
