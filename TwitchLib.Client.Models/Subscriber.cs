using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class Subscriber : UserNoticeBase
{
    /// <summary>
    /// The total number of months the user has subscribed.
    /// </summary>
    public int MsgParamCumulativeMonths { get; protected set; }

    /// <summary>
    /// A Boolean value that indicates whether the user wants their streaks shared.
    /// </summary>
    public bool MsgParamShouldShareStreak { get; protected set; }

    /// <summary>
    /// The number of consecutive months the user has subscribed.
    /// </summary>
    public int MsgParamStreakMonths { get; protected set; }

    /// <summary>
    /// The type of subscription plan being used.
    /// </summary>
    public SubscriptionPlan MsgParamSubPlan { get; protected set; }

    /// <summary>
    /// The display name of the subscription plan. This may be a default name or one created by the channel owner.
    /// </summary>
    public string MsgParamSubPlanName { get; protected set; } = default!;

    public string ResubMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscriber"/> class.
    /// </summary>
    public Subscriber(IrcMessage ircMessage) : base(ircMessage)
    {
        ResubMessage = ircMessage.Message;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamCumulativeMonths:
                MsgParamCumulativeMonths = int.Parse(tag.Value);
                break;
            case Tags.MsgParamShouldShareStreak:
                MsgParamShouldShareStreak = TagHelper.ToBool(tag.Value);
                break;
            case Tags.MsgParamStreakMonths:
                MsgParamStreakMonths = int.Parse(tag.Value);
                break;
            case Tags.MsgParamSubPlan:
                MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                break;
            case Tags.MsgParamSubPlanName:
                MsgParamSubPlanName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
