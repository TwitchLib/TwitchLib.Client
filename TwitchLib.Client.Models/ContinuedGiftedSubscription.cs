using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

// giftpaidupgrade
public class ContinuedGiftedSubscription : UserNoticeBase
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
    /// The login name of the user who gifted the subscription.
    /// </summary>
    public string MsgParamSenderLogin { get; protected set; } = default!;

    /// <summary>
    /// The display name of the user who gifted the subscription.
    /// </summary>
    public string MsgParamSenderName { get; protected set; } = default!;


    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuedGiftedSubscription"/> class.
    /// </summary>
    public ContinuedGiftedSubscription(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuedGiftedSubscription"/> class.
    /// </summary>
    public ContinuedGiftedSubscription(
        List<KeyValuePair<string, string>> badgeInfo,
        List<KeyValuePair<string, string>> badges,
        string hexColor,
        string displayName,
        string emotes,
        string id,
        string login,
        string msgId,
        string roomId,
        string systemMsg,
        DateTimeOffset tmiSent,
        UserDetail userDetail,
        string userId,
        UserType userType,
        Dictionary<string, string>? undocumentedTags,
        int msgParamPromoGiftTotal,
        string msgParamPromoName,
        string msgParamSenderLogin,
        string msgParamSenderName)
        : base(badgeInfo,
            badges,
            hexColor,
            displayName,
            emotes,
            id,
            login,
            msgId,
            roomId,
            systemMsg,
            tmiSent,
            userDetail,
            userId,
            userType,
            undocumentedTags)
    {
        MsgParamPromoGiftTotal = msgParamPromoGiftTotal;
        MsgParamPromoName = msgParamPromoName;
        MsgParamSenderLogin = msgParamSenderLogin;
        MsgParamSenderName = msgParamSenderName;
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
            case Tags.MsgParamSenderLogin:
                MsgParamSenderLogin = tag.Value;
                break;
            case Tags.MsgParamSenderName:
                MsgParamSenderName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
