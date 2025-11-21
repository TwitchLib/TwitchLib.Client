using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public abstract class UserNoticeBase : IHexColorProperty
{
    internal const string AnonymousGifterUserId = "274598607";

    /// <summary>
    /// Contains metadata related to the chat badges in the <see cref="Badges"/> tag.
    /// </summary>
    public List<KeyValuePair<string, string>> BadgeInfo { get; protected set; } = default!;

    /// <summary>
    /// List of chat badges.
    /// </summary>
    public List<KeyValuePair<string, string>> Badges { get; protected set; } = default!;

    /// <inheritdoc/>
    public string HexColor { get; protected set; } = default!;

    /// <summary>
    /// The user’s display name, escaped as described in the IRCv3 spec.
    /// </summary>
    public string DisplayName { get; protected set; } = default!;

    /// <summary>
    ///  List of emotes and their positions in the message.
    /// </summary>
    public string Emotes { get; protected set; } = default!;

    /// <summary>
    /// An ID that uniquely identifies this message.
    /// </summary>
    public string Id { get; protected set; } = default!;

    /// <summary>
    /// The login name of the user whose action generated the message.
    /// </summary>
    public string Login { get; protected set; } = default!;

    /// <summary>
    /// The type of notice (not the ID).
    /// </summary>
    public string MsgId { get; protected set; } = default!;

    /// <summary>
    /// An ID that identifies the chat room (channel).
    /// </summary>
    public string RoomId { get; protected set; } = default!;

    /// <summary>
    /// The message Twitch shows in the chat room for this notice.
    /// </summary>
    public string SystemMsg { get; protected set; } = default!;

    /// <summary>
    /// The time for when the Twitch IRC server received the message.
    /// </summary>
    public DateTimeOffset TmiSent { get; protected set; }

    public UserDetail UserDetail { get; protected set; }

    /// <summary>
    /// The user’s ID.
    /// </summary>
    public string UserId { get; protected set; } = default!;

    /// <summary>
    /// he type of user sending the whisper message.
    /// </summary>
    public UserType UserType { get; protected set; }

    public Dictionary<string, string>? UndocumentedTags { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNoticeBase"/> class.
    /// </summary>
    protected UserNoticeBase(IrcMessage ircMessage)
    {
        var userDetails = UserDetails.None;
        foreach (var tag in ircMessage.Tags)
        {
            switch (tag.Key)
            {
                case Tags.BadgeInfo:
                    BadgeInfo = TagHelper.ToBadges(tag.Value);
                    break;
                case Tags.Badges:
                    Badges = TagHelper.ToBadges(tag.Value);
                    break;
                case Tags.Color:
                    HexColor = tag.Value;
                    break;
                case Tags.DisplayName:
                    DisplayName = tag.Value;
                    break;
                case Tags.Emotes:
                    Emotes = tag.Value;
                    break;
                case Tags.Id:
                    Id = tag.Value;
                    break;
                case Tags.Login:
                    Login = tag.Value;
                    break;
                case Tags.Mod:
                    if (TagHelper.ToBool(tag.Value))
                        userDetails |= UserDetails.Moderator;
                    break;
                case Tags.MsgId:
                    MsgId = tag.Value;
                    break;
                case Tags.RoomId:
                    RoomId = tag.Value;
                    break;
                case Tags.Subscriber:
                    if (TagHelper.ToBool(tag.Value))
                        userDetails |= UserDetails.Subscriber;
                    break;
                case Tags.SystemMsg:
                    SystemMsg = tag.Value.Replace("\\s", " ");
                    break;
                case Tags.TmiSentTs:
                    TmiSent = TagHelper.ToDateTimeOffsetFromUnixMs(tag.Value);
                    break;
                case Tags.Turbo:
                    if (TagHelper.ToBool(tag.Value))
                        userDetails |= UserDetails.Turbo;
                    break;
                case Tags.UserId:
                    UserId = tag.Value;
                    break;
                case Tags.UserType:
                    UserType = TagHelper.ToUserType(tag.Value);
                    break;
                default:
                    if (!TrySet(tag))
                        (UndocumentedTags ??= new()).Add(tag.Key, tag.Value);
                    break;
            }
        }
        UserDetail = new UserDetail(userDetails, Badges);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNoticeBase"/> class.
    /// </summary>
    protected UserNoticeBase(
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
        Dictionary<string, string>? undocumentedTags)
    {
        BadgeInfo = badgeInfo;
        Badges = badges;
        HexColor = hexColor;
        DisplayName = displayName;
        Emotes = emotes;
        Id = id;
        Login = login;
        MsgId = msgId;
        RoomId = roomId;
        SystemMsg = systemMsg;
        TmiSent = tmiSent;
        UserDetail = userDetail;
        UserId = userId;
        UserType = userType;
        UndocumentedTags = undocumentedTags;
    }

    protected abstract bool TrySet(KeyValuePair<string, string> tag);
}
