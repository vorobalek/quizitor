using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Quizitor.Common;

public static class EnumExtensions
{
    private static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
    {
        var type = enumValue.GetType();
        var memInfo = type.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);
        var attributes = memInfo?.GetCustomAttributes<T>(false);
        return attributes?.FirstOrDefault();
    }

    public static string? GetDisplayName(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<DisplayAttribute>();
        return attribute == null ? enumValue.ToString() : attribute.Name;
    }
}