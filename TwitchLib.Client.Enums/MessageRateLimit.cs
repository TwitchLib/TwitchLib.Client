namespace TwitchLib.Client.Enums
{
    /// <summary>
    ///     <see href="https://dev.twitch.tv/docs/irc/#rate-limits"/>
    /// </summary>
    public enum MessageRateLimit : uint
    {
        Limit_20_in_30_Seconds = 20,
        Limit_100_in_30_Seconds = 100,
        Limit_7500_in_30_Seconds = 7_500,
    }
}
