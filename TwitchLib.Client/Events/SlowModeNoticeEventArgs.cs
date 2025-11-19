namespace TwitchLib.Client.Events;

public class SlowModeNoticeEventArgs : NoticeEventArgs
{
    public bool IsActive { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SlowModeNoticeEventArgs"/> class.
    /// </summary>
    public SlowModeNoticeEventArgs(string channel, string message, bool isActive) : base(channel, message)
    {
        IsActive = isActive;
    }
}