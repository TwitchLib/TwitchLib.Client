namespace TwitchLib.Client.Models.Extensions;

#if NETSTANDARD2_0
internal static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        TValue? value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }
}
#endif
