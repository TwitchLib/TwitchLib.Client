using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to <see cref="IClient"/>
    /// </summary>
    public interface ITwitchClient_Client
    {
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        bool IsInitialized { get; }
    }
}
