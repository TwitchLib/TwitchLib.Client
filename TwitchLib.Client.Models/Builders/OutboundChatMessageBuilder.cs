#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public sealed class OutboundChatMessageBuilder : IBuilder<OutboundChatMessage>
    {
        private string _channel;
        private string _message;

        private OutboundChatMessageBuilder()
        {
        }

        public static OutboundChatMessageBuilder Create()
        {
            return new OutboundChatMessageBuilder();
        }

        public OutboundChatMessageBuilder WithChannel(string channel)
        {
            _channel = channel;
            return this;
        }

        public OutboundChatMessageBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public OutboundChatMessage Build()
        {
            return new OutboundChatMessage(_channel, _message);
        }
    }
}