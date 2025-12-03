using System.ComponentModel;

namespace Quizitor.Common;

public static class EnvironmentExtensions
{
    extension(string variableName)
    {
        public string? EnvironmentValue =>
            Environment.GetEnvironmentVariable(variableName);

        public string RequiredEnvironmentValue
        {
            get
            {
                var value = variableName.EnvironmentValue;
                return !string.IsNullOrWhiteSpace(value)
                    ? value
                    : throw new ArgumentNullException(variableName, $"Environment variable '{variableName}' is not defined.");
            }
        }

        public T GetEnvironmentValueWithFallback<T>(T fallbackValue)
        {
            var value = variableName.EnvironmentValue;
            return (string.IsNullOrWhiteSpace(value)
                ? fallbackValue
                : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value)!)!;
        }
    }
}