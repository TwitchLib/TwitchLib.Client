using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

//submysterygift
public class CommunitySubscription : UserNoticeBase
{
    private Goal? _goal;

    public bool IsAnonymous { get; }

    public Goal? MsgParamGoal { get => _goal; protected set => _goal = value; }

    public string MsgParamGiftTheme { get; protected set; } = default!;

    public int MsgParamMassGiftCount { get; protected set; }

    public string MsgParamOriginId { get; protected set; } = default!;

    public int MsgParamSenderCount { get; protected set; }

    /// <summary>
    /// The type of subscription plan being used.
    /// </summary>
    public SubscriptionPlan MsgParamSubPlan { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunitySubscription"/> class.
    /// </summary>
    public CommunitySubscription(IrcMessage ircMessage) : base(ircMessage)
    {
        IsAnonymous = UserId == AnonymousGifterUserId;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamGiftTheme:
                MsgParamGiftTheme = tag.Value;
                break;
            case Tags.MsgParamMassGiftCount:
                MsgParamMassGiftCount = int.Parse(tag.Value);
                break;
            case Tags.MsgParamSenderCount:
                MsgParamSenderCount = int.Parse(tag.Value);
                break;
            case Tags.MsgParamSubPlan:
                MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                break;
            default:
                return Goal.TrySetTag(ref _goal, tag);
        }
        return true;
    }
}
