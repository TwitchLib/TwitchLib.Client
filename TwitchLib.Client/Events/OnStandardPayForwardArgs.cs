using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnStandardPayForwardArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The StandardPayForward
    /// </summary>
    public StandardPayForward StandardPayForward { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnRaidNotificationArgs"/> class.
    /// </summary>
    public OnStandardPayForwardArgs(string channel, StandardPayForward standardPayForward)
    {
        Channel = channel;
        StandardPayForward = standardPayForward;
    }

    internal OnStandardPayForwardArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        StandardPayForward = new(ircMessage);
    }
}
