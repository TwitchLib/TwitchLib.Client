using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client a message was not sent because the channel is in r9k mode
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnR9kModeArgs : AChannelStateSwitchAbleEventArgs
    {
        // marker-class
    }
}