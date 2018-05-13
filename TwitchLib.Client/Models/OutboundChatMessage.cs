namespace TwitchLib.Client.Models
{
    public class OutboundChatMessage
    {        
        public string Channel { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return $":{Username.ToLower()}!{Username.ToLower()}@{Username.ToLower()}.tmi.twitch.tv PRIVMSG #{Channel.ToLower()} :{Message}";
        }
    }
}
