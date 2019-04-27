using System;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class ChannelStateBuilder : IBuilder<ChannelState>, IFromIrcMessageBuilder<ChannelState>
    {
        private string _broadcasterLanguage;
        private string _channel;
        private bool _emoteOnly;
        private TimeSpan _followersOnly;
        private bool _mercury;
        private bool _r9K;
        private bool _rituals;
        private string _roomId;
        private int _slowMode;
        private bool _subOnly;

        private ChannelStateBuilder()
        {
            //todo: add with8 methods
        }

        public static ChannelStateBuilder Create()
        {
            return new ChannelStateBuilder();
        }

        public ChannelState Build()
        {
            return new ChannelState(
                _r9K,
                _rituals,
                _subOnly,
                _slowMode,
                _emoteOnly,
                _broadcasterLanguage,
                _channel,
                _followersOnly,
                _mercury,
                _roomId);
        }

        public ChannelState BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new ChannelState(fromIrcMessageBuilderDataObject.Message);
        }
    }
}
