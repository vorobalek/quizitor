using System.Text.Json;
using Quizitor.Common;

namespace Quizitor.Redis.Serializers;

// ReSharper disable once ClassNeverInstantiated.Global
internal class RedisDefaultSerializer<T> : IJsonSerializer<T>
{
    public string Serialize(T @object)
    {
        return SerializeInternal(@object);
    }

    public T Deserialize(string json)
    {
        return DeserializeInternal(json);
    }

    public T? TryDeserialize(string json)
    {
        try
        {
            return Deserialize(json);
        }
        catch
        {
            return default;
        }
    }

    private static string SerializeInternal(T @object)
    {
        return JsonSerializer.Serialize(@object);
    }

    private static T DeserializeInternal(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}