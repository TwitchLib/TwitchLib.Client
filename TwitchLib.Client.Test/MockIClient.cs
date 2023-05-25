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

        public event AsyncEventHandler<OnConnectedEventArgs> OnConnected;
        public event AsyncEventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event AsyncEventHandler<OnErrorEventArgs> OnError;
        public event AsyncEventHandler<OnFatalErrorEventArgs> OnFatality;
        public event AsyncEventHandler<OnMessageEventArgs> OnMessage;
        public event AsyncEventHandler<OnSendFailedEventArgs> OnSendFailed;
        public event AsyncEventHandler<OnConnectedEventArgs> OnReconnected;

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

        public async Task<bool> OpenAsync()
        {
            IsConnected = true;
            await OnConnected?.Invoke(this, new OnConnectedEventArgs());
            return true;
        }

        public async Task<bool> ReconnectAsync()
        {
            IsConnected = true;
            await OnReconnected?.Invoke(this, new OnConnectedEventArgs());
            return true;
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

        public async Task ReceiveMessage(string message)
        {

           await OnMessage?.Invoke(this, new OnMessageEventArgs(message));

        }

        public bool SendWhisper(string data)
        {
            throw new NotImplementedException();
        }
    }
}
