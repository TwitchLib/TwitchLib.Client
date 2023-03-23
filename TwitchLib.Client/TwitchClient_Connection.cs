using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Connection
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region properties public
        public bool IsConnected => IsInitialized && Client != null && Client.IsConnected;
        public ConnectionCredentials ConnectionCredentials { get; private set; }
        #endregion properties public


        #region events public
        public event EventHandler<OnConnectedArgs> OnConnected;
        public event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;
        public event EventHandler<OnDisconnectedArgs> OnDisconnected;
        public event EventHandler<OnConnectionErrorArgs> OnConnectionError;
        public event EventHandler<OnConnectedArgs> OnReconnected;
        #endregion events public


        #region methods public
        public bool Connect()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Connection));
            if (!IsInitialized) HandleNotInitialized();
            Log($"Connecting to: {ConnectionCredentials.TwitchWebsocketURI}");

            if (Client.Open())
            {
                Log("Should be connected!");
                return true;
            }
            // ChannelManager gets started via TwitchClient_Client.Client_OnConnected!
            return false;
        }
        public void Disconnect()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Connection));
            Log("Disconnect Twitch Chat Client...");

            if (!IsInitialized) HandleNotInitialized();
            Client.Close();
            ChannelManager.Stop();
        }
        public void Reconnect()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Connection));
            if (!IsInitialized) HandleNotInitialized();
            Log($"Reconnecting to Twitch");
            Client.Reconnect();
        }
        public void SetConnectionCredentials(ConnectionCredentials credentials)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Connection));
            if (!IsInitialized)
                HandleNotInitialized();
            if (IsConnected)
                throw new IllegalAssignmentException("While the client is connected, you are unable to change the connection credentials. Please disconnect first and then change them.");

            ConnectionCredentials = credentials;
        }
        #endregion methods public


        #region methods protected
        protected static void HandleNotConnected()
        {
            throw new ClientNotConnectedException("In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.");
        }
        #endregion methods protected
    }
}
