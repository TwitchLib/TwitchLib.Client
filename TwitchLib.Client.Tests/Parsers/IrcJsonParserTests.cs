using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Parsers;

using Xunit;

namespace TwitchLib.Client.Tests.Parsers;
[SuppressMessage("Style", "IDE0058")]
public class IrcJsonParserTests {
    [Theory]
    // --------------CMD-----USER-------CH---TMS--TAGS--META-------------------MSG
    [InlineData("001", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "Welcome, GLHF!")]
    [InlineData("002", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "Your host is tmi.twitch.tv")]
    [InlineData("003", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "This server is rather new")]
    [InlineData("004", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "-")]
    [InlineData("372", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "You are in a maze of twisty passages, all alike.")]
    [InlineData("375", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", "-")]
    [InlineData("376", "testuser", null, true, "", "tmi.twitch.tv {0} {1}", ">")]
    //
    // --------------CMD---USER--CH--TMS--TAGS--META------------------------MSG
    [InlineData("CAP", null, null, true, "", "tmi.twitch.tv {0} * ACK", "twitch.tv/membership")]
    [InlineData("NOTICE", null, null, true, "", "tmi.twitch.tv {0} *", "Login authentication failed")]
    //
    // --------------CMD-------USER---------CH---------TMS--TAGS----META-------------------------------MSG
    [InlineData("353", "testuser", "testchannel", true, "", "{1}.tmi.twitch.tv {0} {1} = #{2}", "usera userb")]
    [InlineData("366", "testuser", "testchannel", true, "", "{1}.tmi.twitch.tv {0} {1} #{2}", "End of /NAMES list")]
    //
    // --------------CMD-------USER---------CH----------TMS--TAGS----META-------------------------------MSG
    [InlineData("JOIN", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", null)]
    [InlineData("JOIN", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "another_user")]
    [InlineData("PART", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", null)]
    [InlineData("PART", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "another_user")]
    // ping-pong
    // --------------CMD----USER-CH---TMS--TAGS--META---MSG
    [InlineData("PING", null, null, false, "", "{0}", "tmi.twitch.tv")]
    [InlineData("PONG", null, null, false, "", "{0}", "tmi.twitch.tv")]
    //
    [InlineData("SERVERCHANGE", null, null, true, "", "tmi.twitch.tv {0}", null)]
    [InlineData("RECONNECT", null, null, true, "", "tmi.twitch.tv {0}", null)]
    //
    // timeout
    // --------------CMD-------------USER----------CH--------TMS----TAGS-------------------------------------------------------------META------------------MSG
    [InlineData("CLEARCHAT", "testuser", "testchannel", true, "@ban-duration=60;room-id=0;target-user-id=1;tmi-sent-ts=2", "tmi.twitch.tv {0} #{2}", "{1}")]
    // ban
    [InlineData("CLEARCHAT", "testuser", "testchannel", true, "@room-id=0;target-user-id=1;tmi-sent-ts=2", "tmi.twitch.tv {0} #{2}", "{1}")]
    //
    [InlineData("MODE", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "+o {1}")]
    [InlineData("MODE", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", "-o {1}")]
    //
    [InlineData("USERSTATE", "testuser", "testchannel", true, "", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", null)]
    [InlineData("USERSTATE", "testuser", "testchannel", true, "@id=msg_id_hash", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", null)]
    //
    [InlineData("CLEARMSG", "testuser", "testchannel", true, "@login={1};room-id=;target-msg-id=some_msg_id_hash;tmi-sent-ts=1", "tmi.twitch.tv {0} #{2}", "some message")]
    [InlineData("NOTICE", null, "testchannel", true, "@msg-id=msg_channel_suspended", "tmi.twitch.tv {0} #{2}", "This channel does not exist or has been suspended.")]
    [InlineData("PRIVMSG", "testuser", "testchannel", true, "@badge-info=subscriber/36;badges=broadcaster/1,subscriber/3036,sub-gifter/5;client-nonce=hash;color=#B22222;display-name={1};emote-only=1;emotes=1:0-1;first-msg=0;flags=;id=some_msg_id_hash;mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1;turbo=0;user-id=1;user-type=", "{1}!{1}@{1}.tmi.twitch.tv {0} #{2}", ":)")]
    [InlineData("ROOMSTATE", null, "testchannel", true, "@emote-only=0;followers-only=0;r9k=0;room-id=1;slow=0;subs-only=0", "tmi.twitch.tv {0} #{2}", null)]
    [InlineData("USERNOTICE", "testuser", "testchannel", true, "@badge-info=;badges=;color=#FF0000;display-name={1};emotes=;flags=;id=some_msg_id_hash;login={1};mod=0;msg-id=resub;msg-param-cumulative-months=5;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=1;msg-param-streak-months=1;msg-param-sub-plan-name=bla;msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=0;subscriber=1;system-msg={1}\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s5\\smonths,\\scurrently\\son\\sa\\s1\\smonth\\sstreak!;tmi-sent-ts=1;user-id=0;user-type=", "tmi.twitch.tv {0} #{2}", null)]
    public void ParseTest(string cmd, string user, string channel, bool tagMetaSeperator, string tags, string meta, string message) {
        string? tagsPart = Format(tags, cmd, user, channel);
        string? metaPart = Format(meta, cmd, user, channel);
        string? messagePart = Format(message, cmd, user, channel);
        StringBuilder ircBuilder = new StringBuilder();
        ircBuilder.Append(tagsPart);
        if (tagMetaSeperator) {
            ircBuilder.Append(" :");
        }
        ircBuilder.Append(metaPart);
        if (messagePart != null && !messagePart.IsNullOrEmptyOrWhitespace()) {
            ircBuilder.Append(" :");
            ircBuilder.Append(messagePart);
        }

        string irc = ircBuilder.ToString().Trim();
        JObject ircMessage = IrcJsonParser.Parse(irc);
        Assert.Equal(cmd, ircMessage.Value<string>("command"));
        Assert.Equal(irc, ircMessage.Value<string>("irc"));
        Assert.Equal(channel, ircMessage.Value<string>("channel"));
        if (meta.Contains("{1}")) {
            Assert.Equal(user, ircMessage.Value<string>("user"));
        }
        Assert.Equal(messagePart, ircMessage.Value<string>("message"));

    }
    [Fact]
    public void ParseTestDetailed() {
        string irc = "@badge-info=subscriber/22;badges=subscriber/18,bits/1000;client-nonce=a_hash;color=#1E90FF;display-name=testuser;emote-only=1;emotes=1:0-1,8-9/555555584:4-5;emote-sets=0,33,50,237,793,2126,3517,4578,5569,9400,10337,12239;first-msg=0;flags=;id=msg_id_hash;mod=0;returning-chatter=0;room-id=0;subscriber=1;tmi-sent-ts=1678800000000;turbo=0;user-id=1;user-type= :testuser!testuser@testuser.tmi.twitch.tv PRIVMSG #testchannel ::)  <3  :)";
        JObject ircMessage = IrcJsonParser.Parse(irc);
        Assert.Equal("PRIVMSG", ircMessage.Value<string>("command"));
        Assert.Equal("testuser", ircMessage.Value<string>("user"));
        Assert.Equal("testchannel", ircMessage.Value<string>("channel"));
        Assert.Equal(":)  <3  :)", ircMessage.Value<string>("message"));
        Assert.Equal(irc, ircMessage.Value<string>("irc"));
        //
        Assert.NotNull(ircMessage["tags"]);
        JObject? tags = Assert.IsType<JObject>(ircMessage["tags"]);
        Assert.NotNull(tags);
        Assert.NotNull(tags.Properties());
        Assert.Equal(19, tags.Properties().Count());
        Assert.Equal("a_hash", tags.Value<string>("client-nonce"));
        Assert.Equal("#1E90FF", tags.Value<string>("color"));
        Assert.Equal("testuser", tags.Value<string>("display-name"));
        Assert.Equal("1", tags.Value<string>("emote-only"));
        Assert.Equal("0", tags.Value<string>("first-msg"));
        Assert.Null(tags.Value<string>("flags"));
        Assert.Equal("msg_id_hash", tags.Value<string>("id"));
        Assert.Equal("0", tags.Value<string>("mod"));
        Assert.Equal("0", tags.Value<string>("returning-chatter"));
        Assert.Equal("0", tags.Value<string>("room-id"));
        Assert.Equal("1", tags.Value<string>("subscriber"));
        Assert.Equal("1678800000000", tags.Value<string>("tmi-sent-ts"));
        Assert.Equal("0", tags.Value<string>("turbo"));
        Assert.Equal("1", tags.Value<string>("user-id"));
        Assert.Null(tags.Value<string>("user-type"));
        //
        Assert.NotNull(tags["badge-info"]);
        JObject? badgeInfo = Assert.IsType<JObject>(tags["badge-info"]);
        Assert.NotNull(badgeInfo);
        Assert.NotNull(badgeInfo.Properties());
        Assert.Single(badgeInfo.Properties());
        Assert.Equal("22", badgeInfo.Value<string>("subscriber"));
        //
        Assert.NotNull(tags["badges"]);
        JObject? badges = Assert.IsType<JObject>(tags["badges"]);
        Assert.NotNull(badges);
        Assert.NotNull(badges.Properties());
        Assert.Equal(2, badges.Properties().Count());
        Assert.Equal("18", badges.Value<string>("subscriber"));
        Assert.Equal("1000", badges.Value<string>("bits"));
        //
        Assert.NotNull(tags["emotes"]);
        JObject? emotes = Assert.IsType<JObject>(tags["emotes"]);
        Assert.NotNull(emotes);
        Assert.NotNull(emotes.Properties());
        Assert.Equal(3, emotes.Properties().Count());
        //
        // emote-id 1
        string idA = "1";
        string nameA = ":)";
        CheckEmoteObject(emotes,
                         idA,
                         new[] { idA, idA },
                         new[] { nameA, nameA },
                         new[] { "0", "8" },
                         new[] { "1", "9" });
        //
        // emote-id 555555584
        string idB = "555555584";
        string nameB = "<3";
        CheckEmoteObject(emotes,
                         idB,
                         new[] { idB },
                         new[] { nameB },
                         new[] { "4" },
                         new[] { "5" });
        //
        string orderedIndexObjectsPropertyName = "orderedIndexObjects";
        JArray? orderedIndexObjects = Assert.IsType<JArray>(emotes[orderedIndexObjectsPropertyName]);
        Assert.NotNull(orderedIndexObjects);
        Assert.Equal(3, orderedIndexObjects.Count);
        CheckEmoteObject(emotes,
                         orderedIndexObjectsPropertyName,
                         new[] { "1", "555555584", "1" },
                         new[] { ":)", "<3", ":)" },
                         new[] { "0", "4", "8" },
                         new[] { "1", "5", "9" });
        //
        Assert.NotNull(tags["emote-sets"]);
        JArray? emoteSets = Assert.IsType<JArray>(tags["emote-sets"]);
        Assert.NotNull(emoteSets);
        int[] emoteSetIds = new int[] { 0, 33, 50, 237, 793, 2126, 3517, 4578, 5569, 9400, 10337, 12239 };
        Assert.Equal(emoteSetIds.Length, emoteSets.Count);
        Assert.Equal(emoteSetIds, emoteSets.Values<int>());

    }

    /// <param name="emotes">
    ///     emotes-object
    /// </param>
    /// <param name="propertyName">
    ///     expected id of the emote or "orderedIndexObjects"
    /// </param>
    /// <param name="ids">
    ///     <see langword="string"/>[] with ids of emotes in order of expected appearance
    /// </param>
    /// <param name="names">
    ///     <see langword="string"/>[] with names of emotes in order of expected appearance
    /// </param>
    /// <param name="froms">
    ///     <see langword="string"/>[] with from-indices of emotes in order of expected appearance
    /// </param>
    /// <param name="tos">
    ///     <see langword="string"/>[] with to-indices of emotes in order of expected appearance
    /// </param>
    private static void CheckEmoteObject(JObject emotes, string propertyName, string[] ids, string[] names, string[] froms, string[] tos) {
        JArray? indices = Assert.IsType<JArray>(emotes[propertyName]);
        Assert.NotNull(indices);
        Assert.Equal(names.Length, indices.Count);
        for (int i = 0; i < names.Length; i++) {
            string id = ids[i];
            string name = names[i];
            string from = froms[i];
            string to = tos[i];
            JObject indexObject = (JObject) indices[i];
            CheckEmoteIndexObject(id, name, from, to, indexObject);
        }
    }

    private static void CheckEmoteIndexObject(string id, string name, string from, string to, JObject indexObject) {
        Assert.Equal(id, indexObject.Value<string>("id"));
        Assert.Equal(name, indexObject.Value<string>("name"));
        Assert.Equal(from, indexObject.Value<string>("from"));
        Assert.Equal(to, indexObject.Value<string>("to"));
    }

    private static string? Format(string stringToFormat, string cmd, string user, string channel) {
        if (stringToFormat == null) {
            return null;
        }

        return String.Format(stringToFormat, new string[] { cmd, user, channel });
    }
}
