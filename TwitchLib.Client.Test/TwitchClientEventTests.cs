using System;
using TwitchLib.Websockets;
using Xunit;

namespace TwitchLib.Client.Test
{
    public class TwitchClientEventTests
    {
        private const string TWITCH_BOT_USERNAME = "testuser";
        private const string TWITCH_CHANNEL = "testchannel";

        [Fact]
        public void ClientCanReceiveData()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocketClient();
            var client = new TwitchClient(testClient: websocket);

            var onSendReceiveDataFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnSendReceiveData += (sender, e) =>

            {
                onSendReceiveDataFired = true;
            };


            client.Connect();
            websocket.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");

            while (!onSendReceiveDataFired && DateTime.Now < finish)
            {

            }

            Assert.True(onSendReceiveDataFired);
        }

        [Fact]
        public void ClientConnectedTest()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocketClient();
            var client = new TwitchClient(testClient: websocket);

            var onConnectedFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnConnected += (sender, e) =>
            {
                onConnectedFired = true;
            };
         
            client.Connect();
            websocket.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            websocket.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            websocket.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            websocket.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            websocket.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
            
            while (!onConnectedFired && DateTime.Now< finish)
            {

            }
          
            Assert.True(onConnectedFired);
        }
        
        [Fact]
        public void ClientCanJoinChannels()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocketClient();
            var client = new TwitchClient(testClient: websocket);

            var onJoinChannelFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnConnected += (sender, e) =>
            {
                client.JoinChannel(TWITCH_CHANNEL);

                websocket.ReceiveMessage($"@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}");
            };

            client.OnJoinedChannel += (sender, e) =>
            {
                onJoinChannelFired = true;
            };


            client.Connect();
            websocket.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            websocket.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            websocket.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            websocket.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            websocket.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");

            while (!onJoinChannelFired && DateTime.Now < finish)
            {

            }

            Assert.True(onJoinChannelFired);
        }

        [Fact]
        public void ClientNewChatterRitualTest()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocketClient();
            var client = new TwitchClient(testClient: websocket);

            var newChatterRitualFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnRitualNewChatter += (sender, e) =>
            {
                newChatterRitualFired = true;
            };

            client.OnConnected += (sender, e) =>
            {
                websocket.ReceiveMessage(
                    "@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\\sis\\snew\\shere.\\sSay\\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #thorlar kittyjinxu > #thorlar: HeyGuys");
            };

            client.Connect();
            websocket.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            websocket.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            websocket.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            websocket.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            websocket.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            websocket.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            websocket.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");

            while (!newChatterRitualFired && DateTime.Now < finish)
            {

            }

            Assert.True(newChatterRitualFired);
        }
    }
}
