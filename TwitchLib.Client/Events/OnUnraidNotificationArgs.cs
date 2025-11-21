using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnUnraidNotificationArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The unraid notification
    /// </summary>
    public UnraidNotification RaidNotification { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnUnraidNotificationArgs"/> class.
    /// </summary>
    public OnUnraidNotificationArgs(string channel, UnraidNotification unraidNotification)
    {
        Channel = channel;
        RaidNotification = unraidNotification;
    }

    internal OnUnraidNotificationArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        RaidNotification = new(ircMessage);
    }
}
