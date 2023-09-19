using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

//SubGift
public class GiftedSubscription : UserNoticeBase
{
    private Goal? _goal;

    public Goal? MsgParamGoal { get => _goal; protected set => _goal = value; }

    public bool IsAnonymous { get; }

    /// <summary>
    /// The total number of months the user has subscribed. 
    /// </summary>
    public string MsgParamMonths { get; protected set; } = default!;

    /// <summary>
    /// The display name of the subscription gift recipient.
    /// </summary>
    public string MsgParamRecipientDisplayName { get; protected set; } = default!;

    /// <summary>
    /// The user ID of the subscription gift recipient.
    /// </summary>
    public string MsgParamRecipientId { get; protected set; } = default!;

    /// <summary>
    /// The user name of the subscription gift recipient.
    /// </summary>
    public string MsgParamRecipientUserName { get; protected set; } = default!;

    public int MsgParamSenderCount { get; protected set; }

    /// <summary>
    /// The type of subscription plan being used.
    /// </summary>
    public SubscriptionPlan MsgParamSubPlan { get; protected set; }

    /// <summary>
    /// The display name of the subscription plan. This may be a default name or one created by the channel owner.
    /// </summary>
    public string MsgParamSubPlanName { get; protected set; } = default!;

    /// <summary>
    /// The number of months gifted as part of a single, multi-month gift.
    /// </summary>
    public int MsgParamMultiMonthGiftDuration { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GiftedSubscription"/> class.
    /// </summary>
    public GiftedSubscription(IrcMessage ircMessage) : base(ircMessage)
    {
        IsAnonymous = UserId == AnonymousGifterUserId;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamMonths:
                MsgParamMonths = tag.Value;
                break;
            case Tags.MsgParamRecipientDisplayname:
                MsgParamRecipientDisplayName = tag.Value;
                break;
            case Tags.MsgParamRecipientId:
                MsgParamRecipientId = tag.Value;
                break;
            case Tags.MsgParamRecipientUsername:
                MsgParamRecipientUserName = tag.Value;
                break;
            case Tags.MsgParamSubPlanName:
                MsgParamSubPlanName = tag.Value;
                break;
            case Tags.MsgParamSubPlan:
                MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                break;
            case Tags.MsgParamMultiMonthGiftDuration:
                MsgParamMultiMonthGiftDuration = int.Parse(tag.Value);
                break;
            default:
                return Goal.TrySetTag(ref _goal, tag);
        }
        return true;
    }
}
