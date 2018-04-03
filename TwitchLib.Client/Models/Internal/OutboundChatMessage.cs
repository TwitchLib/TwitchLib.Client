namespace TwitchLib.Client.Models.Internal
{
    public class OutboundChatMessage
    {
        public string Username { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{Channel} :{Message}";
        }
    }
}
