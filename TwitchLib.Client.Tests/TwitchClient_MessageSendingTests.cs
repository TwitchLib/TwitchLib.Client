using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_MessageSendingTests : ATwitchClientTests<ITwitchClient_MessageSending>
    {
        [Fact]
        public void TwitchClient_Raises_OnMessageSent() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnMessageThrottled()
        {
            uint sendsAllowedInPeriod = 0;
            TimeSpan throttlingTimeSpan = TimeSpan.FromSeconds(30);
            string messageNotSent = "This message has not been sent!";
            string reason = "Some reason";

            OnThrottledEventArgs onThrottledEventArgs = new OnThrottledEventArgs()
            {
                AllowedInPeriod = sendsAllowedInPeriod,
                SentCount = sendsAllowedInPeriod,
                Period = throttlingTimeSpan,
                ItemNotSent = messageNotSent,
                Reason = reason
            };

            OnMessageThrottledEventArgs onMessageThrottledEventArgs = new OnMessageThrottledEventArgs(onThrottledEventArgs);

            Mock<IClient> clientMock = new Mock<IClient>();

            clientMock.SetupAdd(c => c.OnMessageThrottled += It.IsAny<EventHandler<OnMessageThrottledEventArgs>>());
            clientMock.Setup(c => c.Send(It.IsAny<string>()))
                .Returns(false)
                .Raises(c => c.OnMessageThrottled += null, onMessageThrottledEventArgs);

            IClient communicationClient = clientMock.Object;

            // create one logger per test-method! - cause one file per test-method is generated
            ILogger<ITwitchClient> logger = TestLogHelper.GetLogger<ITwitchClient>();
            ITwitchClient client = new TwitchClient(communicationClient, logger: logger);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnMessageThrottledEventArgs> assertion = Assert.Raises<OnMessageThrottledEventArgs>(
                    h => client.OnMessageThrottled += h,
                    h => client.OnMessageThrottled -= h,
                    () =>
                    {
                        client.OnMessageThrottled += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_Username, TWITCH_OAuth));
                        communicationClient.Send(messageNotSent);
                        Assert.True(pauseCheck.WaitOne(WaitOneDuration));
                    });
            Assert.NotNull(assertion.Arguments);
            Assert.NotNull(assertion.Arguments.Reason);

            Assert.Equal(messageNotSent, assertion.Arguments.ItemNotSent);
            Assert.Equal(reason, assertion.Arguments.Reason);
            Assert.Equal(throttlingTimeSpan, assertion.Arguments.Period);
            Assert.Equal(sendsAllowedInPeriod, assertion.Arguments.AllowedInPeriod);
        }
    }
}
