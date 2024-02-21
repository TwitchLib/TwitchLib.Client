using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Extractors;

internal static class UserDetailsExtractor
{
    public static UserDetails Extract(List<KeyValuePair<string, string>> badges)
    {
        var userDetails = UserDetails.None;
        foreach (var item in badges)
        {
            userDetails |= item.Key switch
            {
                "founder" or "subscriber" => UserDetails.Subscriber,
                "admin" or "staff" => UserDetails.Staff,
                "partner" => UserDetails.Partner,
                "vip" => UserDetails.Vip,
                _ => default
            };
        }
        return userDetails;
    }
}
