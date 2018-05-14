namespace TwitchLib.Client.Models
{
    public class OutboundChatMessage
    {        
        public string Channel { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
<<<<<<< HEAD
            var user = Username.ToLower();
            var channel = Channel.ToLower();
            return $":{user}!{user}@{user}.tmi.twitch.tv PRIVMSG #{channel} :{Message}";
=======
            return $":{Username.ToLower()}!{Username.ToLower()}@{Username.ToLower()}.tmi.twitch.tv PRIVMSG #{Channel.ToLower()} :{Message}";
>>>>>>> aabddb03ffa6068b654852fd31b2e0873875ba79
        }
    }
}
