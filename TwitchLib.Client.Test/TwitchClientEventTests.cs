using System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitchLib.Client.Test
{
    [TestClass]
    public class TwitchClientEventTests
    {
        private const string TWITCH_BOT_USERNAME = "testuser";
        private const string TWITCH_CHANNEL = "testchannel";

        [TestMethod]
        public void ClientCanReceiveData()
        {

            var Finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocket();
            var client = new TwitchClient(websocket, null);

            var onSendReceiveDataFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnSendReceiveData += (object sender, Events.OnSendReceiveDataArgs e) =>
            {
                onSendReceiveDataFired = true;
            };


            client.Connect();
            websocket.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");

            while (!onSendReceiveDataFired && DateTime.Now < Finish)
            {

            }


            Assert.IsTrue(onSendReceiveDataFired);

        }

        [TestMethod]
        public void ClientConnectedTest()
        {
            var Finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocket();
            var client = new TwitchClient(websocket, null);

            var onConnectedFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnConnected += (object sender, Events.OnConnectedArgs e) =>
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
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
            
            while (!onConnectedFired && DateTime.Now< Finish)
            {

            }

          
            Assert.IsTrue(onConnectedFired);

        }


        [TestMethod]
        public void ClientCanJoinChannels()
        {
            var Finish = DateTime.Now.AddSeconds(10);
            var websocket = new MockTwitchWebSocket();
            var client = new TwitchClient(websocket, null);

            var onJoinChannelFired = false;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));

            client.OnConnected += (object sender, Events.OnConnectedArgs e) =>
            {
                client.JoinChannel(TWITCH_CHANNEL);
                
                websocket.ReceiveMessage($":{TWITCH_BOT_USERNAME}!{TWITCH_BOT_USERNAME}@{TWITCH_BOT_USERNAME}.tmi.twitch.tv JOIN #{TWITCH_CHANNEL}");
            };

            client.OnJoinedChannel += (object sender, Events.OnJoinedChannelArgs e) =>
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
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            websocket.ReceiveMessage($":tmi.twitch.tv CAP * ACK :twitch.tv/tags");

            while (!onJoinChannelFired && DateTime.Now < Finish)
            {

            }


            Assert.IsTrue(onJoinChannelFired);

        }
    }
}
