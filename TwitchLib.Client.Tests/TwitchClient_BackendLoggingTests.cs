using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_BackendLoggingTests : ATwitchClientTests<ITwitchClient_BackendLogging>
    {
        [Fact]
        public void TwitchClient_Raises_OnSendReceiveData()
        {
            string message = $":tmi.twitch.tv 001 {TWITCH_Username} :Welcome, GLHF!";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.Raises<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    () =>
                    {
                        client.OnSendReceiveData += (sender, args) =>
                        {
                            Assert.NotNull(args);
                            Assert.Equal(SendReceiveDirection.Received, args.Direction);
                            Assert.Equal(message, args.Data);
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
