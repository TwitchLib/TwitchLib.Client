using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Parsing
{
    /// <summary>
    /// Twitch IRCv3 message parser.
    /// </summary>
    public static class IrcParser
    {
        /// <summary>
        /// Parses a raw Twitch IRCv3 message line into an IrcMessage object.
        /// </summary>
        /// <param name="rawMessage">Raw IRC message</param>
        /// <returns><see cref="IrcMessage"/> that can be consumed by a variety of TwitchLib.Client types.</returns>
        /// <exception cref="FormatException">Thrown if the message is invalid.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if the message is invalid.</exception>
        public static IrcMessage ParseMessage(string rawMessage)
        {
            if (rawMessage is not { Length: >= 3 })
            {
                ThrowInvalidMessage();
            }

            // Sequentially parse each segment, advancing the span as we go.
            // Adapted from https://github.com/neon-sunset/warpskimmer
            var source = rawMessage.AsSpan();
            var tags = ParseTags(ref source);
            var (user, hostmask) = ParsePrefix(ref source);
            var command = ParseCommand(ref source);

            string parameters, message;
            var messageOffset = source.IndexOf(" :".AsSpan());
            if (messageOffset >= 0)
            {
                parameters = source.Slice(0, messageOffset).ToString();
                message = source.Slice(messageOffset + 2).ToString();
            }
            else
            {
                parameters = source.ToString();
                message = "";
            }

            return new IrcMessage(rawMessage, command, new[] { parameters, message }, user, hostmask, tags);
        }

        private static Dictionary<string, string>? ParseTags(ref ReadOnlySpan<char> source)
        {
            var local = source;
            if (local[0] != '@')
            {
                return null;
            }
            local = local.Slice(1);

            var tags = new Dictionary<string, string>();

            while (true)
            {
                var delimiter = local.IndexOfAny(';', ' ');
                var (key, value) = local.Slice(0, delimiter).SplitFirst('=');
                tags[key.ToString()] = value.ToString();

                if (local[delimiter] is ' ')
                {
                    source = local.Slice(delimiter + 1);
                    break;
                }

                local = local.Slice(delimiter + 1);
            }

            return tags;
        }

        private static (string User, string Hostmask) ParsePrefix(ref ReadOnlySpan<char> source)
        {
            var local = source;
            if (local[0] != ':')
            {
                return default;
            }

            var delimiter = local.IndexOf(' ');
            var (user, hostmask) = local.Slice(1, delimiter - 1).SplitFirst('!');
            if (hostmask.IsEmpty)
            {
                hostmask = user;
            }

            source = local.Slice(delimiter + 1);
            return (user.ToString(), hostmask.ToString());
        }

        private static IrcCommand ParseCommand(ref ReadOnlySpan<char> source)
        {
            (var command, source) = source.SplitFirst(' ');

            return command switch
            {
                "PRIVMSG" => IrcCommand.PrivMsg,
                "NOTICE" => IrcCommand.Notice,
                "PING" => IrcCommand.Ping,
                "PONG" => IrcCommand.Pong,
                "CLEARCHAT" => IrcCommand.ClearChat,
                "CLEARMSG" => IrcCommand.ClearMsg,
                "USERSTATE" => IrcCommand.UserState,
                "GLOBALUSERSTATE" => IrcCommand.GlobalUserState,
                "NICK" => IrcCommand.Nick,
                "JOIN" => IrcCommand.Join,
                "PART" => IrcCommand.Part,
                "PASS" => IrcCommand.Pass,
                "CAP" => IrcCommand.Cap,
                "001" => IrcCommand.RPL_001,
                "002" => IrcCommand.RPL_002,
                "003" => IrcCommand.RPL_003,
                "004" => IrcCommand.RPL_004,
                "353" => IrcCommand.RPL_353,
                "366" => IrcCommand.RPL_366,
                "372" => IrcCommand.RPL_372,
                "375" => IrcCommand.RPL_375,
                "376" => IrcCommand.RPL_376,
                "WHISPER" => IrcCommand.Whisper,
                "SERVERCHANGE" => IrcCommand.ServerChange,
                "RECONNECT" => IrcCommand.Reconnect,
                "ROOMSTATE" => IrcCommand.RoomState,
                "USERNOTICE" => IrcCommand.UserNotice,
                "MODE" => IrcCommand.Mode,
                _ => IrcCommand.Unknown,
            };
        }

        // Not throwing message contents because it may contain sensitive information.
        private static void ThrowInvalidMessage()
        {
            throw new FormatException(
                "Unexpected IRC message format: must be not null and longer than 3 characters.");
        }
    }
}
