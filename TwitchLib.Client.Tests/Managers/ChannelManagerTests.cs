using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelper;
using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests.Managers
{
    public class ChannelManagerTests
    {
        /// <summary>
        ///     implicitly tests <see cref="ChannelManager.JoinChannel(System.String)"/>
        /// </summary>
        [Fact]
        public void JoinChannels_Incorrect_Test()
        {
            Mock<IClient> mock = new Mock<IClient>();
            IClient client = mock.Object;
            ILogger<ChannelManager> logger = TestLogHelper.GetLogger<ChannelManager>();
            ChannelManager channelManager = new ChannelManager(client, null, null, logger);
            string[] channels = new string[] {
                "",
                null,
                " "
            };
            channelManager.JoinChannels(channels);
            Assert.Empty(channelManager.AutoJoinChannels);
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinedChannels);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinChannelRequested);
        }
        [Theory]
        [InlineData("testchannel", "testchannel")]
        [InlineData("Testchannel", "testchannel")]
        [InlineData("testchannel ", "testchannel")]
        [InlineData("Testchannel ", "testchannel")]
        [InlineData(" testchannel ", "testchannel")]
        [InlineData(" Testchannel ", "testchannel")]
        [InlineData(" testchannel", "testchannel")]
        [InlineData(" Testchannel", "testchannel")]
        [InlineData("#testchannel", "testchannel")]
        [InlineData("#Testchannel", "testchannel")]
        [InlineData("#testchannel ", "testchannel")]
        [InlineData("#Testchannel ", "testchannel")]
        [InlineData(" #testchannel ", "testchannel")]
        [InlineData(" #Testchannel ", "testchannel")]
        [InlineData(" #testchannel", "testchannel")]
        [InlineData(" #Testchannel", "testchannel")]
        public void JoinChannels_Canceled_Test(string channel, string expected)
        {
            Mock<IClient> mock = new Mock<IClient>();
            IClient client = mock.Object;
            ILogger<ChannelManager> logger = TestLogHelper.GetLogger<ChannelManager>();
            ChannelManager channelManager = new ChannelManager(client, null, null, logger)
            {
                Credentials = new ConnectionCredentials("testusername", "testoauth")
            };
            //
            channelManager.JoinChannels(new string[] { channel });
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Single(channelManager.JoiningChannels);
            Assert.Equal(expected, channelManager.JoiningChannels.First());
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Start();
            Task.Delay(channelManager.JoinRequestDelay.Add(TimeSpan.FromMilliseconds(200))).GetAwaiter().GetResult();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Single(channelManager.JoinChannelRequested);
            Assert.Equal(expected, channelManager.JoinChannelRequested.First());
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.JoinCanceld(channel);
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Stop();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
        }
        [Theory]
        [InlineData("testchannel", "testchannel")]
        [InlineData("Testchannel", "testchannel")]
        [InlineData("testchannel ", "testchannel")]
        [InlineData("Testchannel ", "testchannel")]
        [InlineData(" testchannel ", "testchannel")]
        [InlineData(" Testchannel ", "testchannel")]
        [InlineData(" testchannel", "testchannel")]
        [InlineData(" Testchannel", "testchannel")]
        [InlineData("#testchannel", "testchannel")]
        [InlineData("#Testchannel", "testchannel")]
        [InlineData("#testchannel ", "testchannel")]
        [InlineData("#Testchannel ", "testchannel")]
        [InlineData(" #testchannel ", "testchannel")]
        [InlineData(" #Testchannel ", "testchannel")]
        [InlineData(" #testchannel", "testchannel")]
        [InlineData(" #Testchannel", "testchannel")]
        public void JoinChannels_Completed_Test(string channel, string expected)
        {
            Mock<IClient> mock = new Mock<IClient>();
            IClient client = mock.Object;
            ILogger<ChannelManager> logger = TestLogHelper.GetLogger<ChannelManager>();
            ChannelManager channelManager = new ChannelManager(client, null, null, logger)
            {
                Credentials = new ConnectionCredentials("testusername", "testoauth")
            };
            //
            channelManager.JoinChannels(new string[] { channel });
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Single(channelManager.JoiningChannels);
            Assert.Equal(expected, channelManager.JoiningChannels.First());
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Start();
            Task.Delay(channelManager.JoinRequestDelay.Add(TimeSpan.FromMilliseconds(200))).GetAwaiter().GetResult();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Single(channelManager.JoinChannelRequested);
            Assert.Equal(expected, channelManager.JoinChannelRequested.First());
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.JoinCompleted(channel);
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Single(channelManager.JoinedChannels);
            Assert.NotNull(channelManager.JoinedChannels.First());
            Assert.Equal(expected, channelManager.JoinedChannels.First().Channel);
            //
            JoinedChannel joinedChannel = channelManager.GetJoinedChannel(channel);
            Assert.NotNull(joinedChannel);
            Assert.Equal(expected, joinedChannel.Channel);
            //
            channelManager.Stop();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
        }
        [Theory]
        [InlineData("testchannel", "testchannel")]
        [InlineData("Testchannel", "testchannel")]
        [InlineData("testchannel ", "testchannel")]
        [InlineData("Testchannel ", "testchannel")]
        [InlineData(" testchannel ", "testchannel")]
        [InlineData(" Testchannel ", "testchannel")]
        [InlineData(" testchannel", "testchannel")]
        [InlineData(" Testchannel", "testchannel")]
        [InlineData("#testchannel", "testchannel")]
        [InlineData("#Testchannel", "testchannel")]
        [InlineData("#testchannel ", "testchannel")]
        [InlineData("#Testchannel ", "testchannel")]
        [InlineData(" #testchannel ", "testchannel")]
        [InlineData(" #Testchannel ", "testchannel")]
        [InlineData(" #testchannel", "testchannel")]
        [InlineData(" #Testchannel", "testchannel")]
        public void JoinChannels_Completed_Leave_Test(string channel, string expected)
        {
            Mock<IClient> mock = new Mock<IClient>();
            IClient client = mock.Object;
            ILogger<ChannelManager> logger = TestLogHelper.GetLogger<ChannelManager>();
            ChannelManager channelManager = new ChannelManager(client, null, null, logger)
            {
                Credentials = new ConnectionCredentials("testusername", "testoauth")
            };
            //
            channelManager.JoinChannels(new string[] { channel });
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Single(channelManager.JoiningChannels);
            Assert.Equal(expected, channelManager.JoiningChannels.First());
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Start();
            Task.Delay(channelManager.JoinRequestDelay.Add(TimeSpan.FromMilliseconds(200))).GetAwaiter().GetResult();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Single(channelManager.JoinChannelRequested);
            Assert.Equal(expected, channelManager.JoinChannelRequested.First());
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.JoinCompleted(channel);
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Single(channelManager.JoinedChannels);
            Assert.NotNull(channelManager.JoinedChannels.First());
            Assert.Equal(expected, channelManager.JoinedChannels.First().Channel);
            //
            JoinedChannel joinedChannel = channelManager.GetJoinedChannel(channel);
            Assert.NotNull(joinedChannel);
            Assert.Equal(expected, joinedChannel.Channel);
            channelManager.LeaveChannel(joinedChannel);
            Assert.Empty(channelManager.AutoJoinChannels);
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Stop();
            Assert.Empty(channelManager.AutoJoinChannels);
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
        }
        [Theory]
        [InlineData("testchannel", "testchannel")]
        [InlineData("Testchannel", "testchannel")]
        [InlineData("testchannel ", "testchannel")]
        [InlineData("Testchannel ", "testchannel")]
        [InlineData(" testchannel ", "testchannel")]
        [InlineData(" Testchannel ", "testchannel")]
        [InlineData(" testchannel", "testchannel")]
        [InlineData(" Testchannel", "testchannel")]
        [InlineData("#testchannel", "testchannel")]
        [InlineData("#Testchannel", "testchannel")]
        [InlineData("#testchannel ", "testchannel")]
        [InlineData("#Testchannel ", "testchannel")]
        [InlineData(" #testchannel ", "testchannel")]
        [InlineData(" #Testchannel ", "testchannel")]
        [InlineData(" #testchannel", "testchannel")]
        [InlineData(" #Testchannel", "testchannel")]
        public void JoinChannels_Leave_Before_CanceledCompleted_Test(string channel, string expected)
        {
            Mock<IClient> mock = new Mock<IClient>();
            IClient client = mock.Object;
            ILogger<ChannelManager> logger = TestLogHelper.GetLogger<ChannelManager>();
            ChannelManager channelManager = new ChannelManager(client, null, null, logger)
            {
                Credentials = new ConnectionCredentials("testusername", "testoauth")
            };
            //
            channelManager.JoinChannels(new string[] { channel });
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Single(channelManager.JoiningChannels);
            Assert.Equal(expected, channelManager.JoiningChannels.First());
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Start();
            Task.Delay(channelManager.JoinRequestDelay.Add(TimeSpan.FromMilliseconds(200))).GetAwaiter().GetResult();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Single(channelManager.JoinChannelRequested);
            Assert.Equal(expected, channelManager.JoinChannelRequested.First());
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.LeaveChannel(channel);
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Single(channelManager.JoinChannelRequested);
            Assert.Single(channelManager.JoiningChannelsExceptions);
            Assert.Equal(expected, channelManager.JoiningChannelsExceptions.First());
            Assert.Empty(channelManager.JoinedChannels);
            //
            channelManager.Stop();
            Assert.Single(channelManager.AutoJoinChannels);
            Assert.Equal(expected, channelManager.AutoJoinChannels.First());
            Assert.Empty(channelManager.JoiningChannels);
            Assert.Empty(channelManager.JoinChannelRequested);
            Assert.Empty(channelManager.JoiningChannelsExceptions);
            Assert.Empty(channelManager.JoinedChannels);
        }

        // TODO: add corner case tests (null, whitespace, ...)
    }
}
