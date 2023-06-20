using System.Collections.Generic;
using System.Linq;
using System.Text;

using TwitchLib.Client.Enums.Internal;

namespace TwitchLib.Client.Models.Internal
{
    public class IrcMessage
    {
        /// <summary>
        /// The channel the message was sent in
        /// </summary>
        public string Channel => _channel ??= Params.StartsWith("#") ? Params.Remove(0, 1) : Params;
        private string _channel;

        public string Params => _parameters?.Length > 0 ? _parameters[0] : "";

        /// <summary>
        /// Message itself
        /// </summary>
        public string Message => Trailing;

        public string Trailing => _parameters?.Length > 1 ? _parameters[_parameters.Length - 1] : "";

        /// <summary>
        /// Command parameters
        /// </summary>
        private readonly string[] _parameters;

        /// <summary>
        /// The user whose message it is
        /// </summary>
        public readonly string User;

        /// <summary>
        /// Hostmask of the user
        /// </summary>
        public readonly string Hostmask;

        /// <summary>
        /// Raw Command
        /// </summary>
        public readonly IrcCommand Command;

        /// <summary>
        /// IRCv3 tags
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Tags;

        private string _rawString;

        /// <summary>
        /// Create an INCOMPLETE IrcMessage only carrying username
        /// </summary>
        /// <param name="user"></param>
        public IrcMessage(string user)
        {
            _parameters = null;
            User = user;
            Hostmask = null;
            Command = IrcCommand.Unknown;
            Tags = null;
        }

        /// <summary>
        /// Create an IrcMessage
        /// </summary>
        /// <param name="command">IRC Command</param>
        /// <param name="parameters">Command params</param>
        /// <param name="hostmask">User</param>
        /// <param name="tags">IRCv3 tags</param>
        public IrcMessage(
            IrcCommand command,
            string[] parameters,
            string hostmask,
            Dictionary<string, string> tags = null)
        {
            var idx = hostmask.IndexOf('!');
            User = idx >= 0 ? hostmask.Substring(0, idx) : hostmask;
            Hostmask = hostmask;
            _parameters = parameters;
            Command = command;
            Tags = tags;

            if (command == IrcCommand.RPL_353
                && Params.Length > 0
                && Params.Contains("#"))
            {
                _parameters[0] = $"#{_parameters[0].Split('#')[1]}";
            }
        }

        /// <summary>
        /// Create an IrcMessage, settings its raw string.
        /// IrcParser *must* use this constructor, otherwise the raw string
        /// will be re-generated on each PRIVMSG (90% of all messages)
        /// </summary>
        /// <param name="raw">Raw IRC message</param>
        /// <param name="command">IRC Command</param>
        /// <param name="parameters">Command params</param>
        /// <param name="user">User</param>
        /// <param name="hostmask">Hostmask</param>
        /// <param name="tags">IRCv3 tags</param>
        internal IrcMessage(
            string raw,
            IrcCommand command,
            string[] parameters,
            string user,
            string hostmask,
            Dictionary<string, string> tags = null)
        {
            User = user;
            Hostmask = hostmask;
            Command = command;
            Tags = tags;

            _rawString = raw;
            _parameters = parameters;
            if (command == IrcCommand.RPL_353
                && Params.Length > 0
                && Params.Contains("#"))
            {
                _parameters[0] = $"#{_parameters[0].Split('#')[1]}";
            }
        }

        /// <inheritdoc/>
        public override string ToString() => _rawString ??= GenerateToString();

        private string GenerateToString()
        {
            var raw = new StringBuilder(64);
            if (Tags?.Count > 0)
            {
                raw.Append("@");
                foreach (var tag in Tags)
                {
                    raw.Append(tag.Key).Append('=').Append(tag.Value).Append(';');
                }
                raw[raw.Length - 1] = ' ';
            }

            if (!string.IsNullOrEmpty(Hostmask))
            {
                raw.Append(":").Append(Hostmask).Append(" ");
            }

            raw.Append(Command.ToString().ToUpper().Replace("RPL_", ""));
            if (_parameters == null || _parameters.Length == 0)
                return raw.ToString();

            if (!string.IsNullOrEmpty(_parameters[0]))
            {
                raw.Append(" ").Append(_parameters[0]);
            }

            if (_parameters.Length > 1 && _parameters[1]?.Length > 0)
            {
                raw.Append(" :").Append(_parameters[1]);
            }

            return raw.ToString();
        }
    }
}
