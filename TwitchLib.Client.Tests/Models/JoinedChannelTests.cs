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
        [Fact]
        public void RunTest()
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

        private UserState GetUserState(bool join)
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
