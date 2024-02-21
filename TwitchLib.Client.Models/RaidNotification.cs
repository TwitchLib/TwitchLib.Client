using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class RaidNotification : UserNoticeBase
{
    /// <summary>
    /// The display name of the broadcaster raiding this channel.
    /// </summary>
    public string MsgParamDisplayName { get; protected set; } = default!;

    /// <summary>
    /// The login name of the broadcaster raiding this channel.
    /// </summary>
    public string MsgParamLogin { get; protected set; } = default!;

    /// <summary>
    /// The number of viewers raiding this channel from the broadcaster’s channel.
    /// </summary>
    public string MsgParamViewerCount { get; protected set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidNotification"/> class.
    /// </summary>
    public RaidNotification(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidNotification"/> class.
    /// </summary>
    public RaidNotification(
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
       string msgParamDisplayName,
       string msgParamLogin,
       string msgParamViewerCount)
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
        MsgParamDisplayName = msgParamDisplayName;
        MsgParamLogin = msgParamLogin;
        MsgParamViewerCount = msgParamViewerCount;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamDisplayname:
                MsgParamDisplayName = tag.Value;
                break;
            case Tags.MsgParamLogin:
                MsgParamLogin = tag.Value;
                break;
            case Tags.MsgParamViewerCount:
                MsgParamViewerCount = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
