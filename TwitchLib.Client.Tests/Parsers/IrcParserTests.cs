using System.Linq;
using System.Text;

using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Client.Parsers;

using Xunit;

namespace TwitchLib.Client.Tests.Parsers;

public class IrcParserTests
{
    [Theory]
    [InlineData("PRIVMSG", IrcCommand.PrivMsg)]
    [InlineData("NOTICE", IrcCommand.Notice)]
    [InlineData("PING", IrcCommand.Ping)]
    [InlineData("PONG", IrcCommand.Pong)]
    [InlineData("CLEARCHAT", IrcCommand.ClearChat)]
    [InlineData("CLEARMSG", IrcCommand.ClearMsg)]
    [InlineData("USERSTATE", IrcCommand.UserState)]
    [InlineData("GLOBALUSERSTATE", IrcCommand.GlobalUserState)]
    [InlineData("NICK", IrcCommand.Nick)]
    [InlineData("JOIN", IrcCommand.Join)]
    [InlineData("PART", IrcCommand.Part)]
    [InlineData("PASS", IrcCommand.Pass)]
    [InlineData("CAP", IrcCommand.Cap)]
    [InlineData("001", IrcCommand.RPL_001)]
    [InlineData("002", IrcCommand.RPL_002)]
    [InlineData("003", IrcCommand.RPL_003)]
    [InlineData("004", IrcCommand.RPL_004)]
    [InlineData("353", IrcCommand.RPL_353)]
    [InlineData("366", IrcCommand.RPL_366)]
    [InlineData("372", IrcCommand.RPL_372)]
    [InlineData("375", IrcCommand.RPL_375)]
    [InlineData("376", IrcCommand.RPL_376)]
    [InlineData("WHISPER", IrcCommand.Whisper)]
    [InlineData("SERVERCHANGE", IrcCommand.ServerChange)]
    [InlineData("RECONNECT", IrcCommand.Reconnect)]
    [InlineData("ROOMSTATE", IrcCommand.RoomState)]
    [InlineData("USERNOTICE", IrcCommand.UserNotice)]
    [InlineData("MODE", IrcCommand.Mode)]
    [InlineData("", IrcCommand.Unknown)]
    [InlineData("a", IrcCommand.Unknown)]
    [InlineData("0", IrcCommand.Unknown)]
    [InlineData("*", IrcCommand.Unknown)]
    [InlineData(null, IrcCommand.Unknown)]
    public void GetIrcCommandFromStringTest(string commandAsString, IrcCommand expectedCommand)
    {
        IrcCommand command = IrcParser.GetIrcCommandFromString(commandAsString);
        Assert.Equal(expectedCommand, command);
    }
    [Theory]
    [InlineData("", "tmi.twitch.tv {0} {1}", "001", "Welcome, GLHF!", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "002", "Your host is tmi.twitch.tv", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "003", "This server is rather new", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "004", "-", "testuser", "", true)]
    [InlineData("", "{1}.tmi.twitch.tv {0} {1} = #{2}", "353", "usera userb", "testuser", "testchannel", true)]
    [InlineData("", "{1}.tmi.twitch.tv {0} {1} #{2}", "366", "End of /NAMES list", "testuser", "testchannel", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "372", "You are in a maze of twisty passages, all alike.", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "375", "-", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} {1}", "376", ">", "testuser", "", true)]
    [InlineData("", "tmi.twitch.tv {0} * ACK", "CAP", "twitch.tv/membership", "", "*", true)]
    [InlineData("@ban-duration=60;room-id=0;target-user-id=1;tmi-sent-ts=2", "tmi.twitch.tv {0} #{2}", "CLEARCHAT", "testuser", "", "testchannel", true)]
    [InlineData("@login={1};room-id=;target-msg-id=some_msg_id_hash;tmi-sent-ts=1", "tmi.twitch.tv {0} #{2}", "CLEARMSG", "some message", "testuser", "testchannel", true)]
    [InlineData("", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "JOIN", "", "testuser", "testchannel", true)]
    [InlineData("@msg-id=msg_channel_suspended", "tmi.twitch.tv {0} #{2}", "NOTICE", "This channel does not exist or has been suspended.", "", "testchannel", true)]
    [InlineData("", "tmi.twitch.tv {0} {2}", "NOTICE", "Login authentication failed", "", "*", true)]
    [InlineData("", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "PART", "", "testuser", "testchannel", true)]
    [InlineData("", "{0}", "PING", "tmi.twitch.tv", "", "", false)]
    [InlineData("", "{0}", "PONG", "tmi.twitch.tv", "", "", false)]
    [InlineData("@badge-info=subscriber/36;badges=broadcaster/1,subscriber/3036,sub-gifter/5;client-nonce=hash;color=#B22222;display-name={1};emote-only=1;emotes=1:0-1;first-msg=0;flags=;id=some_msg_id_hash;mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1;turbo=0;user-id=1;user-type=", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "PRIVMSG", ":)", "testuser", "testchannel", true)]
    [InlineData("@emote-only=0;followers-only=0;r9k=0;room-id=1;slow=0;subs-only=0", "tmi.twitch.tv {0} #{2}", "ROOMSTATE", "", "", "testchannel", true)]
    [InlineData("@badge-info=;badges=;color=#FF0000;display-name={1};emotes=;flags=;id=some_msg_id_hash;login={1};mod=0;msg-id=resub;msg-param-cumulative-months=5;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=1;msg-param-streak-months=1;msg-param-sub-plan-name=bla;msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=0;subscriber=1;system-msg={1}\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s5\\smonths,\\scurrently\\son\\sa\\s1\\smonth\\sstreak!;tmi-sent-ts=1;user-id=0;user-type=", "tmi.twitch.tv {0} #{2}", "USERNOTICE", "", "testuser", "testchannel", true)]
    public void ParseTest(string tags, string meta, string cmd, string message, string user, string channel, bool tagMetaSeperator)
    {
        string tagsPart = Format(tags, cmd, user, channel);
        string metaPart = Format(meta, cmd, user, channel);
        string messagePart = Format(message, cmd, user, channel);
        StringBuilder ircBuilder = new StringBuilder();
        ircBuilder.Append(tagsPart);
        if (tagMetaSeperator)
        {
            ircBuilder.Append(" :");
        }
        ircBuilder.Append(metaPart);
        if (!messagePart.IsNullOrEmptyOrWhitespace())
        {
            ircBuilder.Append(" :");
            ircBuilder.Append(messagePart);
        }

        string irc = ircBuilder.ToString().Trim();
        IrcMessage ircMessage = IrcParser.ParseIrcMessage(irc);
        Assert.NotNull(ircMessage);
        Assert.Equal(messagePart, ircMessage.Message);
        Assert.Equal(channel, ircMessage.Channel);
        Assert.Equal(user, ircMessage.User);
        if (!tagsPart.IsNullOrEmptyOrWhitespace())
        {
            Assert.True(ircMessage.Tags.Any());
        }
        IrcCommand expectedIrcCommand = IrcParser.GetIrcCommandFromString(cmd);
        Assert.NotNull(expectedIrcCommand);
        Assert.Equal(expectedIrcCommand, ircMessage.Command);
    }
    private static string Format(string stringToFormat, string cmd, string user, string channel)
    {
        return System.String.Format(stringToFormat, new string[] { cmd, user, channel });
    }
}
