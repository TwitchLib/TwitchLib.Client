using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class ViewerMilestone : UserNoticeBase
{
    public string MsgParamCategory { get; protected set; } = default!;
    public int MsgParamValue { get; protected set; }
    public string MsgParamId { get; protected set; } = default!;
    public int MsgParamCopoReward { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewerMilestone"/> class.
    /// </summary>
    public ViewerMilestone(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamCategory:
                MsgParamCategory = tag.Value;
                break;
            case Tags.MsgParamValue:
                MsgParamValue = int.Parse(tag.Value);
                break;
            case Tags.MsgParamId:
                MsgParamId = tag.Value;
                break;
            case Tags.MsgParamCopoReward:
                MsgParamCopoReward = int.Parse(tag.Value);
                break;
            default:
                return false;
        }
        return true;
    }
}
