using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record CompanionId
{
    public Guid Value { get; }

    public CompanionId(Guid value)
    {
        Value = Guard.NotDefault(value);
    }

    public static CompanionId New() => new(Guid.NewGuid());
    public static CompanionId From(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
