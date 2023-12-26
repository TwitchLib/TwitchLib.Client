using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class StandardPayForward : UserNoticeBase
{
    public bool MsgParamPriorGifterAnonymous { get; protected set; }

    public string MsgParamPriorGifterDisplayName { get; protected set; } = default!;

    public long MsgParamPriorGifterId { get; protected set; }

    public string MsgParamPriorGifterUserName { get; protected set; } = default!;

    public string? MsgParamRecipientDisplayName { get; protected set; }

    public long? MsgParamRecipientId { get; protected set; }

    public string? MsgParamRecipientUserName { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardPayForward"/> class.
    /// </summary>
    public StandardPayForward(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardPayForward"/> class.
    /// </summary>
    public StandardPayForward(
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
        bool msgParamPriorGifterAnonymous, 
        string msgParamPriorGifterDisplayName,
        long msgParamPriorGifterId, 
        string msgParamPriorGifterUserName,
        string? msgParamRecipientDisplayName,
        long? msgParamRecipientId,
        string? msgParamRecipientUserName)
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
        MsgParamPriorGifterAnonymous = msgParamPriorGifterAnonymous;
        MsgParamPriorGifterDisplayName = msgParamPriorGifterDisplayName;
        MsgParamPriorGifterId = msgParamPriorGifterId;
        MsgParamPriorGifterUserName = msgParamPriorGifterUserName;
        MsgParamRecipientDisplayName = msgParamRecipientDisplayName;
        MsgParamRecipientId = msgParamRecipientId;
        MsgParamRecipientUserName = msgParamRecipientUserName;
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
            case Tags.MsgParamRecipientDisplayName:
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