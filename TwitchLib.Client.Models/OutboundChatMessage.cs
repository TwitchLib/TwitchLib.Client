namespace TwitchLib.Client.Models
{
    public class OutboundChatMessage
    {
        public string Channel { get; set; }

        public string Message { get; set; }

        public string Username { get; set; }

        public string ReplyToId { get; set; }

        public override string ToString()
        {
            var channel = Channel.ToLower();
            if(ReplyToId == null)
            {
                var user = Username.ToLower();
                return $":{user}!{user}@{user}.tmi.twitch.tv PRIVMSG #{channel} :{Message}";
            } 
            else
            {
                return $"@reply-parent-msg-id={ReplyToId} PRIVMSG #{channel} :{Message}";
            }
            
        }
    }
}
