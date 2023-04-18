using System;
using System.Text;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnMessageThrottledArgs
    {
        public string Reason { get; }
        public OutboundChatMessage ItemNotSent { get; }
        public TimeSpan Period { get; }
        public uint AllowedInPeriod { get;}
        
        public OnMessageThrottledArgs(string reason, OutboundChatMessage itemNotSent, TimeSpan period, uint allowedInPeriod)
        {
            Reason = reason;
            ItemNotSent = itemNotSent;
            Period = period;
            AllowedInPeriod = allowedInPeriod;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            
            builder.AppendLine($"{nameof(Reason)}: {Reason}");
            builder.AppendLine($"{nameof(ItemNotSent)}: {ItemNotSent}");
            builder.AppendLine($"{nameof(Period)}: {Period}");
            builder.AppendLine($"{nameof(AllowedInPeriod)}: {AllowedInPeriod}");
            
            return builder.ToString();
        }
    }
}