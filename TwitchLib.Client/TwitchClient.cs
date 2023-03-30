using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client
{
    /// <summary>
    ///     Represents a client connected to a Twitch channel.
    ///     Implements the <see cref="TwitchLib.Client.Interfaces.ITwitchClient" />
    /// </summary>
    [SuppressMessage("Style", "IDE0058")]
    public partial class TwitchClient : ITwitchClient
    {
        #region Properties private
        private ISet<char> ChatCommandIdentifiers { get; } = new HashSet<char>();
        private ILogger<ITwitchClient> LOGGER { get; }
        private ConnectionStateManager ConnectionStateManager { get; } = new ConnectionStateManager();
        #endregion Properties private


        #region Properties public
        public Version Version => Assembly.GetEntryAssembly().GetName().Version;
        public string TwitchUsername => ConnectionCredentials?.TwitchUsername;
        public bool DisableAutoPong { get; set; } = false;
        public bool WillReplaceEmotes { get; set; } = false;
        #endregion Properties public


        #region ctor
        public TwitchClient(ConnectionCredentials credentials,
                            IClient client = null,
                            ClientProtocol protocol = ClientProtocol.WebSocket,
                            ILogger<ITwitchClient> logger = null)
        {
            LOGGER = logger;
            LOGGER?.TraceMethodCall(GetType());
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials), "ConnectionCredentials are mandatory");
            }
            Protocol = protocol;
            Client = client;
            if (Client == null)
            {
                switch (Protocol)
                {
                    case ClientProtocol.TCP:
                        Client = new TcpClient();
                        break;
                    case ClientProtocol.WebSocket:
                        Client = new WebSocketClient();
                        break;
                }
            }
            Debug.Assert(Client != null, nameof(Client) + " != null");
            InitializeClient();
            ChannelManager = new ChannelManager(Client, Log, LogError, LOGGER);
            ChannelManager.Subscribe(this);
            // has to be done here,
            // cause credentials are also set into ChannelManager
            SetConnectionCredentials(credentials);
            ChatCommandIdentifiers.Add('!');
        }
        #endregion ctor


        #region Command Identifiers

        public void AddChatCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (identifier != '\0')
                ChatCommandIdentifiers.Add(identifier);
        }

        public void RemoveChatCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(GetType());
            ChatCommandIdentifiers.Remove(identifier);
        }
        #endregion

    }
}
