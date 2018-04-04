using System.Linq;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class BeingHostedNotification
    {
        public string Channel { get; }
        public string BotUsername { get; }
        public string HostedByChannel { get; }
        public int Viewers { get; }
        public bool IsAutoHosted { get; }

        public BeingHostedNotification(string botUsername, IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            BotUsername = botUsername;
            HostedByChannel = ircMessage.Message.Split(' ').First();

            if (ircMessage.Message.Contains("up to"))
                Viewers = int.Parse(ircMessage.Message.Split(' ')[ircMessage.Message.Length - 2]);

            if (ircMessage.Message.Contains("auto hosting"))
                IsAutoHosted = true;
        }
    }
}