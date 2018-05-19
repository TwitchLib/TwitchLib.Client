using System.Linq;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class BeingHostedNotification
    {
        public string BotUsername { get; }
        public string Channel { get; }        
        public string HostedByChannel { get; }
        public bool IsAutoHosted { get; }
        public int Viewers { get; }
        

        public BeingHostedNotification(string botUsername, IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            BotUsername = botUsername;
            HostedByChannel = ircMessage.Message.Split(' ').First();

            if (ircMessage.Message.Contains("up to"))
                Viewers = int.Parse(ircMessage.Message.Split(' ')[ircMessage.Message.Split(' ').Count() - 2]);

            if (ircMessage.Message.Contains("auto hosting"))
                IsAutoHosted = true;
        }

        internal BeingHostedNotification(string channel, string botUsername, string hostedByChannel, int viewers, bool isAutoHosted)
        {
            Channel = channel;
            BotUsername = botUsername;
            HostedByChannel = hostedByChannel;
            Viewers = viewers;
            IsAutoHosted = isAutoHosted;
        }
    }
}
