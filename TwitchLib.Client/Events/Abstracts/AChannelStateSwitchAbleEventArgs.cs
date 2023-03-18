namespace TwitchLib.Client.Events.Abstracts
{
    public abstract class AChannelStateSwitchAbleEventArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string Message { get; set; }
        public bool IsOn { get; set; }
    }
}
