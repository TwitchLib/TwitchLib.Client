namespace TwitchLib.Client.Events.Abstracts
{
    public abstract class AChannelStateSwitchAbleEventArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; }
        public bool IsOn { get; } = false;
        public AChannelStateSwitchAbleEventArgs(string channel, string message, bool isOn) : base(channel)
        {
            Message = message;
            IsOn = isOn;
        }
    }
}
