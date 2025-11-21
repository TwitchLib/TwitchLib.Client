using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnCommunityPayForwardArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The CommunityPayForward
    /// </summary>
    public CommunityPayForward CommunityPayForward { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnCommunityPayForwardArgs"/> class.
    /// </summary>
    public OnCommunityPayForwardArgs(string channel, CommunityPayForward communityPayForward)
    {
        Channel = channel;
        CommunityPayForward = communityPayForward;
    }

    internal OnCommunityPayForwardArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        CommunityPayForward = new(ircMessage);
    }
}
