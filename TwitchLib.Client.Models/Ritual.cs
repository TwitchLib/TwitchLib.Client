using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class Ritual : UserNoticeBase
{
    /// <summary>
    /// The name of the ritual being celebrated.
    /// </summary>
    public string MsgParamRitualName { get; protected set; } = default!;

    public string Message { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ritual"/> class.
    /// </summary>
    public Ritual(IrcMessage ircMessage) : base(ircMessage)
    {
        Message = ircMessage.Message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ritual"/> class.
    /// </summary>
    public Ritual(
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
       string msgParamRitualName,
       string message)
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
        MsgParamRitualName = msgParamRitualName;
        Message = message;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamRitualName:
                MsgParamRitualName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}