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
    public string BotUsername;
}
