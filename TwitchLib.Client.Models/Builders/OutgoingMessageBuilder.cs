namespace TwitchLib.Client.Models.Builders
{
    public sealed class OutgoingMessageBuilder : IBuilder<OutgoingMessage>
    {
        private string _channel;
        private string _message;
        private int _nonce;
        private string _sender;
        private MessageState _messageState;

        private OutgoingMessageBuilder()
        {
        }

        public static OutgoingMessageBuilder Create()
        {
            return new OutgoingMessageBuilder();
        }

        public OutgoingMessageBuilder WithChannel(string channel)
        {
            _channel = channel;
            return this;
        }

        public OutgoingMessageBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public OutgoingMessageBuilder WithNonce(int nonce)
        {
            _nonce = nonce;
            return this;
        }

        public OutgoingMessageBuilder WithSender(string sender)
        {
            _sender = sender;
            return this;
        }

        public OutgoingMessageBuilder WithMessageState(MessageState messageState)
        {
            _messageState = messageState;
            return this;
        }

        public OutgoingMessage Build()
        {
            return new OutgoingMessage
            {
                Channel = _channel,
                Message = _message,
                Nonce = _nonce,
                Sender = _sender,
                State = _messageState
            };
        }
    }

    public sealed class OutboundWhisperMessageBuilder : IBuilder<OutboundWhisperMessage>
    {
        private string _username;
        private string _receiver;
        private string _message;

        private OutboundWhisperMessageBuilder()
        {
        }

        public OutboundWhisperMessageBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public OutboundWhisperMessageBuilder WithReceiver(string receiver)
        {
            _receiver = receiver;
            return this;
        }

        public OutboundWhisperMessageBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public static OutboundWhisperMessageBuilder Create()
        {
            return new OutboundWhisperMessageBuilder();
        }

        public OutboundWhisperMessage Build()
        {
            return new OutboundWhisperMessage
            {
                Message = _message,
                Receiver = _receiver,
                Username = _username
            };
        }
    }
}