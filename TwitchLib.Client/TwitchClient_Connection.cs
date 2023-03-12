using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Connection
    {
        public bool IsConnected => IsInitialized && Client != null && Client.IsConnected;
        public ConnectionCredentials ConnectionCredentials { get; private set; }
        public event EventHandler<OnConnectedArgs> OnConnected;
        public event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnConnectionErrorArgs> OnConnectionError;
        public event EventHandler<OnReconnectedEventArgs> OnReconnected;
        public bool Connect()
        {
            if (!IsInitialized) HandleNotInitialized();
            Log($"Connecting to: {ConnectionCredentials.TwitchWebsocketURI}");

            // Clear instance data
            JoinedChannelManager.Clear();

            if (Client.Open())
            {
                Log("Should be connected!");
                return true;
            }
            return false;
        }
        public void Disconnect()
        {
            Log("Disconnect Twitch Chat Client...");

            if (!IsInitialized) HandleNotInitialized();
            Client.Close();

            // Clear instance data
            JoinedChannelManager.Clear();
        }
        public void Reconnect()
        {
            if (!IsInitialized) HandleNotInitialized();
            Log($"Reconnecting to Twitch");
            Client.Reconnect();
        }
        public void SetConnectionCredentials(ConnectionCredentials credentials)
        {
            if (!IsInitialized)
                HandleNotInitialized();
            if (IsConnected)
                throw new IllegalAssignmentException("While the client is connected, you are unable to change the connection credentials. Please disconnect first and then change them.");

            ConnectionCredentials = credentials;
        }

        protected static void HandleNotConnected()
        {
            throw new ClientNotConnectedException("In order to perform this action, the client must be connected to Twitch. To confirm connection, try performing this action in or after the OnConnected event has been fired.");
        }
    }
}
