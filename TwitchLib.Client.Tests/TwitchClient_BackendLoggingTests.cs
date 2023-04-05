using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelpers;
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
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(connectionCredentials, communicationClient, logger: logger);
            IClientMocker.SetIsConnected(Mock.Get<IClient>(communicationClient), true);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnSendReceiveDataArgs> assertion = Assert.Raises<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    () =>
                    {
                        client.OnSendReceiveData += (sender, args) => Assert.True(pauseCheck.Set());
                        // send is our trigger, to make the IClient-Mock raise OnMessage!
                        Assert.True(communicationClient.Send(String.Empty));
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.Equal(SendReceiveDirection.Received, assertion.Arguments.Direction);
            Assert.Equal(message, assertion.Arguments.Data);
        }
    }
}
