using System;
using System.Threading;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    /// <summary>
    ///     <see href="https://github.com/Moq/moq4/wiki/Quickstart"/>
    ///     <br></br>
    ///     <br></br>
    ///     <see href="https://github.com/Moq/moq4/wiki/Quickstart#events"/>
    /// </summary>
    public class TwitchClientEventTests
    {
        private static string TWITCH_Username => "Username";
        private static string TWITCH_OAuth => "OAuth";
        private static string TWITCH_CHANNEL => "testchannel";
        private static int WaitOneDuration => 5_000;

        public TwitchClientEventTests() { }

        [Fact]
        public void ClientCanReceiveData()
        {
            string msg = $":tmi.twitch.tv 001 {TWITCH_Username} :Welcome, GLHF!";

            Mock<IClient> mock = new Mock<IClient>();
            mock.SetupAdd(c => c.OnConnected += It.IsAny<EventHandler<OnConnectedEventArgs>>());
            mock.Setup<bool>(c => c.Open())
                .Returns(true)
                .Raises(c => c.OnConnected += null, new OnConnectedEventArgs());

            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            mock.Setup(c => c.Send(It.IsAny<string>())).Returns(true)
                .Raises(c => c.OnMessage += null, this, new OnMessageEventArgs() { Message = msg });

            ManualResetEvent pauseCheck = new ManualResetEvent(false);

            IClient CommunicationClient = mock.Object;
            TwitchClient client = new TwitchClient(CommunicationClient);
            Assert.Raises<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    () =>
                    {
                        client.OnSendReceiveData += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.Equal(SendReceiveDirection.Received, args.Direction);
                            Assert.Equal(msg, args.Data);
                            pauseCheck.Set();
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        Assert.True(client.Connect());
                        pauseCheck.WaitOne(WaitOneDuration);
                    });
        }

        //[Fact]
        //public void ClientCanJoinChannels()
        //{
        //    TwitchClient client = new TwitchClient(CommunicationClient);
        //    client.OnConnected += (sender, e) =>
        //    {
        //        client.JoinChannel(TWITCH_CHANNEL);
        //        ReceivedRoomState();
        //    };

        //    Assert.Raises<OnJoinedChannelArgs>(
        //           h => client.OnJoinedChannel += h,
        //           h => client.OnJoinedChannel -= h,
        //           () =>
        //           {
        //               client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //               client.Connect();
        //               ReceivedTwitchConnected();
        //           });

        //}

        //[Fact]
        //public void MessageEmoteCollectionFilled()
        //{
        //    DateTime finish = DateTime.Now.AddSeconds(10);
        //    TwitchClient client = new TwitchClient(_mockClient);
        //    int emoteCount = 0;
        //    client.OnConnected += (sender, e) => ReceivedTestMessage();
        //    client.OnMessageReceived += (sender, e) => emoteCount = e.ChatMessage.EmoteSet.Emotes.Count;

        //    client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //    client.Connect();
        //    ReceivedTwitchConnected();

        //    while (emoteCount == 0 && DateTime.Now < finish)
        //    { }

        //    Assert.NotEqual(0, emoteCount);
        //}

        //[Fact]
        //public void ClientRaisesOnConnected()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);

        //    Assert.Raises<OnConnectedArgs>(
        //            h => client.OnConnected += h,
        //            h => client.OnConnected -= h,
        //            () =>
        //            {
        //                client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //                client.Connect();
        //                ReceivedTwitchConnected();
        //            });
        //}

        //[Fact]
        //public void ClientRaisesOnMessageReceived()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);

        //    Assert.Raises<OnMessageReceivedArgs>(
        //          h => client.OnMessageReceived += h,
        //          h => client.OnMessageReceived -= h,
        //          () =>
        //          {
        //              client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //              client.Connect();
        //              ReceivedTestMessage();
        //          });
        //}

        //[Fact]
        //public void ClientRaisesOnJoinedChannel()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);

        //    Assert.Raises<OnJoinedChannelArgs>(
        //          h => client.OnJoinedChannel += h,
        //          h => client.OnJoinedChannel -= h,
        //          () =>
        //          {
        //              client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //              client.Connect();
        //              ReceivedTwitchConnected();
        //              client.JoinChannel(TWITCH_CHANNEL);
        //              ReceivedRoomState();
        //          });
        //}

        //[Fact]
        //public void ClientChannelAddedToJoinedChannels()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);
        //    client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //    client.Connect();
        //    ReceivedTwitchConnected();
        //    client.JoinChannel(TWITCH_CHANNEL);

        //    Assert.Equal(1, client.JoinedChannels.Count);
        //}

        //[Fact]
        //public void ClientRaisesOnDisconnected()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);

        //    Assert.Raises<OnDisconnectedEventArgs>(
        //          h => client.OnDisconnected += h,
        //          h => client.OnDisconnected -= h,
        //          () =>
        //          {
        //              client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //              client.Connect();
        //              ReceivedTwitchConnected();
        //              client.JoinChannel(TWITCH_CHANNEL);
        //              ReceivedRoomState();
        //              client.Disconnect();
        //          });
        //}

        //[Fact]
        //public void ClientReconnectsOk()
        //{
        //    TwitchClient client = new TwitchClient(_mockClient);
        //    ManualResetEvent pauseConnected = new ManualResetEvent(false);
        //    ManualResetEvent pauseReconnected = new ManualResetEvent(false);

        //    Assert.Raises<OnReconnectedEventArgs>(
        //        h => client.OnReconnected += h,
        //        h => client.OnReconnected -= h,
        //        () =>
        //        {
        //            client.OnConnected += (s, e) =>
        //            {
        //                pauseConnected.Set();
        //                client.Disconnect();
        //            };

        //            client.OnDisconnected += (s, e) => client.Reconnect();
        //            client.OnReconnected += (s, e) => pauseReconnected.Set();

        //            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
        //            client.Connect();
        //            ReceivedTwitchConnected();

        //            Assert.True(pauseConnected.WaitOne(5000));
        //            Assert.True(pauseReconnected.WaitOne(60000));
        //        });
        //}

        //#region Messages for Tests
        //private void ReceivedUserNoticeMessage()
        //{
        //    _mockClient.ReceiveMessage("@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\\sis\\snew\\shere.\\sSay\\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #testchannel kittyjinxu > #testchannel: HeyGuys");
        //}

        //private void ReceivedTestMessage()
        //{
        //    _mockClient.ReceiveMessage($"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");
        //}

        //private void ReceivedTwitchConnected()
        //{
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
        //    _mockClient.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
        //    _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
        //    _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
        //    _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
        //}

        //private void ReceivedRoomState()
        //{
        //    _mockClient.ReceiveMessage($"@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}");
        //}

        //#endregion
    }
}
