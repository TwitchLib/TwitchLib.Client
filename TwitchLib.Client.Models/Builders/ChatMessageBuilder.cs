using System.Collections.Generic;

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class ChatMessageBuilder : IBuilder<ChatMessage>
    {
        private TwitchLibMessage _twitchLibMessage;
        private List<KeyValuePair<string, string>> BadgeInfo = new List<KeyValuePair<string, string>>();
        private int _bits;
        private double _bitsInDollars;
        private string _channel;
        private CheerBadge _cheerBadge;
        private string _emoteReplacedMessage;
        private string _id;
        private bool _isBroadcaster;
        private bool _isMe;
        private bool _isModerator;
        private bool _isSubscriber;
        private string _message;
        private Noisy _noisy;
        private string _rawIrcMessage;
        private string _roomId;
        private int _subscribedMonthCount;

        private ChatMessageBuilder()
        {
            // todo: add with* methods
        }

        public static ChatMessageBuilder Create()
        {
            return new ChatMessageBuilder();
        }

        public ChatMessage Build()
        {
            return new ChatMessage(
                _twitchLibMessage.BotUsername,
                _twitchLibMessage.UserId,
                _twitchLibMessage.Username,
                _twitchLibMessage.DisplayName,
                _twitchLibMessage.ColorHex,
                _twitchLibMessage.Color,
                _twitchLibMessage.EmoteSet,
                _message,
                _twitchLibMessage.UserType,
                _channel,
                _id,
                _isSubscriber,
                _subscribedMonthCount,
                _roomId,
                _twitchLibMessage.IsTurbo,
                _isModerator,
                _isMe,
                _isBroadcaster,
                _noisy,
                _rawIrcMessage,
                _emoteReplacedMessage,
                _twitchLibMessage.Badges,
                _cheerBadge,
                _bits,
                _bitsInDollars);
        }
    }
}
