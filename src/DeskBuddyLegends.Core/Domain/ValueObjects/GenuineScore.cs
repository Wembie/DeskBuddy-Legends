using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

/// <summary>
/// Normalized score [0.0, 1.0] representing how genuine (human) the activity session was.
/// Below 0.25 triggers XP withholding.
/// </summary>
public sealed record GenuineScore
{
    public const double MinValue = 0.0;
    public const double MaxValue = 1.0;
    public const double AbuseThreshold = 0.25;

    public double Value { get; }

    public GenuineScore(double value)
    {
        Value = Guard.InRange(value, MinValue, MaxValue);
    }

    public static readonly GenuineScore Perfect = new(1.0);
    public static readonly GenuineScore Zero = new(0.0);

    public bool IsAboveAbuseThreshold => Value >= AbuseThreshold;
    public bool IsSuspicious => Value < AbuseThreshold;

    public static GenuineScore Clamp(double rawValue) =>
        new(Math.Clamp(rawValue, MinValue, MaxValue));

    public override string ToString() => $"{Value:P0}";
}
