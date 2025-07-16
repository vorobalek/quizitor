using System.Security.Cryptography;

namespace Quizitor.Tests;

public static class Unique
{
    public static Guid Guid()
    {
        return System.Guid.NewGuid();
    }

    public static int Int32()
    {
        return BitConverter.ToInt32(RandomNumberGenerator.GetBytes(4));
    }

    public static long Int64()
    {
        return BitConverter.ToInt64(RandomNumberGenerator.GetBytes(8));
    }

    public static DateTimeOffset DateTimeOffset()
    {
        return System.DateTimeOffset.FromUnixTimeSeconds(Int32());
    }

    public static TEnum Enum<TEnum>() where TEnum : struct, Enum
    {
        return RandomNumberGenerator.GetItems<TEnum>(System.Enum.GetValues<TEnum>(), 1)[0];
    }

    public static string String(int? lenght = null)
    {
        return RandomNumberGenerator.GetString(
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_", lenght ?? 32);
    }

    public static string TelegramBotToken()
    {
        return $"{Int32()}:{String()}";
    }
}