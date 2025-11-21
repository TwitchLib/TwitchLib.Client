using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

/// <summary>Class representing Announcement in a Twitch channel.</summary>
public class Announcement : UserNoticeBase
{
    /// <summary>Property representing the color value of the announcement.</summary>
    public string MsgParamColor { get; protected set; } = default!;

    /// <summary>Property representing the message of the announcement.</summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Announcement"/> class.
    /// </summary>
    /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
    public Announcement(IrcMessage ircMessage) : base(ircMessage)
    {
        Message = ircMessage.Message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Announcement"/> class.
    /// </summary>
    public Announcement(
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
        string msgParamColor,
        string message)
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
        MsgParamColor = msgParamColor;
        Message = message;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamColor:
                MsgParamColor = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
