using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class HostingStopped
    {
        /// <summary>Property representing hosting channel.</summary>
        public string HostingChannel;
        /// <summary>Property representing number of viewers that were in hosting channel.</summary>
        public int Viewers;

        public HostingStopped(IrcMessage ircMessage)
        {
            HostingChannel = ircMessage.Channel;
            Viewers = int.Parse(ircMessage.Message.Split(' ')[1]);
        }
    }
}