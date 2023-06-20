using System;
using System.Collections.Generic;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Internal.Parsing
{
    /// <summary>
    /// Class IrcParser.
    /// </summary>
    internal static class IrcParser
    {
        /// <summary>
        /// Builds an IrcMessage from a string.
        /// </summary>
        /// <param name="raw">Raw IRC message</param>
        /// <returns>IrcMessage object</returns>
        public static IrcMessage ParseMessage(string raw)
        {
            // Sequentially parse each msg segment, advancing the span as we go
            var source = raw.AsSpan();
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

            return new IrcMessage(raw, command, new[] { parameters, message }, user, hostmask, tags);
        }

        private static Dictionary<string, string> ParseTags(ref ReadOnlySpan<char> source)
        {
            var local = source;
            if (local[0] != '@')
            {
                return null;
            }

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
    }
}
