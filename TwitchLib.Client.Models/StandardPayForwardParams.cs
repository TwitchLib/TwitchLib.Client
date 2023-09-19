using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class StandardPayForwardParams : UserNoticeBase
{
    public bool MsgParamPriorGifterAnonymous { get; protected set; }

    public string MsgParamPriorGifterDisplayName { get; protected set; } = default!;

    public long MsgParamPriorGifterId { get; protected set; }

    public string MsgParamPriorGifterUserName { get; protected set; } = default!;

    public string? MsgParamRecipientDisplayName { get; protected set; }

    public long? MsgParamRecipientId { get; protected set; }

    public string? MsgParamRecipientUserName { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardPayForwardParams"/> class.
    /// </summary>
    public StandardPayForwardParams(IrcMessage ircMessage) : base(ircMessage)
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
                MsgParamPriorGifterId = long.Parse(tag.Value);
                break;
            case Tags.MsgParamPriorGifterUserName:
                MsgParamPriorGifterUserName = tag.Value;
                break;
            case Tags.MsgParamRecipientDisplayname:
                MsgParamRecipientDisplayName = tag.Value;
                break;
            case Tags.MsgParamRecipientId:
                MsgParamRecipientId = long.Parse(tag.Value);
                break;
            case Tags.MsgParamRecipientUsername:
                MsgParamRecipientUserName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}