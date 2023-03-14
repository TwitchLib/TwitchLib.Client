using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Tests.Helpers;
using TwitchLib.Communication.Interfaces;

using TwitchLib.Communication.Tests.Helper;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_MessageReceivingTests : ATwitchClientTests<ITwitchClient_MessageReceiving>
    {
        [Fact]
        public void TwitchClient_Raises_OnMessageReceived()
        {
            string message = $"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE";

            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnMessageReceivedArgs>(
                    h => client.OnMessageReceived += h,
                    h => client.OnMessageReceived -= h,
                    () =>
                    {
                        client.OnMessageReceived += (sender, args) =>
                        {
                            // here, we dont want to test the IrcParser
                            // but we want to check at least msg-id
                            Assert.NotNull(args);
                            Assert.NotNull(args.ChatMessage);
                            Assert.NotNull(args.ChatMessage.Id);
                            Assert.Equal("fefffeeb-1e87-4adf-9912-ca371a18cbfd", args.ChatMessage.Id);
                            Assert.True(pauseCheck.Set());
                        };
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
        }
    }
}
