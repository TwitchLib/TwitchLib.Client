using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Exceptions
{
    /// <summary>
    /// Exception representing bad login credentials for the client
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <inheritdoc />
    public class IncorrectLoginException : Exception
    {
        /// <summary>
        /// Username that had the exception.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; protected set; }

        /// <summary>
        /// Exception construtor.
        /// </summary>
        /// <param name="reasoning">The reasoning.</param>
        /// <param name="twitchUsername">The twitch username.</param>
        /// <inheritdoc />
        public IncorrectLoginException(string reasoning, string twitchUsername)
            : base(reasoning)
        {
            Username = twitchUsername;
        }
    }
}
