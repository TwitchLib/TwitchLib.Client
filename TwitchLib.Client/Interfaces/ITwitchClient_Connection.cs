using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everything related to the Connection itself
    /// </summary>
    public interface ITwitchClient_Connection
    {
        #region properties public
        /// <summary>
        ///     <see cref="Models.ConnectionCredentials"/>
        /// </summary>
        ConnectionCredentials ConnectionCredentials { get; }
        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        bool IsConnected { get; }
        #endregion properties public


        #region events public
        /// <summary>
        ///     Occurs when [on connected].
        /// </summary>
        event EventHandler<OnConnectedArgs> OnConnected;
        /// <summary>
        ///     Occurs when [on connection error].
        /// </summary>
        event EventHandler<OnConnectionErrorArgs> OnConnectionError;
        /// <summary>
        ///     Occurs when [on disconnected].
        /// </summary>
        event EventHandler<OnDisconnectedArgs> OnDisconnected;
        /// <summary>
        ///     Occurs when [on incorrect login].
        /// </summary>
        event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;
        /// <summary>
        ///     Occurs when [on reconnected].
        /// </summary>
        event EventHandler<OnReconnectedEventArgs> OnReconnected;
        #endregion events public


        #region methods public
        /// <summary>
        ///     Sets the connection credentials.
        /// </summary>
        /// <param name="credentials">
        ///     <see cref="Models.ConnectionCredentials"/>
        /// </param>
        void SetConnectionCredentials(ConnectionCredentials credentials);
        /// <summary>
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if a connection could be established; <see langword="false"/> otherwise
        /// </returns>
        bool Connect();
        /// <summary>
        ///     Disconnects this instance.
        /// </summary>
        void Disconnect();
        /// <summary>
        ///     Reconnects this instance.
        /// </summary>
        void Reconnect();
        #endregion methods public
    }
}
