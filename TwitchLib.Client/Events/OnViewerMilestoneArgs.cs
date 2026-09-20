using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnViewerMilestoneArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The ViewerMilestone
    /// </summary>
    public ViewerMilestone ViewerMilestone { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnRaidNotificationArgs"/> class.
    /// </summary>
    public OnViewerMilestoneArgs(string channel, ViewerMilestone viewerMilestone)
    {
        Channel = channel;
        ViewerMilestone = viewerMilestone;
    }

    internal OnViewerMilestoneArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        ViewerMilestone = new(ircMessage);
    }
}
