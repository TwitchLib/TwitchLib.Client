using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Extractors;

namespace TwitchLib.Client.Models;

public readonly struct UserDetail
{
    readonly UserDetails _flags;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDetail"/> class.
    /// </summary>
    public UserDetail(UserDetails userDetails)
    {
        _flags = userDetails;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDetail"/> class.
    /// </summary>
    internal UserDetail(UserDetails userDetails, List<KeyValuePair<string, string>> badges)
    {
        var badgesUserDetails = UserDetailsExtractor.Extract(badges);
        _flags = userDetails | badgesUserDetails;
    }

    /// <summary>
    /// A Boolean value that determines whether the user is a moderator.
    /// </summary>
    public bool IsModerator => _flags.HasFlag(UserDetails.Moderator);

    /// <summary>
    /// A Boolean value that determines whether the user is a subscriber.
    /// </summary>
    public bool IsSubscriber => _flags.HasFlag(UserDetails.Subscriber);

    /// <summary>
    /// A Boolean value that indicates whether the user has site-wide commercial free mode enabled.
    /// </summary>
    public bool HasTurbo => _flags.HasFlag(UserDetails.Turbo);

    /// <summary>
    /// Message is from channel VIP.
    /// </summary>
    public bool IsVip => _flags.HasFlag(UserDetails.Vip);

    /// <summary>
    /// Message is from a Twitch Partner.
    /// </summary>
    public bool IsPartner => _flags.HasFlag(UserDetails.Partner);

    /// <summary>
    /// Message is from a Twitch Staff member.
    /// </summary>
    public bool IsStaff => _flags.HasFlag(UserDetails.Staff);

    /// <inheritdoc/>
    public override string ToString()
    {
        return _flags.ToString();
    }
}
