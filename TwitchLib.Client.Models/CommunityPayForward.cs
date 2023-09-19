using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class CommunityPayForward : UserNoticeBase
{
    public bool MsgParamPriorGifterAnonymous { get; protected set; }

    public string MsgParamPriorGifterDisplayName { get; protected set; } = default!;

    public string MsgParamPriorGifterId { get; protected set; } = default!;

    public string MsgParamPriorGifterUserName { get; protected set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityPayForward"/> class.
    /// </summary>
    public CommunityPayForward(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamPriorGifterAnonymous:
                MsgParamPriorGifterAnonymous = bool.Parse(tag.Value);
                break;
            case Tags.MsgParamPriorGifterDisplayName:
                MsgParamPriorGifterDisplayName = tag.Value;
                break;
            case Tags.MsgParamPriorGifterId:
                MsgParamPriorGifterId = tag.Value;
                break;
            case Tags.MsgParamPriorGifterUserName:
                MsgParamPriorGifterUserName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
