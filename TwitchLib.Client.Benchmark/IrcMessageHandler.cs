using BenchmarkDotNet.Attributes;
using TwitchLib.Client.Internal.Parsing;

namespace TwitchLib.Client.Benchmark
{
    [MemoryDiagnoser]
    public class IrcMessageHandlerBenchmark
    {
        [Benchmark]
        public bool PreconditionCheck()
        {
            // Extracted from TwitchClient.HandleIrcMessageAsync
            // This is to demonstrate the degenerate case most users encounter:
            // IrcCommand was re-generated back to raw format right after parsing.
            // Raw message passthrough alleviates the problem.
            // The cost to parse and then validate precondition:
            // Before: 1.226 us 4.63 KB
            // After: 838.2 ns 2.52 KB
            // (M1 Pro, with updated parser implementation)
            return IrcParser
                .ParseMessage(@"@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\sis\snew\shere.\sSay\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #thorlar kittyjinxu > #thorlar: HeyGuys")
                .ToString()
                .StartsWith(":tmi.twitch.tv NOTICE * :Login authentication failed");
        }
    }
}
