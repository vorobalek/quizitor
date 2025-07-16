namespace Quizitor.Common;

public static class StringExtensions
{
    public static string EscapeHtml(this string input)
    {
        return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
            ;
    }

    public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
    {
        return value.Length > maxLength
            ? value[..(maxLength - truncationSuffix.Length)] + truncationSuffix
            : value;
    }
}