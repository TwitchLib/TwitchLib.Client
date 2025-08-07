using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

var loggerFactory = LoggerFactory.Create(c => c
    .AddConsole()
    //.SetMinimumLevel(LogLevel.Trace) // uncomment to view raw messages received from twitch
);

var credentials = new ConnectionCredentials(); // anonymous user, add Username and OAuth token to get the ability to send messages
var client = new TwitchClient(loggerFactory: loggerFactory)
{
    ChatCommandIdentifiers = { "!", "?" }, // you can customize the command identifiers, if not set, defaults to '!'
};

client.Initialize(credentials);
client.OnConnected += Client_OnConnected;
client.OnJoinedChannel += Client_OnJoinedChannel;
client.OnMessageReceived += Client_OnMessageReceived;

client.OnChatCommandReceived += Client_OnChatCommandReceived;

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

async Task Client_OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
{
    var channel = e.ChatMessage.Channel;
    var command = e.Command;

    switch (command.Name.ToLower())
    {
        case "info":
            await client.SendMessageAsync(channel, "Hi, I am a bot created using TwitchLib.Client.");
            break;
        case "repeat":
            if (string.IsNullOrWhiteSpace(command.ArgumentsAsString))
                return;
            await client.SendMessageAsync(channel, command.ArgumentsAsString);
            break;
        case "leave":
            if (!e.ChatMessage.IsBroadcaster)
                return;
            await client.SendMessageAsync(channel, "Goodbye! I am leaving the channel.");
            await client.LeaveChannelAsync(channel);
            break;
        case "help":
            if (command.ArgumentsAsList.Count is 0)
            {
                var helpText = "Available commands: !info, !repeat <message>, !help [command]";
                if (e.ChatMessage.IsBroadcaster)
                    helpText += ", !leave";

                await client.SendMessageAsync(channel, helpText);
                return;
            }
            switch (command.ArgumentsAsList[0])
            {
                case "info":
                    await client.SendMessageAsync(channel, "The !info command provides information about the bot.");
                    break;
                case "repeat":
                    await client.SendMessageAsync(channel, "The !repeat command repeats the message you provide.");
                    break;
                case "help":
                    await client.SendMessageAsync(channel, "The !help command provides information about available commands.");
                    break;
                case "leave":
                    if (!e.ChatMessage.IsBroadcaster)
                        return;

                    await client.SendMessageAsync(channel, "The !leave command makes the bot leave the channel.");
                    break;
                default:
                    await client.SendMessageAsync(channel, $"No help available for command: {command.ArgumentsAsList[0]}");
                    break;
            }
            break;
        default:
            Console.WriteLine($"Unknown command: {command.Name}");
            break;
    }
}
