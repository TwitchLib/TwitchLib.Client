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
using TwitchLib.Client.Models.Interfaces;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials">
        ///     <inheritdoc cref="ConnectionCredentials"/>
        ///     <br></br>
        ///     see
        ///     <br></br>
        ///     <see cref="Models.ConnectionCredentials"/>
        /// </param>
        /// <param name="client">
        ///     <inheritdoc cref="IClient"/>
        ///     <br></br>
        ///     see
        ///     <br></br>
        ///     <see cref="IClient"/>
        /// </param>
        /// <param name="protocol">
        ///     <inheritdoc cref="ClientProtocol"/>
        ///     <br></br>
        ///     see
        ///     <br></br>
        ///     <see cref="ClientProtocol"/>
        /// </param>
        /// <param name="sendOptions">
        ///     <inheritdoc cref="ISendOptions"/>
        ///     <br></br>
        ///     by leaving it <see langword="null"/>,
        ///     <see langword="default"/> <see cref="SendOptions"/>
        ///     with the minimum <see cref="MessageRateLimit.Limit_20_in_30_Seconds"/>
        ///     is going to be applied
        /// </param>
        /// <param name="logger">
        ///     an <see cref="ILogger"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="credentials"/> is <see langword="null"/>
        /// </exception>
        public TwitchClient(ConnectionCredentials credentials,
                            IClient client = null,
                            ClientProtocol protocol = ClientProtocol.WebSocket,
                            ISendOptions sendOptions = null,
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
            //
            ConnectionStateManager.Subscribe(this);
            //
            ChannelManager = new ChannelManager(Client, Log, LogError, LOGGER);
            ChannelManager.Subscribe(this);
            // has to be done here,
            // cause credentials are also set into ChannelManager
            SetConnectionCredentials(credentials);
            ChatCommandIdentifiers.Add('!');
            if (sendOptions == null) sendOptions = new SendOptions((uint) MessageRateLimit.Limit_20_in_30_Seconds);
            ThrottlerService = new Services.ThrottlerService(Client,
                                                             this,
                                                             sendOptions,
                                                             LOGGER);
            ThrottlerService.Subscribe(this);
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
