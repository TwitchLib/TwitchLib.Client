using System;
using System.Collections.Generic;

using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Parsers
{
    /// <summary>
    /// Class IrcParser.
    /// </summary>
    internal static class IrcParser
    {

        /// <summary>
        /// Builds an IrcMessage from a raw string
        /// </summary>
        /// <param name="raw">Raw IRC message</param>
        /// <returns>IrcMessage object</returns>
        public static IrcMessage ParseIrcMessage(string raw)
        {
            Dictionary<string, string> tagDict = new Dictionary<string, string>();

            ParserState state = ParserState.STATE_NONE;
            int[] starts = new[] { 0, 0, 0, 0, 0, 0 };
            int[] lens = new[] { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < raw.Length; ++i)
            {
                lens[(int) state] = i - starts[(int) state] - 1;
                if (state == ParserState.STATE_NONE && raw[i] == '@')
                {
                    state = ParserState.STATE_V3;
                    starts[(int) state] = ++i;

                    int start = i;
                    string key = null;
                    for (; i < raw.Length; ++i)
                    {
                        if (raw[i] == '=')
                        {
                            key = raw.Substring(start, i - start);
                            start = i + 1;
                        }
                        else if (raw[i] == ';')
                        {
                            if (key == null)
                                tagDict[raw.Substring(start, i - start)] = "1";
                            else
                                tagDict[key] = raw.Substring(start, i - start);
                            start = i + 1;
                        }
                        else if (raw[i] == ' ')
                        {
                            if (key == null)
                                tagDict[raw.Substring(start, i - start)] = "1";
                            else
                                tagDict[key] = raw.Substring(start, i - start);
                            break;
                        }
                    }
                }
                else if (state < ParserState.STATE_PREFIX && raw[i] == ':')
                {
                    state = ParserState.STATE_PREFIX;
                    starts[(int) state] = ++i;
                }
                else if (state < ParserState.STATE_COMMAND)
                {
                    state = ParserState.STATE_COMMAND;
                    starts[(int) state] = i;
                }
                else if (state < ParserState.STATE_TRAILING && raw[i] == ':')
                {
                    state = ParserState.STATE_TRAILING;
                    starts[(int) state] = ++i;
                    break;
                }
                else if ((state < ParserState.STATE_TRAILING && raw[i] == '+') || (state < ParserState.STATE_TRAILING && raw[i] == '-'))
                {
                    state = ParserState.STATE_TRAILING;
                    starts[(int) state] = i;
                    break;
                }
                else if (state == ParserState.STATE_COMMAND)
                {
                    state = ParserState.STATE_PARAM;
                    starts[(int) state] = i;
                }

                while (i < raw.Length && raw[i] != ' ')
                    ++i;
            }

            lens[(int) state] = raw.Length - starts[(int) state];
            string cmd = raw.Substring(starts[(int) ParserState.STATE_COMMAND],
                lens[(int) ParserState.STATE_COMMAND]);

            IrcCommand command = GetIrcCommandFromString(cmd);

            string parameters = raw.Substring(starts[(int) ParserState.STATE_PARAM],
                lens[(int) ParserState.STATE_PARAM]);
            string message = raw.Substring(starts[(int) ParserState.STATE_TRAILING],
                lens[(int) ParserState.STATE_TRAILING]);
            string hostmask = raw.Substring(starts[(int) ParserState.STATE_PREFIX],
                lens[(int) ParserState.STATE_PREFIX]);
            return new IrcMessage(command, new[] { parameters, message }, hostmask, tagDict);
        }
        public static IrcCommand GetIrcCommandFromString(string cmd)
        {
            IrcCommand result;
            // first try to parse with prefix "rpl_"
            // otherwise the parser interprets the numeric string as ordinal
            bool parsed = Enum.TryParse<IrcCommand>($"rpl_{cmd}", true, out result);
            if (!parsed) parsed = Enum.TryParse<IrcCommand>(cmd, true, out result);
            if (!parsed) result = IrcCommand.Unknown;
            return result;
        }
        /// <summary>
        /// Enum ParserState
        /// </summary>
        private enum ParserState
        {
            /// <summary>
            /// The state none
            /// </summary>
            STATE_NONE,
            /// <summary>
            /// The state v3
            /// </summary>
            STATE_V3,
            /// <summary>
            /// The state prefix
            /// </summary>
            STATE_PREFIX,
            /// <summary>
            /// The state command
            /// </summary>
            STATE_COMMAND,
            /// <summary>
            /// The state parameter
            /// </summary>
            STATE_PARAM,
            /// <summary>
            /// The state trailing
            /// </summary>
            STATE_TRAILING
        };
    }
}
