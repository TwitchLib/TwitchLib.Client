# Changelog

## Version 4.0.0
### Addresses
##### Issues
- https://github.com/TwitchLib/TwitchLib.Client/issues/215
- https://github.com/TwitchLib/TwitchLib.Client/issues/206
- https://github.com/TwitchLib/TwitchLib.Client/issues/171
- https://github.com/TwitchLib/TwitchLib.Client/issues/207
- https://github.com/TwitchLib/TwitchLib.Client/issues/163
#### Pull Requests
- https://github.com/TwitchLib/TwitchLib.Client/pull/208
- https://github.com/TwitchLib/TwitchLib.Client/pull/213
- https://github.com/TwitchLib/TwitchLib.Client/pull/214

---
### Changes
---
#### ITwitchClient and TwitchClient
##### added
- `TwitchLib.Client.Interfaces.ITwitchClient` now provides all `public` `members` of `TwitchLib.Client.TwitchClient`
- the following methods now have `EventArgs`
    - `event EventHandler<EventArgs> OnSelfRaidError;`
    - `event EventHandler<EventArgs> OnNoPermissionError;`
    - `event EventHandler<EventArgs> OnRaidedChannelIsMatureAudience;`
- `event EventHandler<OnRaidNotificationArgs> OnUnRaidNotification;`
    - handles `MsgIds.UnRaid = "unraid"`
    - take care, both `events`, `OnRaid...` and `OnUnRaid...`, raise `OnRaidNotification` - what happens is determined by the `event` itself
- `bool HandleIrcMessage(string ircMessage)`
    - see `bool HandleIrcMessage(IrcMessage ircMessage)`
    - parses the given `ircMessage` as `IrcMessage` and calls `bool HandleIrcMessage(IrcMessage ircMessage)` with it
    - `return`s `false`, if the given `IrcMessage` could not be handled and `OnUnaccountedFor` would have been/was raised
        - `true` otherwise
- `bool HandleIrcMessage(IrcMessage ircMessage)`
    - handles the given `IrcMessage`
    - `return`s `false`, if the given `IrcMessage` could not be handled and `OnUnaccountedFor` would have been/was raised
        - `true` otherwise
- due to removal of initialization-stuff `void JoinChannels(IEnumerable<string> channels)` got added/introduced

##### removed
- `bool AutoReListenOnException { get; set; }`
    - has never been used
- `public MessageEmoteCollection ChannelEmotes => _channelEmotes;`
    - property, call ChannelEmotes with a `Type` of `MessageEmoteCollection`
        - none of it was correct
- `void OnReadLineTest(string rawIrc);`
    - got removed
    - please make use of
        - `ITwitchClient.HandleIrcMessage(string ircMessage)`
        - or `ITwitchClient.HandleIrcMessage(IrcMessage ircMessage)` in combination with `TwitchLib.Client.Parsers.IrcParser`
- `void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!');`
    - `ConnectionCredentials` got a constructor-arg
    - exclamationmark is added within ctor
        - can be removed via `ITwitchClient.RemoveChatCommandIdentifier(char identifier)`
    - channel can be added to 'auto-join-channels' via `ITwitchClient.JoinChannel(...)`
- `void Initialize(ConnectionCredentials credentials, List<string> channels, char chatCommandIdentifier = '!');`
    - `ConnectionCredentials` got a constructor-arg
    - exclamationmark is added within ctor
        - can be removed via `ITwitchClient.RemoveChatCommandIdentifier(char identifier)`
    - channels can be added to 'auto-join-channels' via `ITwitchClient.JoinChannels(...)`

##### changed
- constructor (ctor)
    - now takes `ConnectionCredentials` as first argument
        - can be changed, if the `ITwitchClient` is not connected, via `ITwitchClient.SetConnectionCredentials(ConnectionCredentials credentials)`
    - 
- `event EventHandler<OnDisconnectedEventArgs> OnDisconnected;`
    - is now `event EventHandler<OnDisconnectedArgs> OnDisconnected;`
    - now it has `TwitchLib.Client.Events.OnDisconnectedArgs` instead of `TwitchLib.Communication.Events.OnDisconnectedEventArgs`
- Parameter `bool overridecheck = false` is removed
    - channels cannot be joined more than once by a `TwitchClient`
- `event EventHandler<OnReconnectedEventArgs> OnReconnected;`
    - is now `event EventHandler<OnConnectedArgs> OnReconnected;`
    - now it has `TwitchLib.Client.Events.OnConnectedArgs` instead of `TwitchLib.Communication.Events.OnConnectedArgs`
- `events`
    - `OnFollowersOnly`
        - `OnFollowersOnlyArgs`
            - has a new property `IsOn`
            - that new property is `true` for the following `MsgIds`
                - `followers_on`
                - `followers_on_zero`
            - that new property is `false` for the following `MsgIds`
    - `OnSubsOnly`
        - `OnSubsOnlyArgs`
            - has a new property `IsOn`
            - that new property is `true` for the following `MsgIds`
                - `subs_on`
            - that new property is `false` for the following `MsgIds`
                - `subs_off`
    - `OnEmoteOnly`
        - `OnEmoteOnlyArgs`
            - has a new property `IsOn`
            - that new property is `true` for the following `MsgIds`
                - `emote_only_on`
            - that new property is `false` for the following `MsgIds`
                - `emote_only_off`
    - `OnSlowMode`
        - `OnSlowModeArgs`
            - has a new property `IsOn`
            - that new property is `true` for the following `MsgIds`
                - `slow_on`
            - that new property is `false` for the following `MsgIds`
                - `slow_off`
    - `OnR9kMode`
        - `OnR9kModeArgs`
            - has a new property `IsOn`
            - that new property is `true` for the following `MsgIds`
                - `r9k_on`
            - that new property is `false` for the following `MsgIds`
                - `r9k_off`
- according to changed `event`s mentioned before and to https://github.com/TwitchLib/TwitchLib.Client/issues/207
    - `ITwitchClient.OnChannelStateChanged` always raises the complete `TwitchLib.Client.Models.ChannelState`
    - the initial Status is received after joining a channel and held by `TwitchLib.Client.Models.JoinedChannel`
    - afterwards the `JoinedChannel` applies the received changes to the held `ChannelState` and raises the complete ones
- `ITwitchCLient.OnConnected` and `ITwitchClient.OnReConnected`
    - their `EventArgs` are `OnConnectedArgs`
    - yes, also `ITwitchClient.OnReConnected`
    - the `event` itself determines wether its a connect or reconnect
- according to https://dev.twitch.tv/docs/irc/#keepalive-messages
    - after receiving `IrcCommand.Pong` from twitch, `ITwitchClient` raises `OnError`

---

#### OnConnectedArgs
- removed public variable `public string AutoJoinChannel;`
- added public property `public IEnumerable<string> AutoJoinChannels { get; set; }`
    - `ITwitchClient`/`TwitchClient` can automatically join more than one channel when it connects

---

#### IrcParser
- went from `TwitchLib.Client.Internal.Parsing` to `TwitchLib.Client.Parsers`
- is now
    - `public`ly visible
    - `static`

---

#### IrcCommand
- went from `TwitchLib.Client.Enums.Internal` to `TwitchLib.Client.Enums`

---

#### ASubscriptionBase
- the following `class`es now have a common `abstract` `base`-`class` `TwitchLib.Client.Models.ASubscriptionBase`

---

#### SentMessage
- `TwitchLib.Client.Models.SentMessage` is gone
    - now, a `JoinedChannel` handles messages, `TwitchClient` sends
        - whenever we really send a message, we are going to get it back from twitch as `ChatMessage`
        - that 'returned' `ChatMessage` is going to be held by `JoinedChannel`
        - twitch is also going to send us a `USERSTATE`-Message, that contains the tag `id` with the unique twitch-message-id
        - that `USERSTATE`-message indicates we sent a message and which one we sent
        - if/whenever we receive the `USERSTATE`-message, `JoinedChannel` is going to raise the previously (determined by id) received `ChatMessage`

---

#### SentMessageBuilder
- `TwitchLib.Client.Models.Builders.SentMessageBuilder` is gone
    - cause `SentMessage` is gone

---

#### OnSentMessageEventArgs
- the `SentMessage` propertys `Type` has changed from `TwitchLib.Client.Models.SentMessage` to `TwitchLib.Client.Models.ChatMessage` due to changes mentioned before

---

#### EventInvocationExt
- `TwitchLib.Client.Extensions.EventInvocationExt.InvokeMessageSent(...)`
    - now takes the parameters to create a `ChatMessage` due to changes mentioned before

---

#### ClientNotConnectedException
- according to removal of `ITwitchClient` initialization-stuff, etsy

---

#### ClientNotInitializedException
- according to removal of `ITwitchClient` initialization-stuff, etsy

---

#### Whispers
- see also: https://discuss.dev.twitch.tv/t/deprecation-of-chat-commands-through-irc/40486
- everything related to `WHISPERS` is gone
    - `OnWhisperSentArgs`
    - `OnWhisperReceivedArgs`
    - `OnWhisperCommandReceivedArgs`
    - `WhisperMessage`
    - `WhisperCommand`
    - `OutboundWhisperMessage`
    - `WhisperMessageBuilder`
    - `WhisperCommandBuilder`
    - `OutboundWhisperMessageBuilder`

---

#### Commands
- see also: https://discuss.dev.twitch.tv/t/deprecation-of-chat-commands-through-irc/40486
- the following is gone
    - `TwitchLib.Client.Events`
        - `OnBadHostErrorArgs`
        - `OnBadHostRateExceededArgs`
        - `OnModeratorsReceivedArgs`
        - `OnVIPsReceivedArgs`
    - `TwitchLib.Client.Extensions`
        - `AnnouncementExt`
        - `BanUserExt`
        - `ChangeChatColorExt`
        - `ClearChatExt`
        - `CommercialExt`
        - `DeleteMessageExt`
        - `EmoteOnlyExt`
        - `FollowersOnlyExt`
        - `GetChannelModeratorsExt`
        - `MarkerExt`
        - `ModExt`
        - `RaidExt`
        - `ReplyWhisperExt`
        - `SlowModeExt`
        - `SubscribersOnlyExt`
        - `TimeoutUserExt`
        - `UnbanUserExt`
        - `VipExt`
        - corresponding invoker-methods within `EventInvocationExt`
    - `TwitchLib.Client.TwitchClient`/`TwitchLib.Client.ITwitchClient`
        - `event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;`
        - `event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;`
    - `TwitchLib.Client.Enums`
        - `CommercialLengths`

---

### Code Example

```
ConnectionCredentials credentials = new ConnectionCredentials("username", "auth-token");
ITwitchClient twitchClient = new TwitchClient(credentials);
twitchClient.RemoveChatCommandIdentifier('!');
twitchClient.AddChatCommandIdentifier('!');
// nothing happens, its already added
twitchClient.AddChatCommandIdentifier('!');
// each channel gets added to 'auto-join-channels' and is going to be re-joined after reconnecting with this instance of `ITwitchClient`
twitchClient.JoinChannel("testchannel");
// nothing happens, we already want to join or joined the "testchannel"
twitchClient.JoinChannel("testchannel");
twitchClient.JoinChannels(new string[]{"testchannel_a", "testchannel_b"});
// subscribe to events ...
// and connect
twitchClient.Connect();
...
```