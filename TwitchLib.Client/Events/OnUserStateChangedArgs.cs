using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <summary>
    /// Args representing on user state changed event.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    /// <inheritdoc />
    public class OnUserStateChangedArgs : EventArgs
    {
        /// <summary>
        /// Property representing user state object.
        /// </summary>
        public UserState UserState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnUserStateChangedArgs"/> class.
        /// </summary>
        public OnUserStateChangedArgs(UserState userState)
        {
            UserState = userState;
        }
    }
}
