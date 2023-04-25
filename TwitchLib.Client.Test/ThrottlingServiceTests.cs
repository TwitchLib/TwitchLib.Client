using Moq;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Throttling;
using TwitchLib.Communication.Interfaces;
using Xunit;

namespace TwitchLib.Client.Test
{
    public class ThrottlingServiceTests
    {
        [Theory]
        [InlineData(false, 1, true, false)]
        [InlineData(true, 0, true, false)]
        [InlineData(true, 0, false, false)]
        [InlineData(true, 1, true, true)]
        [InlineData(true, 1, false, false)]
        public void EnqueueMessageTests(bool isConnected, uint queueCapacity, bool withMessage, bool expected)
        {
            var sendOptions = new SendOptions(20, queueCapacity);
            var clientMock = new Mock<IClient>();
            clientMock.Setup(c => c.IsConnected)
                .Returns(isConnected);
            var client = clientMock.Object;
            var throttlerService = new ThrottlingService(client, sendOptions);
            OutboundChatMessage message = null;
            
            if (withMessage)
            {
                message = new OutboundChatMessage();
            }
            
            var enqueued = throttlerService.Enqueue(message);
            Assert.Equal(expected, enqueued);
        }
    }
}