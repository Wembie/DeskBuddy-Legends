using DeskBuddyLegends.Core.Domain.ValueObjects;
using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.Entities;

public enum AchievementType
{
    Activity = 0,
    Collection = 1,
    Progression = 2,
    Discovery = 3,
    Temporal = 4,
    Meta = 5,
    Seasonal = 6,
    Secret = 7
}

public enum ComparisonOperator
{
    GreaterThanOrEqual = 0,
    GreaterThan = 1,
    Equal = 2,
    LessThanOrEqual = 3,
    LessThan = 4
}

public sealed record CriteriaDefinition(
    string Metric,
    ComparisonOperator Operator,
    double Threshold,
    TimeSpan? Window = null
);

public sealed record AchievementProgress(
    double CurrentValue,
    double RequiredValue,
    bool IsCompleted
)
{
    public double Percentage => RequiredValue > 0
        ? Math.Min(100.0, CurrentValue / RequiredValue * 100.0)
        : IsCompleted ? 100.0 : 0.0;
}

public sealed class Achievement
{
    public AchievementId Id { get; }
    public string ResourceKey { get; }
    public string SteamApiKey { get; }
    public AchievementType Type { get; }
    public CriteriaDefinition Criteria { get; }
    public bool IsSecret { get; }
    public AchievementProgress Progress { get; private set; }
    public DateTime? UnlockedAtUtc { get; private set; }

    public bool IsUnlocked => UnlockedAtUtc.HasValue;

    public Achievement(
        AchievementId id,
        string resourceKey,
        string steamApiKey,
        AchievementType type,
        CriteriaDefinition criteria,
        bool isSecret = false)
    {
        Id = Guard.NotNull(id);
        ResourceKey = Guard.NotNullOrWhiteSpace(resourceKey);
        SteamApiKey = Guard.NotNullOrWhiteSpace(steamApiKey);
        Type = type;
        Criteria = Guard.NotNull(criteria);
        IsSecret = isSecret;
        Progress = new AchievementProgress(0, criteria.Threshold, false);
    }

    public void UpdateProgress(double currentValue)
    {
        if (IsUnlocked) return;

        var completed = Evaluate(currentValue, Criteria.Operator, Criteria.Threshold);
        Progress = new AchievementProgress(currentValue, Criteria.Threshold, completed);
    }

    public bool TryUnlock(double currentValue)
    {
        if (IsUnlocked) return false;

        UpdateProgress(currentValue);

        if (!Progress.IsCompleted) return false;

        UnlockedAtUtc = DateTime.UtcNow;
        return true;
    }

    private static bool Evaluate(double value, ComparisonOperator op, double threshold) =>
        op switch
        {
            ComparisonOperator.GreaterThanOrEqual => value >= threshold,
            ComparisonOperator.GreaterThan => value > threshold,
            ComparisonOperator.Equal => Math.Abs(value - threshold) < 0.001,
            ComparisonOperator.LessThanOrEqual => value <= threshold,
            ComparisonOperator.LessThan => value < threshold,
            _ => false
        };
}
