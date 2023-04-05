using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a PRIVMSG that represents a User Intro
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnUserIntroArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing the PRIVMSG
        /// </summary>
        public ChatMessage ChatMessage { get; }
        public OnUserIntroArgs(string channel, ChatMessage chatMessage) : base(channel)
        {
            ChatMessage = chatMessage;
        }
    }
}