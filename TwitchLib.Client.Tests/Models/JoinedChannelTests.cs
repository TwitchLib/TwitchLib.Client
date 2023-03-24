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
        private static string Username => "testusername";
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
        [Fact]
        public void UserState_And_SentMessage_Test()
        {
            ITwitchClient client = new TwitchClient();
            JoinedChannel joinedChannel = new JoinedChannel(Channel, Username);
            ChatMessage chatMessage = GetChatMessage();
            joinedChannel.HandlePRIVMSG(chatMessage);
            Assert.NotEmpty(joinedChannel.UnRaisedSentMessages);
            Assert.Single(joinedChannel.UnRaisedSentMessages);
            UserState userStateJoin = GetUserState(true);
            joinedChannel.HandleUSERSTATE(userStateJoin, client);
            Assert.NotEmpty(joinedChannel.UnRaisedSentMessages);
            Assert.Single(joinedChannel.UnRaisedSentMessages);
            UserState userStateSent = GetUserState(false);
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
            joinedChannel.HandleROOMSTATE(initialIrcMessage, null);
            TypeInfo typeInfo = joinedChannel.GetType().GetTypeInfo();
            PropertyInfo? property = typeInfo.GetDeclaredProperty("State");
            Assert.NotNull(property);
            object? value = property.GetValue(joinedChannel);
            Assert.NotNull(value);
            Assert.IsType<ChannelState>(value);
            ChannelState channelState = (ChannelState) value;
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
            joinedChannel.HandleROOMSTATE(changeIrcMessage, null);

            Assert.NotNull(channelState);
            Assert.Equal(channel, channelState.Channel);
            Assert.Equal(roomId, channelState.RoomId);
            Assert.True(channelState.EmoteOnly);
        }
        private static UserState GetUserState(bool join)
        {
            string irc = $":{Username}!{Username}@{Username}.tmi.twitch.tv USERSTATE #{Channel}";
            if (!join)
            {
                irc = $"@id={MessageIdHash} " + irc;
            }
            IrcMessage ircMessage = IrcParser.ParseIrcMessage(irc);
            UserState userState = new UserState(ircMessage);
            return userState;
        }
        private static ChatMessage GetChatMessage()
        {
            string irc = $"@badge-info=subscriber/22;badges=subscriber/18,bits/1000;client-nonce=a_hash;color=#1E90FF;display-name=testuser;emote-only=1;emotes=1:0-1,8-9/555555584:4-5;first-msg=0;flags=;id={MessageIdHash};mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=1;user-type= :{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{Channel} ::)  <3  :)";
            IrcMessage ircMessage = IrcParser.ParseIrcMessage(irc);
            ChatMessage chatMessage = new ChatMessage(Username, ircMessage);
            return chatMessage;
        }
    }
}
