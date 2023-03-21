using TwitchLib.Client.Events;
using TwitchLib.Client.Models.Internal;
using Xunit;

namespace TwitchLib.Client.Test {
    public class IrcParserTests {
        private readonly MockIClient _mockClient;

        public IrcParserTests() {
            _mockClient = new MockIClient();
        }

        [Theory()]
        [InlineData(MsgIds.Announcement,
            "@badge-info=;badges=broadcaster/1,ambassador/1;color=#033700;display-name=BarryCarlyon;emotes=;flags=;id=d2c97aaa-b921-45ca-b1f5-df6bbcedb289;login=barrycarlyon;mod=0;msg-id=announcement;msg-param-color=PRIMARY;room-id=15185913;subscriber=0;system-msg=;tmi-sent-ts=1665265341857;user-id=15185913;user-type= :tmi.twitch.tv USERNOTICE #barrycarlyon :test")]
        [InlineData(MsgIds.ReSubscription,
            @"@badges=subscriber/1,turbo/1;color=#2B119C;display-name=JustFunkIt;emotes=;id=9dasn-asdibas-asdba-as8as;login=justfunkit;mod=0;msg-id=resub;msg-param-months=2;room-id=44338537;subscriber=1;system-msg=JustFunkIt\\ssubscribed\\sfor\\s2\\smonths\\sin\\sa\\srow!;turbo=1;user-id=26526370;user-type= :tmi.twitch.tv USERNOTICE #burkeblack :AVAST YEE SCURVY DOG")]
        [InlineData(MsgIds.ContinuedGiftedSubscription,
            @"@badge-info=;badges=channel-subscriber/1;color=#0000FF;display-name=ExampleUser;emotes=;id=1234-abcd-5678-efgh-9012ijklmnop;login=example_user;mod=0;msg-id=giftpaidupgrade;msg-param-promo-gift-total=5;msg-param-promo-name=gift\\\spromo\\\sname;msg-param-recipient-display-name=ExampleRecipient;msg-param-recipient-id=123456789;msg-param-recipient-user-name=example_recipient;msg-param-sender-name=ExampleSender;msg-param-sub-plan=1000;msg-param-sub-plan-name=Channel\\\sSubscription\\\s(ExampleChannel);room-id=123456789;subscriber=1;system-msg=ExampleUser\\\sgifted\\\s1\\\ssub\\\sto\\\sExampleRecipient!\\\sThis\\\sis\\\sahuge\\\ssupport\\\sand\\\sExampleRecipient\\\sis\\\snow\\\sin\\\sthe\\\sExampleSender\\\sfamily!;tmi-sent-ts=1556721455111;user-id=123456;user-type= :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.Subscription,
            @"@badge-info=;badges=channel-subscriber/1;color=#FF0000;display-name=ExampleUser;emotes=;id=1234-abcd-5678-efgh-9012ijklmnop;login=example_user;mod=0;msg-id=sub;msg-param-cumulative-months=3;msg-param-months=0;msg-param-should-share-streak=1;msg-param-sub-plan-name=Channel\\\sSubscription\\\s(ExampleChannel);msg-param-sub-plan=1000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\\ssubscribed\\\sat\\\sTier\\\s1.\\\sThey've\\\ssubscribed\\\sfor\\\s3\\\smo\\ns!;tmi-sent-ts=1556721455111;user-id=123456;user-type= :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.BadUnbanNoBan, "@msg-id=bad_unban_no_ban :tmi.twitch.tv NOTICE #example_channel :No active ban for specified user/channel")]
        [InlineData(MsgIds.ModeratorsReceived, "@msg-id=room_mods :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.NoMods, "@msg-id=no_mods :tmi.twitch.tv NOTICE #example_channel :There are no moderators of this room.")]
        [InlineData(MsgIds.NoVIPs, "@msg-id=no_vips :tmi.twitch.tv NOTICE #example_channel :There are no VIPs of this channel.")]
        [InlineData(MsgIds.MsgBannedEmailAlias,
            "@msg-id=msg_banned_email_alias :tmi.twitch.tv NOTICE #example_channel :Your account has been suspended for terms of service violations. For more information, see https://help.twitch.tv/customer/portal/articles/725568-terms-of-service.")]
        [InlineData(MsgIds.MsgChannelSuspended, "@msg-id=msg_channel_suspended :tmi.twitch.tv NOTICE #example_channel :This channel has been closed.")]
        [InlineData(MsgIds.MsgRequiresVerifiedPhoneNumber,
            "@msg-id=msg_requires_verified_phone_number :tmi.twitch.tv NOTICE #example_channel :This command requires a verified phone number.")]
        [InlineData(MsgIds.MsgVerifiedEmail, "@msg-id=msg_verified_email :tmi.twitch.tv NOTICE #example_channel :This command requires a verified email address.")]
        [InlineData(MsgIds.MsgRateLimit, "@msg-id=msg_ratelimit :tmi.twitch.tv NOTICE #example_channel :Your message was not sent because you are sending messages too quickly.")]
        [InlineData(MsgIds.MsgDuplicate, "@msg-id=msg_duplicate :tmi.twitch.tv NOTICE #example_channel :Your message was not sent because you are sending the same message too quickly.")]
        [InlineData(MsgIds.MsgR9k, "@msg-id=msg_r9k :tmi.twitch.tv NOTICE #example_channel :This room is in r9k mode.")]
        [InlineData(MsgIds.MsgFollowersOnly, "@msg-id=msg_followersonly :tmi.twitch.tv NOTICE #example_channel :This room is in followers-only mode. Follow the channel to chat.")]
        [InlineData(MsgIds.MsgSubsOnly, "@msg-id=msg_subsonly :tmi.twitch.tv NOTICE #example_channel :This room is in subscribers-only mode.")]
        [InlineData(MsgIds.MsgEmoteOnly, "@msg-id=msg_emoteonly :tmi.twitch.tv NOTICE #example_channel :This room is in emote-only mode.")]
        [InlineData(MsgIds.MsgSuspended, "@msg-id=msg_suspended :tmi.twitch.tv NOTICE #example_channel :This account has been suspended.")]
        [InlineData(MsgIds.MsgBanned, "@msg-id=msg_banned :tmi.twitch.tv NOTICE #example_channel :You are permanently banned from talking in example_channel.")]
        [InlineData(MsgIds.MsgSlowMode, "@msg-id=msg_slowmode :tmi.twitch.tv NOTICE #example_channel :This room is in slow mode. You may send messages every 120 seconds.")]
        [InlineData(MsgIds.NoPermission, "@msg-id=no_permission :tmi.twitch.tv NOTICE #example_channel :You don't have permission to perform that action.")]
        [InlineData(MsgIds.Raid,
            "@badge-info=;badges=;color=;display-name=Raiders;emotes=;flags=;id=12345678-abcd-90ef-ghij-1234567890kl;login=raiduser;mod=0;msg-id=raid;room-id=123456789;subscriber=0;system-msg=123 raiders from raiduser have joined!;tmi-sent-ts=1556721496528;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.RaidErrorSelf, "@msg-id=raid_error_self :tmi.twitch.tv NOTICE #example_channel :You cannot raid your own channel.")]
        [InlineData(MsgIds.RaidNoticeMature, "@msg-id=raid_notice_mature :tmi.twitch.tv NOTICE #example_channel :This channel is intended for mature audiences.")]
        [InlineData(MsgIds.R9KOff, "@msg-id=r9k_off :tmi.twitch.tv NOTICE #example_channel :r9k mode is no longer active in this room.")]
        [InlineData(MsgIds.SubGift,
            "@badge-info=;badges=staff/1,partner/1;color=#00FF7F;display-name=ExampleUser;emotes=;id=1234-abcd-5678-efgh-9012ijklmnop;login=example_user;mod=0;msg-id=subgift;msg-param-months=2;msg-param-recipient-display-name=ExampleReceiver;msg-param-recipient-id=987654321;msg-param-recipient-user-name=\"@ExampleReceiver;msg-param-sender-count=1;msg-param-sender-login=example_user;msg-param-sender-name=ExampleUser;room-id=123456789;subscriber=0;system-msg=ExampleUser\\sgifted\\sa\\sTier\\s1\\ssub\\sto\\sExampleReceiver!\\sThey\\shave\\sgiven\\s10\\sGift\\ssubs\\sin\\sthe\\schannel!;tmi-sent-ts=1556721654203;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.CommunitySubscription,
            "@badge-info=subscriber/1,founder/1;badges=partner/1,bits-leader/1;color=#FF0000;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=submysterygift;msg-param-gift-count=100;msg-param-sender-count=1;msg-param-sender-login=example_sender;msg-param-sender-name=ExampleSender;msg-param-sub-plan-name=ExamplePlan;msg-param-sub-plan=3000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\sis\\sgifting\\s100\\sTier\\s3\\ssubs\\sto\\sExampleSender's\\scommunity!;tmi-sent-ts=1556721639178;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.Subscription,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,turbo/1;color=#19E6E6;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=sub;msg-param-cumulative-months=2;msg-param-streak-months=2;msg-param-sub-plan-name=ExamplePlan;msg-param-sub-plan=1000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\sjust\\ssubscribed\\swith\\sTwitch\\sPrime.\\sExampleUser\\sSubscribed\\sfor\\s2\\smonths\\sin\\sa\\srow!;tmi-sent-ts=1556721453273;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.SubsOff,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,turbo/1;color=#19E6E6;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=subs_off;room-id=123456789;subscriber=1;system-msg=Subscription\\smode\\sis\\sno\\slonger\\senabled; the\\scommunity\\sis\\sno\\slonger\\sin\\ssub-only\\smode!;tmi-sent-ts=1556721830279;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.SubsOn,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,turbo/1;color=#19E6E6;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=subs_on;room-id=123456789;subscriber=1;system-msg=This\\sroom\\sis\\snow\\sin\\ssubscribers-only\\smodus.;tmi-sent-ts=1556721866501;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.UserIntro,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,premium/1;color=#008000;display-name=ExampleUser;emotes=;id=1234abcd-5678-efgh-ijkl-90qwerty123;login=example_user;mod=0;msg-id=user-intro;room-id=123456789;subscriber=1;system-msg=Welcome\\sto\\sExampleChannel,\\sExampleUser!;tmi-sent-ts=1556721453273;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.VIPsSuccess,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,turbo/1;color=#008000;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=vips_success;msg-param-mods=ExampleUser;room-id=123456789;subscriber=1;tmi-sent-ts=1556721883215;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.SubGift,
            "@badge-info=subscriber/12;badges=broadcaster/1,subscriber/3,turbo/1;color=#008000;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=subgift;msg-param-months=1;msg-param-recipient-display-name=ExampleRecipient;msg-param-recipient-id=123456789;msg-param-recipient-user-name=example_recipient;msg-param-sender-count=1;msg-param-sub-plan-name=Channel\\sSubscription\\s(gift);msg-param-sub-plan=1000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\sgifted\\sa\\sTier\\s1\\ssub\\sto\\sExampleRecipient!;tmi-sent-ts=1556721453273;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.Subscription,
            "@badge-info=subscriber/1,founder/0;badges=broadcaster/1,turbo/1;color=#008000;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=sub;msg-param-cumulative-months=2;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\s(Tier\\s1);msg-param-sub-plan=1000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\ssubscribed\\swith\\sTwitch\\sPrime.\\sThey've\\ssubscribed\\sfor\\s2\\smonths,\\scurrently\\son\\sa\\s1\\smonth\\sstreak!;tmi-sent-ts=1556721453273;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.R9KOn, "@msg-id=r9k_on :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.SubsOff, "@msg-id=subs_off :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.SubsOn, "@msg-id=subs_on :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.EmoteOnlyOff, "@msg-id=emote_only_off :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.EmoteOnlyOn, "@msg-id=emote_only_on :tmi.twitch.tv ROOMSTATE #example_channel")]
        [InlineData(MsgIds.PrimePaidUprade,
            "@badge-info=subscriber/1;badges=partner/1,subscriber/3;color=#FF69B4;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=primepaidupgrade;msg-param-sub-plan-name=Channel\\sSubscription\\s(Tier\\s2);msg-param-sub-plan=2000;room-id=123456789;subscriber=1;system-msg=ExampleUser\\supgraded\\sto\\sa\\sTier\\s2\\ssub!;tmi-sent-ts=1562779429124;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.SubGift,
            "@badge-info=subscriber/0;badges=broadcaster/1;color=;display-name=ExampleUser;emotes=;id=12345678-90ab-cdef-1234-567890abcdef;login=example_user;mod=0;msg-id=subgift;msg-param-months=1;msg-param-recipient-display-name=GifteeUserName;msg-param-recipient-id=123456789;msg-param-recipient-user-name=giftee_user_name;msg-param-sender-count=1;msg-param-sub-plan-name=Channel\\sSubscription\\s(Tier\\s1);msg-param-sub-plan=1000;room-id=123456789;subscriber=0;system-msg=ExampleUser\\sgifted\\sa\\sTier\\s1\\ssub\\sto\\sgiftee_user_name!\\sThey\\shave\\ssubscribed\\sfor\\s1\\smonth!\\s\\sGift\\sanother\\ssub\\sto\\skeep\\sthe\\scommunity\\sgoing!;tmi-sent-ts=1597970678659;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]
        [InlineData(MsgIds.HighlightedMessage,
            "@badge-info=;badges=;color=#1E90FF;display-name=ExampleUser;emotes=;id=abcd-1234-efgh-5678-ijklm;login=example_user;mod=0;msg-id=highlighted-message;msg-param-flags=highlighted-message;room-id=123456789;subscriber=0;system-msg=ExampleUser\\sjust\\sshared\\syour\\smessage\\swith\\s1\\speople!\\sCheck\\sout\\shighlighted\\smessage\\s1\\sfrom\\sExampleUser:\\sHello\\severyone!\\sHope\\severyone\\sis\\shaving\\sa\\sgreat\\sday!;tmi-sent-ts=1556337148000;user-id=123456789;user-type=staff :tmi.twitch.tv USERNOTICE #example_channel")]

        /*
         * MISSING:
         * // color changed
        // timeout successfulkl
        // unban successful
        // UnrecognizedCmd
         */

        public void IrcParserMsgIdsTests(string expectedMessageId, string rawIrcMessage) {
            var client = new TwitchClient(_mockClient);

            var ircMsgResult = client.ParseRawIrcMessage(rawIrcMessage);
            var successMsgId = ircMsgResult.Tags.TryGetValue(Tags.MsgId, out var msgId);

            Assert.NotNull(ircMsgResult);
            Assert.True(successMsgId);
            Assert.Equal(expectedMessageId, msgId);
        }
    }
}