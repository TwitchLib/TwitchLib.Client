using System;

using TwitchLib.Client.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_MessageSendingTests : ATwitchClientTests<ITwitchClient_MessageSending>
    {
        [Fact]
        public void TwitchClient_Raises_OnMessageSent() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnMessageThrottled() { throw new NotImplementedException(); }
    }
}
