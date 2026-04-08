namespace WatchWithMeAPI.Utils;

public static class StringExtensions
{
    public static T ToEnum<T>(this string value, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrEmpty(value)) return defaultValue;

        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }
}