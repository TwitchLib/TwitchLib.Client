using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class UserTimeout
    {
        /// <summary>Channel that had timeout event.</summary>
        public string Channel;
        /// <summary>Viewer that was timedout.</summary>
        public string Username;
        /// <summary>Duration of timeout IN SECONDS.</summary>
        public int TimeoutDuration;
        /// <summary>Reason for timeout, if it was provided.</summary>
        public string TimeoutReason;

        public UserTimeout(IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            Username = ircMessage.Message;

            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.BanDuration:
                        TimeoutDuration = int.Parse(tagValue);
                        break;
                    case Tags.BanReason:
                        TimeoutReason = tagValue;
                        break;
                }
            }
        }
    }
}