using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Serilog.Events;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.RealWorld.TestHelpers;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests.RealWorld;

public class ClientRunnerTest {
    private static readonly ILogger<ITwitchClient> LOGGER = TestLogHelper.GetLogger<ITwitchClient>(typeof(ClientRunnerTest),
                                                                                                   LogEventLevel.Information,
                                                                                                   callerMemberName: typeof(ClientRunnerTest).Name);
    private string TwitchUsername => throw new NotImplementedException();
    private string TwitchOAuth => throw new NotImplementedException();
    private IEnumerable<string> Channels => throw new NotImplementedException();
    /// <summary>
    ///     just to see what happens within this time span
    /// </summary>
    private int WaitSecondsAfterDisconnect => 60;
    /// <summary>
    ///     with a value &gt; 5
    ///     <br></br>
    ///     PING-PONG appears in Log-File
    /// </summary>
    private int ListenDurationInMinutes => 1;
    /// <summary>
    ///     its rather a run and read the log-file test than a run and try to dynamically assert stuff test
    /// </summary>
    [Fact(Skip = "should be run manually")]
    public void Run() {
        ConnectionCredentials credentials = new ConnectionCredentials(this.TwitchUsername, this.TwitchOAuth);
        IClient communicationClient = new WebSocketClient(logger: LOGGER);
        ITwitchClient twitchClient = new TwitchClient(credentials: credentials,
                                                      client: communicationClient,
                                                      logger: LOGGER);
        twitchClient.JoinChannels(this.Channels);
        twitchClient.OnConnected += this.OnConnected;
        twitchClient.OnDisconnected += this.OnDisconnected;
        twitchClient.OnReconnected += this.OnReconnected;
        twitchClient.OnJoinedChannel += this.OnJoinedChannel;
        twitchClient.OnUserJoined += this.OnUserJoined;
        twitchClient.OnSendReceiveData += this.OnSendReceiveData;
        twitchClient.OnExistingUsersDetected += this.OnExistingUsersDetected;
        // initial connect
        Assert.True(twitchClient.Connect());
        Task.Delay(TimeSpan.FromMinutes(this.ListenDurationInMinutes)).GetAwaiter().GetResult();
        // reconnect while connected
        twitchClient.Reconnect();
        Task.Delay(TimeSpan.FromMinutes(this.ListenDurationInMinutes)).GetAwaiter().GetResult();
        // disconnect gracefully
        twitchClient.Disconnect();
        Task.Delay(TimeSpan.FromSeconds(this.WaitSecondsAfterDisconnect)).GetAwaiter().GetResult();
        // reconnect while disconnected
        twitchClient.Reconnect();
        Task.Delay(TimeSpan.FromMinutes(this.ListenDurationInMinutes)).GetAwaiter().GetResult();
        // disconnect gracefully
        twitchClient.Disconnect();
        Task.Delay(TimeSpan.FromSeconds(this.WaitSecondsAfterDisconnect)).GetAwaiter().GetResult();
        // connect gracefully again
        Assert.True(twitchClient.Connect());
        Task.Delay(TimeSpan.FromMinutes(this.ListenDurationInMinutes)).GetAwaiter().GetResult();
        // disconnect gracefully
        twitchClient.Disconnect();
        Task.Delay(TimeSpan.FromSeconds(this.WaitSecondsAfterDisconnect)).GetAwaiter().GetResult();
    }
    private void OnConnected(object? sender, OnConnectedArgs args) {
        LOGGER.LogInformation("connected");
    }
    private void OnDisconnected(object? sender, OnDisconnectedArgs args) {
        LOGGER.LogInformation("disconnected");
    }
    private void OnReconnected(object? sender, OnConnectedArgs args) {
        LOGGER.LogInformation("reconnected");
    }
    private void OnJoinedChannel(object? sender, OnJoinedChannelArgs args) {
        LOGGER.LogInformation("#{channel} joined", args.Channel);
    }
    private void OnLeftChannel(object? sender, OnLeftChannelArgs args) {
        LOGGER.LogInformation("#{channel} left", args.Channel);
    }
    private void OnUserJoined(object? sender, OnUserJoinedArgs args) {
        LOGGER.LogInformation("{username} joined #{channel}", args.Username, args.Channel);
    }
    private void OnUserLeft(object? sender, OnUserLeftArgs args) {
        LOGGER.LogInformation("{username} left #{channel}", args.Username, args.Channel);
    }
    private void OnExistingUsersDetected(object? sender, OnExistingUsersDetectedArgs args) {
        LOGGER.LogInformation("{username} detected in #{channel}", String.Join(", ", args.Users), args.Channel);
    }
    private void OnSendReceiveData(object? sender, OnSendReceiveDataArgs args) {
        LOGGER.LogInformation("data {direction}: {irc}", args.Direction, args.Data);
    }

}
