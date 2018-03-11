using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Enums.Internal;

namespace TwitchLib.Client.Models
{
    public class IrcMessage
    {
        /// <summary>
        /// The channel the message was sent in
        /// </summary>
        public string Channel => Params;

        public string Params => Parameters != null && Parameters.Length > 0 ? Parameters[0] : "";

        /// <summary>
        /// Message itself
        /// </summary>r
        public string Message => Trailing;

        public string Trailing => Parameters != null && Parameters.Length > 1 ? Parameters[Parameters.Length - 1] : "";

        /// <summary>
        /// Command parameters
        /// </summary>
        private readonly string[] Parameters;

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
        public readonly Dictionary<string, string> Tags;

        /// <summary>
        /// Create an INCOMPLETE IrcMessage only carrying username
        /// </summary>
        /// <param name="user"></param>
        public IrcMessage(string user)
        {
            Parameters = null;
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
        public IrcMessage(IrcCommand command, string[] parameters, string hostmask, Dictionary<string, string> tags = null)
        {
            var idx = hostmask.IndexOf('!');
            User = idx != -1 ? hostmask.Substring(0, idx) : hostmask;
            Hostmask = hostmask;
            Parameters = parameters;
            Command = command;
            Tags = tags;
        }

        public new string ToString()
        {
            var raw = new StringBuilder(32);
            if (Tags != null)
            {
                var tags = new string[Tags.Count];
                var i = 0;
                foreach (var tag in Tags)
                {
                    tags[i] = tag.Key + "=" + tag.Value;
                    ++i;
                }
                if (tags.Length > 0)
                {
                    raw.Append("@").Append(string.Join(";", tags)).Append(" ");
                }
            }
            if (!string.IsNullOrEmpty(Hostmask))
            {
                raw.Append(":").Append(Hostmask).Append(" ");
            }
            raw.Append(Command.ToString().ToUpper().Replace("RPL_", ""));
            if (Parameters.Length <= 0) return raw.ToString();

            if (Parameters[0] != null && Parameters[0].Length > 0)
            {
                raw.Append(" ").Append(Parameters[0]);
            }
            if (Parameters.Length > 1 && Parameters[1] != null && Parameters[1].Length > 0)
            {
                raw.Append(" :").Append(Parameters[1]);
            }
            return raw.ToString();
        }
    }
}