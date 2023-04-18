using System;
using System.Threading.Tasks;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Test
{
    public class MockIClient : IClient
    {
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
        public event EventHandler<OnConnectedEventArgs> OnReconnected;

        public Task CloseAsync()
        {
            IsConnected = false;
            OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs());
            return Task.CompletedTask;
        }

        public void Dispose()
        { }

        public void Dispose(bool waitForSendsToComplete)
        { }

        public Task<bool> OpenAsync()
        {
            IsConnected = true;
            OnConnected?.Invoke(this, new OnConnectedEventArgs());
            return Task.FromResult(true);
        }

        public Task<bool> ReconnectAsync()
        {
            IsConnected = true;
            OnReconnected?.Invoke(this, new OnConnectedEventArgs());
            return Task.FromResult(true);
        }

        public void SendFailed(OnSendFailedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(OnErrorEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendAsync(string data)
        {
            return Task.FromResult(true);
        }

        public void ReceiveMessage(string message)
        {
            OnMessage?.Invoke(this, new OnMessageEventArgs { Message = message });
        }

        public bool SendWhisper(string data)
        {
            throw new NotImplementedException();
        }
    }
}
