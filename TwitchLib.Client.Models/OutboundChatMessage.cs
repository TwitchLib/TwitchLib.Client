namespace TwitchLib.Client.Models
{
    public class OutboundChatMessage
    {
        public string Channel { get; set; }

        public string Message { get; set; }

        public string Username { get; set; }

        public override string ToString()
        {
            var user = Username.ToLower();
            var channel = Channel.ToLower();
            return $":{user}!{user}@{user}.tmi.twitch.tv PRIVMSG #{channel} :{Message}";
        }
    }
}
