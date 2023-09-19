using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class AnonGiftPaidUpgrade : UserNoticeBase
{
    /// <summary>
    /// The number of gifts the gifter has given during the promo indicated by <see cref="MsgParamPromoName"/>.
    /// </summary>
    public int MsgParamPromoGiftTotal { get; protected set; }

    /// <summary>
    /// The subscriptions promo, if any, that is ongoing (for example, Subtember 2018).
    /// </summary>
    public string MsgParamPromoName { get; protected set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonGiftPaidUpgrade"/> class.
    /// </summary>
    public AnonGiftPaidUpgrade(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamPromoGiftTotal:
                MsgParamPromoGiftTotal = int.Parse(tag.Value);
                break;
            case Tags.MsgParamPromoName:
                MsgParamPromoName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}