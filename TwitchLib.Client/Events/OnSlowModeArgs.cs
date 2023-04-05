using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client a message was not sent because its in slow mode.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnSlowModeArgs : AChannelStateSwitchAbleEventArgs
    {
        public OnSlowModeArgs(string channel, string message, bool isOn) : base(channel, message, isOn) { }
    }
}
