using System.ComponentModel;

namespace Quizitor.Common;

public static class EnvironmentExtensions
{
    public static string? GetEnvironmentVariable(this string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName);
    }

    public static string GetEnvironmentVariableOrThrowIfNullOrWhiteSpace(this string variableName)
    {
        var value = variableName.GetEnvironmentVariable();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(variableName, $"Environment variable '{variableName}' is not defined.");

        return value;
    }

    public static T GetEnvironmentVariableWithFallbackValue<T>(
        this string variableName,
        T fallbackValue)
    {
        var value = variableName.GetEnvironmentVariable();
        return (string.IsNullOrWhiteSpace(value)
            ? fallbackValue
            : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value)!)!;
    }
}