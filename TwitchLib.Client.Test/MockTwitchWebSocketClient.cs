using System;
using System.Collections.Generic;
using SuperSocket.ClientEngine;
using TwitchLib.Client.Interfaces;
using WebSocket4Net;

namespace TwitchLib.Client.Test
{
    public class MockTwitchWebSocket : ITwitchWebSocket
    {
        private WebSocketState _state;
        public WebSocketState State
        {
            get { return _state; }
        }

        public IProxyConnector Proxy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Closed;
        public event EventHandler Opened;

        public void Close(int statusCode, string reason)
        {
        }

        public void Close(string reason)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            _state = WebSocketState.Open;
            Opened?.Invoke(this, new EventArgs());
        }

        public void Send(byte[] data, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public void Send(string message)
        {
        }

        public void Send(IList<ArraySegment<byte>> segments)
        {
        }

        public void ReceiveMessage(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }
    }
}
