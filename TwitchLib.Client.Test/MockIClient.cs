using System;

using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Test
{
    public class MockIClient : IClient
    {
        public void WhisperThrottled(OnWhisperThrottledEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public TimeSpan DefaultKeepAliveInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SendQueueLength => throw new NotImplementedException();

        public bool IsConnected { get; private set; }
        public IClientOptions Options { get; set; }

        public int WhisperQueueLength => throw new NotImplementedException();

        public event EventHandler<OnConnectedEventArgs> OnConnected;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnErrorEventArgs> OnError;
        public event EventHandler<OnFatalErrorEventArgs> OnFatality;
        public event EventHandler<OnMessageEventArgs> OnMessage;
        public event EventHandler<OnSendFailedEventArgs> OnSendFailed;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<OnReconnectedEventArgs> OnReconnected;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
        public event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;

        public void Close()
        {
            IsConnected = false;
            OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Open()
        {
            IsConnected = true;
            OnConnected?.Invoke(this, new OnConnectedEventArgs());
            return true;
        }

        public bool Reconnect()
        {
            IsConnected = true;
            OnReconnected?.Invoke(this, new OnReconnectedEventArgs());
            return true;
        }

        public void MessageThrottled(OnMessageThrottledEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public void SendFailed(OnSendFailedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(OnErrorEventArgs eventArgs)
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

        public bool SendWhisper(string data)
        {
            throw new NotImplementedException();
        }

        public bool SendPONG()
        {
            throw new NotImplementedException();
        }
    }
}
