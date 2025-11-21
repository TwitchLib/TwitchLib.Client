using BenchmarkDotNet.Attributes;
using TwitchLib.Client.Parsing;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Benchmark
{
    [ShortRunJob, MemoryDiagnoser]
    public class ChatMessageBenchmark
    {
        private static readonly IrcMessage Message = IrcParser.ParseMessage(
            "@badge-info=subscriber/22;badges=subscriber/3012;color=#FFFF00;display-name=FELYP8;emote-only=1;emotes=521050:0-6,8-14,16-22,24-30,32-38,40-46,48-54,56-62,64-70,72-78,80-86,88-94,96-102,104-110,148-154,156-162,164-170,172-178,180-186,188-194,196-202,204-210,212-218,220-226,228-234,236-242,244-250,252-258,260-266/302827730:112-119/302827734:121-128/302827735:130-137/302827737:139-146;first-msg=0;flags=;id=1844235a-c24e-4e18-937b-805d6601aebe;mod=0;returning-chatter=0;room-id=22484632;subscriber=1;tmi-sent-ts=1685664001040;turbo=0;user-id=162760707;user-type= :felyp8!felyp8@felyp8.tmi.twitch.tv PRIVMSG #forsen :forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE1 forsenE2 forsenE3 forsenE4 forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE forsenE");

        private readonly MessageEmoteCollection ChannelEmotes = new();

        [Benchmark]
        public ChatMessage ClientScenarioCreate()
        {
            return new("test_name", Message, ChannelEmotes);
        }
    }
}
