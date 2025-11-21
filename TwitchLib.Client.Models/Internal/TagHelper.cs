using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Extensions;

namespace TwitchLib.Client.Models.Internal;

internal static class TagHelper
{
    /// <summary>
    /// Parses the badges field in GLOBALUSERSTATE, PRIVMSG, USERNOTICE, USERSTATE, etc
    /// </summary>
    /// <param name="badgesStr">The data.</param>
    /// <returns>List of keyvalue pairs representing each badge and value associated</returns>
    public static List<KeyValuePair<string, string>> ToBadges(string badgesStr)
    {
        var badges = new List<KeyValuePair<string, string>>();

        if (badgesStr.Contains('/'))
        {
            foreach (var badge in new SpanSliceEnumerator(badgesStr, ','))
            {
                var index = badge.IndexOf('/');
                var key = badge.Slice(0, index).ToString();
                var value = badge.Slice(index + 1).ToString();
                badges.Add(new KeyValuePair<string, string>(key, value));
            }
        }
        return badges;
    }

    public static UserType ToUserType(string s)
    {
        return s switch
        {
            "mod" => UserType.Moderator,
            "global_mod" => UserType.GlobalModerator,
            "admin" => UserType.Admin,
            "staff" => UserType.Staff,
            _ => UserType.Viewer,
        };
    }

    public static SubscriptionPlan ToSubscriptionPlan(string s)
    {
        return s switch
        {
            "Prime" or "prime" => SubscriptionPlan.Prime,
            "1000" => SubscriptionPlan.Tier1,
            "2000" => SubscriptionPlan.Tier2,
            "3000" => SubscriptionPlan.Tier3,
            _ => throw new ArgumentException($"Unhandled value {s} for {nameof(UserType)}", nameof(s)),
        };
    }

    public static bool ToBool(string s)
    {
        return s == "1";
    }

    public static DateTimeOffset ToDateTimeOffsetFromUnixMs(string s)
    {
        var timestamp = long.Parse(s);
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
    }
}
