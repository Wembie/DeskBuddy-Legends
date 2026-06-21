using System.Runtime.CompilerServices;

namespace DeskBuddyLegends.Core.SharedKernel;

public static class Guard
{
    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
        return value;
    }

    public static string NotNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        return value;
    }

    public static T NotDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new ArgumentException("Value cannot be default.", paramName);
        return value;
    }

    public static long NonNegative(long value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be non-negative.");
        return value;
    }

    public static double InRange(double value, double min, double max, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
        return value;
    }

    public static int Positive(int value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");
        return value;
    }
}
