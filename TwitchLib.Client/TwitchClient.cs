using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Consts.Internal;
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
        #region Private Variables
        private ISet<char> ChatCommandIdentifiers { get; } = new HashSet<char>();
        private ILogger<ITwitchClient> LOGGER { get; }

        #endregion

        #region Public Variables
        public Version Version => Assembly.GetEntryAssembly().GetName().Version;
        public string TwitchUsername { get; private set; }
        public bool DisableAutoPong { get; set; } = false;
        public bool WillReplaceEmotes { get; set; } = false;
        #endregion

        #region Construction Work

        public TwitchClient(IClient client = null, ClientProtocol protocol = ClientProtocol.WebSocket, ILogger<ITwitchClient> logger = null)
        {
            LOGGER = logger;
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
        }

        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        public void Initialize(ConnectionCredentials credentials,
                               string channel = null,
                               char chatCommandIdentifier = '!',
                               char whisperCommandIdentifier = '!')
        {
            LOGGER?.TraceMethodCall(GetType());
            if (channel != null && channel[0] == '#')
                channel = channel.Substring(1);
            InitializeHelper(credentials, new List<string>() { channel }, chatCommandIdentifier);
        }

        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessageParameter)]
        public void Initialize(ConnectionCredentials credentials,
                               List<string> channels,
                               char chatCommandIdentifier = '!',
                               char whisperCommandIdentifier = '!')
        {
            LOGGER?.TraceMethodCall(GetType());
            channels = channels.Select(x => x[0] == '#' ? x.Substring(1) : x).ToList();
            InitializeHelper(credentials, channels, chatCommandIdentifier);
        }

        private void InitializeHelper(ConnectionCredentials credentials,
                                      List<string> channels,
                                      char chatCommandIdentifier = '!')
        {
            LOGGER?.TraceMethodCall(GetType());
            Log($"TwitchLib-TwitchClient initialized, assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");
            ConnectionCredentials = credentials;
            TwitchUsername = ConnectionCredentials.TwitchUsername;
            if (chatCommandIdentifier != '\0')
                ChatCommandIdentifiers.Add(chatCommandIdentifier);

            ChannelManager.JoinChannels(channels);
        }

        #endregion

        #region Command Identifiers

        public void AddChatCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (!IsInitialized)
                HandleNotInitialized();
            ChatCommandIdentifiers.Add(identifier);
        }

        public void RemoveChatCommandIdentifier(char identifier)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (!IsInitialized)
                HandleNotInitialized();
            ChatCommandIdentifiers.Remove(identifier);
        }
        #endregion

    }
}
