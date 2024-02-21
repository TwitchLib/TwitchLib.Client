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

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunitySubscription"/> class.
    /// </summary>
    public CommunitySubscription(
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
        Goal? msgParamGoal,
        string msgParamGiftTheme,
        int msgParamMassGiftCount,
        string msgParamOriginId, 
        int msgParamSenderCount,
        SubscriptionPlan msgParamSubPlan)
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
        IsAnonymous = userId == AnonymousGifterUserId;
        MsgParamGoal = msgParamGoal;
        MsgParamGiftTheme = msgParamGiftTheme;
        MsgParamMassGiftCount = msgParamMassGiftCount;
        MsgParamOriginId = msgParamOriginId;
        MsgParamSenderCount = msgParamSenderCount;
        MsgParamSubPlan = msgParamSubPlan;
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
