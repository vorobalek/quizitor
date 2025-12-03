using System.Text.Json;

namespace Quizitor.Common;

public static class JsonSerializerHelper
{
    public static TValue? TryDeserialize<TValue>(string json, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Deserialize<TValue>(json, options);
        }
        catch
        {
            return default;
        }
    }
}