namespace Quizitor.Common;

public static class StringExtensions
{
    extension(string input)
    {
        public string Html =>
            input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");

        public string Truncate(int maxLength, string truncationSuffix = "…")
        {
            return input.Length > maxLength
                ? input[..(maxLength - truncationSuffix.Length)] + truncationSuffix
                : input;
        }
    }
}