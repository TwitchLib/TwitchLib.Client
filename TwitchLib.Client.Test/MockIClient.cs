using System;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Test
{
    public class MockIClient : IClient
    {
        public TimeSpan DefaultKeepAliveInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SendQueueLength => throw new NotImplementedException();

        public bool IsConnected => true;

        public int WhisperQueueLength => throw new NotImplementedException();

        public event EventHandler<OnConnectedEventArgs> OnConnected;
        public event EventHandler<OnDataEventArgs> OnData;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnErrorEventArgs> OnError;
        public event EventHandler<OnFatalErrorEventArgs> OnFatality;
        public event EventHandler<OnMessageEventArgs> OnMessage;
        public event EventHandler<OnSendFailedEventArgs> OnSendFailed;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
        public event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;

        public void Close()
        {
            OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs());
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
    }
}
