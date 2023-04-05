using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing a NOTICE telling the client a message was not sent because its subs only mode.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnSubsOnlyArgs : AChannelStateSwitchAbleEventArgs
    {
        public OnSubsOnlyArgs(string channel, string message, bool isOn) : base(channel, message, isOn) { }
    }
}
