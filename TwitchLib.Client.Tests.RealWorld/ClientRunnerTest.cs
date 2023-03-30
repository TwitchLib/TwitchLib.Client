using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

using Xunit;

namespace TwitchLib.Client.Tests.RealWorld
{
    public class ClientRunnerTest
    {
        private string TwitchUsername => throw new NotImplementedException();
        private string TwitchOAuth => throw new NotImplementedException();
        private IEnumerable<string> Channels => throw new NotImplementedException();
        [Fact]
        public void Run()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(TwitchUsername, TwitchOAuth);
            ITwitchClient twitchClient = new TwitchClient(credentials);
            twitchClient.JoinChannels(Channels);
            twitchClient.OnConnected += OnConnected;
            twitchClient.OnDisconnected += OnDisconnected;
            twitchClient.OnReconnected += OnReconnected;
            twitchClient.OnJoinedChannel += OnJoinedChannel;
            twitchClient.OnUserJoined += OnUserJoined;
            twitchClient.OnSendReceiveData += OnSendReceiveData;
            twitchClient.Connect();
            // TODO: assertions...
            // TODO: disconnect
            // TODO: connect
            // TODO: reconnect
        }
        private void OnConnected(object? sender, OnConnectedArgs args) { }
        private void OnDisconnected(object? sender, OnDisconnectedArgs args) { }
        private void OnReconnected(object? sender, OnConnectedArgs args) { }
        private void OnJoinedChannel(object? sender, OnJoinedChannelArgs args) { }
        private void OnUserJoined(object? sender, OnUserJoinedArgs args) { }
        private void OnSendReceiveData(object? sender, OnSendReceiveDataArgs args) { }

    }
}
