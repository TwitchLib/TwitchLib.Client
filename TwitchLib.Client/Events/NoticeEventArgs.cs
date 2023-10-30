namespace TwitchLib.Client.Events;

/// <summary>
/// EventArgs representing a NOTICE.
/// </summary>
public class NoticeEventArgs
{
    /// <summary>
    /// Property representing message send with the NOTICE
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Property representing channel bot is connected to.
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoticeEventArgs"/> class.
    /// </summary>
    public NoticeEventArgs(string channel, string message)
    {
        Message = message;
        Channel = channel;
    }
}
