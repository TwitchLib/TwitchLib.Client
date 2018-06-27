using System;
using TwitchLib.Client.Events;
using Xunit;

namespace TwitchLib.Client.Test
{
    public class TwitchClientEventTests
    {
        private const string TWITCH_BOT_USERNAME = "testuser";
        private const string TWITCH_CHANNEL = "testchannel";
        private readonly MockTwitchWebSocketClient _mockTwitchWebSocketClient;

        public TwitchClientEventTests()
        {
            _mockTwitchWebSocketClient = new MockTwitchWebSocketClient();
        }

        [Fact]
        public void ClientCanReceiveData()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            Assert.Raises<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        client.Connect();
                        _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
                    });
        }

        [Fact]
        public void ClientCanJoinChannels()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            client.OnConnected += (sender, e) =>
            {
                client.JoinChannel(TWITCH_CHANNEL);
                ReceivedRoomState();
            };

            Assert.Raises<OnJoinedChannelArgs>(
                   h => client.OnJoinedChannel += h,
                   h => client.OnJoinedChannel -= h,
                   () =>
                   {
                       client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                       client.Connect();
                       ReceivedTwitchConnected();
                   });

        }

        [Fact]
        public void ClientNewChatterRitualTest()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            client.OnConnected += (sender, e) => ReceivedUserNoticeMessage();

            Assert.Raises<OnRitualNewChatterArgs>(
                  h => client.OnRitualNewChatter += h,
                  h => client.OnRitualNewChatter -= h,
                  () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      ReceivedTwitchConnected();
                  });
        }

        [Fact]
        public void MessageEmoteCollectionFilled()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            var emoteCount = 0;
            client.OnConnected += (sender, e) => ReceivedUserNoticeMessage();
            client.OnMessageReceived += (sender, e) => emoteCount = e.ChatMessage.EmoteSet.Emotes.Count;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            client.Connect();
            ReceivedTwitchConnected();

            while (emoteCount == 0 && DateTime.Now < finish)
            { }

            Assert.NotEqual(0, emoteCount);
        }

        [Fact]
        public void ClientRaisesOnConnected()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);

            Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        client.Connect();
                        ReceivedTwitchConnected();
                    });
        }

        [Fact]
        public void ClientRaisesOnMessageReceived()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            
            Assert.Raises<OnMessageReceivedArgs>(
                  h => client.OnMessageReceived += h,
                  h => client.OnMessageReceived -= h,
                  () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      ReceivedTestMessage();
                  });
        }

        [Fact]
        public void ClientRaisesOnJoinedChannel()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);

            Assert.Raises<OnJoinedChannelArgs>(
                  h => client.OnJoinedChannel += h,
                  h => client.OnJoinedChannel -= h,
                  () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      ReceivedTwitchConnected();
                      client.JoinChannel(TWITCH_CHANNEL);
                      ReceivedRoomState();
                  });
        }

        [Fact]
        public void ClientChannelAddedToJoinedChannels()
        {
            var client = new TwitchClient(testClient: _mockTwitchWebSocketClient);
            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            client.Connect();
            ReceivedTwitchConnected();
            client.JoinChannel(TWITCH_CHANNEL);

            Assert.Equal(1, client.JoinedChannels.Count);
        }

        #region Messages for Tests

        private void ReceivedUserNoticeMessage()
        {
            _mockTwitchWebSocketClient.ReceiveMessage("@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\\sis\\snew\\shere.\\sSay\\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #testchannel kittyjinxu > #testchannel: HeyGuys");
        }

        private void ReceivedTestMessage()
        {
            _mockTwitchWebSocketClient.ReceiveMessage($"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");
        }

        private void ReceivedTwitchConnected()
        {
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            _mockTwitchWebSocketClient.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            _mockTwitchWebSocketClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            _mockTwitchWebSocketClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            _mockTwitchWebSocketClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
        }

        private void ReceivedRoomState()
        {
            _mockTwitchWebSocketClient.ReceiveMessage($"@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}");
        }

        #endregion
    }
}

