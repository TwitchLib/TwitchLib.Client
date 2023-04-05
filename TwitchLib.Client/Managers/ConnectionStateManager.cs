using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace TwitchLib.Client.Managers
{
    /// <summary>
    ///     <see langword="class"/> to determine,
    ///     <br></br>
    ///     wether to raise <see cref="ITwitchClient.OnConnected"/>
    ///     <br></br>
    ///     or to raise <see cref="ITwitchClient.OnReconnected"/>
    ///     <br></br>
    ///     whenever <see cref="ITwitchClient"/> receives <see cref="Enums.IrcCommand.RPL_004"/>
    /// </summary>
    internal class ConnectionStateManager
    {
        #region properties public
        /// <summary>
        ///     <see langword="true"/>, if the Client was connected previously
        /// </summary>
        public bool WasConnected { get; private set; } = false;
        #endregion properties public


        #region ctor
        public ConnectionStateManager() { }
        #endregion ctor


        #region methods public

        #region subscribe to ITwitchClient-Events
        /// <summary>
        ///     subscribes to
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="ITwitchClient.OnConnected"/> to make a call to <see cref="SetConnected"/> and to indicate <see cref="ITwitchClient"/> was/is connected
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="twitchClient">
        ///     <see cref="ITwitchClient"/>
        /// </param>
        public void Subscribe(ITwitchClient twitchClient)
        {
            twitchClient.OnConnected += TwitchClientOnConnected;
        }
        #endregion subscribe to ITwitchClient-Events

        #region methods that should only be used for testing-purposes and by ITwitchClient-EventHandlers
        /// <summary>
        ///     sets <see cref="WasConnected"/> to <see langword="true"/>
        ///     <br></br>
        ///     should only be called after receiving <see cref="Enums.IrcCommand.RPL_004"/>
        /// </summary>
        public void SetConnected()
        {
            WasConnected = true;
        }
        #endregion methods that should only be used for testing-purposes and by ITwitchClient-EventHandlers


        /// <summary>
        ///     sets <see cref="WasConnected"/> to false
        ///     <br></br>
        ///     should only be called on manual <see cref="ITwitchClient.Disconnect()"/>
        ///     <br></br>
        ///     can not be made event-driven
        /// </summary>
        public void ResetConnected()
        {
            WasConnected = false;
        }
        #endregion methods public


        #region methods private

        #region ITwitchClient-EventHandlers
        private void TwitchClientOnConnected(object sender, OnConnectedArgs e) { SetConnected(); }
        #endregion ITwitchClient-EventHandlers

        #endregion methods private
    }
}
