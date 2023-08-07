using TwitchLib.Client.Enums;

public class HypeChat
{
    /// <summary>
    /// The value of the Hype Chat sent by the user.
    /// </summary>
    public int Amount { get; internal set; }

    public double CalculatedAmount => Exponent == 0
        ? Amount
        : Amount / (10 * Exponent);

    /// <summary>
    /// The ISO 4217 alphabetic currency code the user has sent the Hype Chat in.
    /// </summary>
    public string Currency { get; internal set; } = default!;

    /// <summary>
    /// Indicates how many decimal points this currency represents partial amounts in. Decimal points start from the right side of the value defined in pinned-chat-paid-amount.
    /// </summary>
    public int Exponent { get; internal set; }

    /// <summary>
    /// The level of the Hype Chat, in English.Possible values are:
    /// </summary>
    public PaidLevel Level { get; internal set; }

    /// <summary>
    /// A Boolean value that determines if the message sent with the Hype Chat was filled in by the system.
    /// </summary>
    /// <remarks>
    /// If true (1), the user entered no message and the body message was automatically filled in by the system.
    /// If false (0), the user provided their own message to send with the Hype Chat.
    /// </remarks>
    public bool IsSystemMessage { get; internal set; }
}
