using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Events;

public class OnRitualArgs : EventArgs
{
    /// <summary>
    /// The channel
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The ritual
    /// </summary>
    public Ritual Ritual { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnRitualArgs"/> class.
    /// </summary>
    public OnRitualArgs(string channel, Ritual ritual)
    {
        Channel = channel;
        Ritual = ritual;
    }

    internal OnRitualArgs(IrcMessage ircMessage)
    {
        Channel = ircMessage.Channel;
        Ritual = new(ircMessage);
    }
}
