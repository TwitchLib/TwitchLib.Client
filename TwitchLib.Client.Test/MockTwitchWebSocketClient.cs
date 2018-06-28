using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using TwitchLib.WebSocket;
using TwitchLib.WebSocket.Events;

namespace TwitchLib.Client.Test
{
    public class MockTwitchWebSocketClient : IWebsocketClient
    {
        public TimeSpan DefaultKeepAliveInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SendQueueLength => throw new NotImplementedException();

        public WebSocketState State => WebSocketState.Open;

        public event EventHandler<OnConnectedEventArgs> OnConnected;
        public event EventHandler<OnDataEventArgs> OnData;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnErrorEventArgs> OnError;
        public event EventHandler<OnFatalErrorEventArgs> OnFatality;
        public event EventHandler<OnMessageEventArgs> OnMessage;
        public event EventHandler<OnSendFailedEventArgs> OnSendFailed;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;

        public void Close(WebSocketCloseStatus reason)
        {
            OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs { Reason = reason });
        }

        public void Close()
        {
            OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs { Reason = WebSocketCloseStatus.Empty });
        }

        public void Dispose()
        { }

        public void Dispose(bool waitForSendsToComplete)
        { }

        public bool Open()
        {
            OnConnected?.Invoke(this, new OnConnectedEventArgs());
            return true;
        }

        public bool Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool Send(string data)
        {
            return true;
        }

        public void ReceiveMessage(string message)
        {
            OnMessage?.Invoke(this, new OnMessageEventArgs { Message = message });
        }
    }
}
