using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{

    public class OnMessageThrottledArgs : EventArgs
    {
        public string? Reason { get; set; }
        public OutboundChatMessage? ItemNotSent { get; set; }
        public TimeSpan? Period { get; set; }
        public uint? AllowedInPeriod { get; set; }
        [SuppressMessage("Style", "IDE0058")]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            // Suppress IDE0058 - we dont daisy-chain here
            builder.AppendLine($"{nameof(Reason)}: {Reason}");
            builder.AppendLine($"{nameof(ItemNotSent)}: {ItemNotSent}");
            builder.AppendLine($"{nameof(Period)}: {Period}");
            builder.AppendLine($"{nameof(AllowedInPeriod)}: {AllowedInPeriod}");
            return builder.ToString();
        }
    }
}
