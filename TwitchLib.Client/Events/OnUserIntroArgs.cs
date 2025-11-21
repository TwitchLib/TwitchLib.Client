using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a PRIVMSG that represents a User Intro
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnUserIntroArgs : EventArgs
    {
        /// <summary>
        /// Property representing the PRIVMSG
        /// </summary>
        public ChatMessage ChatMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnUserIntroArgs"/> class.
        /// </summary>
        public OnUserIntroArgs(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }
    }
}