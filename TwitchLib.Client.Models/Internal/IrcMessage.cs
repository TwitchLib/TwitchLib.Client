using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Internal
{
    public class IrcMessage
    {
        /// <summary>
        /// The channel the message was sent in
        /// </summary>
        public string Channel => Params.StartsWith("#") ? Params.Remove(0, 1) : Params;

        public string Params => Parameters != null && Parameters.Length > 0 ? Parameters[0] : "";

        /// <summary>
        /// Message itself
        /// </summary>
        public string Message => Trailing;

        public string Trailing => Parameters != null && Parameters.Length > 1 ? Parameters[Parameters.Length - 1] : "";

        /// <summary>
        /// Command parameters
        /// </summary>
        private string[] Parameters { get; } = Array.Empty<string>();

        /// <summary>
        /// The user whose message it is
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Hostmask of the user
        /// </summary>
        public string? Hostmask { get; }

        /// <summary>
        /// Raw Command
        /// </summary>
        public IrcCommand Command { get; }

        /// <summary>
        /// IRCv3 tags
        /// </summary>
        public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Create an INCOMPLETE IrcMessage only carrying username
        /// </summary>
        /// <param name="user"></param>
        public IrcMessage(string user)
        {
            User = user;
            Hostmask = null;
            Command = IrcCommand.Unknown;
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
            Dictionary<string, string>? tags = null)
        {
            int idx = hostmask.IndexOf('!');
            User = idx != -1 ? hostmask.Substring(0, idx) : hostmask;
            Hostmask = hostmask;
            Parameters = parameters;
            Command = command;
            Tags = tags ?? new Dictionary<string, string>();

            if (command == IrcCommand.RPL_353)
            {
                if (Params.Length > 0 && Params.Contains("#"))
                {
                    Parameters[0] = $"#{Parameters[0].Split('#')[1]}";
                }
            }
        }

        [SuppressMessage("Style", "IDE0058")]
        public new string ToString()
        {
            // SuppressMessage IDE0058 - no daisy chaining with StringBuilder
            StringBuilder raw = new StringBuilder(32);
            if (Tags != null)
            {
                string[] tags = new string[Tags.Count];
                int i = 0;
                foreach (KeyValuePair<string, string> tag in Tags)
                {
                    tags[i] = tag.Key + "=" + tag.Value;
                    ++i;
                }

                if (tags.Length > 0)
                {
                    raw.Append("@").Append(System.String.Join(";", tags)).Append(" ");
                }
            }

            if (!System.String.IsNullOrEmpty(Hostmask))
            {
                raw.Append(":").Append(Hostmask).Append(" ");
            }

            raw.Append(Command.ToString().ToUpper().Replace("RPL_", ""));
            if (Parameters.Length <= 0)
                return raw.ToString();

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