using TwitchLib.Client.Interfaces;

namespace TwitchLib.Client.Managers
{
    internal class ConnectionStateManager
    {
        /// <summary>
        ///     <see langword="true"/>, if the Client was connected previously
        /// </summary>
        public bool WasConnected { get; private set; } = false;
        /// <summary>
        ///     sets <see cref="WasConnected"/> to <see langword="true"/>
        ///     <br></br>
        ///     should only be called after receiving <see cref="Enums.IrcCommand.RPL_004"/>
        /// </summary>
        public void SetConnected()
        {
            WasConnected = true;
        }

        /// <summary>
        ///     sets <see cref="WasConnected"/> to false
        ///     <br></br>
        ///     should only be called on manual <see cref="ITwitchClient.Disconnect()"/>
        /// </summary>
        public void ResetConnected()
        {
            WasConnected = false;
        }
    }
}
