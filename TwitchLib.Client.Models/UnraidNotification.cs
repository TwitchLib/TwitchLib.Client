using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class UnraidNotification : UserNoticeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnraidNotification"/> class.
    /// </summary>
    public UnraidNotification(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnraidNotification"/> class.
    /// </summary>
    public UnraidNotification(
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
        Dictionary<string, string>? undocumentedTags)
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
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        return false;
    }
}