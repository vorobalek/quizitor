using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Quizitor.Common;

public static class EnumExtensions
{
    extension(Enum enumValue)
    {
        public string? DisplayName
        {
            get
            {
                var type = enumValue.GetType();
                var fieldInfo = type.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);
                var attributes = fieldInfo?.GetCustomAttributes<DisplayAttribute>(false);
                var attribute = attributes?.FirstOrDefault();
                return attribute is not null ? attribute.Name : enumValue.ToString();
            }
        }
    }
}