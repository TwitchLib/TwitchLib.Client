using System;
using System.Reflection;

using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Parsers;

using Xunit;

namespace TwitchLib.Client.Models.Test;
public class ChannelStateTests
{
    [Theory]
    [InlineData("emote-only", "1", "EmoteOnly", true)]
    [InlineData("r9k", "1", "R9K", true)]
    [InlineData("subs-only", "1", "SubOnly", true)]
    [InlineData("slow", "60", "SlowMode", 60)]
    [InlineData("slow", "-1", "SlowMode", 0)]
    [InlineData("followers-only", "0", "FollowersOnly", 0)]
    [InlineData("followers-only", "60", "FollowersOnly", 60)]
    [InlineData("followers-only", "-1", "FollowersOnly", null)]
    public void ChannelStateTest(string key, string value, string propertyName, object expectation)
    {
        string roomId = "1";
        string channel = "testchannel";
        string initialRawIRC = $"@emote-only=0;followers-only=-1;r9k=0;room-id={roomId};slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{channel}";
        IrcMessage initialIrcMessage = IrcParser.ParseIrcMessage(initialRawIRC);
        ChannelState state = new ChannelState(initialIrcMessage);
        Assert.Equal(channel, state.Channel);
        Assert.Equal(roomId, state.RoomId);
        Assert.False(state.EmoteOnly);
        Assert.False(state.R9K);
        Assert.False(state.SubOnly);
        Assert.Null(state.FollowersOnly);
        Assert.Equal(0, state.SlowMode);

        string roomIdOther = "9";
        string channelOther = "anothertestchannel";
        string changeRawIRC = $"@room-id={roomIdOther};{key}={value} :tmi.twitch.tv ROOMSTATE #{channelOther}";
        IrcMessage changeIrcMessage = IrcParser.ParseIrcMessage(changeRawIRC);
        state.Apply(changeIrcMessage);
        TypeInfo typeInfo = state.GetType().GetTypeInfo();
        PropertyInfo? property = typeInfo.GetProperty(propertyName);
        Assert.NotNull(property);
        object? actual = property.GetValue(state);
        if (expectation == null)
        {
            Assert.Null(actual);
        }
        else
        {
            Assert.NotNull(actual);
            if (String.Equals("followers-only", key))
            {
                Assert.Equal(TimeSpan.FromMinutes((int) expectation), actual);
            }
            else
            {
                Assert.Equal(expectation, actual);
            }
        }

        Assert.Equal(channel, state.Channel);
        Assert.Equal(roomId, state.RoomId);

        Assert.NotEqual(channelOther, state.Channel);
        Assert.NotEqual(roomIdOther, state.RoomId);
    }
}
