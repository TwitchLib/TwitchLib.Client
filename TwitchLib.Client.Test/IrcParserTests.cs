using TwitchLib.Client.Events;
using TwitchLib.Client.Models.Internal;
using Xunit;

namespace TwitchLib.Client.Test
{
    public class IrcParserTests
    {
        private readonly MockIClient _mockClient;

        public IrcParserTests()
        {
            _mockClient = new MockIClient();
        }

        [Theory()]
        [InlineData(MsgIds.Announcement, "@badge-info=;badges=broadcaster/1,ambassador/1;color=#033700;display-name=BarryCarlyon;emotes=;flags=;id=d2c97aaa-b921-45ca-b1f5-df6bbcedb289;login=barrycarlyon;mod=0;msg-id=announcement;msg-param-color=PRIMARY;room-id=15185913;subscriber=0;system-msg=;tmi-sent-ts=1665265341857;user-id=15185913;user-type= :tmi.twitch.tv USERNOTICE #barrycarlyon :test")]
        [InlineData(MsgIds.ContinuedGiftedSubscription, @"@badge-info=subscriber/11;badges=subscriber/9;color=#DAA520;display-name=Varanid;emotes=;flags=;id=a2d384c1-c30a-409e-8001-9e7d8f9c784d;login=varanid;mod=0;msg-id=giftpaidupgrade;msg-param-sender-login=cletusbueford;msg-param-sender-name=CletusBueford;room-id=44338537;subscriber=1;system-msg=Varanid\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\sCletusBueford!;tmi-sent-ts=1612497386372;user-id=67505836;user-type= :tmi.twitch.tv USERNOTICE #burkeblack")]
        [InlineData(MsgIds.ReSubscription, @"@badges=subscriber/1,turbo/1;color=#2B119C;display-name=JustFunkIt;emotes=;id=9dasn-asdibas-asdba-as8as;login=justfunkit;mod=0;msg-id=resub;msg-param-months=2;room-id=44338537;subscriber=1;system-msg=JustFunkIt\ssubscribed\sfor\s2\smonths\sin\sa\srow!;turbo=1;user-id=26526370;user-type= :tmi.twitch.tv USERNOTICE #burkeblack :AVAST YEE SCURVY DOG")]
        [InlineData(MsgIds.RaidNoticeMature, "@msg-id=raid_notice_mature :tmi.twitch.tv NOTICE #swiftyspiffy :This channel is intended for mature audiences.")]
        [InlineData(MsgIds.NoPermission, "@msg-id=no_permission :tmi.twitch.tv NOTICE #swiftyspiffy :You don't have permission to perform that action.")]
        [InlineData(MsgIds.RaidErrorSelf, "@msg-id=raid_error_self :tmi.twitch.tv NOTICE #swiftyspiffy :A channel cannot raid itself.")]
        public void IrcParserMsgIdsTests(string expectedMessageId, string rawIrcMessage)
        {
            var client = new TwitchClient(_mockClient);

            var ircMsgResult = client.ParseRawIrcMessage(rawIrcMessage);
            var successMsgId = ircMsgResult.Tags.TryGetValue(Tags.MsgId, out var msgId);
             
            Assert.NotNull(ircMsgResult);
            Assert.True(successMsgId);
            Assert.Equal(expectedMessageId, msgId);
        }
    }
}