using System.Collections.Generic;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class ChatCommandBuilder : IBuilder<ChatCommand>
    {
        private List<string> _argumentsAsList;
        private string _argumentsAsString;
        private ChatMessage _chatMessage;
        private char _commandIdentifier;
        private string _commandText;

        private ChatCommandBuilder()
        {
            // todo: add with* methods
        }

        public static ChatCommandBuilder Create()
        {
            return new ChatCommandBuilder();
        }

        public ChatCommand Build()
        {
            return new ChatCommand(
                _chatMessage,
                _commandText,
                _argumentsAsString,
                _argumentsAsList,
                _commandIdentifier);
        }

        public ChatCommand BuildFromChatMessage(ChatMessage chatMessage)
        {
            return new ChatCommand(chatMessage);
        }
    }
}
