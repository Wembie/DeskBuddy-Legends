using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record AchievementId
{
    public string Value { get; }

    public AchievementId(string value)
    {
        Value = Guard.NotNullOrWhiteSpace(value);
    }

    public static AchievementId From(string value) => new(value);

    public override string ToString() => Value;
}
