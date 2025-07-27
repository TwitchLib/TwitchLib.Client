using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

var loggerFactory = LoggerFactory.Create(c => c
    .AddConsole()
    //.SetMinimumLevel(LogLevel.Trace) // uncomment to view raw messages received from twitch
);

var credentials = new ConnectionCredentials(); // anonymous user, add Username and OAuth token to get the ability to send messages
var client = new TwitchClient(loggerFactory: loggerFactory);

client.Initialize(credentials);
client.OnConnected += Client_OnConnected;
client.OnJoinedChannel += Client_OnJoinedChannel;
client.OnMessageReceived += Client_OnMessageReceived;

await client.ConnectAsync();
await Task.Delay(Timeout.Infinite);


async Task Client_OnConnected(object? sender, OnConnectedEventArgs e)
{
    await client.JoinChannelAsync("channel_name"); // replace with the channel you want to join
}

async Task Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
{
    Console.WriteLine($"Connected to {e.Channel}");
    await client.SendMessageAsync(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
}

async Task Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
{
    Console.WriteLine($"{e.ChatMessage.Username}#{e.ChatMessage.Channel}: {e.ChatMessage.Message}");
}
