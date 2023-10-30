using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class BitsBadgeTier : UserNoticeBase
{
    /// <summary>
    /// The tier of the Bits badge the user just earned. For example, 100, 1000, or 10000.
    /// </summary>
    public int MsgParamThreshold { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitsBadgeTier"/> class.
    /// </summary>
    public BitsBadgeTier(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitsBadgeTier"/> class.
    /// </summary>
    public BitsBadgeTier(
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
       int msgParamThreshold)
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
        MsgParamThreshold = msgParamThreshold;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamThreshold:
                MsgParamThreshold = int.Parse(tag.Value);
                break;
        }
        return true;
    }
}
