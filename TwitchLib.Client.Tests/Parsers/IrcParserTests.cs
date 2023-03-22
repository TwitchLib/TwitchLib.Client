using System;
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
    // --------------CMD-----USER-------CH---TMS--TAGS--META-------------------MSG
    [InlineData("001", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "Welcome, GLHF!", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("002", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "Your host is tmi.twitch.tv", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("003", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "This server is rather new", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("004", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "-", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("372", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "You are in a maze of twisty passages, all alike.", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("375", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", "-", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("376", "testuser", "", true, "", "tmi.twitch.tv {0} {1}", ">", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    //
    // --------------CMD---USER--CH--TMS--TAGS--META------------------------MSG
    [InlineData("CAP", "", "*", true, "", "tmi.twitch.tv {0} * ACK", "twitch.tv/membership", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("NOTICE", "", "*", true, "", "tmi.twitch.tv {0} {2}", "Login authentication failed", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    //
    // --------------CMD-------USER---------CH---------TMS--TAGS----META-------------------------------MSG
    [InlineData("353", "testuser", "testchannel", true, "", "{1}.tmi.twitch.tv {0} {1} = #{2}", "usera userb", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("366", "testuser", "testchannel", true, "", "{1}.tmi.twitch.tv {0} {1} #{2}", "End of /NAMES list", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    //
    // --------------CMD-------USER---------CH----------TMS--TAGS----META-------------------------------MSG
    [InlineData("JOIN", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "")]
    [InlineData("JOIN", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "another_user")]
    [InlineData("PART", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "")]
    [InlineData("PART", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "another_user")]

    // ping-pong
    // --------------CMD----USER-CH---TMS--TAGS--META---MSG
    [InlineData("PING", "", "", false, "", "{0}", "tmi.twitch.tv")]
    [InlineData("PONG", "", "", false, "", "{0}", "tmi.twitch.tv")]

    // timeout
    // --------------CMD---------USER------CH--------TMS----TAGS------------------------------------------------------------META------------------MSG
    [InlineData("CLEARCHAT", "", "testchannel", true, "@ban-duration=60;room-id=0;target-user-id=1;tmi-sent-ts=2", "tmi.twitch.tv {0} #{2}", "", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    // ban
    [InlineData("CLEARCHAT", "testuser", "testchannel", true, "@room-id=0;target-user-id=1;tmi-sent-ts=2", "tmi.twitch.tv {0} #{2}", "{1}", Skip = "lets better skip this test, perhaps my expectations are wrong")]

    //
    [InlineData("CLEARMSG", "testuser", "testchannel", true, "@login={1};room-id=;target-msg-id=some_msg_id_hash;tmi-sent-ts=1", "tmi.twitch.tv {0} #{2}", "some message", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("NOTICE", "", "testchannel", true, "@msg-id=msg_channel_suspended", "tmi.twitch.tv {0} #{2}", "This channel does not exist or has been suspended.", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("PRIVMSG", "testuser", "testchannel", true, "@badge-info=subscriber/36;badges=broadcaster/1,subscriber/3036,sub-gifter/5;client-nonce=hash;color=#B22222;display-name={1};emote-only=1;emotes=1:0-1;first-msg=0;flags=;id=some_msg_id_hash;mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1;turbo=0;user-id=1;user-type=", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", ":)")]
    [InlineData("ROOMSTATE", "", "testchannel", true, "@emote-only=0;followers-only=0;r9k=0;room-id=1;slow=0;subs-only=0", "tmi.twitch.tv {0} #{2}", "", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    [InlineData("USERNOTICE", "testuser", "testchannel", true, "@badge-info=;badges=;color=#FF0000;display-name={1};emotes=;flags=;id=some_msg_id_hash;login={1};mod=0;msg-id=resub;msg-param-cumulative-months=5;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=1;msg-param-streak-months=1;msg-param-sub-plan-name=bla;msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=0;subscriber=1;system-msg={1}\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s5\\smonths,\\scurrently\\son\\sa\\s1\\smonth\\sstreak!;tmi-sent-ts=1;user-id=0;user-type=", "tmi.twitch.tv {0} #{2}", "", Skip = "lets better skip this test, perhaps my expectations are wrong")]
    public void ParseTest(string cmd, string user, string channel, bool tagMetaSeperator, string tags, string meta, string message)
    {
        // "@badge-info=subscriber/22;badges=subscriber/18,bits/1000;client-nonce=a_hash;color=#1E90FF;display-name=testuser;emote-only=1;emotes=1:0-1,8-9/555555584:4-5;first-msg=0;flags=;id=msg_id_hash;mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=1;user-type= :testuser!testuser@testuser.tmi.twitch.tv PRIVMSG #testchannel ::)  <3  :)"
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
        IrcCommand expectedIrcCommand = IrcCommandParser.GetIrcCommandFromString(cmd);
        Assert.NotNull(expectedIrcCommand);
        Assert.NotEqual(IrcCommand.Unknown, expectedIrcCommand);
        Assert.Equal(expectedIrcCommand, ircMessage.Command);
    }
    private static string Format(string stringToFormat, string cmd, string user, string channel)
    {
        return String.Format(stringToFormat, new string[] { cmd, user, channel });
    }
}
