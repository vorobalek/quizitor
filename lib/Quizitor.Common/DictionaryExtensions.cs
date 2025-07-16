namespace Quizitor.Common;

public static class DictionaryExtensions
{
    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> factory)
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;
        value = await factory(key);
        dictionary.Add(key, value);
        return value;
    }
}