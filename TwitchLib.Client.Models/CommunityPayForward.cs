using TwitchLib.Client.Enums;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityPayForward"/> class.
    /// </summary>
    public CommunityPayForward(
        List<KeyValuePair<string, string>> badgeInfo,
        List<KeyValuePair<string, string>> badges,
        string hexColor,
        string displayName,
        string emotes,
        string id,
        string login,
        bool isModerator,
        string msgId,
        string roomId,
        bool isSubscriber,
        string systemMsg,
        DateTimeOffset tmiSent,
        bool isTurbo,
        string userId,
        UserType userType,
        Dictionary<string, string>? undocumentedTags,
        bool msgParamPriorGifterAnonymous, 
        string msgParamPriorGifterDisplayName,
        string msgParamPriorGifterId,
        string msgParamPriorGifterUserName)
        : base(badgeInfo,
            badges,
            hexColor,
            displayName,
            emotes,
            id,
            login,
            isModerator,
            msgId,
            roomId,
            isSubscriber,
            systemMsg,
            tmiSent,
            isTurbo,
            userId,
            userType,
            undocumentedTags)
    {
        MsgParamPriorGifterAnonymous = msgParamPriorGifterAnonymous;
        MsgParamPriorGifterDisplayName = msgParamPriorGifterDisplayName;
        MsgParamPriorGifterId = msgParamPriorGifterId;
        MsgParamPriorGifterUserName = msgParamPriorGifterUserName;
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
