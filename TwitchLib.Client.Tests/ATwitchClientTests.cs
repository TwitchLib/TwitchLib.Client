namespace TwitchLib.Client.Tests
{
    public abstract class ATwitchClientTests<T>
    {
        protected static string TWITCH_Username => "Username";
        protected static string TWITCH_UsernameAnother => "UsernameAnother";
        protected static string TWITCH_OAuth => "OAuth";
        protected static string TWITCH_CHANNEL => "testchannel";
        protected static int WaitOneDuration => 5_000;
        protected static int WaitOneDurationShort => 500;
    }
}
