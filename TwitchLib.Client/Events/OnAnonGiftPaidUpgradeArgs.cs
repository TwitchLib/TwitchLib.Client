using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnAnonGiftPaidUpgradeArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The AnonGiftPaidUpgrade
    /// </summary>
    public AnonGiftPaidUpgrade AnonGiftPaidUpgrade { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnBitsBadgeTierArgs"/> class.
    /// </summary>
    public OnAnonGiftPaidUpgradeArgs(string channel, AnonGiftPaidUpgrade anonGiftPaidUpgrade)
    {
        Channel = channel;
        AnonGiftPaidUpgrade = anonGiftPaidUpgrade;
    }

    internal OnAnonGiftPaidUpgradeArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        AnonGiftPaidUpgrade = new(ircMessage);
    }
}