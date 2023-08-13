using BenchmarkDotNet.Attributes;
using TwitchLib.Client.Parsing;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Benchmark
{
    [MemoryDiagnoser]
    public class IrcParserBenchmark
    {
        [Params(
            "@msg-id=host_on :tmi.twitch.tv NOTICE #burkeblack :Now hosting DjTechlive.",
            ":blubott.tmi.twitch.tv 366 blubott #monstercat :End of /NAMES list",
            ":jtv!jtv@jtv.tmi.twitch.tv PRIVMSG (HOSTED):(HOSTER) is now hosting you for (VIEWERS_TOTAL) viewers.",
            "@msg-id=raid_notice_mature :tmi.twitch.tv NOTICE #swiftyspiffy :This channel is intended for mature audiences.",
            @"@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\sis\snew\shere.\sSay\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #thorlar kittyjinxu > #thorlar: HeyGuys",
            "@badge-info=subscriber/22;badges=subscriber/3012;color=#FFFF00;display-name=FELYP8;emote-only=1;emotes=521050:0-6,8-14,16-22,24-30,32-38,40-46,48-54,56-62,64-70,72-78,80-86,88-94,96-102,104-110,148-154,156-162,164-170,172-178,180-186,188-194,196-202,204-210,212-218,220-226,228-234,236-242,244-250,252-258,260-266/302827730:112-119/302827734:121-128/302827735:130-137/302827737:139-146;first-msg=0;flags=;id=1844235a-c24e-4e18-937b-805d6601aebe;mod=0;returning-chatter=0;room-id=22484632;subscriber=1;tmi-sent-ts=1685664001040;turbo=0;user-id=162760707;user-type= :felyp8!felyp8@felyp8.tmi.twitch.tv PRIVMSG #forsen :forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE1 forsenE2 forsenE3 forsenE4 forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE"
        )]
        public string? Message;

        [Benchmark]
        public IrcMessage Parse() => IrcParser.ParseMessage(Message!);
    }
}
