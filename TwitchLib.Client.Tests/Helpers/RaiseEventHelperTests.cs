using System.Threading;

using TwitchLib.Client.Events;
using TwitchLib.Client.Helpers;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Tests.TestHelper;

using TwitchLib.Communication.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests.Helpers
{
    public class RaiseEventHelperTests
    {
        [Fact]
        public void RaiseEventTest()
        {
            string message = "unused but needed";
            IClient communicationClient = IClientMocker.GetMessageRaisingICLient(message);
            ITwitchClient client = new TwitchClient(communicationClient);
            ManualResetEvent pauseCheck = new ManualResetEvent(false);
            Assert.RaisedEvent<OnConnectedArgs> assertion = Assert.Raises<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    () =>
                    {
                        client.OnConnected += (sender, args) => Assert.True(pauseCheck.Set());
                        client.Initialize(new ConnectionCredentials("testusername", "testoauth"));
                        RaiseEventHelper.RaiseEvent(client, nameof(client.OnConnected), new OnConnectedArgs());
                        Assert.True(pauseCheck.WaitOne(5_000));
                    });
            Assert.NotNull(assertion.Arguments);
        }
    }
}
