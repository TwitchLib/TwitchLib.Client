using System.Reflection;

using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Parsers;

using Xunit;

namespace TwitchLib.Client.Tests.Models
{
    public class JoinedChannelTests
    {
        private static string Channel => "testchannel";
        private static string MessageIdHash => "msg_id_hash";
        /// <summary>
        ///     only tests handling
        ///     <br></br>
        ///     event-raising is testet over there:
        ///     <br></br>
        ///     <br></br>
        ///     <seealso cref="TwitchClient_MessageReceivingTests.TwitchClient_Raises_OnUserStateChanged"/>
        ///     <br></br>
        ///     <seealso cref="TwitchClient_MessageSendingTests.TwitchClient_Raises_OnMessageSent"/>
        /// </summary>
        [Theory]
        [InlineData("botusername", "testusername", 0)]
        [InlineData("botusername", "botusername", 1)]
        public void UserState_And_SentMessage_Test(string botusername, string senderUsername, int expectedSize)
        {
            ITwitchClient client = new TwitchClient(new ConnectionCredentials(botusername, "testoauth"));
            JoinedChannel joinedChannel = new JoinedChannel(Channel, botusername);
            ChatMessage chatMessage = GetChatMessage(senderUsername);
            joinedChannel.HandlePRIVMSG(chatMessage);
            Assert.Equal(expectedSize, joinedChannel.UnRaisedSentMessages.Count);
            UserState userStateJoin = GetUserState(true, senderUsername);
            joinedChannel.HandleUSERSTATE(userStateJoin, client);
            Assert.Equal(expectedSize, joinedChannel.UnRaisedSentMessages.Count);
            UserState userStateSent = GetUserState(false, senderUsername);
            joinedChannel.HandleUSERSTATE(userStateSent, client);
            Assert.Empty(joinedChannel.UnRaisedSentMessages);
        }
        /// <summary>
        ///     only tests handling
        ///     <br></br>
        ///     event-raising is testet over there:
        ///     <br></br>
        ///     <br></br>
        ///     <see cref="TwitchClient_ChannelTests.TwitchClient_Raises_OnChannelStateChanged(System.String)"/>
        /// </summary>
        [Fact]
        public void RoomState_Test()
        {
            string roomId = "1";
            string channel = "testchannel";
            string botUsername = "botUsername";
            JoinedChannel joinedChannel = new JoinedChannel(channel, botUsername);
            // initial irc after joining a channel
            string initialRawIRC = $"@emote-only=0;followers-only=-1;r9k=0;room-id={roomId};slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{channel}";
            IrcMessage initialIrcMessage = IrcParser.ParseIrcMessage(initialRawIRC);
#pragma warning disable CS8625 // null-literal: not needed
            joinedChannel.HandleROOMSTATE(initialIrcMessage, null);
#pragma warning restore CS8625 // null-literal: not needed
            TypeInfo typeInfo = joinedChannel.GetType().GetTypeInfo();
            PropertyInfo? property = typeInfo.GetDeclaredProperty("State");
            Assert.NotNull(property);
            object? value = property.GetValue(joinedChannel);
            Assert.NotNull(value);
            ChannelState channelState = Assert.IsType<ChannelState>(value);
            Assert.NotNull(channelState);

            Assert.Equal(channel, channelState.Channel);
            Assert.Equal(roomId, channelState.RoomId);
            Assert.False(channelState.EmoteOnly);
            Assert.False(channelState.R9K);
            Assert.False(channelState.SubOnly);
            Assert.Null(channelState.FollowersOnly);
            Assert.Equal(0, channelState.SlowMode);

            string changeRawIRC = $"@room-id={roomId};emote-only=1 :tmi.twitch.tv ROOMSTATE #{channel}";
            IrcMessage changeIrcMessage = IrcParser.ParseIrcMessage(changeRawIRC);
#pragma warning disable CS8625 // null-literal: not needed
            joinedChannel.HandleROOMSTATE(changeIrcMessage, null);
#pragma warning restore CS8625 // null-literal: not needed

            Assert.NotNull(channelState);
            Assert.Equal(channel, channelState.Channel);
            Assert.Equal(roomId, channelState.RoomId);
            Assert.True(channelState.EmoteOnly);
        }
        private static UserState GetUserState(bool join, string senderUsername)
        {
            string irc = $":{senderUsername}!{senderUsername}@{senderUsername}.tmi.twitch.tv USERSTATE #{Channel}";
            if (!join)
            {
                irc = $"@id={MessageIdHash} " + irc;
            }
            IrcMessage ircMessage = IrcParser.ParseIrcMessage(irc);
            UserState userState = new UserState(ircMessage);
            return userState;
        }
        private static ChatMessage GetChatMessage(string senderUsername)
        {
            string irc = $"@badge-info=subscriber/22;badges=subscriber/18,bits/1000;client-nonce=a_hash;color=#1E90FF;display-name=testuser;emote-only=1;emotes=1:0-1,8-9/555555584:4-5;first-msg=0;flags=;id={MessageIdHash};mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=1;user-type= :{senderUsername}!{senderUsername}@{senderUsername}.tmi.twitch.tv PRIVMSG #{Channel} ::)  <3  :)";
            IrcMessage ircMessage = IrcParser.ParseIrcMessage(irc);
            ChatMessage chatMessage = new ChatMessage(senderUsername, ircMessage);
            return chatMessage;
        }
    }
}
