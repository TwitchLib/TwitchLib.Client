﻿using System;
using System.Collections.Generic;
using System.Text;
#if NET452
using SuperSocket.ClientEngine;
using WebSocket4Net;
#endif


namespace TwitchLib.Client.Interfaces
{
    public interface ITwitchWebSocket
    {
#if NET452
        event EventHandler<ErrorEventArgs> Error;
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler Closed;
        event EventHandler Opened;
        WebSocketState State { get; }
        IProxyConnector Proxy { get; set; }
        void Send(byte[] data, int offset, int length);
        void Send(string message);
        void Send(IList<ArraySegment<byte>> segments);
        void Close(int statusCode, string reason);
        void Close(string reason);
        void Close();
        void Dispose();
        void Open();
#endif
    }
}
