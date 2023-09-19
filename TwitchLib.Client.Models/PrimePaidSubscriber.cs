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
