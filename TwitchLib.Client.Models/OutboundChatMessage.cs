namespace TwitchLib.Client.Models
{
    public class OutboundChatMessage
    {
        public string Channel { get; set; }

        public string Message { get; set; }

        public string? ReplyToId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundChatMessage"/> class.
        /// </summary>
        public OutboundChatMessage(string channel, string message)
        {
            Channel = channel;
            Message = message;
        }
        
        /// <inheritdoc/>
        public override string ToString()
        {
            var channel = Channel.ToLower();
            return ReplyToId is null 
                ? $"PRIVMSG #{channel} :{Message}"
                : $"@reply-parent-msg-id={ReplyToId} PRIVMSG #{channel} :{Message}";
        }
    }
}
