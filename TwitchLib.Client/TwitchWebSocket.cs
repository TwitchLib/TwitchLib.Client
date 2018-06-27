using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using TwitchLib.Client.Interfaces;

#if NET452
using WebSocket4Net;
#endif

namespace TwitchLib.Client
{
#if NET452
    public class TwitchWebSocket : WebSocket, ITwitchWebSocket
    {
        public TwitchWebSocket(string uri, string subProtocol, WebSocketVersion version) : base(uri, subProtocol, version)
        {

        }
        public TwitchWebSocket(string uri, string subProtocol = "", List<KeyValuePair<string, string>> cookies = null, List<KeyValuePair<string, string>> customHeaderItems = null, string userAgent = "", string origin = "", WebSocketVersion version = WebSocketVersion.None, EndPoint httpConnectProxy = null, SslProtocols sslProtocols = SslProtocols.None, int receiveBufferSize = 0)
            : base(uri, subProtocol, cookies, customHeaderItems, userAgent, origin, version, httpConnectProxy, sslProtocols, receiveBufferSize)
        {

        }
    }
#endif
}