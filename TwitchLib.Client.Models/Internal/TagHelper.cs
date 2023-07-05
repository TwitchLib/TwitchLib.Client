using System.Drawing;
using System.Globalization;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Extensions;

namespace TwitchLib.Client.Models.Internal;

internal static class TagHelper
{
    public static Color ToColor(string s)
    {
        if (string.IsNullOrEmpty(s))
            return default;
#if NETSTANDARD2_0
        var rgb = int.Parse(s.Substring(1), NumberStyles.HexNumber);
#else
        var rgb = int.Parse(s.AsSpan(1), NumberStyles.HexNumber);
#endif
        return Color.FromArgb(rgb);
    }


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
            "prime" => SubscriptionPlan.Prime,
            "1000" => SubscriptionPlan.Tier1,
            "2000" => SubscriptionPlan.Tier2,
            "3000" => SubscriptionPlan.Tier3,
        };
    }

    public static bool ToBool(string s)
    {
        return s == "1";
    }
}
