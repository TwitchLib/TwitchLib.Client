namespace TwitchLib.Client.Events;

/// <summary>
/// Args representing on connected event.
/// Implements the <see cref="System.EventArgs" />
/// </summary>
public class OnConnectedEventArgs : EventArgs
{
    /// <summary>
    /// Property representing bot username.
    /// </summary>
    public string BotUsername { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnConnectedEventArgs"/> class.
    /// </summary>
    public OnConnectedEventArgs(string botUsername)
    {
        BotUsername = botUsername;
    }
}
