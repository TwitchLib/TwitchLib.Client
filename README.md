# TwitchLib.Client
Client component of TwitchLib.

> [!TIP]
> With the introduction of Chat on EventSub, it is recommended to upgrade your chatbots that are using Twitch IRC to use EventSub (for reading chat messages and roomstates) and Twitch API (for sending chat messages).

## Installation

| NuGet            |       | [![TwitchLib.Client][1]][2]                                       |
| :--------------- | ----: | :---------------------------------------------------------------- |
| Package Manager  | `PM>` | `Install-Package TwitchLib.Client -Version 4.0.1`                 |
| .NET CLI         | `>`   | `dotnet add package TwitchLib.Client --version 4.0.1`             |
| PackageReference |       | `<PackageReference Include="TwitchLib.Client" Version="4.0.1" />` |
| Paket CLI        | `>`   | `paket add TwitchLib.Client --version 4.0.1`                      |

[1]: https://img.shields.io/nuget/v/TwitchLib.Client.svg?label=TwitchLib.Client
[2]: https://www.nuget.org/packages/TwitchLib.Client

## ⚠ Breaking Changes in Version 4.0.1 ⚠

Version 4.0.1 contains breaking changes.
- Removed obsolete methods.
- Methods are now asynchronous. (The return value changed from `void` to `Task` and gains `Async` suffix)
- Events are now asynchronous (return value changed from `void` to `Task`)
- `Add/RemoveChatCommandIdentifier` methods were removed, use `ChatCommandIdentifiers` property instead (same applies to whisper);
- `OnLog` event was removed (you can still use `ILoggerFactory` to get logs)
- removed builders classes (removed `TwitchLib.Client.Models.Builders namespace`)
- changed public fields to properties
- rewritten all models in `TwitchLib.Client.Models`
    - some props/classes can be slightly renamed
    - some properties (`IsModerator`, `IsSubscriber`, `HasTurbo`, `IsVip`, `IsPartner`, `IsStaff`) moved to the `UserDetails` property

## Minimal Setup
Step 1: Create a new Console project

Step 2: Install the `TwitchLib.Client` NuGet package. (See above on how to do that)

Step 2.5: (Optional) Install the [`Microsoft.Extensions.Logging.Console`](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) NuGet package (or some other compatible logging provider) to see logs.

Step 3: Add the following code to your `Program.cs` file:
```cs
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

// Optional logger
var loggerFactory = LoggerFactory.Create(c => c
    .AddConsole()
//    .SetMinimumLevel(LogLevel.Trace) // uncomment to view raw messages received from twitch
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
```
Step 4: Change the `channel_name` in the `Client_OnConnected` method to the name of the channel you want to join and run the application.

More complete examples can be found in the [TwitchLib.Client.Example](/TwitchLib.Client.Example/)
