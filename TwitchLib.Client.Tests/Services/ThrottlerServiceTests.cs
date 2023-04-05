using System;
using System.Threading;

using Moq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Services;
using TwitchLib.Communication.Events;

using Xunit;

namespace TwitchLib.Client.Tests.Services
{
    public class ThrottlerServiceTests
    {
        [Theory]
        [InlineData(false, 1, true, false)]
        [InlineData(true, 0, true, false)]
        [InlineData(true, 0, false, false)]
        [InlineData(true, 1, true, true)]
        [InlineData(true, 1, false, false)]
        public void EnqueTests(bool isConnected, uint queueCapacity, bool withMessage, bool expected)
        {
            uint sendsAllowedInPeriod = (uint) MessageRateLimit.Limit_20_in_30_Seconds;
            ISendOptions sendOptions = new SendOptions(sendsAllowedInPeriod, queueCapacity);
            Mock<ITwitchClient> twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(c => c.IsConnected)
                .Returns(isConnected);
            ITwitchClient twitchClient = twitchClientMock.Object;
            ThrottlerService throttlerService = new ThrottlerService(twitchClient, sendOptions);
            OutboundChatMessage? message = null;
            if (withMessage)
            {
                message = new OutboundChatMessage();
            }
#pragma warning disable CS8604 // null-literal: should get tested
            bool enqueued = throttlerService.Enqueue(message);
#pragma warning restore CS8604 // null-literal: should get tested
            Assert.Equal(expected, enqueued);
        }
        [Fact]
        public void StartTests()
        {
            uint sendsAllowedInPeriod = (uint) MessageRateLimit.Limit_20_in_30_Seconds;
            ISendOptions sendOptions = new SendOptions(sendsAllowedInPeriod);
            ITwitchClient twitchClient = new TwitchClient(new ConnectionCredentials("user", "auth"));
            ThrottlerService throttlerService = new ThrottlerService(twitchClient, sendOptions);
            try
            {
                ManualResetEvent pauseCheck = new ManualResetEvent(false);
                Assert.RaisedEvent<OnErrorEventArgs> assertion = Assert.Raises<OnErrorEventArgs>(
                    h => twitchClient.OnError += h,
                    h => twitchClient.OnError -= h,
                    () =>
                    {
                        twitchClient.OnError += (sender, args) => Assert.True(pauseCheck.Set());
                        throttlerService.Start(null, null);
                        Assert.False(pauseCheck.WaitOne(500));
                    });
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
                Assert.StartsWith("(No event was raised)", ex.Message);
            }
            {
                ManualResetEvent pauseCheck = new ManualResetEvent(false);
                Assert.RaisedEvent<OnErrorEventArgs> assertion = Assert.Raises<OnErrorEventArgs>(
                    h => twitchClient.OnError += h,
                    h => twitchClient.OnError -= h,
                    () =>
                    {
                        twitchClient.OnError += (sender, args) => Assert.True(pauseCheck.Set());
                        throttlerService.Start(null, null);
                        Assert.True(pauseCheck.WaitOne(5000));
                    });
                Assert.NotNull(assertion.Arguments);
            }
        }
    }
}
