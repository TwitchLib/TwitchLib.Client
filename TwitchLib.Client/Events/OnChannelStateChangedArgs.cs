using TwitchLib.Client.Events.Abstracts;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing on channel state changed event.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnChannelStateChangedArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     <b>always</b> represents the <b>complete and new</b> <see cref="Models.ChannelState"/>!
        ///     <br></br>
        ///     <br></br>
        ///     <see href="https://dev.twitch.tv/docs/irc/tags/#roomstate-tags"/>
        ///     <br></br>
        ///     <br></br>
        ///     <b>2nd paragraph:</b>
        ///     <br></br>
        ///     For JOIN messages, the message contains all chat room setting tags,
        ///     <br></br>
        ///     but for actions that change a single chat room setting,
        ///     <br></br>
        ///     the message includes only that chat room setting tag.
        ///     <br></br>
        ///     For example,
        ///     <br></br>
        ///     if the moderator turned on unique chat,
        ///     <br></br>
        ///     the message includes only the r9k tag.
        /// </summary>
        public ChannelState ChannelState { get; set; }
    }
}
