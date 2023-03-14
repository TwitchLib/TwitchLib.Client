using System;

using TwitchLib.Client.Interfaces;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClient_ChannelTests : ATwitchClientTests<ITwitchClient_Channel>
    {
        [Fact]
        public void TwitchClient_Raises_OnJoinedChannel() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnChannelStateChanged() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnLeftChannel() { throw new NotImplementedException(); }
        [Fact]
        public void TwitchClient_Raises_OnFailureToReceiveJoinConfirmation() { throw new NotImplementedException(); }
    }
}
