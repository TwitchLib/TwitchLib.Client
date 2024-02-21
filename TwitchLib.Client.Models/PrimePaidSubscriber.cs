using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class PrimePaidSubscriber : UserNoticeBase
{
    /// <summary>
    /// The type of subscription plan being used.
    /// </summary>
    public SubscriptionPlan MsgParamSubPlan { get; protected set; }

    /// <summary>
    /// Property representing system message.
    /// </summary>
    public string ResubMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Announcement"/> class.
    /// </summary>
    /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
    public PrimePaidSubscriber(IrcMessage ircMessage) : base(ircMessage)
    {
        ResubMessage = ircMessage.Message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Announcement"/> class.
    /// </summary>
    public PrimePaidSubscriber(
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
        SubscriptionPlan msgParamSubPlan,
        string resubMessage)
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
        MsgParamSubPlan = msgParamSubPlan;
        ResubMessage = resubMessage;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamSubPlan:
                MsgParamSubPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                break;
            default:
                return false;
        }
        return true;
    }
}
