using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnBitsBadgeTierArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The BitsBadgeTier
    /// </summary>
    public BitsBadgeTier BitsBadgeTier { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnBitsBadgeTierArgs"/> class.
    /// </summary>
    public OnBitsBadgeTierArgs(string channel, BitsBadgeTier bitsBadgeTier)
    {
        Channel = channel;
        BitsBadgeTier = bitsBadgeTier;
    }

    internal OnBitsBadgeTierArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        BitsBadgeTier = new(ircMessage);
    }
}
