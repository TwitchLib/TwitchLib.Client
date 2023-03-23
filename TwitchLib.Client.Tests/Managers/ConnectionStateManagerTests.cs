using TwitchLib.Client.Managers;

using Xunit;

namespace TwitchLib.Client.Tests.Managers
{
    public class ConnectionStateManagerTests
    {
        [Fact]
        public void CompleteTest()
        {
            ConnectionStateManager stateManager = new ConnectionStateManager();
            Assert.False(stateManager.WasConnected);
            stateManager.SetConnected();
            Assert.True(stateManager.WasConnected);
            stateManager.ResetConnected();
            Assert.False(stateManager.WasConnected);
        }
    }
}
