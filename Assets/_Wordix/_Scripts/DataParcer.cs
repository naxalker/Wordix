using System.Globalization;

public static class DataParser
{
    public static int ParseInt(string value, int fallback = 0)
    {
        if (string.IsNullOrEmpty(value)) return fallback;
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
            ? result
            : fallback;
    }

    public static float ParseFloat(string value, float fallback = 0f)
    {
        if (string.IsNullOrEmpty(value)) return fallback;
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)
            ? result
            : fallback;
    }

    public static bool ParseBoolFromInt(string value, bool fallback = false)
    {
        if (string.IsNullOrEmpty(value)) return fallback;
        return ParseInt(value, fallback ? 1 : 0) == 1;
    }
}