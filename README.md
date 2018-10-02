# TwitchLib.Client
Client component of TwitchLib.

For a general overview and example, refer to https://github.com/TwitchLib/TwitchLib/blob/master/README.md

Throttler Example
```csharp
    var clientOptions = new ClientOptions
    {
        MessagesAllowedInPeriod = 100,
        ThrottlingPeriod = TimeSpan.FromSeconds(60)
    };
    var customClient = new WebSocketClient(clientOptions);

    var client = new TwitchClient(customClient);
```
