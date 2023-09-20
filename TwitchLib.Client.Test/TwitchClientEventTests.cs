using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using Xunit;

namespace TwitchLib.Client.Test
{
    public class TwitchClientEventTests
    {
        private const string TWITCH_BOT_USERNAME = "testuser";
        private const string TWITCH_CHANNEL = "testchannel";
        private readonly MockIClient _mockClient;

        public TwitchClientEventTests()
        {
            _mockClient = new MockIClient();
        }

        [Fact]
        public async Task ClientCanReceiveData()
        {
            var client = new TwitchClient(_mockClient);
            await MyAssert.RaisesAsync<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    async () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        await client.ConnectAsync();
                        await _mockClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
                    });
        }

        [Fact]
        public async Task ClientCanJoinChannels()
        {
            var client = new TwitchClient(_mockClient);
            client.OnConnected += async (sender, e) =>
            {
                await client.JoinChannelAsync(TWITCH_CHANNEL);
                await ReceivedRoomState();
            };

            await MyAssert.RaisesAsync<OnJoinedChannelArgs>(
                   h => client.OnJoinedChannel += h,
                   h => client.OnJoinedChannel -= h,
                   async () =>
                   {
                       client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                       await client.ConnectAsync();
                       await ReceivedTwitchConnected();
                   });

        }

        [Fact]
        public async Task MessageEmoteCollectionFilled()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var client = new TwitchClient(_mockClient);
            var emoteCount = 0;
            client.OnConnected += (sender, e) => ReceivedTestMessage();
            client.OnMessageReceived += async (sender, e) =>  emoteCount = e.ChatMessage.EmoteSet.Emotes.Count;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            await client.ConnectAsync();
            await ReceivedTwitchConnected();

            while (emoteCount == 0 && DateTime.Now < finish)
            { }

            Assert.NotEqual((double)0, emoteCount, 0);
        }

        [Fact]
        public async void ClientRaisesOnConnected()
        {
            var client = new TwitchClient(_mockClient);

            await MyAssert.RaisesAsync<Events.OnConnectedEventArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    async () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        await client.ConnectAsync();
                        await ReceivedTwitchConnected();
                    });
        }

        [Fact]
        public async Task ClientRaisesOnMessageReceived()
        {
            var client = new TwitchClient(_mockClient);

            await MyAssert.RaisesAsync<OnMessageReceivedArgs>(
                  h => client.OnMessageReceived += h,
                  h => client.OnMessageReceived -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      await client.ConnectAsync();
                      await ReceivedTestMessage();
                  });
        }

        [Fact]
        public async Task ClientRaisesOnJoinedChannel()
        {
            var client = new TwitchClient(_mockClient);

            await MyAssert.RaisesAsync<OnJoinedChannelArgs>(
                  h => client.OnJoinedChannel += h,
                  h => client.OnJoinedChannel -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      await client.ConnectAsync();
                      await ReceivedTwitchConnected();
                      await client.JoinChannelAsync(TWITCH_CHANNEL);
                      await ReceivedRoomState();
                  });
        }

        [Fact]
        public async Task ClientChannelAddedToJoinedChannels()
        {
            var client = new TwitchClient(_mockClient);
            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            await client.ConnectAsync();
            await ReceivedTwitchConnected();
            await client.JoinChannelAsync(TWITCH_CHANNEL);

            Assert.Equal((double)1, client.JoinedChannels.Count,0);
        }

        [Fact]
        public async Task ClientRaisesOnDisconnected()
        {
            var client = new TwitchClient(_mockClient);

            await MyAssert.RaisesAsync<OnDisconnectedEventArgs>(
                  h => client.OnDisconnected += h,
                  h => client.OnDisconnected -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      await client.ConnectAsync();
                      await ReceivedTwitchConnected();
                      await client.JoinChannelAsync(TWITCH_CHANNEL);
                      await ReceivedRoomState();
                      await client.DisconnectAsync();
                  });
        }

        [Fact]
        public async Task ClientReconnectsOk()
        {
            var client = new TwitchClient(_mockClient);
            var pauseConnected = new ManualResetEvent(false);
            var pauseReconnected = new ManualResetEvent(false);

            await MyAssert.RaisesAsync<Events.OnConnectedEventArgs>(
                h => client.OnReconnected += h,
                h => client.OnReconnected -= h,
                async () =>
                {
                    client.OnConnected += async (s, e) =>
                    {
                        pauseConnected.Set();
                        await client.DisconnectAsync();
                    };

                    client.OnDisconnected += async (s, e) => { await client.ReconnectAsync(); };
                    client.OnReconnected += async (s, e) => { pauseReconnected.Set(); };

                    client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                    await client.ConnectAsync();
                    await ReceivedTwitchConnected();

                    Assert.True(pauseConnected.WaitOne(5000));
                    Assert.True(pauseReconnected.WaitOne(60000));
                });
        }

        #region Messages for Tests
        private async Task ReceivedUserNoticeMessage()
        {
            await _mockClient.ReceiveMessage("@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\\sis\\snew\\shere.\\sSay\\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #testchannel kittyjinxu > #testchannel: HeyGuys");
        }

        private async Task ReceivedTestMessage()
        {
            await _mockClient.ReceiveMessage("@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");
            // await _mockClient.ReceiveMessage(":jtv!jtv@jtv.tmi.twitch.tv PRIVMSG (HOSTED):(HOSTER) is now hosting you for (VIEWERS_TOTAL) viewers.");
        }

        private async Task ReceivedTwitchConnected()
        {
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
        }

        private async Task ReceivedRoomState()
        {
            await _mockClient.ReceiveMessage($"@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}");
        }
        #endregion
    } 
}
